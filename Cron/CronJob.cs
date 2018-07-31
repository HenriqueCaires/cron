using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cron
{
    public interface ICronJob
    {
        void Run(DateTime date_time);
        void Cancel();
    }

    public class CronJob : ICronJob
    {
        private readonly ICronSchedule _cron_schedule = new CronSchedule();
        private readonly Action _action;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;
        private object _lock = new object();

        public CronJob(string schedule, Action action)
        {
            _cron_schedule = new CronSchedule(schedule);
            _action = action;
            _cancellationTokenSource = new CancellationTokenSource();
            _task = new Task(action, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);

        }
        
        public void Run(DateTime dateTime)
        {
            lock (_lock)
            {
                if (_cron_schedule.IsTime(dateTime) && _task.Status != TaskStatus.Running)
                {
                    _task = new Task(_action, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
                    _task.Start();
                }
            }
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

    }
}
