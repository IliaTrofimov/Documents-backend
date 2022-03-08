using Documents_backend.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using AutoMapper;
using System.Net;

namespace Documents_backend.Controllers
{
    public class SignsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);


        [HttpGet]
        public IEnumerable<SignDTO> Get(int? documentId = null, int? userId = null)
        {
            if (documentId == null && userId == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            if (documentId != null && userId != null)
            {
                var sign = db.Sign.Find(documentId, userId);
                if (sign == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                return mapper.Map<IEnumerable<SignDTO>>(new Sign[]{ sign });
            }
            else
            {
                var signs = from sign in db.Sign
                            where userId != null && sign.UserId == userId || documentId != null && sign.DocumentId == documentId
                            select sign;
                if (signs == null)
                    throw new HttpResponseException(HttpStatusCode.NoContent);
                return mapper.Map<IEnumerable<SignDTO>>(signs);
            }
        }


        [HttpPost]
        public void Post([FromBody] int userId, [FromBody] int documentId)
        {
           db.Sign.Add(new Sign() { UserId = userId, DocumentId = documentId });
        }


        [HttpPut]
        public void Put([FromBody] int userId, [FromBody] int documentId, [FromBody] bool signed = false)
        {
            Sign sign = db.Sign.Find(documentId, userId);
            if (sign == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            sign.Signed = signed;
            db.Entry(sign).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [HttpDelete]
        public void Delete([FromBody] int userId, [FromBody] int documentId)
        {
            Sign sign = db.Sign.Find(documentId, userId);
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
