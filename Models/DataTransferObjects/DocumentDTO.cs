using System;


namespace Documents_backend.Models
{
    public partial class DocumentDTO
    {      

        public int Id { get; set; }

        public string Name { get; set; }

        public int TemplateId { get; set; }

        public int Type { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime? ExpireDate { get; set; }

        public User User { get; set; }

    }
}
