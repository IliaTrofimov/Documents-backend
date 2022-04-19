using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("TemplateTypes")]
    public partial class TemplateType
    {
        public TemplateType()
        {
            Template = new HashSet<Template>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Template> Template { get; set; }
    }
}
