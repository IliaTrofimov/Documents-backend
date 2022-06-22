namespace Documents.Models.POST
{
    public class SignPOST
    {
        public int DocumentId;
        public int UserId;
        public int InitiatorId;
        public bool? Signed;
    }
}