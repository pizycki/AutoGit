using System;
using System.Linq;
using Optional.Linq;
using AutoGit.Core;
using Hangfire;
using Microsoft.Extensions.CommandLineUtils;
using Optional;
using static System.Console;
using static Hangfire.Cron;
using static AutoGit.DotNet.FuncExtensions;
// ReSharper disable MemberCanBePrivate.Global

namespace AutoGit.DotNet
{
    class Program
    {
        static class ExitCodes
        {
            public const int Success = 0;
            public const int Failure = 1;
        }

        const string HelpPattern = "-?|-h|--help";

        static void Main(string[] args)
        {
            var cmdLineApp = new CommandLineApplication();
            cmdLineApp.HelpOption(HelpPattern);

            cmdLineApp.Command("start", cmd => ConfigureStart(cmd));

            cmdLineApp.Execute(args);
        }

        private static CommandLineApplication ConfigureStart(CommandLineApplication cmd)
        {
            // TODO  You can (but shouldnt) set another HelpOption on cmdLineApp - the unit test would be nice here
            cmd.HelpOption(HelpPattern);

            var source =
                cmd.Option("-s | --src",
                           "Path to directory Git repository root",
                           CommandOptionType.SingleValue);

            var username =
                cmd.Option("-un | --username",
                           "Name of author which will pasted into commit",
                           CommandOptionType.SingleValue);

            var email =
                cmd.Option("-ue | --email",
                           "Author email",
                           CommandOptionType.SingleValue);

            var commitInterval =
                cmd.Option("-ci | --commit-interval",
                           "Committing interval in minutes",
                           CommandOptionType.SingleValue);

            Func<int> fun = () =>
            {
                // TODO figure out how to retrieve user from .gitconfig

                using (var scheduler = Scheduler.Create())
                {
                    var commitIntervalValue = commitInterval.GetValue(Convert.ToInt32).ValueOr(() => 5);

                    var startCommitJob =
                        from un in username.GetValue().WithException<Failure>("User name is required.")
                        from ue in email.GetValue().WithException<Failure>("User email is required.")
                        from src in source.GetValue().WithException<Failure>("Path to repository is required.")
                        select func(() =>
                            scheduler.AddCronJob(
                                methodCall: () => CommitAll(un, ue, src),
                                cron: () => MinuteInterval(commitIntervalValue)));
                    
                    scheduler.AddCronJob(
                        // ReSharper disable once AccessToDisposedClosure
                        methodCall: () => PrintRemainingTime(nameof(CommitAll), scheduler),
                        cron: () => MinuteInterval(1));

                    ShowPrompt();
                }

                return ExitCodes.Success;
            };

            cmd.OnExecute(fun);

            return cmd;
        }


        public static Action CommitAll(string name, string email, string source) => () =>
        {
            var author = new GitUser(name, email);
            var repository = new RepositorySettings(source, author);
            var comitter = new Comitter(repository);
            comitter.CommitChanges();
        };

        public static void PrintRemainingTime(string jobMethodName, Scheduler scheduler)
        {
            var time = scheduler.GetTimeToNextRun(jobMethodName);
            WriteLine($"{time:m\\:ss} minutes to next {jobMethodName} run.");
        }

        private static void ShowPrompt()
        {
            WriteLine("AutoGit is running");
            WriteLine("Press any key to exit.");
            WriteLine("======================");
            ReadKey();
        }
    }

    public static class CommandOptionExtensions
    {
        public static Option<T> GetValue<T>(this CommandOption cmdOpt, Func<string, T> converter) =>
            cmdOpt == null ? throw new ArgumentNullException(nameof(cmdOpt))
          : (cmdOpt.HasValue() ? converter(cmdOpt.Value()).Some()
          : Option.None<T>());

        public static Option<string> GetValue(this CommandOption cmdOpt) =>
            cmdOpt == null ? throw new ArgumentNullException(nameof(cmdOpt))
          : (cmdOpt.HasValue() ? cmdOpt.Value().Some()
          : Option.None<string>());
    }

    public class Failure
    {
        private Failure(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public static implicit operator Failure(string message) => new Failure(message);
    }

    public static class FuncExtensions
    {
        public static Func<T> func<T>(Func<T> f) => f;
    }

}
