using Documents_backend.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using AutoMapper;

namespace Documents_backend.Controllers
{
    public class SignsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);


        [HttpGet]   
        public IEnumerable<SignDTO> Get(int? documentId, int? userId)
        {
            if (documentId == null && userId == null)
                BadRequest();
            else if (documentId == null)
                return mapper.Map<IEnumerable<SignDTO>>(from sign in db.Sign where sign.UserId == userId select sign);
            else if (userId == null)
                return mapper.Map<IEnumerable<SignDTO>>(from sign in db.Sign where sign.DocumentId == documentId select sign);


            return mapper.Map<IEnumerable<SignDTO>>(db.Sign.Find(documentId, userId));
        }


        [HttpPost]
        public void Post([FromBody] SignDTO sign)
        {
           db.Sign.Add(mapper.Map<Sign>(sign));
        }


        [HttpPut]
        public void Put([FromBody] SignDTO sign)
        {
            db.Entry(mapper.Map<Sign>(sign)).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [HttpDelete]
        public void Delete(int id)
        {
            Sign sign = db.Sign.Find(id);
            if (sign != null)
            {
                db.Sign.Remove(sign);
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
