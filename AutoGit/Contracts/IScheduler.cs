﻿using System;
using System.Linq.Expressions;

namespace AutoGit.Contracts
{
    public interface IScheduler
    {
        void AddCronJob(Expression<Action> methodCall, Func<string> cron);
        void Dispose();
    }
}