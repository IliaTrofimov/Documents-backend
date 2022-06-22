using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Documents.Models.Entities
{
    [Table("Templates")]
    public class Template
    {
        public Template()
        {
            Document = new HashSet<Document>();
            TemplateItems = new HashSet<TemplateItem>();
            UpdateDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(300)]
        public string Name { get; set; }


        [Column(TypeName = "date")]
        [DefaultValue("getutcdate()")]
        public DateTime? UpdateDate { get; set; }

        [DefaultValue(0)]
        public bool Depricated { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Document> Document { get; set; }

        public virtual int? TemplateTypeId { get; set; }
        public virtual TemplateType TemplateType { get; set; }


        [NotMapped]
        public string AuthorName => Author != null ? Author.GetFIO() : "Неизвестно";
        public int? AuthorId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual User Author { get; set; }


        public virtual ICollection<TemplateItem> TemplateItems { get; set; }


        public Template SortTemplateItems()
        {
            if (TemplateItems.Count != 0)
            {
                TemplateItems = TemplateItems
                   .Where(item => !(item is TemplateField tf) || tf.TemplateTableId == null)
                   .OrderBy(item => item.Order)
                   .ToList();
            }
            return this;
        }


        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
