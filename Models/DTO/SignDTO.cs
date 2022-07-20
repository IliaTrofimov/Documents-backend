using System;

namespace Documents.Models.DTO
{
    public partial class SignDTO
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public int SignerPositionId { get; set; }
        public string UserCWID { get; set; } = null;
        public string InitiatorCWID { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string SignerShortname { get; set; }
        public string InitiatorShortname { get; set; }
        public string PositionName { get; set; }
        public bool? Signed { get; set; }
    }
}
