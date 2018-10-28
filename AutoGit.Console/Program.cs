using System;
using AutoGit.Core;
using Microsoft.Extensions.CommandLineUtils;
using static System.Console;
using static Hangfire.Cron;
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
        
        static void Main(string[] args)
        {
            var helpPattern = "-?|-h|--help";

            var cmdLineApp = new CommandLineApplication();
            cmdLineApp.HelpOption(helpPattern);

            cmdLineApp.Command("start", cmd =>
            {
                // TODO  You can (but shouldnt) set another HelpOption on cmdLineApp - the unit test would be nice here
                cmd.HelpOption(helpPattern);

                var sourceOpt = cmd.Option("-s | --src",
                                           "Path to directory Git repository root",
                                           CommandOptionType.SingleValue);

                var usernameOpt = cmd.Option("-un | --username",
                                             "Name of author which will pasted into commit.",
                                             CommandOptionType.SingleValue);

                var emailOpt = cmd.Option("-ue | --email",
                                          "Author email",
                                          CommandOptionType.SingleValue);

                Func<int> fun = () =>
                {
                    if (!TryGetOptionValue(sourceOpt, cmd, "Path to repository is required.", out var source)) return ExitCodes.Failure;
                    if (!TryGetOptionValue(usernameOpt, cmd, "User name is required.", out var username)) return ExitCodes.Failure;
                    if (!TryGetOptionValue(emailOpt, cmd, "User email is required.", out var email)) return ExitCodes.Failure;
                    // TODO figure out how to retrieve user from .gitconfig

                    using (var scheduler = Scheduler.Create())
                    {
                        scheduler.AddCronJob(
                            methodCall: () => CommitAll(username, email, source),
                            cron: () => MinuteInterval(5));

                        scheduler.AddCronJob(
                            // ReSharper disable once AccessToDisposedClosure
                            methodCall: () => PrintRemainingTime(nameof(CommitAll), scheduler),
                            cron: () => MinuteInterval(1));

                        ShowPrompt();
                    }

                    return ExitCodes.Success;
                };

                cmd.OnExecute(fun);

            });

            cmdLineApp.Execute(args);
        }

        public static bool TryGetOptionValue(CommandOption option, CommandLineApplication cmd, string error, out string value)
        {
            if (option.HasValue())
            {
                value = option.Value();
                return true;
            }
            else
            {
                cmd.Error.WriteLine(error);
                value = null;
                return false;
            }
        }

        public static void CommitAll(string name, string email, string source)
        {
            var author = new GitUser(name, email);
            var repository = new RepositorySettings(source, author);
            var comitter = new Comitter(repository);
            comitter.CommitChanges();
        }

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
}
