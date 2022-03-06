using AutoMapper;
using Documents_backend.Models;
using System.Web.Http;
using System.Web.Mvc;


namespace Documents_backend
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        static public MapperConfiguration mapperConfig;

        protected void Application_Start()
        {
            mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Document, DocumentDTO>();
                cfg.CreateMap<DocumentDTO, Document>();

                cfg.CreateMap<DocumentDataItem, DocumentDataItemDTO>();
                cfg.CreateMap<DocumentTableCell, DocumentTableCellDTO>();

                cfg.CreateMap<Template, TemplateDTO>();
                cfg.CreateMap<TemplateField, TemplateFieldDTO>();
                cfg.CreateMap<TemplateType, TemplateTypeDTO>();

                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Sign, SignDTO>();

            });
            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
