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
using System.Security.Principal;

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
        public async Task<IHttpActionResult> Count(int position = -1, int permissions = -1)
        {
            int count = await db.Users.CountAsync(user => position == -1 || user.PositionId == position &&
                (permissions == -1 || (user.Permissions & permissions) == permissions));
 
            return Ok(count);
        }


        [HttpGet]
        [ActionName("whoami")]
        [ResponseType(typeof(UserDTOFull))]
        public async Task<IHttpActionResult> WhoAmI()
        {
            if (!RequestContext.Principal.Identity.IsAuthenticated)
                return Ok(mapper.Map<UserDTOFull>(await db.Users.Include("Position").FirstOrDefaultAsync()));
            User userData = await db.Users.Include("Position").FirstOrDefaultAsync(u => RequestContext.Principal.Identity.Name == u.CWID);
            if (userData == null)
                this.ThrowResponseException(HttpStatusCode.Unauthorized, "Cannot find user with such CWID");

            return Ok(mapper.Map<UserDTOFull>(userData));
        }

        [HttpGet]
        [ActionName("list")]
        [ResponseType(typeof(List<UserDTO>))]
        public async Task<IHttpActionResult> Get(int page = 0,
                                                 int pageSize = -1,
                                                 int position = -1,
                                                 int permissions = -1,
                                                 bool full = false)
        {
            List<User> users;
            if (pageSize != -1)
                users = await db.Users.Include(user => user.Position)
                    .OrderBy(user => user.CWID)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Where(user => (position == -1 || user.PositionId == position) && 
                        (permissions == -1 || (user.Permissions & permissions) == permissions))
                    .ToListAsync();
            else 
                users = await db.Users.Include(user => user.Position)
                   .OrderBy(user => user.CWID)
                   .Where(user => (position == -1 || user.PositionId == position) && 
                        (permissions == -1 || (user.Permissions & permissions) == permissions))
                   .ToListAsync();

            if (users == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            
            return Ok(full ? mapper.Map<IEnumerable<UserDTOFull>>(users) : mapper.Map<IEnumerable<UserDTO>>(users));
        }

        [HttpGet]
        [ActionName("list-full")]
        [ResponseType(typeof(List<UserDTOFull>))]
        public async Task<IHttpActionResult> GetFull(int page = 0,
                                                     int pageSize = -1,
                                                     int position = -1,
                                                     int permissions = -1)
        {
            List<User> users;
            if (pageSize != -1)
                users = await db.Users.Include(user => user.Position)
                    .OrderBy(user => user.CWID)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Where(user => (position == -1 || user.PositionId == position) &&
                        (permissions == -1 || (user.Permissions & permissions) == permissions))
                    .ToListAsync();
            else
                users = await db.Users.Include(user => user.Position)
                   .OrderBy(user => user.CWID)
                   .Where(user => (position == -1 || user.PositionId == position) &&
                        (permissions == -1 || (user.Permissions & permissions) == permissions))
                   .ToListAsync();

            if (users == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);

            return Ok(mapper.Map<IEnumerable<UserDTOFull>>(users));
        }

        [HttpGet]
        [ActionName("get")]
        [ResponseType(typeof(UserDTOFull))]
        public async Task<IHttpActionResult> Get(string id)
        {
            User user = await db.Users
                .Include(_user => _user.Position)
                .FirstOrDefaultAsync(_user => _user.CWID == id);

            if (user == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            
            return Ok(mapper.Map<UserDTOFull>(user));
        }


        [HttpPost]
        [ActionName("post")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Post([FromBody] User user)
        {
            if ((await db.Users.FindAsync(user.CWID)) != null)
                this.ThrowResponseException(HttpStatusCode.Conflict, "Cannot create new user, user with given CWID already exists");

            User newUser = db.Users.Add(user);
            await db.SaveChangesAsync();
            return Ok(newUser.CWID);
        }


        [HttpPut]
        [ActionName("put")]
        [ResponseType(typeof(UserDTOFull))]
        public async Task<IHttpActionResult> Put(string id, [FromBody] UserDTOFull user)
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
            found.CompanyCode = user.CompanyCode;
            found.EmployeeType = user.EmployeeType;
            found.ExternalCompany = user.ExternalCompany;
            found.LeadingSubgroup = user.LeadingSubgroup;
            found.OrgName = user.OrgName;

            if (db.Users.Find(user.ManagerCWID) != null)
                found.ManagerCWID = user.ManagerCWID;

            await db.SaveChangesAsync();
            db.Entry(found).Reference("Position").Load();
            return Ok(mapper.Map<UserDTOFull>(found));
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
