using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using Check_Out_App_ULC.Controllers;
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

    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailJob>()
                .WithIdentity("job1", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(21, 15))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}