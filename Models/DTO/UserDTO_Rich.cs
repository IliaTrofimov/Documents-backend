using Documents.Entities;
using System.Collections.Generic;

namespace Documents.Models.DTO
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

        public ICollection<DocumentDTO> Documents { get; set; }

        public ICollection<SignDTO> Signs { get; set; }

        public ICollection<TemplateDTO> Templates { get; set; }

        public Position Position { get; set; }

        public int PositionId { get; set; }
        public string Email { get; set; }
    }
}
