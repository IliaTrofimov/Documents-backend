using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("TemplateType")]
    public partial class TemplateType
    {
        public TemplateType()
        {
            Template = new HashSet<Template>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Template> Template { get; set; }
    }
}
