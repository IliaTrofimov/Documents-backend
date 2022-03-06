using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("TemplateField")]
    public partial class TemplateField
    {
        [StringLength(300)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Order { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Newtonsoft.Json.JsonIgnore]
        public int TemplateId { get; set; }

        public string Restriction { get; set; }

        public int RestrictionType { get; set; }

        public bool Required { get; set; }

        public int DataType { get; set; }

        public int? Row { get; set; }

        public int? Col { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual Template Template { get; set; }
    }
}
