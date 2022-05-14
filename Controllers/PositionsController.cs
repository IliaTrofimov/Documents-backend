using System.Net;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;

using Documents_backend.Utility;
using Documents_backend.Models;


namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class PositionsController : ApiController
    {
        DataContext db = new DataContext();

        [HttpGet]
        [ActionName("list")]
        public IEnumerable<Position> Get()
        {
            var groups = db.Positions;
            if (groups == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return groups.ToList();
        }

        [HttpGet]
        [ActionName("get")]
        public Position Get(int id)
        {
            Position group = db.Positions.Find(id);
            if (group == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return group;
        }


        [HttpPost]
        [ActionName("post")]
        public int Post([FromBody] Position body)
        {
            if (body == null)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Empty POST request");

            Position pos = db.Positions.Add(new Position() { Name = body.Name });
            db.SaveChanges();
            return pos.Id;
        }


        [HttpPut]
        [ActionName("put")]
        public Position Put(int id, [FromBody] Position body)
        {
            if (body == null)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Empty POST request");

            Position pos = db.Positions.Find(id);

            if (pos == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update group, group not found");

            pos.Name = body.Name;
            db.SaveChanges();
            return pos;
        }

        [HttpDelete]
        [ActionName("delete")]
        public void Delete(int id)
        {
            Position pos = db.Positions.Find(id);
            if (pos != null)
            {
                db.Positions.Remove(pos);
                db.SaveChanges();
            }
            else this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot delete group, group not found");
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
