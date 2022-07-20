namespace Documents.Models.POST
{
    public class SignPOST
    {
        public int DocumentId;
        public string UserCWID = null;
        public string InitiatorCWID;
        public bool? Signed;
    }
}