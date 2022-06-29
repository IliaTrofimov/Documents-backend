using System.Net;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Data.Entity;

using Documents.Utility;
using Documents.Models.Entities;
using Documents.Models;

namespace Documents.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class PositionsController : ApiController
    {
        DataContext db = new DataContext();

        [HttpGet]
        [ActionName("count")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Count()
        {
            int count = await db.Positions.CountAsync();
            return Ok(count);
        }

        [HttpGet]
        [ActionName("list")]
        [ResponseType(typeof(List<Position>))]
        public async Task<IHttpActionResult> Get(int page = 0, int pageSize = -1)
        {
            List<Position> postions;
            if (pageSize != -1)
                postions = await db.Positions.OrderBy(pos => pos.Id)
                    .Skip(page * pageSize)
                    .Take(pageSize).ToListAsync();
            else
                postions = await db.Positions.ToListAsync();

            if (postions == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return Ok(postions);
        }

        [HttpGet]
        [ActionName("get")]
        [ResponseType(typeof(Position))]
        public async Task<IHttpActionResult> Get(int id)
        {
            Position position = await db.Positions.FindAsync(id);
            if (position == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return Ok(position);
        }


        [HttpPost]
        [ActionName("post")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Post([FromBody] Position body)
        {
            if (body == null)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Empty POST request");

            Position pos = db.Positions.Add(new Position() { Name = body.Name });
            await db.SaveChangesAsync();
            return Ok(pos.Id);
        }


        [HttpPut]
        [ActionName("put")]
        [ResponseType(typeof(Position))]
        public async Task<IHttpActionResult> Put(int id, [FromBody] Position body)
        {
            if (body == null)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Empty POST request");

            Position pos = await db.Positions.FindAsync(id);

            if (pos == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update position, position not found");

            pos.Name = body.Name;
            await db.SaveChangesAsync();
            return Ok(pos);
        }

        [HttpDelete]
        [ActionName("delete")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            Position pos = db.Positions.Find(id);
            if (pos != null)
            {
                db.Positions.Remove(pos);
                await db.SaveChangesAsync();
                return Ok();
            }
            else return NotFound();
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
