using AutoMapper;
using Documents_backend.Models;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

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
            return db.TemplateTypes.Add(new TemplateType() { Name = name }).Id;
        }


        [HttpPut]
        public void Put([FromBody] int id, [FromBody] string name)
        {
            TemplateType type = db.TemplateTypes.Find(id);
            if (type == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            type.Name = name;
            db.Entry(type).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [HttpDelete]
        public void Delete(int id)
        {
            TemplateType type = db.TemplateTypes.Find(id);
            if (type != null)
            {
                db.TemplateTypes.Remove(type);
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
