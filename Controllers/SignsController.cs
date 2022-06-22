using AutoMapper;
using System;
using System.Net;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Data.Entity;

using Documents.Utility;
using Documents.Models.DTO;
using Documents.Models.Entities;
using Documents.Models.POST;
using Documents.Models;

namespace Documents.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class SignsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        [ActionName("count")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Count(int documentId = -1, int userId = -1)
        {
            int count =  await db.Signs.CountAsync(sign => (documentId == -1 || sign.DocumentId == documentId) && (userId == -1 || sign.UserId == userId));
            return Ok(count);
        }

        [HttpGet]
        [ActionName("get")]
        [ResponseType(typeof(IEnumerable<SignDTO>))]
        public async Task<IHttpActionResult> Get(int documentId = -1, int userId = -1, int page = 0, int pageSize = -1)
        {
            if (documentId == -1 && userId == -1)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Cannot find signatory, document or user were not specified");
            if (documentId != -1 && userId != -1)
            {
                var sign = db.Signs.FirstOrDefault(s => s.DocumentId == documentId && s.UserId == userId);
                if (sign == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                return Ok(mapper.Map<IEnumerable<SignDTO>>(new Sign[]{ sign }));
            }
            else
            {
                List<Sign> signs;
                if (pageSize != -1)
                    signs = await db.Signs.OrderBy(sign => sign.SignerPositionId)
                        .Skip(page * pageSize)
                        .Take(pageSize)
                        .Where(sign => (documentId == -1 || sign.DocumentId == documentId) && (userId == -1 || sign.UserId == userId))
                        .ToListAsync();
                else
                    signs = await db.Signs.OrderBy(sign => sign.SignerPositionId)
                        .Where(sign => (documentId == -1 || sign.DocumentId == documentId) && (userId == -1 || sign.UserId == userId))
                        .ToListAsync();
                        
                if (signs == null)
                    throw new HttpResponseException(HttpStatusCode.NoContent);
                return Ok(mapper.Map<IEnumerable<SignDTO>>(signs));
            }
        }


        [HttpPost]
        [ActionName("post")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Post([FromBody] Sign body)
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
            await db.SaveChangesAsync();
            return Ok();
        }


        [HttpPut]
        [ActionName("put")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put([FromBody] SignPOST body)
        {
            Sign sign = db.Signs.Find(body.DocumentId, body.UserId);
            if (sign == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update signatory, signatory not found");

            Services.Mailing.SignatoryNotification(sign);
            sign.Signed = body.Signed;
            sign.UpdateDate = DateTime.Now;
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [ActionName("delete")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete([FromBody] SignPOST body)
        {
            Sign sign = db.Signs.Find(body.DocumentId, body.UserId);
            if (sign != null)
            {
                db.Signs.Remove(sign);
                await db.SaveChangesAsync();
                return Ok();
            }
            else return NotFound();
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
