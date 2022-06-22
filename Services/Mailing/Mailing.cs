using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MailKit.Net.Smtp;
using MimeKit;

using Documents.Models.Entities;
using Documents.Services.Mailing.Templates;
using Documents.Properties;


namespace Documents.Services.Mailing
{
    public abstract class Mailing
    {
        private static string EmailHost { get; set; } = Settings.Default.EmailHost;
        private static int EmailPort { get; set; } = Settings.Default.EmailPort;
        private static string EmailPassword { get; set; } = Settings.Default.YandexPassword;
        private static string EmailLogin { get; set; } = Settings.Default.YandexLogin;
        private static string EmailFrom { get; set; } = Settings.Default.EmailFrom;
        private static string BaseUrl { get; set; } = Settings.Default.BaseUrl;

        private static Regex RegexEmail = new Regex(@"^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@([a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*[a-z]{2,}$");



        /// <summary> Рассылает уведомление о статусе подписи документа </summary>
        /// <param name="sign">
        /// Если sign.Signed == null, то высылает подписанту уведомление о необходимости просмотреть документ.
        /// если sign.Signed != null, то высылает инициатору уведомление об изменении статуса подписи.
        /// </param>
        public static void SignatoryNotification(Sign sign)
        {
            if (!RegexEmail.IsMatch(sign.User.Email)) return;

            using (var client = new SmtpClient())
            {
                client.Connect(EmailHost, EmailPort, true);
                client.Authenticate(EmailLogin, EmailPassword);

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(EmailFrom, Settings.Default.EmailLogin));
                message.Subject = "Визирование";

                message.To.Add(new MailboxAddress(sign.User.GetFIO(), sign.User.Email));
                if (sign.Signed == null)
                    message.Body = new TextPart("html") { Text = new NewSignMail(sign, BaseUrl).TransformText() };
                else
                    message.Body = new TextPart("html") { Text = new UpdatedSignMail(sign, BaseUrl).TransformText() };

                client.SendAsync(message).Start();
                client.Disconnect(true);
            }
        }

        public static void Test(string text = "test message", string subject = "Test")
        {
            using (var client = new SmtpClient())
            {
                client.Connect(EmailHost, EmailPort, true);
                client.Authenticate(EmailLogin, EmailPassword);

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(EmailFrom, Settings.Default.EmailLogin));
                message.Subject = subject;

                message.To.Add(new MailboxAddress("Илья Трофимов", Settings.Default.EmailLogin));
                message.Body = new TextPart("plain") { Text = text };

                client.SendAsync(message).Start();
                client.Disconnect(true);
            }
        }


        /// <summary> Рассылает владельцам документов уведомления с указанием даты истечения срока действия документа </summary>
        /// <returns> Количество успешно отправленных писем </returns>
        public static int ExpireNotification(IEnumerable<Document> documents)
        {
            int count = 0;

            using (var client = new SmtpClient())
            {
                client.Connect(EmailHost, EmailPort, true);
                client.Authenticate(EmailLogin, EmailPassword);

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(EmailFrom, Settings.Default.EmailLogin));
                message.Subject = "Напоминание";

                foreach (Document document in documents)
                {
                    if (RegexEmail.IsMatch(document.Author.Email))
                    {
                        message.To.Add(new MailboxAddress(document.AuthorName, document.Author.Email));
                        message.Body = new TextPart("html") { Text = new ExpireMail(document, BaseUrl).TransformText() };

                        try
                        {
                            client.SendAsync(message).Start();
                            count++;
                        }
                        catch (Exception) { }
                    }
                }

                client.Disconnect(true);
            }
            return count;
        }


    }
}
