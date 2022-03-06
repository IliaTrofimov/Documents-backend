using System;
using System.Collections.Generic;

namespace Documents_backend.Models
{
    public partial class TemplateDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool Depricated { get; set; }

        public ICollection<Document> Document { get => null; }

        public string TemplateType { get; set; }

        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
    }
}
