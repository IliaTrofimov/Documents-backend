using Documents_backend.Models;
using System.Collections.Generic;
using System.Web.Http;


namespace Documents_backend.Controllers
{
    public class TemplateTypesController : ApiController
    {
        DataContext db = new DataContext();

        [HttpGet]
        public IEnumerable<TemplateType> Get()
        {
            return db.TemplateType;
        }

        [HttpGet]
        public TemplateType Get(int id)
        {
            TemplateType type = db.TemplateType.Find(id);
            return type;
        }


        [HttpPost]
        public void Post([FromBody] TemplateType type)
        {
            db.TemplateType.Add(type);
        }


        [HttpPut]
        public void Put(int id, [FromBody] TemplateType type)
        {
            if (id == type.Id)
            {
                db.Entry(type).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else BadRequest();
        }

        [HttpDelete]
        public void Delete(int id)
        {
            TemplateType type = db.TemplateType.Find(id);
            if (type != null)
            {
                db.TemplateType.Remove(type);
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
