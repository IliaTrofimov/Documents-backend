using AutoMapper;
using Documents_backend.Models;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Linq;
using System.Web.Http.Cors;
using System.Net.Http;


namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class DocumentsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        [ActionName("list")]
        public IEnumerable<DocumentDTO> Get()
        {
            var documents = db.Documents;
            if (documents == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return mapper.Map<IEnumerable<DocumentDTO>>(documents.ToList());
        }

        [HttpGet]
        [ActionName("get")]
        public Document Get(int id)
        {
            Document document = db.Documents.Find(id);
            if (document == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            document.DocumentDataItems = (from c in document.DocumentDataItems
                                          orderby c.Field, c.Row, c.Col
                                          select c).ToList();
            document.Template.TemplateItems = document.Template.TemplateItems.OrderBy(f => f.Order).ToList();
            return document;
        }   


        [HttpPost]
        [ActionName("post")]
        public int Post([FromBody] int templateId, [FromBody] int authorId)
        {
            if (authorId == -1 || db.Users.Find(authorId) == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            Template template = db.Templates.Find(templateId);
            if (template == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            Document document = new Document() { TemplateId = templateId, AuthorId = authorId, UpdateDate = System.DateTime.Now };
            foreach (var item in template.TemplateItems)
            {
                if (item is TemplateField field)
                    document.DocumentDataItems.Add(new DocumentDataItem() { Field = item.Id});               
                else if (item is TemplateTable table)
                {
                    foreach (var col in table.TemplateFields)
                        for (int i = 0; i < table.Rows; i++)
                            document.DocumentDataItems.Add(new DocumentDataItem() { Field = item.Id, Col = col.Id, Row = i });
                }
            }

            db.Documents.Add(document);
            db.SaveChanges();
            return document.Id;
        }

        [HttpPost]
        [ActionName("from-existing")]
        public int FromExisting([FromBody] int documentId, [FromBody] int authorId)
        {
            if (authorId == -1 || db.Users.Find(authorId) == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            Document oldDocument = db.Documents.Find(documentId);
            if (oldDocument == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            Document document = new Document() { TemplateId = oldDocument.TemplateId, AuthorId = authorId, UpdateDate = System.DateTime.Now };
            foreach (var item in oldDocument.DocumentDataItems)
                document.DocumentDataItems.Add(new DocumentDataItem() { Field = item.Id, Col = item.Col, Row = item.Row, Value = item.Value });

            db.Documents.Add(document);
            db.SaveChanges();
            return document.Id;
        }


        [HttpPut]
        [ActionName("put")]
        public void Put([FromBody] Document document)
        {
            document.UpdateDate = System.DateTime.Now;
            db.Entry(document).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return;
        }

        [HttpPut]
        [ActionName("put-field")]
        public Document UpdateField(int id, [FromBody] DocumentDataItem item)
        {
            Document document = db.Documents.Find(id);
            if (document == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            item.Document = document;
            if (item.Id == -1)
                db.DocumentDataItems.Add(item);
            else
            {
                var found = db.DocumentDataItems.Find(item.Id);
                found.Value = item.Value;
                found.Col = item.Col;
                found.Row = item.Row;
            }
            db.SaveChanges();
            return document;
        }


        [HttpDelete]
        [ActionName("delete")]
        public HttpResponseMessage Delete(int id)
        {
            Document info = db.Documents.Find(id);
            if (info != null)
            {
                db.Documents.Remove(info);
                db.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.Accepted);
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