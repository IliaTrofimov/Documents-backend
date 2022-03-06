using System.Collections.Generic;

namespace Documents_backend.Models
{
    public partial class UserDTO: User
    {
        public new ICollection<Document> Document { get => null; }

        public new ICollection<Sign> Sign { get => null; }

        public new ICollection<Template> Template { get => null; }
    }
}
