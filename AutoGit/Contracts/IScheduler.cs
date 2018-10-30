using System;
using System.Linq.Expressions;

namespace AutoGit.Core.Contracts
{
    public interface IScheduler
    {
        Unit AddCronJob(Expression<Action> methodCall, Func<string> cron);
        void Dispose();
    }
}