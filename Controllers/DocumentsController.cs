using AutoMapper;
using Documents_backend.Models;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Linq;
using System.Web.Http.Cors;
using System.Net.Http;
using Documents_backend.Utility.Helpers;

namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class DocumentsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var documents = db.Documents;
            if (documents == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return Request.CreateResponse(mapper.Map<IEnumerable<DocumentDTO>>(documents.ToList()));
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            Document document = db.Documents.Find(id);
            if (document == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            document.DocumentDataItems = (from c in document.DocumentDataItems
                                          orderby c.Field, c.Row, c.Col
                                          select c).ToList();
            document.Template.TemplateField = document.Template.TemplateField.OrderBy(f => f, new TemplateFieldComparer()).ToList();
            return Request.CreateResponse(document);
        }   


        [HttpPost]
        public HttpResponseMessage Post([FromBody] int templateId, [FromBody] int authorId)
        {
            if (authorId == -1 || db.Users.Find(authorId) == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            return Request.CreateResponse(db.Documents.Add(new Document() { TemplateId = templateId, AuthorId = authorId }).Id);
        }


        [HttpPut]
        public HttpResponseMessage Put([FromBody] Document document)
        {
            db.Entry(document).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        [HttpDelete]
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