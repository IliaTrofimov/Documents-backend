using AutoMapper;
using Documents_backend.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using System.Linq;
using System.Web.Http.Cors;
using Documents_backend.Utility.Helpers;

namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class TemplatesController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);

        [HttpGet]
        [ActionName("list")]
        public IEnumerable<TemplateDTO> Get()
        {
            var templates = db.Templates;
            if (templates == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return mapper.Map<IEnumerable<TemplateDTO>>(templates.ToList());
        }

        [HttpGet]
        [ActionName("get")]
        public Template Get(int id)
        {
            Template template = db.Templates.Find(id);
            if (template == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            template.TemplateItems = template.TemplateItems
                .Where(item => !(item is TemplateField tf) || tf.TemplateTableId == null)
                .OrderBy(item => item.Order).ToList();
            return template;
        }


        [HttpPost]
        [ActionName("post")]
        public int Post([FromBody] int authorId)
        {
            if (authorId == -1 || db.Users.Find(authorId) == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            return db.Templates.Add(new Template() { AuthorId = authorId, UpdateDate = System.DateTime.Now }).Id;
        }


        [HttpPut]
        [ActionName("put")]
        public Template Put(int id, [FromBody] Template template)
        {
            template.UpdateDate = System.DateTime.Now;
            db.Entry(template).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            foreach (var item in template.TemplateItems)
            {
                if (item is TemplateField field)
                    _UpdateField(template, field);
                else if (item is TemplateTable table)
                {
                    _UpdateTable(template, table);
                    foreach (var col in table.TemplateFields)
                        _UpdateField(template, col);
                }
                db.SaveChanges();
            }

            return template;
        }

        [HttpPut]
        [ActionName("put-table")]
        public Template UpdateTable(int id, [FromBody] TemplateTable table)
        {
            Template template = db.Templates.Find(id);
            if (template == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _UpdateTable(template, table);
            db.SaveChanges();
            return template;
        }

        [HttpPut]
        [ActionName("put-field")]
        public Template UpdateField(int id, [FromBody] TemplateField field)
        {
            Template template = db.Templates.Find(id);
            if (template == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _UpdateField(template, field);
            db.SaveChanges();
            return template;
        }


        [HttpDelete]
        [ActionName("delete")]
        public void Delete(int id)
        {
            Template template = db.Templates.Find(id);
            if (template != null)
            {
                if (db.Documents.First(d => d.TemplateId == id) != null)
                    throw new HttpResponseException(HttpStatusCode.Conflict);

                db.Templates.Remove(template);
                db.SaveChanges();
                Ok();
            }
            else NotFound();
        }

        [HttpDelete]
        [ActionName("delete-field")]
        public void DeleteField(int id, int childId)
        {
            Template template = db.Templates.Find(id);
            TemplateField item = db.TemplateFields.Find(childId);
            if (template != null && item != null)
            {
                db.TemplateFields.Remove(item);
                db.SaveChanges();
                Ok();
            }
            else NotFound();
        }

        [HttpDelete]
        [ActionName("delete-table")]
        public void DeleteTable(int id, int childId)
        {
            Template template = db.Templates.Find(id);
            TemplateTable item = db.TemplateTables.Find(childId);
            if (template != null && item != null)
            {
                db.TemplateFields.RemoveRange(db.TemplateFields.Where(x => x.TemplateTableId == item.Id));
                db.TemplateTables.Remove(item);
                db.SaveChanges();
                Ok();
            }
            else NotFound();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private void _UpdateField(Template template, TemplateField field)
        {
            field.TemplateId = template.Id;
            if (field.Id == -1)
                db.TemplateFields.Add(field);
            else
            {
                var found = db.TemplateFields.Find(field.Id);
                found.Order = field.Order;
                found.Name = field.Name;
                found.DataType = field.DataType;
                found.Required = field.Required;
                found.Restriction = field.Restriction;
            }
        }

        private void _UpdateTable(Template template, TemplateTable table)
        {
            table.TemplateId = template.Id;
            if (table.Id == -1)
                db.TemplateTables.Add(table);
            else
            {
                var found = db.TemplateTables.Find(table.Id);
                found.Order = table.Order;
                found.Name = table.Name;
                found.Rows = table.Rows;
            }
        }
    }
}
