using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("Templates")]
    public partial class Template
    {
        public Template()
        {
            Document = new HashSet<Document>();
            TemplateField = new HashSet<TemplateField>();
            TemplateTable = new HashSet<TemplateTable>();
            UpdateDate = DateTime.Now;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(300)]
        public string Name { get; set; }



        [Column(TypeName = "date")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("getutcdate()")]
        public DateTime? UpdateDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue(0)]
        public bool Depricated { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Document> Document { get; set; }

        public virtual TemplateType TemplateType { get; set; }


        [NotMapped]
        public string AuthorName => Author != null ? Author.GetFIO() : "Неизвестно";
        public int? AuthorId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual User Author { get; set; }


        public virtual ICollection<TemplateField> TemplateField { get; set; }
        public virtual ICollection<TemplateTable> TemplateTable { get; set; }
    }
}
