using Documents_backend.Models;
using System.Collections.Generic;
using System.Web.Http;


namespace Documents_backend.Controllers
{
    public class DocumentDataController : ApiController
    {
        DataModel db = new DataModel();

        [HttpGet]
        public IEnumerable<Document> Get()
        {
            return db.Document;
        }

        [HttpGet]
        public Document Get(int id)
        {
            Document info = db.Document.Find(id);
            return info;
        }


        [HttpPost]
        public void Post([FromBody] Document info)
        {
            db.Document.Add(info);
        }


        [HttpPut]
        public void Put(int id, [FromBody] Document info)
        {
            if (id == info.Id)
            {
                db.Entry(info).State = System.Data.Entity.EntityState.Modified;
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