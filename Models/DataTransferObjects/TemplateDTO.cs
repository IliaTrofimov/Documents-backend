using System;

namespace Documents_backend.Models
{
    public partial class TemplateDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool Depricated { get; set; }

        public User User { get; set; }
    }
}
