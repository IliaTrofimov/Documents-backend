namespace Documents_backend.Models
{
    public partial class SignDTO
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }

        public int UserId { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fathersname { get; set; }

        public bool Signed { get; set; }
    }
}
