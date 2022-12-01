using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quartz;
using Quartz.Impl;



namespace AlarmReportMaker
{
    public class jobSchedule
    {
        public static EventHandler jobWorking;

        private static jobSchedule instance;
        static StdSchedulerFactory schedulerFactory;
        public static jobSchedule getInstance(string _schedule)
        {
            if (instance == null)
            {
                instance = new jobSchedule(_schedule);
                schedulerFactory = new StdSchedulerFactory();
                //schedulerFactory.GetScheduler().Start();
            }
            return instance;
        }

        private jobSchedule(string _schedule)
        {
            MainAsync(_schedule);
            //Task.Factory.StartNew(() => );
        }

        public static async Task MainAsync(string _schedule)
        {
            schedulerFactory = new StdSchedulerFactory();
            var scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();
            Console.WriteLine($"任務排程器已啟動");

            //建立作業和觸發器
            var jobDetail = JobBuilder.Create<HelloQuartzJob>().Build();
            var trigger = TriggerBuilder.Create()
                                        .WithSimpleSchedule(m =>
                                        {
                                            m.RepeatForever();
                                        })
                                        .WithCronSchedule(_schedule)
                                        .Build();

            //新增排程
            await scheduler.ScheduleJob(jobDetail, trigger);
        }
        public class HelloQuartzJob : IJob
        {
            public Task Execute(IJobExecutionContext context)
            {
                return Task.Factory.StartNew(() =>
                {
                    jobWorking?.Invoke(this, null);
                });
            }
        }
    }
}
