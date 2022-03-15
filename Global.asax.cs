using AutoMapper;
using Documents_backend.Handlers;
using Documents_backend.Models;
using Documents_backend.Models.Administrative;
using System.Data.Entity;
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

                cfg.CreateMap<Template, TemplateDTO>()
                    .ForMember(dto => dto.TemplateType, act => act.MapFrom(temp => temp.TemplateType.Name));
                cfg.CreateMap<TemplateType, TemplateTypeDTO>();

                cfg.CreateMap<User, UserDTO>()
                    .ForMember(dto => dto.PermissionsString, act => act.MapFrom(user => UserPermission.PermissionString(user.Permissions)));
                cfg.CreateMap<User, UserDTORich>()
                   .ForMember(dto => dto.PermissionsString, act => act.MapFrom(user => UserPermission.PermissionString(user.Permissions)));
                cfg.CreateMap<Sign, SignDTO>()
                    .ForMember(dto => dto.UserName, act => act.MapFrom(sign => $"{sign.User.Lastname} {sign.User.Firstname[0]}."))
                    .ForMember(dto => dto.DocumentName, act => act.MapFrom(sign => sign.Document.Name));

            });
            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();


            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Migrations.Configuration>());

            GlobalConfiguration.Configuration.MessageHandlers.Add(new MessageLoggingHandler());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
