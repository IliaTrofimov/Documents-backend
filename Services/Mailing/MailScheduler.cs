using Quartz;
using Quartz.Impl;
using System;

namespace Documents.Services.MailingSchedule
{
    public enum TimeUnits
    {
        Seconds, Minutes, Hours 
    }

    public class EmailScheduler
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailSender>().Build();

            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                .StartNow()                            // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInSeconds(20)          // через 1 минуту
                    .WithRepeatCount(2))                   // бесконечное повторение
                .Build();                               // создаем триггер

            await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
        }
    }

    public class MailScheduler<T> where T : IJob
    {
        public static async void Start(Mailing mailing, int steps, DateTimeOffset end, DateTimeOffset start )
        {
            JobDataMap data = new JobDataMap();
            data.Add("Mailing", mailing);

            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<T>().Build();
            TriggerBuilder trigger = TriggerBuilder.Create()
                .WithIdentity("ExpireMailing", "MailingGroup")
                .UsingJobData(data)
                .StartAt(start);

            int seconds = (int)(start - end).TotalSeconds;
            trigger.WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(seconds / steps)
                    .RepeatForever());

            await scheduler.ScheduleJob(job, trigger.Build());
        }

        public static async void Start(Mailing mailing, int timeStep, TimeUnits timeUnits = TimeUnits.Seconds)
        {
            JobDataMap data = new JobDataMap();
            data.Add("Mailing", mailing);

            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<T>().Build();
            TriggerBuilder trigger = TriggerBuilder.Create()
                .WithIdentity("ExpireMailing", "MailingGroup")
                .UsingJobData(data)
                .StartNow();

            int seconds = 0;
            switch (timeUnits)
            {
                case TimeUnits.Seconds: seconds = timeStep; break;
                case TimeUnits.Minutes: seconds = timeStep * 60; break;
                case TimeUnits.Hours: seconds = timeStep * 3600; break;
            }
            trigger.WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(seconds)
                    .RepeatForever());

            await scheduler.ScheduleJob(job, trigger.Build());
        }

        public static async void Start(Mailing mailing, int timeStep, DateTimeOffset start, TimeUnits timeUnits = TimeUnits.Seconds)
        {
            JobDataMap data = new JobDataMap();
            data.Add("Mailing", mailing);

            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<T>().Build();
            TriggerBuilder trigger = TriggerBuilder.Create()
                .WithIdentity("ExpireMailing", "MailingGroup")
                .UsingJobData(data)
                .StartAt(start);

            int seconds = 0;
            switch (timeUnits)
            {
                case TimeUnits.Seconds: seconds = timeStep; break;
                case TimeUnits.Minutes: seconds = timeStep * 60; break;
                case TimeUnits.Hours: seconds = timeStep * 3600; break;
            }
            trigger.WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(seconds)
                    .RepeatForever());

            await scheduler.ScheduleJob(job, trigger.Build());
        }
    }
}