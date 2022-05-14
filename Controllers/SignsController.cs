using AutoMapper;
using System.Net;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;

using Documents_backend.Utility;
using Documents_backend.Models;


namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class SignsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);


        [HttpGet]
        [ActionName("get")]
        public IEnumerable<SignDTO> Get(int? documentId = null, int? userId = null)
        {
            if (documentId == null && userId == null)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Cannot find signatory, document or user were not specified");
            if (documentId != null && userId != null)
            {
                var sign = db.Signs.Find(documentId, userId);
                if (sign == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                return mapper.Map<IEnumerable<SignDTO>>(new Sign[]{ sign });
            }
            else
            {
                var signs = from sign in db.Signs
                            where userId != null && sign.UserId == userId || documentId != null && sign.DocumentId == documentId
                            select sign;
                if (signs == null)
                    throw new HttpResponseException(HttpStatusCode.NoContent);
                return mapper.Map<IEnumerable<SignDTO>>(signs);
            }
        }


        [HttpPost]
        [ActionName("post")]
        public void Post([FromBody] Models.POST.SignPOST body)
        {
            if (db.Users.Find(body.UserId) == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot create signatory, signer not found");
            if (db.Documents.Find(body.DocumentId) == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot create signatory, document not found");

            db.Signs.Add(new Sign() { UserId = body.UserId, DocumentId = body.DocumentId, CreateDate = System.DateTime.Now });
            db.SaveChanges();
        }


        [HttpPut]
        [ActionName("put")]
        public void Put([FromBody] Models.POST.SignPOST body)
        {
            Sign sign = db.Signs.Find(body.DocumentId, body.UserId);
            if (sign == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update signatory, signatory not found");

            sign.Signed = body.Signed;
            sign.UpdateDate = System.DateTime.Now;
            db.SaveChanges();
        }

        [HttpDelete]
        [ActionName("delete")]
        public void Delete([FromBody] Models.POST.SignPOST body)
        {
            Sign sign = db.Signs.Find(body.DocumentId, body.UserId);
            if (sign != null)
            {
                db.Signs.Remove(sign);
                db.SaveChanges();
            }
            else this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot delete signatory, signatory not found");
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
