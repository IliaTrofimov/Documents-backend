using System;
using System.Collections.Generic;
using Documents.Models.Entities;

namespace Documents.Models.DTO
{
    public abstract partial class DocumentDTOBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public int Type { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string AuthorCWID { get; set; }
    }

    public partial class DocumentDTO : DocumentDTOBase
    {      
        public string AuthorName { get; set; }
        public string TemplateName { get; set; }
    }

    public partial class DocumentDTOFull : DocumentDTOBase
    {
        public UserDTO Author { get; set; }
        public IEnumerable<DocumentDataItem> DocumentDataItems { get; set; } = new List<DocumentDataItem>();
        public Template Template { get; set; }
    }
}
