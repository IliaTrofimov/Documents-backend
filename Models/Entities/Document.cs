using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("Document")]
    public partial class Document
    {      
        public Document()
        {
            DocumentDataItem = new HashSet<DocumentDataItem>();
            DocumentTableCell = new HashSet<DocumentTableCell>();
            Sign = new HashSet<Sign>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public int TemplateId { get; set; }

        public int? AuthorId { get; set; }

        [NotMapped]
        public string AuthorName { get => User != null ? User.GetFIO() : "Неизвестно"; }

        public int Type { get; set; }

        [Column(TypeName = "date")]
        public DateTime UpdateDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ExpireDate { get; set; }

        public virtual Template Template { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual User User { get; set; }

        public virtual ICollection<DocumentDataItem> DocumentDataItem { get; set; }

        public virtual ICollection<DocumentTableCell> DocumentTableCell { get; set; }

        public virtual ICollection<Sign> Sign { get; set; }
    }
}
