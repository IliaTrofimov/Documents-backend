using AutoMapper;
using System;
using System.Net;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;

using Documents_backend.Utility;
using Documents_Entities.DTO;
using Documents_Entities.Entities;
using Documents_Entities.POST;

namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class SignsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        [ActionName("count")]
        public int Count(int documentId = -1, int userId = -1)
        {
            return db.Signs.Count(sign => (documentId == -1 || sign.DocumentId == documentId) && (userId == -1 || sign.UserId == userId));
        }

        [HttpGet]
        [ActionName("get")]
        public IEnumerable<SignDTO> Get(int documentId = -1, int userId = -1, int page = 0, int pageSize = -1)
        {
            if (documentId == -1 && userId == -1)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Cannot find signatory, document or user were not specified");
            if (documentId != -1 && userId != -1)
            {
                var sign = db.Signs.FirstOrDefault(s => s.DocumentId == documentId && s.UserId == userId);
                if (sign == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                return mapper.Map<IEnumerable<SignDTO>>(new Sign[]{ sign });
            }
            else
            {
                IQueryable<Sign> signs;
                if (pageSize != -1)
                    signs = db.Signs.OrderBy(sign => sign.SignerPositionId)
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .Where(sign => (documentId == -1 || sign.DocumentId == documentId) && (userId == -1 || sign.UserId == userId));
                else
                    signs = db.Signs.OrderBy(sign => sign.SignerPositionId)
                        .Where(sign => (documentId == -1 || sign.DocumentId == documentId) && (userId == -1 || sign.UserId == userId));
               
                if (signs == null)
                    throw new HttpResponseException(HttpStatusCode.NoContent);
                return mapper.Map<IEnumerable<SignDTO>>(signs);
            }
        }


        [HttpPost]
        [ActionName("post")]
        public void Post([FromBody] Sign body)
        {
            if (db.Users.Find(body.UserId) == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot create signatory, signer not found");
            if (db.Documents.Find(body.DocumentId) == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot create signatory, document not found");

            db.Signs.Add(new Sign() 
            { 
                UserId = body.UserId, 
                DocumentId = body.DocumentId, 
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                SignerPositionId = body.SignerPositionId,
                Signed = null,
                InitiatorId = body.InitiatorId
            });
            db.SaveChanges();
        }


        [HttpPut]
        [ActionName("put")]
        public void Put([FromBody] SignPOST body)
        {
            Sign sign = db.Signs.Find(body.DocumentId, body.UserId);
            if (sign == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update signatory, signatory not found");

            Documents_notifications.Mailing.SignatoryNotification(sign);
            sign.Signed = body.Signed;
            sign.UpdateDate = DateTime.Now;
            db.SaveChanges();
        }

        [HttpDelete]
        [ActionName("delete")]
        public void Delete([FromBody] SignPOST body)
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
