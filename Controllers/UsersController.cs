using AutoMapper;
using Documents_backend.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System.Net;

namespace Documents_backend.Controllers
{
    public class UsersController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        public IEnumerable<UserDTO> Get()
        {
            var users = db.User;
            if (users == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return mapper.Map<IEnumerable<UserDTO>>(users);
        }

        [HttpGet]
        public User Get(int id)
        {
            User user = db.User.Find(id);
            if (user == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return user;
        }


        [HttpPost]
        public void Post([FromBody] UserDTO user)
        {
            db.User.Add(new User() { Firstname = user.Firstname, Lastname = user.Lastname, Fathersname = user.Fathersname});
        }


        [HttpPut]
        public void Put([FromBody] UserDTO user)
        {
            db.Entry(mapper.Map<User>(user)).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [HttpDelete]
        public void Delete(int id)
        {
            User user = db.User.Find(id);
            if (user != null)
            {
                db.User.Remove(user);
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
