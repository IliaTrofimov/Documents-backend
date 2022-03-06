using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("Template")]
    public partial class Template
    {
        public Template()
        {
            Document = new HashSet<Document>();
            TemplateField = new HashSet<TemplateField>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(300)]
        public string Name { get; set; }

        public int? AuthorId { get; set; }

        [NotMapped]
        public string AuthorName { get => User != null ? User.GetFIO() : "Неизвестно"; }

        [Newtonsoft.Json.JsonIgnore]
        public int TypeId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdateDate { get; set; }

        public bool Depricated { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Document> Document { get; set; }

        public virtual TemplateType TemplateType { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual User User { get; set; }

        public virtual ICollection<TemplateField> TemplateField { get; set; }
    }
}
