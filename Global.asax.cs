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

                cfg.CreateMap<DocumentDataItem, DocumentDataItemDTO>();
                cfg.CreateMap<DocumentTableCell, DocumentTableCellDTO>();

                cfg.CreateMap<Template, TemplateDTO>()
                    .ForMember(dto => dto.TemplateType, act => act.MapFrom(temp => temp.TemplateType.Name));
                cfg.CreateMap<TemplateField, TemplateFieldDTO>();
                cfg.CreateMap<TemplateType, TemplateTypeDTO>();

                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Sign, SignDTO>()
                    .ForMember(dto => dto.UserName, act => act.MapFrom(sign => $"{sign.User.Lastname} {sign.User.Firstname[0]}."))
                    .ForMember(dto => dto.DocumentName, act => act.MapFrom(sign => sign.Document.Name));

            });
            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
