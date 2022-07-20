using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Documents.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class DefaultController : ApiController
    {
        [HttpGet]
        [ActionName("list")]
        public Dictionary<string, string> BuildInfo()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("StartupTime", StartupInfo.StartupTime.ToString("H:mm:ss - dd MMM. yyy"));
            result.Add("RequestTime", DateTime.Now.ToString("H:mm:ss - dd MMM. yyyy"));

            return result;
        }

        [HttpGet]
        [ActionName("test")]
        public Dictionary<string, string> Test()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Models.DataContext db = new Models.DataContext();
            
            try
            {
                db.Users.FirstOrDefault();
                result.Add("Users", "ok");
            }
            catch (Exception e) { result.Add("Users", e.Message); }

            try
            {
                db.Documents.FirstOrDefault();
                result.Add("Documents", "ok");
            }
            catch (Exception e) { result.Add("Documents", e.Message); }

            try
            {
                db.DocumentDataItems.FirstOrDefault();
                result.Add("DocumentDataItems", "ok");
            }
            catch (Exception e) { result.Add("DocumentDataItems", e.Message); }

            try
            {
                db.Templates.FirstOrDefault();
                result.Add("Templates", "ok");
            }
            catch (Exception e) { result.Add("Templates", e.Message); }

            try
            {
                db.TemplateFields.FirstOrDefault();
                result.Add("TemplateFields", "ok");
            }
            catch (Exception e) { result.Add("TemplateFields", e.Message); }

            try
            {
                db.TemplateTables.FirstOrDefault();
                result.Add("TemplateTables", "ok");
            }
            catch (Exception e) { result.Add("TemplateTables", e.Message); }

            try
            {
                db.TemplateTypes.FirstOrDefault();
                result.Add("TemplateTypes", "ok");
            }
            catch (Exception e) { result.Add("TemplateTypes", e.Message); }

            try
            {
                db.Positions.FirstOrDefault();
                result.Add("Positions", "ok");
            }
            catch (Exception e) { result.Add("Positions", e.Message); }

            try
            {
                db.Signs.FirstOrDefault();
                result.Add("Signs", "ok");
            }
            catch (Exception e) { result.Add("Signs", e.Message); }
            return result;
        }

        [HttpGet]
        [ActionName("user")]
        public System.Security.Principal.IPrincipal GetUser()
        {
            return RequestContext.Principal;
        }

    }
}
