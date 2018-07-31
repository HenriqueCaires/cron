using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cron
{
    public interface ICronDaemon
    {
        void Add(string schedule, Action action);
        void Start();
        void Stop(bool clearJobs = true);
    }
    public class CronDaemon : ICronDaemon
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task manager;
        private readonly List<ICronJob> cron_jobs = new List<ICronJob>();
        private DateTime _last = DateTime.Now;

        public CronDaemon()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Clear()
        {
            cron_jobs.Clear();
        }

        public void Add(string schedule, Action action)
        {
            cron_jobs.Add(new CronJob(schedule, action));
        }

        public void Start()
        {
            manager = new Task(Run, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            manager.Start();
            manager.ContinueWith((t) =>
            {
                t.Dispose();
            });
        }

        public void Stop(bool clearJobs = true)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            foreach (var job in cron_jobs)
                job.Cancel();

            if (clearJobs)
                Clear();
        }

        private void Run()
        {
            while (true)
            {
                if (DateTime.Now.Minute != _last.Minute)
                {
                    _last = DateTime.Now;
                    foreach (ICronJob job in cron_jobs)
                        job.Run(DateTime.Now);
                }
                Task.Delay(5000).Wait();
            }
        }
    }
}