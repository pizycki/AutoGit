using System;
using System.Linq.Expressions;
using AutoGit.Core.Contracts;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Storage;

namespace AutoGit.Core
{
    public class Scheduler : IDisposable, IScheduler
    {
        private readonly BackgroundJobServer _server;

        private Scheduler()
        {
            _server = new BackgroundJobServer();
        }

        public Unit AddCronJob(Expression<Action> methodCall, Func<string> cron) => 
            Unit.SideEffect(() => RecurringJob.AddOrUpdate(methodCall, cron));

        public TimeSpan GetTimeToNextRun(string jobName)
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var recurringJobs = connection.GetRecurringJobs();
                var job = recurringJobs.Find(j => j.Id.Contains(jobName));

                if (job.NextExecution.HasValue)
                {
                    return (DateTime.UtcNow - job.NextExecution.Value).Negate();
                }

                return default;
            }
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
