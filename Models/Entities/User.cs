using Documents_backend.Models.Administrative;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models
{
    [Table("Users")]
    public partial class User
    {
        public User()
        {
            Documents = new HashSet<Document>();
            Signs = new HashSet<Sign>();
            Templates = new HashSet<Template>();
            Permissions = (byte)PermissionFlag.FullAccess;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(100)]
        public string Firstname { get; set; }

        [StringLength(100)]
        public string Lastname { get; set; }

        [StringLength(100)]
        public string Fathersname { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue((byte)PermissionFlag.FullAccess)]
        public byte Permissions { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        public virtual ICollection<Sign> Signs { get; set; }

        public virtual ICollection<Template> Templates { get; set; }


        public string GetFIO()
        {
            return Fathersname != null ? $"{Lastname} {Firstname[0]}. {Firstname[0]}." : $"{Lastname} {Firstname[0]}.";
        }
    }
}
