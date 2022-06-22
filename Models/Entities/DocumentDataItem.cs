using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents.Models.Entities
{
    public class DocumentDataItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Value { get; set; }

        [Required]
        public int FieldId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [Required]
        public TemplateField Field { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [Required]
        public virtual Document Document { get; set; }

        public int? Row { get; set; }
    }
}
