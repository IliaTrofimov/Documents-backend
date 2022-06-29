using AutoMapper;
using System.Net;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Data.Entity;

using Documents.Models.DTO;
using Documents.Models.Entities;
using Documents.Models.POST;
using Documents.Models;


namespace Documents.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class DocumentsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        [ActionName("count")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Count(int template = -1, int user = -1, int type = -1)
        {
            int count = await db.Documents.CountAsync(d => (type == -1 || d.Type == type) && (user == -1 || d.AuthorId == user) &&
                        (template == -1 || d.TemplateId == template));
            return Ok(count);
        }

        [HttpGet]
        [ActionName("list")]
        [ResponseType(typeof(IEnumerable<DocumentDTO>))]
        public async Task<IHttpActionResult> Get(int page = 0, int pageSize = -1, int template = -1, int user = -1, int type = -1)
        {
            List<Document> documents;
            if (pageSize != -1)
                documents = await db.Documents.Include("Template")
                    .OrderBy(t => t.Id)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Where(d => (type == -1 || d.Type == type) && (user == -1 || d.AuthorId == user) && 
                        (template == -1 || d.TemplateId == template)).ToListAsync();
            else
                documents = await db.Documents.Include("Template")
                    .Where(d => (type == -1 || d.Type == type) && (user == -1 || d.AuthorId == user) &&
                        (template == -1 || d.TemplateId == template)).ToListAsync();

            if (documents == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);

            return Ok(mapper.Map<IEnumerable<DocumentDTO>>(documents));
        }

        [HttpGet]
        [ActionName("get")]
        [ResponseType(typeof(Document))]
        public async Task<IHttpActionResult> Get(int id)
        {
            Document document = await db.Documents
                .Include("Template.TemplateItems")
                .Include("DocumentDataItems")
                .Include("Template.TemplateType.Positions")
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            document.Template = document.Template.SortTemplateItems();
            return Ok(document);
        }   


        [HttpPost]
        [ActionName("post")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Post([FromBody] DocumentPOST body)
        {
            Template template = await db.Templates.Include("TemplateType.Positions").FirstOrDefaultAsync(t => t.Id == body.TemplateId);
            if (template == null)
                BadRequest("Cannot create document from template, template not found");

            var user = db.Users.FirstOrDefault();
            if (user == null)
            {
                user = db.Users.Add(Models.Entities.User.CreateAdmin());
                user.PositionId = 4;
                await db.SaveChangesAsync();
            }

            Document document = db.Documents.Add(new Document()
            {
                TemplateId = body.TemplateId,
                UpdateDate = System.DateTime.Now,
                AuthorId = user.Id,
                Name = body.Name,
                Type = 0
            });
            await db.SaveChangesAsync();

            foreach (var pos in template.TemplateType.Positions)
                db.Signs.Add(new Sign() { DocumentId = document.Id, InitiatorId = document.Author.Id, UpdateDate = System.DateTime.Now, CreateDate = System.DateTime.Now, SignerPositionId = user.PositionId });

            foreach (var item in template.TemplateItems)
            {
                if (item is TemplateField field)
                    document.DocumentDataItems.Add(new DocumentDataItem() { FieldId = item.Id, Document = document });
                else if (item is TemplateTable table)
                {
                    foreach (var col in table.TemplateFields)
                        for (int i = 0; i < table.Rows; i++)
                            document.DocumentDataItems.Add(new DocumentDataItem() 
                            { 
                                FieldId = col.Id, 
                                Document = document,
                                Row = i 
                            });
                }
            }

            await db.SaveChangesAsync();
            return Ok(document.Id);
        }

        [HttpPost]
        [ActionName("from-existing")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> FromExisting([FromBody] int documentId, [FromBody] int authorId)
        {
            Document oldDocument = db.Documents.Find(documentId);
            if (oldDocument == null)
                BadRequest("Cannot create new version of document, previous document not found");

            Document document = new Document() { TemplateId = oldDocument.TemplateId, AuthorId = authorId, UpdateDate = System.DateTime.Now };
            foreach (var item in oldDocument.DocumentDataItems)
                document.DocumentDataItems.Add(new DocumentDataItem() { FieldId = item.FieldId, Row = item.Row, Value = item.Value });

            db.Documents.Add(document);
            await db.SaveChangesAsync();
            return Ok(document.Id);
        }


        [HttpPut]
        [ActionName("put")]
        [ResponseType(typeof(Document))]
        public async Task<IHttpActionResult> Put([FromBody] Document document)
        {
            document.UpdateDate = System.DateTime.Now;
            Document found = db.Documents.Find(document.Id);
            if (found == null)
                BadRequest("Cannot update document, document not found");

            found.UpdateDate = System.DateTime.Now;
            found.Name = document.Name;
            found.ExpireDate = document.ExpireDate;
            found.Type = document.Type;
            await db.SaveChangesAsync();

            document.Template.TemplateItems = document.Template.TemplateItems
                .OrderBy(item => item.Order)
                .Where(item => !(item is TemplateField tf) || tf.TemplateTableId == null)
                .ToList();
            return Ok(document);
        }

        [HttpPut]
        [ActionName("put-field")]
        [ResponseType(typeof(Document))]
        public async Task<IHttpActionResult> UpdateField(int id, [FromBody] DocumentDataItem item)
        {
            Document document = db.Documents.Find(id);
            if (document == null)
                BadRequest("Cannot update document, document not found");

            item.Document = document;
            if (item.Id == -1)
                db.DocumentDataItems.Add(item);
            else
            {
                var found = db.DocumentDataItems.Find(item.Id);
                found.Value = item.Value;
                found.Row = item.Row;
            }
            await db.SaveChangesAsync();

            document.Template.TemplateItems = document.Template.TemplateItems
                .OrderBy(i => i.Order)
                .Where(i => !(i is TemplateField tf) || tf.TemplateTableId == null)
                .ToList();
            return Ok(document);
        }


        [HttpDelete]
        [ActionName("delete")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            Document info = db.Documents.Find(id);
            if (info != null)
            {
                db.DocumentDataItems.RemoveRange(db.DocumentDataItems.Where(i => i.Document.Id == info.Id));
                db.Documents.Remove(info);
                await db.SaveChangesAsync();
                return Ok();
            }
            else return BadRequest("Cannot delete document, document not found");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}