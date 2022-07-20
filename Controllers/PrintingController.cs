using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.IO;
using System.Text;

using Documents.Utility;
using Documents.Models;
using Documents.Services.Printing;
using Documents.Models.Entities;

namespace Documents.Controllers
{

    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST", SupportsCredentials = true)]
    public class PrintingController : ApiController
    {
        DataContext db = new DataContext();


        [ResponseType(typeof(string))]
        [ActionName("add-template")]
        [HttpPost]
        public async Task<IHttpActionResult> AddTemplate(int id)
        {
            if (!Request.Content.IsMimeMultipartContent())
                return this.HttpResponse(HttpStatusCode.UnsupportedMediaType);

            Template template = db.Templates.Find(id);
            if (template == null)
                return this.HttpResponse(HttpStatusCode.NotFound, "Template was not found");

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            if (template.HtmlTemplateId != null)
            {
                template.HtmlTemplate.Data = await provider.Contents[0].ReadAsByteArrayAsync();
                await db.SaveChangesAsync();
                return Ok("Updated");
            }
            else
            {
                var html = db.HtmlTemplates.Add(new HtmlTemplate() { Data = await provider.Contents[0].ReadAsByteArrayAsync() });
                template.HtmlTemplate = html;
                await db.SaveChangesAsync();
                return Ok("Created");
            }
        }


        [ActionName("get-template")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetTemplate(int id, bool download = false)
        {
            Template template = await db.Templates.FindAsync(id);
            if (template == null || template.HtmlTemplateId == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var result = Request.CreateResponse(HttpStatusCode.OK);

            if (!download)
            {
                result.Content = new StringContent(Encoding.UTF8.GetString(template.HtmlTemplate.Data));
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return result;
            }
            else
            {
                result.Content = new StreamContent(new MemoryStream(template.HtmlTemplate.Data));
                result.Content.Headers.ContentLength = template.HtmlTemplate.Data.Length;
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

                if (ContentDispositionHeaderValue.TryParse($"attachment; filename=\"{template.Name.Replace(" ", "_")}.html\"", out ContentDispositionHeaderValue contentDisposition))
                    result.Content.Headers.ContentDisposition = contentDisposition;

                return result;
            }

        }


        [ResponseType(typeof(bool))]
        [ActionName("check-existance")]
        [HttpGet]
        public async Task<IHttpActionResult> CheckExistance(int id)
        {
            Template template = await db.Templates.FindAsync(id);
            if (template == null || template.HtmlTemplateId == null)
                return Ok(false);
            else
                return Ok(true);
        }

        [ActionName("document-html")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetHtml(int id)
        {
            Document document = await db.Documents.FindAsync(id);
            if (document == null || document.Template.HtmlTemplateId == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var result = new HttpResponseMessage();
            result.StatusCode = HttpStatusCode.OK;
            result.Content = new StringContent(DocumentPrinter.GetHtml(document, document.Template).ToString());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            if (ContentDispositionHeaderValue.TryParse($"attachment; filename=\"{document.Name.Replace(" ", "_")}.html\"", out ContentDispositionHeaderValue contentDisposition))
                result.Content.Headers.ContentDisposition = contentDisposition;

            return result;
        }


        [ActionName("document-pdf")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPdf(int id, string pageFormat = "A4")
        {
            Document document = await db.Documents.FindAsync(id);
            if (document == null || document.Template.HtmlTemplateId == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            byte[] buffer = DocumentPrinter.GetPdfBytes(document, document.Template, pageFormat);

            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(new MemoryStream(buffer));
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            result.Content.Headers.ContentLength = buffer.Length;

            if (ContentDispositionHeaderValue.TryParse($"attachment; filename=\"{document.Name.Replace(" ", "_")}.pdf\"", out ContentDispositionHeaderValue contentDisposition))
                result.Content.Headers.ContentDisposition = contentDisposition;
  
            return result;
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
