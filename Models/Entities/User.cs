using Documents.Models.Administrative;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents.Models.Entities
{
    [Table("Users")]
    public  class User
    {
        public User()
        {
            Documents = new HashSet<Document>();
            Signs = new HashSet<Sign>();
            Templates = new HashSet<Template>();
            Permissions = (byte)PermissionFlag.FullAccess;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        public string Firstname { get; set; }

        [StringLength(100)]
        public string Lastname { get; set; }

        [StringLength(100)]
        public string Fathersname { get; set; }

        [Required]
        [DefaultValue((byte)PermissionFlag.FullAccess)]
        public byte Permissions { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        public virtual ICollection<Sign> Signs { get; set; }

        public virtual ICollection<Template> Templates { get; set; }

        public string Email { get; set; }

        public string GetFIO()
        {
            return Fathersname != null ? $"{Lastname} {Firstname[0]}. {Firstname[0]}." : $"{Lastname} {Firstname[0]}.";
        }


        public static User CreateAdmin(string firstname, string lastname, string fathersname) =>
            new User() { Firstname = firstname, Lastname = lastname, Fathersname = firstname, Permissions = (byte)PermissionFlag.FullAccess };

        public static User CreateAdmin() =>
            new User() { Firstname = "Admin", Lastname = "Admin", Fathersname = "Admin", Permissions = (byte)PermissionFlag.FullAccess };


        public override string ToString()
        {
            return $"{Id} - {GetFIO()}";
        }
    }
}
