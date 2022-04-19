using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    public partial class DocumentDataItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Value { get; set; }

        [Index("IX_ItemPlacement", 1, IsUnique = true)]
        public int Field { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [Index("IX_ItemPlacement", 2, IsUnique = true)]
        public virtual Document Document { get; set; }

        [Index("IX_ItemPlacement", 3, IsUnique = true)]
        public int? Row { get; set; }

        [Index("IX_ItemPlacement", 4, IsUnique = true)]
        public int? Col { get; set; }
    }


}
