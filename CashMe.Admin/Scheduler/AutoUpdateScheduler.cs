using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace CashMe.Admin.Scheduler
{
    public class AutoUpdateScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();


            IJobDetail job = JobBuilder.Create<AutoUpdate>().Build();
            IJobDetail job1 = JobBuilder.Create<AutoUpdateTarget>().Build();

            //update all status wallet = 0, user online = 0
            ITrigger trigger = TriggerBuilder.Create()
              .StartNow()
              .WithSimpleSchedule(x => x
              .WithIntervalInMinutes(4)
              .RepeatForever())
              .Build();
            ITrigger trigger1 = TriggerBuilder.Create()
              .StartAt(new DateTime(2017, 7, 30, 00, 01, 0))
              .WithSimpleSchedule(x => x
              .WithIntervalInHours(24)
              .RepeatForever())
              .Build();
            scheduler.ScheduleJob(job, trigger);
            scheduler.ScheduleJob(job1, trigger1);

        }
    }
}