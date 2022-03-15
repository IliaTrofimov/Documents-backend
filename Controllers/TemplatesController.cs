using AutoMapper;
using Documents_backend.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using System.Linq;
using System.Web.Http.Cors;
using Documents_backend.Utility.Helpers;

namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class TemplatesController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        public IEnumerable<TemplateDTO> Get()
        {
            var templates = db.Templates;
            if (templates == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return mapper.Map<IEnumerable<TemplateDTO>>(templates.ToList());
        }

        [HttpGet]
        public Template Get(int id)
        {
            Template template = db.Templates.Find(id);
            if (template == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            template.TemplateField = template.TemplateField.OrderBy(f => f, new TemplateFieldComparer()).ToList();
            return template;
        }


        [HttpPost]
        public int Post([FromBody] int authorId)
        {
            if (authorId == -1 || db.Users.Find(authorId) == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            return db.Templates.Add(new Template() { AuthorId = authorId }).Id;
        }


        [HttpPut]
        public void Put([FromBody] Template template)
        {
            db.Entry(template).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [HttpDelete]
        public void Delete(int id)
        {
            Template template = db.Templates.Find(id);
            if (template != null)
            {
                db.Templates.Remove(template);
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
