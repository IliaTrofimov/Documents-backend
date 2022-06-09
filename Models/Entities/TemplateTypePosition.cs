using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("TemplateTypePositions")]
    public partial class TemplateTypePosition
    {
        [Key]
        public int Id { get; set; }

        public int TemplateTypeId { get; set; }


        public Position Position { get; set; }
        public int PositionId { get; set; }
    }
}
