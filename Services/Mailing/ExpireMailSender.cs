using Documents.Models;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Documents.Services.MailingSchedule
{
    public class ExpireMailSender : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            DataContext db = new DataContext();
            DateTime now = DateTime.Now;
            Mailing mailing = (Mailing)context.JobDetail.JobDataMap["Mailing"];
            await mailing.ExpireNotification(db.Documents.Where(d => 
                d.ExpireDate.HasValue &&
                ((d.ExpireDate.Value - now).Days == 0 ||
                (d.ExpireDate.Value - now).Days == 1 ||
                (d.ExpireDate.Value - now).Days == 3 ||
                (d.ExpireDate.Value - now).Days == 7 ||
                (d.ExpireDate.Value - now).Days == 14 ||
                (d.ExpireDate.Value - now).Days == 30)
            ));
        }
    }

    public class SimpleMailSender : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Mailing mailing = (Mailing)context.JobDetail.JobDataMap["Mailing"];
            string emailTo = (string)context.JobDetail.JobDataMap["EmailTo"];
            string nameTo = (string)context.JobDetail.JobDataMap["NameTo"];
            
            object text, subject;
            context.JobDetail.JobDataMap.TryGetValue("Subject", out subject);
            context.JobDetail.JobDataMap.TryGetValue("Text", out text);
            await mailing.SendAsync(emailTo, nameTo, subject is null ? null : (string)subject, text is null ? null : (string)text);
        }
    }
}