using AutoMapper;
using Documents_backend.Models;
using System.Collections.Generic;
using System.Web.Http;


namespace Documents_backend.Controllers
{
    public class DocumentsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        public IEnumerable<DocumentDTO> Get()
        {
            return mapper.Map<IEnumerable<DocumentDTO>>(db.Document);
        }

        [HttpGet]
        public Document Get(int id)
        {
            Document document = db.Document.Find(id);
            if (document == null)
                StatusCode(System.Net.HttpStatusCode.NotFound);

            db.Entry(document).Reference("DocumentDataItem").Load();
            db.Entry(document).Reference("DocumentTableCell").Load();
            return document;
        }   


        [HttpPost]
        public void Post([FromBody] int templateId, [FromBody] int authorId)
        {
            db.Document.Add(new Document() { TemplateId = templateId, AuthorId = authorId });
        }


        [HttpPut]
        public void Put(int id, [FromBody] Document document)
        {
            if (id == document.Id)
            {
                db.Entry(document).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else BadRequest();
        }

        [HttpDelete]
        public void Delete(int id)
        {
            Document info = db.Document.Find(id);
            if (info != null)
            {
                db.Document.Remove(info);
                db.SaveChanges();
            }
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