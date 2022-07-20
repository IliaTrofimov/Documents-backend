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
        [StringLength(8)]
        public string CWID { get; set; }

        [StringLength(100)]
        public string Firstname { get; set; }

        [StringLength(100)]
        public string Lastname { get; set; }

        [StringLength(100)]
        public string Fathersname { get; set; }

        [Required]
        [DefaultValue((byte)PermissionFlag.FullAccess)]
        public byte Permissions { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(20)]
        public string EmployeeType { get; set; }

        [StringLength(30)]
        public string LeadingSubgroup { get; set; }

        [StringLength(30)]
        public string ExternalCompany { get; set; }

        [StringLength(30)]
        public string OrgName { get; set; }

        [StringLength(30)]
        public string CompanyCode { get; set; }


        public int PositionId { get; set; }
        public Position Position { get; set; }

        [Column("Manager_CWID")]
        public string ManagerCWID { get; set; }
        public User Manager { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        public virtual ICollection<Sign> Signs { get; set; }

        public virtual ICollection<Template> Templates { get; set; }


        public string GetFIO()
        {
            return Fathersname.Length != 0 ? $"{Lastname} {Firstname[0]}. {Fathersname[0]}." : $"{Lastname} {Firstname[0]}.";
        }


        public static User CreateAdmin(string firstname, string lastname, string fathersname) =>
            new User() 
            { 
                Firstname = firstname, 
                Lastname = lastname, 
                Fathersname = fathersname, 
                Permissions = (byte)PermissionFlag.FullAccess, 
                CWID = "00000000",
                Email = Properties.Settings.Default.EmailFrom
            };

        public static User CreateAdmin() =>
            new User() 
            { 
                Firstname = " ", 
                Lastname = "Admin", 
                Permissions = (byte)PermissionFlag.FullAccess, 
                CWID = "00000000",
                Email = Properties.Settings.Default.EmailFrom
            };


        public override string ToString()
        {
            return $"{CWID} - {GetFIO()}";
        }
    }
}
