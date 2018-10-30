using System;
using System.Collections.Generic;
using System.Linq;
using AutoGit.Core;
using Hangfire;
using Microsoft.Extensions.CommandLineUtils;
using Optional;
using Optional.Linq;
using static AutoGit.DotNet.FuncExtensions;

namespace AutoGit.DotNet
{
    public class StartCommand
    {
        private readonly StartCommandOptions CmdOptions;

        public Option<string, Failure> Username => CmdOptions.UserName.GetValue().WithException<Failure>("User name is required.");
        public Option<string, Failure> Email => CmdOptions.Email.GetValue().WithException<Failure>("User email is required.");
        public Option<string, Failure> Source => CmdOptions.Source.GetValue().WithException<Failure>("Path to repository is required.");

        public StartCommand(CommandLineApplication cmd)
        {
            cmd.AddHelp();
            CmdOptions = StartCommandOptions.Create(cmd);

            cmd.OnExecute(func(Start));
        }

        private int Start()
        {
            var username = CmdOptions.UserName.GetValue().WithException<Failure>("User name is required.");
            var email = CmdOptions.Email.GetValue().WithException<Failure>("User email is required.");
            var source = CmdOptions.Source.GetValue().WithException<Failure>("Path to repository is required.");
            var commitInterval = CmdOptions.CommitInterval.GetValue(Convert.ToInt32).ValueOr(() => 5);

            // TODO figure out how to retrieve user from .gitconfig

            using (var scheduler = Scheduler.Create())
            {
                var jobs = new List<Option<Func<Unit>, Failure>>
                {
                    StartCommitJob(username, email, source, commitInterval, scheduler),
                };

                var fails = jobs.Where(j => !j.HasValue);
                if (fails.Any())
                {
                    var _ = fails.Select(fail => Unit.SideEffect(
                            () => fail.MatchNone(err => Console.WriteLine(err.Message))))
                        .ToList();
                    return ExitCodes.Failure;
                }

                jobs.ForEach(job => job.MatchSome(f => f()));

                Console.WriteLine("AutoGit is running");
            }

            return ExitCodes.Success;

        }

        private static Option<Func<Unit>, Failure> StartCommitJob(Option<string, Failure> username, Option<string, Failure> email, Option<string, Failure> source, int commitInterval, Scheduler scheduler) =>
            from un in username
            from ue in email
            from src in source
            select func(() => Unit.SideEffect(() =>
            {
                scheduler.AddCronJob(
                    methodCall: () => CommitAll(un, ue, src),
                    cron: () => Cron.MinuteInterval(commitInterval));

                scheduler.AddCronJob(
                    // ReSharper disable once AccessToDisposedClosure
                    methodCall: () => PrintRemainingTime(nameof(CommitAll), scheduler),
                    cron: () => Cron.MinuteInterval(1));
            }));

        public static Unit CommitAll(string name, string email, string source) =>
            Unit.SideEffect(() =>
            {
                var author = new GitUser(name, email);
                var repository = new RepositorySettings(source, author);
                var comitter = new Comitter(repository);
                comitter.CommitChanges();
            });

        public static void PrintRemainingTime(string jobMethodName, Scheduler scheduler) =>
            Unit.SideEffect(() =>
            {
                var time = scheduler.GetTimeToNextRun(jobMethodName);
                Console.WriteLine($"{time:m\\:ss} minutes to next {jobMethodName} run.");
            });

    }
}