using AutoMapper;
using Documents_backend.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using System.Web.Http.Cors;

namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class UsersController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        [ActionName("whoami")]
        public UserDTO WhoAmI()
        {
            var users = db.Users.Find(0);
            if (users == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return mapper.Map<UserDTO>(users);
        }

        [HttpGet]
        [ActionName("list")]
        public IEnumerable<UserDTO> Get()
        {
            var users = db.Users;
            if (users == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return mapper.Map<IEnumerable<UserDTO>>(users);
        }

        [HttpGet]
        [ActionName("get")]
        public UserDTORich Get(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return mapper.Map<UserDTORich>(user);
        }


        [HttpPost]
        [ActionName("post")]
        public int Post([FromBody] UserDTO user)
        {
            return db.Users.Add(new User() 
            { 
                Firstname = user.Firstname, 
                Lastname = user.Lastname, 
                Fathersname = user.Fathersname
            }).Id;
        }


        [HttpPut]
        [ActionName("put")]
        public void Put([FromBody] UserDTO user)
        {
            db.Entry(mapper.Map<User>(user)).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [HttpDelete]
        [ActionName("delete")]
        public void Delete(int id)
        {
            User user = db.Users.Find(id);
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }   
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
