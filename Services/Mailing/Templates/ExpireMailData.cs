using System;

namespace Documents.Services.Mailing.Templates
{
    partial class ExpireMail
    {
        private int DocumentId;
        private string BaseUrl;
        private string Firstname;
        private string Fathersname;
        private string DocumentName;
        private DateTime? Expire;

        public ExpireMail(Models.Entities.Document document, string baseUrl)
        {
            DocumentName = document.Name;
            DocumentId = document.Id;
            Firstname = document.Author.Firstname;
            Fathersname = document.Author.Fathersname;
            Expire = document.ExpireDate;
            BaseUrl = baseUrl;
        }
    }
}
