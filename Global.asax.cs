using AutoMapper;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

using Documents_Entities.DTO;
using Documents_Entities.Entities;


namespace Documents_backend
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        static public MapperConfiguration mapperConfig;

        protected void Application_Start()
        { 
            mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Document, DocumentDTO>()
                    .ForMember(dto => dto.TemplateName, act => act.MapFrom(doc => doc.Template.Name)); ;

                cfg.CreateMap<DocumentDataItem, DocumentDataItemDTO>();

                cfg.CreateMap<Template, TemplateDTO>()
                    .ForMember(dto => dto.TemplateType, act => act.MapFrom(temp => temp.TemplateType.Name));
                cfg.CreateMap<TemplateType, TemplateTypeDTO>();

                cfg.CreateMap<User, UserDTORich>();
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
        }

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
