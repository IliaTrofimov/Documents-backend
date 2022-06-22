namespace Documents.Services.MailTemplates
{
    partial class UpdatedSignMail
    {
        private int DocumentId;
        private string BaseUrl;
        private string InitiatorFirstname;
        private string InitiatorFathersname;
        private string SignerShortname;
        private string DocumentName;
        private string Message;

        public UpdatedSignMail(Models.Entities.Sign signatory, string baseUrl)
        {
            DocumentName = signatory.Document.Name;
            DocumentId = signatory.DocumentId;
            InitiatorFirstname = signatory.User.Firstname;
            InitiatorFathersname = signatory.User.Fathersname;
            SignerShortname = signatory.Initiator.GetFIO();
            Message = signatory.Signed == true ? "подписал" : "отклонил";
            BaseUrl = baseUrl;
        }
    }
}
