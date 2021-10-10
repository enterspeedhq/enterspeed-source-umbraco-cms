using System.Collections.Generic;
using System.Timers;

namespace Enterspeed.Source.UmbracoCms.V7.Services
{
    public class SchedulerService
    {
        public List<Timer> Tasks { get; } = new List<Timer>();

        /// <summary>
        /// Schedule task for running repeatedly for a given interval.
        /// </summary>
        /// <param name="interval">Interval in miliseconds.</param>
        /// <param name="action">Task to be run.</param>
        public void ScheduleTask(double interval, ElapsedEventHandler action)
        {
            var timer = new Timer(interval)
            {
                AutoReset = true,
                Enabled = true
            };

            timer.Elapsed += action;

            Tasks.Add(timer);
        }
    }
}
