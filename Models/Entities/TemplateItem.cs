using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents.Models.Entities
{

    public abstract class TemplateItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Order { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        public int TemplateId { get; set; }
        [JsonIgnore]
        public virtual Template Template { get; set; }

        public abstract bool IsTable { get; }


        public override string ToString()
        {
            return $"{Id} - {Name}";
        }

    }
}