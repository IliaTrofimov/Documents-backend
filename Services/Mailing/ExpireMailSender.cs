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
            await Mailing.ExpireNotification(db.Documents.Where(d => 
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
}