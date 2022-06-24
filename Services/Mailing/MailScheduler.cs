using Quartz;
using Quartz.Impl;

namespace Documents.Services.MailingSchedule
{
    public class MailScheduler
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<ExpireMailSender>().Build();

            ITrigger trigger = TriggerBuilder.Create()  
                .WithIdentity("ExpireMailing", "MailingGroup")
                .StartNow()
                .WithSimpleSchedule(x => x 
                    .WithIntervalInMinutes(1)
                    .RepeatForever()) 
                .Build();

            await scheduler.ScheduleJob(job, trigger);   
        }
    }
}