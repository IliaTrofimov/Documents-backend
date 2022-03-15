using AutoMapper;
using Documents_backend.Models;
using System.Web.Http;
using System.Net;
using System.Web.Http.Cors;

namespace Documents_backend.Controllers
{
    [EnableCors("http://localhost:4200", "*", "*")]
    public class WhoAmIController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        public UserDTO Get()
        {
            var users = db.Users.Find(0);
            if (users == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return mapper.Map<UserDTO>(users);
        }      
    }
}
