﻿using System;
using System.Collections.Generic;
using System.IO;
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
            result.Add("Message", "Server is running");
            result.Add("StartupTime", StartupInfo.StartupTime.ToString("H:mm:ss - dd MMM. yyy"));
            result.Add("Counter", StartupInfo.Counter.ToString());
            result.Add("RequestTime", DateTime.Now.ToString("H:mm:ss - dd MMM. yyyy"));
            result.Add("Tries", StartupInfo.Tries.ToString());
            result.Add("Msg", StartupInfo.Msg);

            try
            {
                result.Add("BuildDate", DateTime.Parse(
                    File.ReadLines(@"C:\Users\iliat\Source\Repos\IliaTrofimov\Documents-backend\bin\BuildInfo.txt")
                   .Last()
                   .Substring(6).TrimEnd()
                ).ToString("H:mm:ss - dd MMM. yyyy"));
            }
            catch (Exception e)
            {
                result.Add("BuildDate", "unknown");
                result["Message"] += $". Cannot get build time ({e.Message})";
            }
            return result;
        }

        [HttpGet]
        [ActionName("test")]
        public Dictionary<string, string> Test()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Models.DataContext db = new Models.DataContext();
            result.Add("BuildDate", BuildInfo()["BuildDate"]);
            
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
            return User;
        }
    }
}
