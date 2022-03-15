using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("Documents")]
    public partial class Document
    {      
        public Document()
        {
            DocumentDataItems = new HashSet<DocumentDataItem>();
            //DocumentTableCells = new HashSet<DocumentTableCell>();
            Signs = new HashSet<Sign>();
            UpdateDate = DateTime.Now;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue(0)]
        public int Type { get; set; }

        [Column(TypeName = "date")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("getutcdate()")]
        public DateTime UpdateDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ExpireDate { get; set; }


        public int TemplateId { get; set; }
        public virtual Template Template { get; set; }


        [NotMapped]
        public string AuthorName => Author != null ? Author.GetFIO() : "Неизвестно";
        public int? AuthorId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual User Author { get; set; }


        public virtual ICollection<DocumentDataItem> DocumentDataItems { get; set; }

        //public virtual ICollection<DocumentTableCell> DocumentTableCells { get; set; }

        public virtual ICollection<Sign> Signs { get; set; }
    }
}
