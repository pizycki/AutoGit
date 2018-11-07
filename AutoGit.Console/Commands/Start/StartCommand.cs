using AutoGit.Core;
using Hangfire;
using Microsoft.Extensions.CommandLineUtils;
using static System.Console;
using static System.Convert;
using static AutoGit.DotNet.FuncExtensions;

namespace AutoGit.DotNet
{
    public class StartCommand
    {
        private readonly StartCommandOptions CmdOptions;

        public StartCommand(CommandLineApplication cmd)
        {
            cmd.AddHelp();
            CmdOptions = StartCommandOptions.Create(cmd);

            cmd.OnExecute(func(Start));
        }

        private int Start()
        {
            var repository = CmdOptions.Source.GetValue().ValueOr(".");
            var commitInterval = CmdOptions.CommitInterval.GetValue(ToInt32).ValueOr(() => 5);
            var push = CmdOptions.Push.GetValue(ToBoolean).ValueOr(() => false);

            using (var scheduler = Scheduler.Create())
            {
                //StartCommitJob(repository, commitInterval, push,scheduler);

                CommitJobBody(repository, push);

                WriteLine("AutoGit is running");
            }

            return ExitCodes.Success;
        }

        private static Unit StartCommitJob(string repository, int commitInterval, bool push, Scheduler scheduler) =>
            Unit.SideEffect(() =>
            {
                scheduler.AddCronJob(
                    methodCall: () => CommitJobBody(repository, push),
                    cron: () => Cron.MinuteInterval(commitInterval));

                scheduler.AddCronJob(
                    // ReSharper disable once AccessToDisposedClosure
                    methodCall: () => PrintRemainingTime(nameof(CommitJobBody), scheduler),
                    cron: () => Cron.MinuteInterval(1));
            });

        public static Unit CommitJobBody(string repo, bool push) =>
            Unit.SideEffect(() =>
            {
                Git.StageAll.WithRepository(repo).Go();
                Git.Commit.WithRepository(repo).Go();
                if (push) Git.Push.WithRepository(repo).Go();
            });

        public static void PrintRemainingTime(string jobMethodName, Scheduler scheduler) =>
            Unit.SideEffect(() =>
            {
                var time = scheduler.GetTimeToNextRun(jobMethodName);
                WriteLine($"{time:m\\:ss} minutes to next {jobMethodName} run.");
            });

    }
}