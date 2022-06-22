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
using Documents.Models.Entities;
using Documents.Models;

namespace Documents.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class TemplateTypesController : ApiController
    {
        DataContext db = new DataContext();

        [HttpGet]
        [ActionName("count")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Count()
        {
            int count = await db.TemplateTypes.CountAsync();
            return Ok(count);
        }

        [HttpGet]
        [ActionName("list")]
        [ResponseType(typeof(IEnumerable<TemplateType>))]
        public async Task<IHttpActionResult> Get(int page = 0, int pageSize = -1)
        {
            List<TemplateType> types;
            if (pageSize != -1)
                types = await db.TemplateTypes.Include("Positions")
                    .OrderBy(type => type.Id)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            else
                types = await db.TemplateTypes.Include("Positions").ToListAsync();

            if (types == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return Ok(types);
        }

        [HttpGet]
        [ActionName("get")]
        public TemplateType Get(int id)
        {
            TemplateType type = db.TemplateTypes.Find(id);
            if (type == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return type;
        }


        [HttpPost]
        [ActionName("post")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> Post([FromBody] TemplateType type)
        {
            var newType = db.TemplateTypes.Add(new TemplateType() { Name = type.Name });
            foreach (var pos in type.Positions)
            {
                if (!newType.Positions.Contains(pos))
                {
                    var p = db.Positions.Find(pos.Id);
                    if (p != null)
                        newType.Positions.Add(p);
                }
            }
            await db.SaveChangesAsync();
            return Ok(newType.Id);
        }


        [HttpPut]
        [ActionName("put")]
        [ResponseType(typeof(TemplateType))]
        public async Task<IHttpActionResult> Put(int id, [FromBody] TemplateType type)
        {
            TemplateType found = await db.TemplateTypes.FindAsync(id);
            if (type == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update template type, type not found");

            found.Name = type.Name;
            found.Positions = found.Positions.Where(p => type.Positions.Contains(p)).ToList();
            foreach (var pos in type.Positions)
            {
                if (!found.Positions.Contains(pos))
                {
                    var p = db.Positions.Find(pos.Id);
                    if (p != null)
                        found.Positions.Add(p);
                }
            }

            await db.SaveChangesAsync();
            return Ok(found);
        }

        [HttpDelete]
        [ActionName("delete")]
        [ResponseType(typeof(TemplateType))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            TemplateType type = await db.TemplateTypes.FindAsync(id);
            if (type != null)
            {
                if ((await db.Templates.FirstOrDefaultAsync(t => t.TemplateTypeId == id)) != null)
                    this.ThrowResponseException(HttpStatusCode.Conflict, "Cannot delete template type, some assests still use it");

                db.TemplateTypes.Remove(type);
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
