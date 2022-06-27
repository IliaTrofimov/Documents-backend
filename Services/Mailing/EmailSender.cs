using Documents.Properties;
using Quartz;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace Documents.Services
{
    public class EmailSender : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (MailMessage message = new MailMessage(Settings.Default.EmailFrom, Settings.Default.EmailFrom))
            {
                message.Subject = "Новостная рассылка";
                message.Body = "Новости сайта: бла бла бла";
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = Settings.Default.EmailHost,
                    Port = Settings.Default.EmailPort,
                    Credentials = new NetworkCredential(Settings.Default.EmailLogin, Settings.Default.EmailPassword)
                })
                {
                    client.Send(message); 
                }
            }
        }
    }
}