﻿using AutoMapper;
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
    public class DocumentsController : ApiController
    {
        DataContext db = new DataContext();
        Mapper mapper = new Mapper(WebApiApplication.mapperConfig);


        [HttpGet]
        [ActionName("list")]
        public IEnumerable<DocumentDTO> Get()
        {
            var documents = db.Documents;
            if (documents == null)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            return mapper.Map<IEnumerable<DocumentDTO>>(documents.ToList());
        }

        [HttpGet]
        [ActionName("get")]
        public Document Get(int id)
        {
            Document document = db.Documents.Find(id);
            if (document == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            document.Template = document.Template.SortTemplateItems();
            return document;
        }   


        [HttpPost]
        [ActionName("post")]
        public int Post([FromBody] Models.POST.DocumentPOST body)
        {
            Template template = db.Templates.Find(body.TemplateId);
            if (template == null)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Cannot create document from template, template not found");

            Document document = new Document() { TemplateId = body.TemplateId, UpdateDate = System.DateTime.Now };
            db.Documents.Add(document);
            db.SaveChanges();

            foreach (var item in template.TemplateItems)
            {
                if (item is TemplateField field)
                    db.DocumentDataItems.Add(new DocumentDataItem() { FieldId = item.Id, Document = document });
                else if (item is TemplateTable table)
                {
                    foreach (var col in table.TemplateFields)
                        for (int i = 0; i < table.Rows; i++)
                            db.DocumentDataItems.Add(new DocumentDataItem() 
                            { 
                                FieldId = col.Id, 
                                Document = document, 
                                Row = i 
                            });
                }
            }

            db.SaveChanges();
            return document.Id;
        }

        [HttpPost]
        [ActionName("from-existing")]
        public int FromExisting([FromBody] int documentId, [FromBody] int authorId)
        {
            Document oldDocument = db.Documents.Find(documentId);
            if (oldDocument == null)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Cannot create new version of document, previous document not found");

            Document document = new Document() { TemplateId = oldDocument.TemplateId, AuthorId = authorId, UpdateDate = System.DateTime.Now };
            foreach (var item in oldDocument.DocumentDataItems)
                document.DocumentDataItems.Add(new DocumentDataItem() { FieldId = item.FieldId, Row = item.Row, Value = item.Value });

            db.Documents.Add(document);
            db.SaveChanges();
            return document.Id;
        }


        [HttpPut]
        [ActionName("put")]
        public Document Put([FromBody] Document document)
        {
            document.UpdateDate = System.DateTime.Now;
            Document found = db.Documents.Find(document.Id);
            if (found == null)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Cannot update document, document not found");

            found.UpdateDate = System.DateTime.Now;
            found.Name = document.Name;
            found.ExpireDate = document.ExpireDate;
            found.Type = document.Type;
            db.SaveChanges();

            document.Template.TemplateItems = document.Template.TemplateItems
                .OrderBy(item => item.Order)
                .Where(item => !(item is TemplateField tf) || tf.TemplateTableId == null)
                .ToList();
            return document;
        }

        [HttpPut]
        [ActionName("put-field")]
        public Document UpdateField(int id, [FromBody] DocumentDataItem item)
        {
            Document document = db.Documents.Find(id);
            if (document == null)
                this.ThrowResponseException(HttpStatusCode.BadRequest, "Cannot update document, document not found");

            item.Document = document;
            if (item.Id == -1)
                db.DocumentDataItems.Add(item);
            else
            {
                var found = db.DocumentDataItems.Find(item.Id);
                found.Value = item.Value;
                found.Row = item.Row;
            }
            db.SaveChanges();

            document.Template.TemplateItems = document.Template.TemplateItems
                .OrderBy(i => i.Order)
                .Where(i => !(i is TemplateField tf) || tf.TemplateTableId == null)
                .ToList();
            return document;
        }


        [HttpDelete]
        [ActionName("delete")]
        public void Delete(int id)
        {
            Document info = db.Documents.Find(id);
            if (info != null)
            {
                db.Documents.Remove(info);
                db.DocumentDataItems.RemoveRange(db.DocumentDataItems.Where(i => i.DocumentId == id));
                db.SaveChanges();
                Ok();
            }
            else this.ThrowResponseException(HttpStatusCode.BadRequest, "Cannot delete document, document not found");
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