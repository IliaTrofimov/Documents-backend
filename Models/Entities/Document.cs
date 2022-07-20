using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents.Models.Entities
{
    public enum DocumentStatus
    {
        InWork, Signing, InUse, Old
    }

    [Table("Documents")]
    public class Document
    {      
        public Document()
        {
            DocumentDataItems = new HashSet<DocumentDataItem>();
            Signs = new HashSet<Sign>();
            UpdateDate = DateTime.Now;
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public DocumentStatus Type { get; set; } = DocumentStatus.InWork;

        [Column(TypeName = "date")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("getutcdate()")]
        public DateTime UpdateDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ExpireDate { get; set; }


        public int TemplateId { get; set; }
        public virtual Template Template { get; set; }

        [Column("Author_CWID")]
        public string AuthorCWID { get; set; }
        public virtual User Author { get; set; }


        public virtual ICollection<DocumentDataItem> DocumentDataItems { get; set; }
        public virtual ICollection<Sign> Signs { get; set; }


        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
