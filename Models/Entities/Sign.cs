using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents.Models.Entities
{
    [Table("Signs")]
    public class Sign
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DocumentId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        public bool? Signed { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual Document Document { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual User User { get; set; }

        [Required]
        public int SignerPositionId { get; set; }
        public virtual Position SignerPosition { get; set; }

        [Required]
        public int InitiatorId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual User Initiator { get; set; }

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
