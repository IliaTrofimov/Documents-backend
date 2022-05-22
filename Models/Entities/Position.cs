using Documents_backend.Models.Administrative;
using System.Collections.Generic;
using System.ComponentModel;
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
            TemplateTypePositions = new HashSet<TemplateTypePosition>();
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
        public virtual ICollection<TemplateTypePosition> TemplateTypePositions { get; set; }
    }
}
