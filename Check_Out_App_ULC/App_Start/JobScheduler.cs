using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using Check_Out_App_ULC.Controllers;
using Check_Out_App_ULC.Controllers.Api;
using Check_Out_App_ULC.Models;
using System.Net.Mail;
using System.Net;

namespace Check_Out_App_ULC.App_Start
{
    public class EmailJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            // job details
            ReportController controller = new ReportController();
            controller.EmailLateStudents();
            controller.EmailLongtermDueStudents();
            controller.EmailEndOfDayReport();
            tb_LongtermWaitlistController wl = new tb_LongtermWaitlistController();
            wl.WaitlistStatusCheck();
        }
    }

    public class SlingJob : IJob
    {
        private readonly Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();

        public void Execute(IJobExecutionContext context)
        {
            // job details
            SlingController sling = new SlingController();
            var s = sling.SlingGetArticles("0"); // 0 is the newsfeed channel

            // reset the cache before storing
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [tb_SlingCache]");

            foreach (var item in s)
            {
                var slingEntry = new tb_SlingCache();
                slingEntry.PostId = item.PostId;
                slingEntry.UserId = item.UserId;
                slingEntry.PostContent = item.PostContent;
                slingEntry.PostedBy = item.PostedBy;
                slingEntry.PostComments = item.PostComments;
                slingEntry.Posted = item.Posted;
                slingEntry.Retrieved = item.Retrieved;
                db.tb_SlingCache.Add(slingEntry);
                db.SaveChanges();
            }
        }
    }

    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            // email job settings
            IJobDetail job = JobBuilder.Create<EmailJob>()
                .WithIdentity("job1", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(22, 15))
                  )
                .Build();

            // Sling job settings
            IJobDetail slingJob = JobBuilder.Create<SlingJob>()
                .WithIdentity("job2", "group2")
                .Build();

            ITrigger slingTrigger = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group2")
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInMinutes(15)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(07, 30))
                  )
                .Build();

            // schedule the jobs
            scheduler.ScheduleJob(job, trigger);
            scheduler.ScheduleJob(slingJob, slingTrigger);
        }
    }
}