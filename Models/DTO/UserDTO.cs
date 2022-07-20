using Documents.Models.Entities;

namespace Documents.Models.DTO
{
    public partial class UserDTO
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fathersname { get; set; }
        public byte Permissions { get; set; }
        public Position Position { get; set; }
        public int PositionId { get; set; }
        public string Email { get; set; }
        public string CWID { get; set; }
    }

    public partial class UserDTOFull : UserDTO
    {
        public string EmployeeType { get; set; }
        public string LeadingSubgroup { get; set; }
        public string ExternalCompany { get; set; }
        public string OrgName { get; set; }
        public string CompanyCode { get; set; }
        public string ManagerCWID { get; set; } = null;
        public UserDTO Manager { get; set; }
    }
}
