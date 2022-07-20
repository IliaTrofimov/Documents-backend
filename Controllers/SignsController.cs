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
using Documents.Models.Entities;
using Documents.Models.POST;
using Documents.Models.DTO;
using Documents.Models;
using Documents.Services.Mailing;


namespace Documents.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class SignsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);
        MailingClient mailing = new MailingClient();


        [HttpGet]
        [ActionName("count")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Count(int documentId = -1,
                                                   string userId = null,
                                                   string initiatorId = null,
                                                   bool showOld = true,
                                                   bool showUnassigned = true)
        {
            int count =  await db.Signs.CountAsync(sign => (documentId == -1 || sign.DocumentId == documentId) &&
                (userId == null || sign.UserCWID == userId) &&
                (initiatorId == null || sign.InitiatorCWID == initiatorId) && 
                (showUnassigned || sign.UserCWID != null) &&
                (showOld || !sign.Signed.HasValue));
            return Ok(count);
        }

        [HttpGet]
        [ActionName("get")]
        [ResponseType(typeof(SignDTO))]
        public async Task<IHttpActionResult> Get(int id)
        {
            var sign = await db.Signs.FirstOrDefaultAsync(s => s.Id == id);
            if (sign == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return Ok(mapper.Map<SignDTO>(sign));
        }

        [HttpGet]
        [ActionName("list")]
        [ResponseType(typeof(IEnumerable<SignDTO>))]
        public async Task<IHttpActionResult> List(int documentId = -1,
                                                  string userId = null,
                                                  string initiatorId = null,
                                                  int page = 0,
                                                  int pageSize = -1,
                                                  bool showOld = true,
                                                  bool showUnassigned = true)
        {
            // Странным образом используется жадная загрузка связанных сущностей без Include
            // вместе с Include ничего не может найти в таблице
            List<Sign> signs;
            if (pageSize != -1)
                signs = await db.Signs
                    .OrderBy(sign => sign.SignerPositionId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Where(sign => (documentId == -1 || sign.DocumentId == documentId) && 
                        (userId == null || sign.UserCWID == userId) && 
                        (initiatorId == null || sign.InitiatorCWID == initiatorId) &&
                        (showUnassigned || showUnassigned && sign.UserCWID != null) &&
                        (showOld || !sign.Signed.HasValue))
                    //.Include("User").Include("Initiator").Include("SignerPosition")
                    .ToListAsync();
            else
                signs = await db.Signs
                    .OrderBy(sign => sign.SignerPositionId)
                    .Where(sign => (documentId == -1 || sign.DocumentId == documentId) &&
                        (userId == null || sign.UserCWID == userId) &&
                        (initiatorId == null || sign.InitiatorCWID == initiatorId) &&
                        (showUnassigned || sign.UserCWID != null) &&
                        (showOld || !sign.Signed.HasValue))
                    //.Include("User").Include("Initiator").Include("SignerPosition")
                    .ToListAsync();

            if (signs == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);

            return Ok(mapper.Map<IEnumerable<SignDTO>>(signs));
        }


        [HttpPost]
        [ActionName("post")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Post([FromBody] Sign body)
        {
            if (db.Users.Find(body.UserCWID) == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot create signatory, signer not found");
            if (db.Documents.Find(body.DocumentId) == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot create signatory, document not found");

            Sign sign = db.Signs.Add(new Sign() 
            { 
                UserCWID = body.UserCWID, 
                DocumentId = body.DocumentId, 
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                SignerPositionId = body.SignerPositionId,
                Signed = null,
                InitiatorCWID = body.InitiatorCWID
            });
            await db.SaveChangesAsync();
            return Ok(sign.Id);
        }


        [HttpPut]
        [ActionName("put")]
        [ResponseType(typeof(SignDTO))]
        public async Task<IHttpActionResult> Put(int id, [FromBody] SignPOST body)
        {
            Sign sign = await db.Signs.FindAsync(id);
            if (sign == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update signatory, signatory not found");

            Document document = await db.Documents.FindAsync(body.DocumentId);
            if (document == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update signatory, document not found");

            sign.Signed = body.Signed;
            sign.UpdateDate = DateTime.Now;
            sign.UserCWID = body.UserCWID;

            if (sign.Signed != null && sign.UserCWID != null)
                await Notify(id);

            if (document.Signs.All(s => s.Signed == true))
                document.Type = DocumentStatus.InUse;

            await db.SaveChangesAsync();
            return Ok(mapper.Map<SignDTO>(sign));
        }

        [HttpPut]
        [ActionName("notify")]
        [ResponseType(typeof(Dictionary<string, string>))]
        public async Task<IHttpActionResult> Notify(int id)
        {
            Sign sign = await db.Signs.Include("User").Include("Initiator").FirstOrDefaultAsync(s => s.Id == id);
            if (sign == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update signatory, signatory not found");

            await mailing.SignatoryNotificationAsync(sign);
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
