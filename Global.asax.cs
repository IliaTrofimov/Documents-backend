using AutoMapper;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

using Documents.Models.DTO;
using Documents.Models.Entities;
using Documents.Models;
using Documents.Properties;
using Documents.Services.MailingSchedule;

namespace Documents
{
    public static class StartupInfo
    {
        public static System.DateTime StartupTime;
        public static int Counter = 0;
        public static string Msg;
        public static int Tries = 0;
    }

    public class WebApiApplication : HttpApplication
    {
        static public MapperConfiguration mapperConfig;

        protected void Application_Start()
        {
            StartupInfo.StartupTime = System.DateTime.Now;

            mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Document, DocumentDTO>()
                    .ForMember(dto => dto.TemplateName, act => act.MapFrom(doc => doc.Template.Name)); ;

                cfg.CreateMap<DocumentDataItem, DocumentDataItemDTO>();

                cfg.CreateMap<Template, TemplateDTO>()
                    .ForMember(dto => dto.TemplateType, act => act.MapFrom(temp => temp.TemplateType.Name));
                cfg.CreateMap<TemplateType, TemplateTypeDTO>();
                cfg.CreateMap<User, UserDTO>();

                cfg.CreateMap<Sign, SignDTO>()
                    .ForMember(dto => dto.Firstname, act => act.MapFrom(sign =>sign.User.Firstname))
                    .ForMember(dto => dto.Lastname, act => act.MapFrom(sign => sign.User.Lastname))
                    .ForMember(dto => dto.Fathersname, act => act.MapFrom(sign => sign.User.Fathersname))
                    .ForMember(dto => dto.DocumentName, act => act.MapFrom(sign => sign.Document.Name));
                   
            });
            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();


            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Migrations.Configuration>());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            Services.Mailing mailing = new Services.Mailing(Settings.Default.EmailFrom,
                Settings.Default.EmailLogin, 
                Settings.Default.EmailPassword,
                Settings.Default.EmailHost);
            EmailScheduler.Start();
        }

        void Application_BeginRequest(object sender, System.EventArgs e)
        {
            var context = HttpContext.Current;
            var response = context.Response;

            if (false && context.Request.HttpMethod == "OPTIONS")
            {
                response.AddHeader("Access-Control-Allow-Origin", "http://localhost:4200");
                response.AddHeader("X-Frame-Options", "ALLOW-FROM *");
                response.AddHeader("Access-Control-Allow-Credentials", "true");
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST, DELETE, PUT");
                response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                response.AddHeader("Access-Control-Max-Age", "1728000");
                response.End();
            }
        }
    }
}
