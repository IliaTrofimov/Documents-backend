using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("TemplateTypePositions")]
    public partial class TemplateTypePosition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TemplateTypeId { get; set; }

        [Required]
        public Position Position { get; set; }
    }
}
