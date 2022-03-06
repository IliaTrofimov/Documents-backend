using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("DocumentTableCell")]
    public partial class DocumentTableCell
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Newtonsoft.Json.JsonIgnore]
        public int DocumentId { get; set; }

        public string Value { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Field { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Row { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Col { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual Document Document { get; set; }
    }
}
