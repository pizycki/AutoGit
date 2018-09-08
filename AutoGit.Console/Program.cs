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
                    if (!sourceOpt.HasValue())
                    {
                        cmd.Error.WriteLine("Path to repository is required.");
                        return ExitCodes.Failure;
                    }

                    // TODO figure out how to retrieve user from .gitconfig
                    if (!userNameOpt.HasValue())
                    {
                        cmd.Error.WriteLine("Username is required.");
                    }

                    if (!userEmailOpt.HasValue())
                    {
                        cmd.Error.WriteLine("Username is required.");
                    }

                    var author = new GitUser(userNameOpt.Value(), userEmailOpt.Value());
                    var repository = new GitRepositorySettings(sourceOpt.Value(), author);
                    var comitter = new Comitter(repository);
                    using (var scheduler = Scheduler.Create())
                    {
                        scheduler.AddCronJob(() => comitter.CommitChanges(), Cron.Minutely);
                        //scheduler.AddCronJob(() => PrintRemainingTime("CommitChanges", scheduler.GetRemaningTimeToNextRun("CommitChanges")), Cron.Minutely);

                        ShowPrompt();
                    }

                    return ExitCodes.Success;
                };

                cmd.OnExecute(fun);

            });

            cmdLineApp.Execute(args);
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
