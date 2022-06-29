using AutoMapper;
using System.Net;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Data.Entity;

using Documents.Utility;
using Documents.Models.DTO;
using Documents.Models.Entities;
using Documents.Models;

namespace Documents.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class UsersController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        [ActionName("count")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Count(int position = -1)
        {
            int count = await db.Users.CountAsync(user => position == -1 || user.PositionId == position);
            return Ok(count);
        }


        [HttpGet]
        [ActionName("whoami")]
        public UserDTO WhoAmI()
        {
            var user = db.Users.Include("Position").FirstOrDefault();
            if (user == null)
            {
                user = db.Users.Add(Models.Entities.User.CreateAdmin());
                user.PositionId = 4;
                db.SaveChanges();
            }
            return mapper.Map<UserDTO>(user);
        }

        [HttpGet]
        [ActionName("list")]
        [ResponseType(typeof(List<User>))]
        public async Task<IHttpActionResult> Get(int page = 0, int pageSize = -1, int position = -1, int permissions = -1)
        {
            List<User> users;
            if (pageSize != -1)
                users = await db.Users.Include(user => user.Position)
                    .OrderBy(user => user.Id)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Where(user => (position == -1 || user.PositionId == position) && (permissions == -1 || user.Permissions == permissions))
                    .ToListAsync();
            else 
                users = await db.Users.Include(user => user.Position)
                   .OrderBy(user => user.Id)
                   .Where(user => (position == -1 || user.PositionId == position) && (permissions == -1 || user.Permissions == permissions))
                   .ToListAsync();

            if (users == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
      
            return Ok(mapper.Map<IEnumerable<UserDTO>>(users));
        }

        [HttpGet]
        [ActionName("get")]
        [ResponseType(typeof(UserDTO))]
        public async Task<IHttpActionResult> Get(int id)
        {
            User user = await db.Users
                .Include(_user => _user.Position)
                .FirstOrDefaultAsync(_user => _user.Id == id);

            if (user == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            
            return Ok(mapper.Map<UserDTO>(user));
        }


        [HttpPost]
        [ActionName("post")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Post([FromBody] UserDTO user)
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
            await db.SaveChangesAsync();
            return Ok(newUser.Id);
        }


        [HttpPut]
        [ActionName("put")]
        [ResponseType(typeof(UserDTO))]
        public async Task<IHttpActionResult> Put(int id, [FromBody] UserDTO user)
        {
            User found = await db.Users.FindAsync(id); 
            if (found == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update user, user not found");

            found.Fathersname = user.Fathersname;
            found.Firstname = user.Firstname;
            found.Lastname = user.Lastname;
            found.Permissions = user.Permissions;
            found.PositionId = user.PositionId;
            found.Email = user.Email;

            await db.SaveChangesAsync();
            db.Entry(found).Reference("Position").Load();
            return Ok(mapper.Map<UserDTO>(found));
        }


        [HttpDelete]
        [ActionName("delete")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            User user = db.Users.Find(id);
            if (user != null)
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return Ok();
            }   
            else
               return NotFound();
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
