using AutoMapper;
using Documents_backend.Models;
using System.Collections.Generic;
using System.Web.Http;


namespace Documents_backend.Controllers
{
    public class TemplatesController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        public IEnumerable<TemplateDTO> Get()
        {
            return mapper.Map<IEnumerable<TemplateDTO>>(db.Template);
        }

        [HttpGet]
        public Template Get(int id)
        {
            Template template = db.Template.Find(id);
            if (template == null)
                StatusCode(System.Net.HttpStatusCode.NotFound);
            return template;
        }


        [HttpPost]
        public void Post([FromBody] int authorId)
        {
            db.Template.Add(new Template() { AuthorId = authorId });
        }


        [HttpPut]
        public void Put(int id, [FromBody] Template template)
        {
            if (id == template.Id)
            {
                db.Entry(template).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else BadRequest();
        }

        [HttpDelete]
        public void Delete(int id)
        {
            Template template = db.Template.Find(id);
            if (template != null)
            {
                db.Template.Remove(template);
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
