using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace GKHNNC.Jobs
{
    public class AvtoScanZadacha
    {
            public static async void Start()
            {
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<AvtoScanJob>().Build();

                ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("triggerAvtoScan1", "group1")     // идентифицируем триггер с именем и группой
                    .StartNow()                         // запуск сразу после начала выполнения
                    .WithSimpleSchedule(x => x          // настраиваем выполнение действия
                    .WithIntervalInHours(1)          // через каждый час
                    .RepeatForever())                   // бесконечное повторение
                    .Build();                           // создаем триггер

                await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
            }
        
    

}
}