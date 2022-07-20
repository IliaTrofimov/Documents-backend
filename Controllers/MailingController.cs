using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Data.Entity;
using System;

using Documents.Models;
using Documents.Services.Mailing;
using Documents.Utility;


namespace Documents.Controllers
{
    internal class MailingResult
    {
        public int ExpectedMessages { get; set; } = -1;
        public int SentMessages { get; set; } = -1;
        public string RaisedException { get; set; } = null;
        public double TimeSpent { get; set; } = -1;
    }


    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class MailingController : ApiController
    {
        DataContext db = new DataContext();
        MailingClient mailingClient = new MailingClient();

        [HttpGet]
        [ActionName("expiration-alert")]
        [ResponseType(typeof(MailingResult))]
        public async Task<IHttpActionResult> SendDocumentsExpirationAlerts()
        {
            MailingResult result = new MailingResult();
            DateTime now = DateTime.Now;

            try
            {
                var documents = db.Documents.Include("Author")
                    .Where(d => d.ExpireDate.HasValue && (
                        DbFunctions.DiffDays(d.ExpireDate.Value, now) <= 1 ||
                        DbFunctions.DiffDays(d.ExpireDate.Value, now) == 3 ||
                        DbFunctions.DiffDays(d.ExpireDate.Value, now) == 7 ||
                        DbFunctions.DiffDays(d.ExpireDate.Value, now) == 14 ||
                        DbFunctions.DiffDays(d.ExpireDate.Value, now) == 30
                        )
                    );

                result.ExpectedMessages = await documents.CountAsync();
                result.SentMessages = await mailingClient.ExpireNotificationAsync(documents);
            }
            catch (Exception e)
            {
                result.RaisedException = e.Message;
                this.ThrowResponseException(System.Net.HttpStatusCode.InternalServerError, e.Message);
            }

            result.TimeSpent = (DateTime.Now - now).TotalSeconds;
            return Ok(result);
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