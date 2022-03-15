using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("Signs")]
    public partial class Sign
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DocumentId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        public bool Signed { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual Document Document { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual User User { get; set; }
    }
}
