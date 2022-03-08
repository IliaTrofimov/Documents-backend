using AutoMapper;
using Documents_backend.Models;
using System.Collections.Generic;
using System.Net;
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
            var documents = db.Document;
            if (documents == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return mapper.Map<IEnumerable<DocumentDTO>>(documents);
        }

        [HttpGet]
        public Document Get(int id)
        {
            Document document = db.Document.Find(id);
            if (document == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return document;
        }   


        [HttpPost]
        public int Post([FromBody] int templateId, [FromBody] int authorId)
        {
            return db.Document.Add(new Document() { TemplateId = templateId, AuthorId = authorId }).Id;
        }


        [HttpPut]
        public void Put([FromBody] Document document)
        {
            db.Entry(document).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
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