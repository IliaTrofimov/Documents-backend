using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents.Models.Entities
{
    [Table("Signs")]
    public class Sign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DocumentId { get; set; }

        [Column("User_CWID")]
        public string UserCWID { get; set; }
        public virtual User User { get; set; }

        [Required]
        [Column("Initiator_CWID")]
        public string InitiatorCWID { get; set; }
        public virtual User Initiator { get; set; }

        [Required]
        public int SignerPositionId { get; set; }
        public virtual Position SignerPosition { get; set; }


        [Required]
        public System.DateTime UpdateDate { get; set; }
        [Required]
        public System.DateTime CreateDate { get; set; }
        public bool? Signed { get; set; }


        [Newtonsoft.Json.JsonIgnore]
        public virtual Document Document { get; set; }

        [NotMapped]
        public string DocumentName => Document.Name;


        public override string ToString()
        {
            return $"{DocumentId} - {UserCWID}";
        }
    }
}
