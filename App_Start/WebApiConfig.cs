using System.Linq;
using System.Web.Http;


namespace Documents_backend
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();

            config.MapHttpAttributeRoutes();
            

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml"));

            config.Routes.MapHttpRoute(
               name: "action",
               routeTemplate: "api/{controller}/{action}",
               defaults: new { action = "list" }
           );

            config.Routes.MapHttpRoute(
                name: "id-action",
                routeTemplate: "api/{controller}/{id}/{action}",
                defaults: new { id = RouteParameter.Optional, action = "list" }
            );

            config.Routes.MapHttpRoute(
                name: "id-action-id",
                routeTemplate: "api/{controller}/{id}/{action}/{childId}"
            );
        }
    }
}
