using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("TemplateFields")]
    public partial class TemplateField
    {
        [Key]
        public int Id { get; set; }
        public int Order { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public int TemplateId { get; set; }
        [JsonIgnore]
        public virtual Template Template { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("")]
        public string Restriction { get; set; } = string.Empty;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue(0)]
        public int RestrictionType { get; set; } = 0;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue(0)]
        public bool Required { get; set; } = false;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue(0)]
        public int DataType { get; set; } = 0;

        public int? TemplateTableId { get; set; }
        [JsonIgnore]
        public virtual TemplateTable TemplateTable { get; set; }
    }
}
