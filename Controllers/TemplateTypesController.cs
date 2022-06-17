using AutoMapper;
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
    public class TemplateTypesController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);


        [HttpGet]
        [ActionName("count")]
        public int Count()
        {
            return db.TemplateTypes.Count();
        }

        [HttpGet]
        [ActionName("list")]
        public IEnumerable<TemplateType> Get(int page = 0, int pageSize = -1)
        {
            IQueryable<TemplateType> types;
            if (pageSize != -1)
                types = db.TemplateTypes.Include("Positions")
                    .OrderBy(type => type.Id)
                    .Skip(page * pageSize)
                    .Take(pageSize);
            else
                types = db.TemplateTypes.Include("Positions");

            if (types == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return types.ToList();
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
        public int Post([FromBody] TemplateType type)
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
            db.SaveChanges();
            return newType.Id;
        }


        [HttpPut]
        public TemplateType Put(int id, [FromBody] TemplateType type)
        {
            TemplateType found = db.TemplateTypes.Find(id);
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
                    
            db.SaveChanges();
            return found;
        }

        [HttpDelete]
        public void Delete(int id)
        {
            TemplateType type = db.TemplateTypes.Find(id);
            if (type != null)
            {
                if (db.Templates.FirstOrDefault(t => t.TemplateTypeId == id) != null)
                    this.ThrowResponseException(HttpStatusCode.Conflict, "Cannot delete template type, some assests still use it");
                
                db.TemplateTypes.Remove(type);
                db.SaveChanges();
            }
            else this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot delete template type, type not found");
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
