using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("TemplateTables")]
    public partial class TemplateTable
    {
        public TemplateTable()
        {
            TemplateField = new HashSet<TemplateField>();
        }

        [Key]
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue(1)]
        public int Rows { get; set; } = 1;
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<TemplateField> TemplateField { get; set; }
    }
}
