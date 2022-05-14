namespace Documents_backend.Models
{
    public partial class UserDTO
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Fathersname { get; set; }

        public byte Permissions { get; set; }

        public Position Position { get; set; }
        public int? PositionId { get; set; }
    }
}
