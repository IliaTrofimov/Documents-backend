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

        public int? UserId { get; set; }

        public bool? Signed { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual Document Document { get; set; }

        [NotMapped]
        public string DocumentName => Document.Name;

       
        public virtual User User { get; set; }

        //[NotMapped]
        //public virtual string SignerShortname => User.GetFIO();

        [Required]
        public int SignerPositionId { get; set; }

        public virtual Position SignerPosition { get; set; }

        [Required]
        public int InitiatorId { get; set; }

        public virtual User Initiator { get; set; }

       // [NotMapped]
        //public virtual string InitiatorShortname => Initiator.GetFIO();

        [Required]
        public System.DateTime UpdateDate { get; set; }
        [Required]
        public System.DateTime CreateDate { get; set; }


        public override string ToString()
        {
            return $"{DocumentId} - {UserId}";
        }
    }
}
