using System;
using Hangfire;
using Microsoft.Extensions.CommandLineUtils;
using static System.Console;

namespace AutoGit.Console
{
    class Program
    {
        static class ExitCodes
        {
            public const int Success = 0;
            public const int Failure = 1;
        }

        static class Crons
        {
            public static Func<string> Every2Mins = () => "*/2 * * * *";
        }

        static void Main(string[] args)
        {
            var cmdLineApp = new CommandLineApplication();

            cmdLineApp.Command("start", cmd =>
            {
                var sourceOpt = cmd.Option("-s | --src",
                                           "Path to directory Git repository root",
                                           CommandOptionType.SingleValue);

                var userNameOpt = cmd.Option("-un | --username",
                                             "Name of author which will pasted into commit.",
                                             CommandOptionType.SingleValue);

                var userEmailOpt = cmd.Option("-ue | --email",
                                              "Author email",
                                              CommandOptionType.SingleValue);

                Func<int> fun = () =>
                {
                    if (!TryGetOptionValue(sourceOpt, cmd, "Path to repository is required.", out var source)) return ExitCodes.Failure;
                    if (!TryGetOptionValue(userNameOpt, cmd, "User name is required.", out var username)) return ExitCodes.Failure;
                    if (!TryGetOptionValue(userEmailOpt, cmd, "User email is required.", out var email)) return ExitCodes.Failure;
                    // TODO figure out how to retrieve user from .gitconfig

                    using (var scheduler = Scheduler.Create())
                    {
                        scheduler.AddCronJob(() => CommitAll(username, email, source), Cron.Minutely);
                        scheduler.AddCronJob(() => PrintRemainingTime(nameof(CommitAll), scheduler.GetRemaningTimeToNextRun(nameof(CommitAll))), Cron.Minutely);

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
            var repository = new GitRepositorySettings(source, author);
            var comitter = new Comitter(repository);
            comitter.CommitChanges();
        }

        public static void PrintRemainingTime(string job, TimeSpan time)
        {
            WriteLine($"{time:g} to next {job} run.");
        }

        private static void ShowPrompt()
        {
            WriteLine("AutoGit is running");
            WriteLine("Press any key to exit.");
            ReadKey();
        }

        public static void SayHi() => WriteLine("Hi!");
    }
}
