using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents.Models.Entities
{
    [Table("TemplateTypes")]
    public class TemplateType
    {
        public TemplateType()
        {
            Template = new HashSet<Template>();
            Positions = new HashSet<Position>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }


        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Template> Template { get; set; }

        public virtual ICollection<Position> Positions { get; set; }


        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
