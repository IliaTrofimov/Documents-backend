using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

using Documents.Models.Entities;
using Documents.Properties;
using Documents.Services.Mailing.Templates;

namespace Documents.Services.Mailing
{
    /// <summary>
    /// Использует SSL, если заданы свойства EmailPassword и EmailLogin, если они пусты, использует TLS
    /// </summary>
    public class MailingClient
    {
        public string EmailHost { get; set; } = Settings.Default.EmailHost;
        public int EmailPort { get; set; } = Settings.Default.EmailPort;

        /// <summary>Оставьте пустым, чтобы использовать TLS</summary>
        public string EmailPassword { get; set; } = Settings.Default.EmailPassword;

        /// <summary>Оставьте пустым, чтобы использовать TLS</summary>
        public string EmailLogin { get; set; } = Settings.Default.EmailLogin;
        public string EmailFrom { get; set; } = Settings.Default.EmailFrom;
        public string EmailFromName { get; set; } = Settings.Default.EmailFromName;
        public string BaseUrl { get; set; } = Settings.Default.BaseUrl;

        private static readonly Regex RegexEmail = new Regex(@"^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@([a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*[a-z]{2,}$");

        public MailingClient() { }

        public MailingClient(string emailFrom, string emailPassword, string emailHost, int emailPort = 465, string baseUrl = null)
        {
            EmailHost = emailHost;
            EmailPort = emailPort;
            EmailPassword = emailPassword;
            EmailLogin = emailFrom;
            EmailFrom = emailFrom;
            BaseUrl = baseUrl;
        }

        public MailingClient(string emailFrom, string emailLogin, string emailPassword, string emailHost, int emailPort = 465, string baseUrl = null)
        {
            EmailHost = emailHost;
            EmailPort = emailPort;
            EmailPassword = emailPassword;
            EmailLogin = emailLogin;
            EmailFrom = emailFrom;
            BaseUrl = baseUrl;
        }



        /// <summary> Рассылает уведомление о статусе подписи документа </summary>
        /// <param name="sign">
        /// Если sign.Signed == null, то высылает подписанту уведомление о необходимости просмотреть документ.
        /// если sign.Signed != null, то высылает инициатору уведомление об изменении статуса подписи.
        /// </param>
        public async Task SignatoryNotificationAsync(Sign sign)
        {
            if (!RegexEmail.IsMatch(sign.User.Email)) return;

            using (var client = new SmtpClient())
            {
                if (EmailLogin == "" || EmailPassword == "")
                    await client.ConnectAsync(EmailHost, EmailPort, MailKit.Security.SecureSocketOptions.StartTls);
                else
                {
                    await client.ConnectAsync(EmailHost, EmailPort, true);
                    client.Authenticate(EmailLogin, EmailPassword);
                }

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(EmailFromName, EmailFrom));
                message.Subject = "Визирование";

                message.To.Add(new MailboxAddress(sign.User.GetFIO(), sign.User.Email));
                if (sign.Signed == null)
                    message.Body = new TextPart("html") { Text = new NewSignMail(sign, BaseUrl).TransformText() };
                else
                    message.Body = new TextPart("html") { Text = new UpdatedSignMail(sign, BaseUrl).TransformText() };

                await client.SendAsync(message);
                client.Disconnect(true);
            }
            return;
        }

        /// <summary> Рассылает уведомление о статусе подписи документа </summary>
        /// <param name="sign">
        /// Если sign.Signed == null, то высылает подписанту уведомление о необходимости просмотреть документ.
        /// если sign.Signed != null, то высылает инициатору уведомление об изменении статуса подписи.
        /// </param>
        public void SignatoryNotification(Sign sign)
        {
            if (!RegexEmail.IsMatch(sign.User.Email)) return;

            using (var client = new SmtpClient())
            {
                if (EmailLogin == "" || EmailPassword == "")
                    client.Connect(EmailHost, EmailPort, MailKit.Security.SecureSocketOptions.StartTls);
                else
                {
                    client.Connect(EmailHost, EmailPort, true);
                    client.Authenticate(EmailLogin, EmailPassword);
                }

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(EmailFromName, EmailFrom));
                message.Subject = "Визирование";

                message.To.Add(new MailboxAddress(sign.User.GetFIO(), sign.User.Email));
                if (sign.Signed == null)
                    message.Body = new TextPart("html") { Text = new NewSignMail(sign, BaseUrl).TransformText() };
                else
                    message.Body = new TextPart("html") { Text = new UpdatedSignMail(sign, BaseUrl).TransformText() };

                client.Send(message);
                client.Disconnect(true);
            }
            return;
        }

        public async Task SendAsync(string email, string name, string subject = "test mail", string text = "test mail")
        {
            if (!RegexEmail.IsMatch(email)) return;

            using (var client = new SmtpClient())
            {
                if (EmailLogin == "" || EmailPassword == "")
                    await client.ConnectAsync(EmailHost, EmailPort, MailKit.Security.SecureSocketOptions.StartTls);
                else
                {
                    await client.ConnectAsync(EmailHost, EmailPort, true);
                    client.Authenticate(EmailLogin, EmailPassword);
                }

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(EmailFromName, EmailFrom));
                message.Subject = subject;
                message.To.Add(new MailboxAddress(name, email));
                message.Body = new TextPart("plain") { Text = text };
                
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }

        public void Send(string email, string name, string subject = "test mail", string text = "test mail")
        {
            if (!RegexEmail.IsMatch(email)) return;

            using (var client = new SmtpClient())
            {
                if (EmailLogin == "" || EmailPassword == "")
                    client.Connect(EmailHost, EmailPort, MailKit.Security.SecureSocketOptions.StartTls);
                else
                {
                    client.Connect(EmailHost, EmailPort, true);
                    client.Authenticate(EmailLogin, EmailPassword);
                }

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(EmailFromName, EmailFrom));
                message.Subject = subject;
                message.To.Add(new MailboxAddress(name, email));
                message.Body = new TextPart("plain") { Text = text };

                client.Send(message);
                client.Disconnect(true);
            }
        }

        /// <summary> Рассылает владельцам документов уведомления с указанием даты истечения срока действия документа </summary>
        /// <returns> Количество успешно отправленных писем </returns>
        public async Task<int> ExpireNotificationAsync(IEnumerable<Document> documents)
        {
            int count = 0;
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(EmailFromName, EmailFrom));
            message.Subject = "Напоминание";

            foreach (Document document in documents)
            {
                using (var client = new SmtpClient())
                {
                    if (EmailLogin == "" || EmailPassword == "")
                        await client.ConnectAsync(EmailHost, EmailPort, MailKit.Security.SecureSocketOptions.StartTls);
                    else
                    {
                        await client.ConnectAsync(EmailHost, EmailPort, true);
                        client.Authenticate(EmailLogin, EmailPassword);
                    }

                    if (RegexEmail.IsMatch(document.Author.Email))
                    {
                        message.To.Add(new MailboxAddress(document.Author.GetFIO(), document.Author.Email));
                        message.Body = new TextPart("html") { Text = new ExpireMail(document, BaseUrl).TransformText() };

                        try
                        {
                            await client.SendAsync(message);
                            count++;
                        }
                        catch (Exception) { }
                    }
                    client.Disconnect(true);
                }
            }
            return count;
        }
    }
}
