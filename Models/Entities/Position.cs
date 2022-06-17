using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("Position")]
    public partial class Position
    {
        public Position()
        {
            Users = new HashSet<User>();
            TemplateTypes = new HashSet<TemplateType>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [StringLength(300)]
        [Required]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<User> Users { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<TemplateType> TemplateTypes { get; set; }
    }
}
