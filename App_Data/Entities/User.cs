using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Documents_backend.Models.Entities
{
    public class User
    {   
        public int Id { get; set; }
        public string Name { get; set; }

        
        public virtual ICollection<DocumentInfo> Documents { get; set; }

        public virtual ICollection<Template> Templates { get; set; }

        public virtual ICollection<Sign> Signs { get; set; }
    }
}