using AutoMapper;
using System.Net;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;
using System.Data.Entity;

using Documents_backend.Utility;
using Documents_Entities.DTO;
using Documents_Entities.Entities;

namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class UsersController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        [ActionName("count")]
        public int Count(int position = -1)
        {
            return db.Users.Count(user => position == -1 || user.PositionId == position);
        }


        [HttpGet]
        [ActionName("whoami")]
        public UserDTO WhoAmI()
        {
            var user = db.Users.Include("Position").FirstOrDefault();
            if (user == null)
            {
                user = db.Users.Add(Documents_Entities.Entities.User.CreateAdmin());
                user.PositionId = 4;
                db.SaveChanges();
            }
            return mapper.Map<UserDTO>(user);
        }

        [HttpGet]
        [ActionName("list")]
        public IEnumerable<UserDTO> Get(int page = 0, int pageSize = -1, int position = -1, int permissions = -1)
        {
            IQueryable<User> users;
            if (pageSize != -1)
                users = db.Users.Include(user => user.Position)
                    .OrderBy(user => user.Id)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Where(user => (position == -1 || user.PositionId == position) && (permissions == -1 || user.Permissions == permissions));
            else
                users = db.Users.Include(user => user.Position)
                   .OrderBy(user => user.Id)
                   .Where(user => (position == -1 || user.PositionId == position) && (permissions == -1 || user.Permissions == permissions));

            if (users == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
      
            return mapper.Map<IEnumerable<UserDTO>>(users);
        }

        [HttpGet]
        [ActionName("get")]
        public UserDTORich Get(int id)
        {
            User user = db.Users
                .Include(_user => _user.Position)
                .Include(_user => _user.Documents)
                .Include(_user => _user.Templates)
                .Include(_user => _user.Signs)
                .FirstOrDefault(_user => _user.Id == id);

            if (user == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            
            return mapper.Map<UserDTORich>(user);
        }


        [HttpPost]
        [ActionName("post")]
        public int Post([FromBody] UserDTO user)
        {
            User newUser = db.Users.Add(new User()
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Fathersname = user.Fathersname,
                PositionId = user.PositionId,
                Permissions = user.Permissions,
                Email = user.Email
            }) ;
            db.SaveChanges();
            return newUser.Id;
        }


        [HttpPut]
        [ActionName("put")]
        public UserDTO Put(int id, [FromBody] UserDTO user)
        {
            User found = db.Users.Find(id); 
            if (found == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update user, user not found");

            found.Fathersname = user.Fathersname;
            found.Firstname = user.Firstname;
            found.Lastname = user.Lastname;
            found.Permissions = user.Permissions;
            found.PositionId = user.PositionId;
            found.Email = user.Email;

            db.SaveChanges();
            db.Entry(found).Reference("Position").Load();
            return mapper.Map<UserDTO>(found);
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
            else
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot delete user, user not found");
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
