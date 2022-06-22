using System.Web.Http;
using System.Web.Http.Cors;

namespace Documents.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class DefaultController : ApiController
    {
        [HttpGet]
        [ActionName("index")]
        public string Index()
        {
            return "Done";
        }
    }
}
