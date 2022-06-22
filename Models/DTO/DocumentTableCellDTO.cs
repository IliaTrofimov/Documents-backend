namespace Documents.Models.DTO
{
    public partial class DocumentTableCellDTO
    {
        public int DocumentId { get; set; }

        public string Value { get; set; }

        public int Field { get; set; }

        public int Row { get; set; }

        public int Col { get; set; }
    }
}
