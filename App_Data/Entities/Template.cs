using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models.Entities
{
    public class TemplateType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Template
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Fields { get; set; } = "[]";
        public int Depricated { get; set; } = 0;
        public DateTime UpdateDate { get; set; } = DateTime.Now;


        public int TemplateTypeId { get; set; }
        public virtual TemplateType TemplateType { get; set; }

        public virtual ICollection<DocumentInfo> DocumentInfo { get; private set; }
        public virtual ICollection<Sign> Signs { get; private set; }
    }
}