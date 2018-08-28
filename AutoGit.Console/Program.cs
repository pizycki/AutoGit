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

        static void Main(string[] args)
        {
            var cmdLineApp = new CommandLineApplication();

            cmdLineApp.Command("start", cmd =>
            {
                var sourceOpt = cmd.Option("-s | --src",
                                           "Path to directory Git repository root",
                                           CommandOptionType.SingleValue);
                Func<int> fun = () =>
                {
                    if (sourceOpt.HasValue() == false)
                    {
                        cmd.Error.WriteLine("Path to repository is required.");
                        return ExitCodes.Failure;
                    }

                    var author = new GitUser("Paweł Iżycki", "pawelizycki@gmail.com");
                    var repository = new GitRepositorySettings(sourceOpt.Value(), author);
                    var comitter = new Comitter(repository);
                    using (var scheduler = Scheduler.Create())
                    {
                        scheduler.AddCronJob(() => comitter.CommitChanges(), Cron.Minutely);

                        ShowPrompt();
                    }

                    return ExitCodes.Success;
                };

                cmd.OnExecute(fun);

            });

            cmdLineApp.Execute(args);
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
