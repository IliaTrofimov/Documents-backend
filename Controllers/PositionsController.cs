using System.Net;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;

using Documents_backend.Utility;
using Documents_Entities.Entities;

namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class PositionsController : ApiController
    {
        DataContext db = new DataContext();

        [HttpGet]
        [ActionName("count")]
        public int Count()
        {
            return db.Positions.Count();
        }

        [HttpGet]
        [ActionName("list")]
        public IEnumerable<Position> Get(int page = 0, int pageSize = -1)
        {
            IQueryable<Position> postions;
            if (pageSize != -1)
                postions = db.Positions.OrderBy(pos => pos.Id)
                    .Skip(page * pageSize)
                    .Take(pageSize);
            else
                postions = db.Positions;

            if (postions == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return postions.ToList();
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
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update position, position not found");

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
            else this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot delete position, position not found");
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
