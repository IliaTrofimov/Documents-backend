using AutoMapper;
using System.Net;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Collections.Generic;

using Documents_backend.Utility;
using Documents_Entities.Entities;
using Documents_Entities.POST;
using Documents_Entities.DTO;

namespace Documents_backend.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "GET, POST, PUT, DELETE", SupportsCredentials = true)]
    public class TemplatesController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);


        private TemplateField _UpdateItem(Template template, TemplateField field)
        {
            field.TemplateId = template.Id;
            if (field.Id == -1)
            {
                if (db.Documents.FirstOrDefault(d => d.TemplateId == template.Id) != null)
                    this.ThrowResponseException(HttpStatusCode.Conflict, "Cannot change template, some assets are still using it");

                if (template.TemplateItems.Count == 0)
                    field.Order = 0;
                else if (field.TemplateTable != null)
                    field.Order = template.TemplateItems
                           .Where(item => item is TemplateField tf && tf.TemplateTableId == field.TemplateTableId)
                           .Max(item => item.Order) + 1;
                else
                    field.Order = template.TemplateItems
                              .Where(item => !(item is TemplateField tf) || tf.TemplateTableId == field.TemplateTableId)
                              .Max(item => item.Order) + 1;

                db.TemplateFields.Add(field);
                template.TemplateItems.Add(field);
                return field;
            }
            else
            {
                var found = db.TemplateFields.Find(field.Id);

                found.Order = field.Order;
                found.Name = field.Name;
                found.DataType = field.DataType;
                found.Required = field.Required;
                found.Restriction = field.Restriction;
                found.RestrictionType = field.RestrictionType;

                if (field.TemplateTableId != null && db.TemplateTables.Find(field.TemplateTableId) == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                found.TemplateTableId = field.TemplateTableId;
                return found;
            }
        }

        private TemplateTable _UpdateItem(Template template, TemplateTable table)
        {
            table.TemplateId = template.Id;
            if (table.Id == -1)
            {
                if (db.Documents.FirstOrDefault(d => d.TemplateId == template.Id) != null)
                    this.ThrowResponseException(HttpStatusCode.Conflict, "Cannot change template, some assets are still using it");
                
                if (template.TemplateItems.Count == 0)
                    table.Order = 0;
                else 
                    table.Order = template.TemplateItems
                           .Where(item => !(item is TemplateField tf) || tf.TemplateTableId == null)
                           .Max(item => item.Order) + 1;
                db.TemplateTables.Add(table);
                template.TemplateItems.Add(table);
                return table;
            }
            else
            {
                var found = db.TemplateTables.Find(table.Id);
                found.Order = table.Order;
                found.Name = table.Name;
                found.Rows = table.Rows;
                return found;
            }
        }

        private void _ResetOrder(Template template, TemplateItem item)
        {
            int? itemTableId = (item is TemplateField) ? (int?)item.TemplateId : null;

            foreach (TemplateItem i in template.TemplateItems)
            {
                if (i.Order > item.Order)
                {
                    if (i is TemplateField)
                        i.Order--;
                    else if (i is TemplateField f && f.TemplateTableId == itemTableId)
                        i.Order--;
                }
            }
        }


        [HttpGet]
        [ActionName("count")] 
        public int Count(int user = -1, int type = -1, bool showDepricated = true)
        {
            return db.Templates.Count(t => (type == -1 || t.TemplateTypeId == type) &&
                            (user == -1 || t.AuthorId == user) && (showDepricated || !t.Depricated));
        }


        [HttpGet]
        [ActionName("list")]
        public IEnumerable<TemplateDTO> Get(int page = 0, int pageSize = -1, int user = -1, int type = -1, bool showDepricated = true)
        {
            IQueryable<Template> templates;
            if (pageSize != -1)
                templates = db.Templates.OrderBy(t => t.Id)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Where(t => (type == -1 || t.TemplateTypeId == type) && 
                            (user == -1 || t.AuthorId == user) && (showDepricated || !t.Depricated));
            else
                templates = db.Templates.Where(t => (type == -1 || t.TemplateTypeId == type) &&
                            (user == -1 || t.AuthorId == user) && (showDepricated || !t.Depricated));

            if (templates == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);

            return mapper.Map<IEnumerable<TemplateDTO>>(templates.ToList());
        }

        [HttpGet]
        [ActionName("get")]
        public Template Get(int id)
        {
           
            Template template = db.Templates.Include("TemplateType.Positions").FirstOrDefault(t => t.Id == id);
            if (template == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return template.SortTemplateItems();
        }


        [HttpPost]
        [ActionName("post")]
        public int Post([FromBody] Template body)
        {
            var user = db.Users.FirstOrDefault();
            if (user == null)
            {
                user = db.Users.Add(Documents_Entities.Entities.User.CreateAdmin());
                user.PositionId = 4;
                db.SaveChanges();
            }

            Template template = db.Templates.Add(new Template() 
            { 
                Name = body.Name, 
                TemplateTypeId = body.TemplateTypeId,
                UpdateDate = System.DateTime.Now,
                AuthorId = user.Id
            });
            db.SaveChanges();
            return template.Id;
        }


        [HttpPut]
        [ActionName("put")]
        public Template Put(int id, [FromBody] Template template)
        {
            Template found = db.Templates.Find(id);
            if (template == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot change template, template not found");
            if (db.TemplateTypes.Find(template.TemplateTypeId) == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot change template, template type not found");

            found.UpdateDate = System.DateTime.Now;
            found.Depricated = template.Depricated;
            found.Name = template.Name;
            found.TemplateTypeId = template.TemplateTypeId;

            db.SaveChanges();      
            return found.SortTemplateItems();
        }

        [HttpPut]
        [ActionName("put-table")]
        public TemplateTable UpdateTable(int id, [FromBody] TemplateTable table)
        {
            Template template = db.Templates.Find(id);
            if (template == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot change template, template not found");

            table = _UpdateItem(template, table);
            template.UpdateDate = System.DateTime.Now;
            db.SaveChanges();
            return table;
        }

        [HttpPut]
        [ActionName("put-field")]
        public TemplateField UpdateField(int id, [FromBody] TemplateField field)
        {
            Template template = db.Templates.Find(id);
            if (template == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot change template, template not found");

            field = _UpdateItem(template, field);
            template.UpdateDate = System.DateTime.Now;
            db.SaveChanges();
            return field;
        }


        [HttpPut]
        [ActionName("move-items")]
        public Template MoveItem(int id, [FromBody] ItemMovementPOST body)
        {
            Template template = db.Templates.Find(id);
            if (template == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot change template, template not found");

            TemplateItem itemA = template.TemplateItems.FirstOrDefault(item => item.Id == body.FirstItemId);
            TemplateItem itemB = template.TemplateItems.FirstOrDefault(item => item.Id == body.SecondItemId);
            if (itemA == null || itemB == null)
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot move template items, these items cannot be found");

            int temp = itemA.Order;
            itemA.Order = itemB.Order;
            itemB.Order = temp;
            template.UpdateDate = System.DateTime.Now;

            db.SaveChanges();
            return template.SortTemplateItems();
        }


        [HttpDelete]
        [ActionName("delete")]
        public void Delete(int id)
        {
            Template template = db.Templates.Find(id);
            if (template != null)
            {
                if (db.Documents.FirstOrDefault(d => d.TemplateId == id) != null)
                    this.ThrowResponseException(HttpStatusCode.Conflict, "Cannot delete template, some assets are still using it");
                
                db.TemplateFields.RemoveRange(db.TemplateFields.Where(f => f.TemplateId == id));
                db.TemplateTables.RemoveRange(db.TemplateTables.Where(f => f.TemplateId == id));
                db.SaveChanges();
                db.Templates.Remove(template);
                db.SaveChanges();
                Ok();
            }
            else this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot delete template, template not found");
        }

        [HttpDelete]
        [ActionName("delete-field")]
        public void DeleteField(int id, int childId)
        {
            Template template = db.Templates.Find(id);
            TemplateField item = db.TemplateFields.Find(childId);

            if (template != null && item != null)
            {
                if (db.Documents.FirstOrDefault(d => d.TemplateId == id) != null)
                    this.ThrowResponseException(HttpStatusCode.Conflict, "Cannot change template, some assets are still using it");

                template.UpdateDate = System.DateTime.Now;
                db.TemplateFields.Remove(item);
                _ResetOrder(template, item);
                db.SaveChanges();
                Ok();
            }
            else 
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot change template, field you trying delete is not found");
        }

        [HttpDelete]
        [ActionName("delete-table")]
        public void DeleteTable(int id, int childId)
        {
            Template template = db.Templates.Find(id);
            TemplateTable item = db.TemplateTables.Find(childId);

            if (template != null && item != null)
            {
                if (db.Documents.FirstOrDefault(d => d.TemplateId == id) != null)
                    this.ThrowResponseException(HttpStatusCode.Conflict, "Cannot change template, some assets are still using it");

                template.UpdateDate = System.DateTime.Now;
                db.TemplateFields.RemoveRange(db.TemplateFields.Where(x => x.TemplateTableId == item.Id));
                db.TemplateTables.Remove(item);
                _ResetOrder(template, item);

                db.SaveChanges();
                Ok();
            }
            else
                this.ThrowResponseException(HttpStatusCode.NotFound, "Cannot change template, table you trying delete is not found");
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
