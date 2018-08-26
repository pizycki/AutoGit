using System;
using System.Linq.Expressions;
using AutoGit.Contracts;
using Hangfire;
using Hangfire.MemoryStorage;

namespace AutoGit
{
    public class Scheduler : IDisposable, IScheduler
    {
        private readonly BackgroundJobServer _server;

        private Scheduler()
        {
            _server = new BackgroundJobServer();
        }

        public void AddCronJob(Expression<Action> methodCall, Func<string> cron)
        {
            RecurringJob.AddOrUpdate(methodCall, cron);
        }

        public void Dispose()
        {
            _server?.Dispose();
        }

        public static Scheduler Create()
        {
            if (!storageAddedToConfiguration)
            {
                GlobalConfiguration.Configuration.UseMemoryStorage();
            }

            return new Scheduler();
        }

        private static bool storageAddedToConfiguration = false;
    }
}
