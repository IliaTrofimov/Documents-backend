using AutoMapper;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

using Documents.Models.DTO;
using Documents.Models.Entities;
using Documents.Models;


namespace Documents
{
    public static class StartupInfo
    {
        public static System.DateTime StartupTime;
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
                    .ForMember(dto => dto.TemplateName, act => act.MapFrom(doc => doc.Template.Name))
                    .ForMember(dto => dto.AuthorName, act => act.MapFrom(doc => doc.Author.GetFIO()));

                cfg.CreateMap<Document, DocumentDTOFull>();

                cfg.CreateMap<Template, TemplateDTO>()
                    .ForMember(dto => dto.TemplateType, act => act.MapFrom(temp => temp.TemplateType.Name))
                    .ForMember(dto => dto.AuthorName, act => act.MapFrom(temp => temp.Author.GetFIO()));

                cfg.CreateMap<TemplateType, TemplateTypeDTO>();

                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<User, UserDTOFull>();

                cfg.CreateMap<Sign, SignDTO>()
                    .ForMember(dto => dto.InitiatorShortname, act => act.MapFrom(sign => sign.Initiator != null ? sign.Initiator.GetFIO() : "не назначен"))
                    .ForMember(dto => dto.SignerShortname, act => act.MapFrom(sign => sign.User != null ? sign.User.GetFIO() : "не назначен"))
                    .ForMember(dto => dto.DocumentName, act => act.MapFrom(sign => sign.Document.Name))
                    .ForMember(dto => dto.PositionName, act => act.MapFrom(sign => sign.SignerPosition.Name));

            });
            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();


            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Migrations.Configuration>());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        /// <summary>
        /// Добавляет заголовки для CORS, можно удалить
        /// </summary>
        void Application_BeginRequest(object sender, System.EventArgs e)
        {
            var context = HttpContext.Current;
            var response = context.Response;

            if (context.Request.HttpMethod == "OPTIONS")
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
