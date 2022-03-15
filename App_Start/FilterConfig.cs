using Documents_backend.Handlers;
using System.Web.Mvc;

namespace Documents_backend
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
