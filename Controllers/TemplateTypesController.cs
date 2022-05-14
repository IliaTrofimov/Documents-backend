using AutoMapper;
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
    public class TemplateTypesController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);


        [HttpGet]
        [ActionName("list")]
        public IEnumerable<TemplateTypeDTO> Get()
        {
            var types = db.TemplateTypes;
            if (types == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return mapper.Map<IEnumerable<TemplateTypeDTO>>(types);
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
        public int Post([FromBody] string name)
        {
            TemplateType type = db.TemplateTypes.Add(new TemplateType() { Name = name });
            db.SaveChanges();
            return type.Id;
        }


        [HttpPut]
        public void Put(int id, [FromBody] TemplateType type)
        {
            TemplateType found = db.TemplateTypes.Find(id);
            if (type == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot update template type, type not found");

            found.Name = type.Name;
            db.SaveChanges();
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
