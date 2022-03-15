using System.Collections.Generic;

namespace Documents_backend.Models
{
    public partial class UserDTORich
    {
        public UserDTORich()
        {
            Documents = new HashSet<DocumentDTO>();
            Signs = new HashSet<SignDTO>();
            Templates = new HashSet<TemplateDTO>();
        }

        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Fathersname { get; set; }

        public byte Permissions { get; set; }

        public string PermissionsString { get; set; }

        public ICollection<DocumentDTO> Documents { get; set; }

        public ICollection<SignDTO> Signs { get; set; }

        public ICollection<TemplateDTO> Templates { get; set; }
    }
}
