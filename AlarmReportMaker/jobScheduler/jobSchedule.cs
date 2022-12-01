using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;


namespace jobScheduler
{
    public class jobSchedule
    {
        jobSchedule instance;
        private static StdSchedulerFactory schedulerFactory;
        
        public jobSchedule getInstance()
        {
            if (instance == null)
                instance = new jobSchedule();
            return instance;
        }
        jobSchedule()
        {
            schedulerFactory = new StdSchedulerFactory();
        }
        public IScheduler getAllSchedule()
        {
            IScheduler result = null;
            Task.Run(() =>
            {
                result = (IScheduler)schedulerFactory.GetAllSchedulers();
            }).Wait();
            return result;
        }
    }
    class Job
    {

        static EventHandler jobWorking;
        StdSchedulerFactory schedulerFactory;
        string jobName;
        string jobSchedule;
        bool jobIsForever;
        int jobCount;
        Job(ref StdSchedulerFactory _schedulerFactory, string _jobName, string _jobSchedule, bool _jobIsForever, int _jobCount = 0)
        {
            schedulerFactory = _schedulerFactory;
            jobName = _jobName;
            jobSchedule = _jobSchedule;
            jobIsForever = _jobIsForever;
            if (!_jobIsForever)
                jobCount = _jobCount;
        }
        public async Task start()
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var jobDetail = JobBuilder.Create<HelloQuartzJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()
                                        .WithSimpleSchedule(m => {
                                            m.RepeatForever();
                                        })
                                        .WithCronSchedule(jobSchedule)
                                        .Build();
            //新增排程
            await scheduler.ScheduleJob(jobDetail, trigger);
        }
        public void stop()
        {
            schedulerFactory.GetScheduler();
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
