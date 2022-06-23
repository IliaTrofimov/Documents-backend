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
        public string List()
        {
            string response = "Server is running. Last build time is ";
            try
            {
                response += File.ReadLines(@"C:\Users\iliat\Source\Repos\IliaTrofimov\Documents-backend\bin\BuildInfo.txt")
                                  .Last().Substring(6);
            }
            catch (System.Exception ex)
            {
                response += $"unknown (error: {ex.Message})";
            }           
            return response;
        }
    }
}
