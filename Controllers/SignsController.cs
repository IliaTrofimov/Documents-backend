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
using Documents.Services;


namespace Documents.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class SignsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);
        Mailing mailing = new Mailing();


        [HttpGet]
        [ActionName("count")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Count(int documentId = -1, int userId = -1, int initiatorId = -1)
        {
            int count =  await db.Signs.CountAsync(sign => (documentId == -1 || sign.DocumentId == documentId) && (userId == -1 || sign.UserId == userId) && (initiatorId == -1 || sign.InitiatorId == initiatorId));
            return Ok(count);
        }

        [HttpGet]
        [ActionName("get")]
        [ResponseType(typeof(Sign))]
        public async Task<IHttpActionResult> Get(int id)
        {
            var sign = await db.Signs.Include("User").Include("Initiator").FirstOrDefaultAsync(s => s.Id == id);
            if (sign == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return Ok(sign);
        }

        [HttpGet]
        [ActionName("list")]
        [ResponseType(typeof(IEnumerable<Sign>))]
        public async Task<IHttpActionResult> List(int documentId = -1, int userId = -1, int page = 0, int pageSize = -1, int initiatorId = -1)
        {
            List<Sign> signs;
            if (pageSize != -1)
                signs = await db.Signs.Include("Initiator").Include("User")
                    .OrderBy(sign => sign.SignerPositionId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Where(sign => (documentId == -1 || sign.DocumentId == documentId) && (userId == -1 || sign.UserId == userId) && (initiatorId == -1 || sign.InitiatorId == initiatorId))
                    .ToListAsync();
            else
                signs = await db.Signs.Include("Initiator").Include("User")
                    .OrderBy(sign => sign.SignerPositionId)
                    .Where(sign => (documentId == -1 || sign.DocumentId == documentId) && (userId == -1 || sign.UserId == userId))
                    .ToListAsync();

            if (signs == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return Ok(signs);
        }


        [HttpPost]
        [ActionName("post")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Post([FromBody] Sign body)
        {
            if (db.Users.Find(body.UserId) == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot create signatory, signer not found");
            if (db.Documents.Find(body.DocumentId) == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot create signatory, document not found");

            Sign sign = db.Signs.Add(new Sign() 
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
            return Ok(sign.Id);
        }


        [HttpPut]
        [ActionName("put")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(int id, [FromBody] SignPOST body)
        {
            Sign sign = await db.Signs.FindAsync(id);
            if (sign == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update signatory, signatory not found");

            sign.Signed = body.Signed;
            sign.UpdateDate = DateTime.Now;
            sign.UserId = body.UserId;

            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [ActionName("notify")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Notify(int id)
        {
            Sign sign = await db.Signs.Include("User").Include("Initiator").FirstOrDefaultAsync(s => s.Id == id);
            if (sign == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update signatory, signatory not found");

            StartupInfo.Tries += 10;
            StartupInfo.Msg = "Trying: " + sign.User.Email;
            mailing.SignatoryNotification(sign);
            return Ok();
        }


        [HttpDelete]
        [ActionName("delete")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            Sign sign = await db.Signs.FindAsync(id);
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
