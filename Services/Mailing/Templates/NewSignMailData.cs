namespace Documents.Services.MailTemplates
{
    partial class NewSignMail 
    {
        private int DocumentId;
        private string BaseUrl;
        private string SignerFirstname;
        private string SignerFathersname;
        private string InitiatorShortname;
        private string DocumentName;

        public NewSignMail(Models.Entities.Sign signatory, string baseUrl)
        {
            DocumentName = signatory.Document.Name;
            DocumentId = signatory.DocumentId;
            SignerFirstname = signatory.User.Firstname;
            SignerFathersname = signatory.User.Fathersname;
            InitiatorShortname = signatory.Initiator.GetFIO();
            BaseUrl = baseUrl;
        }
    }
}
