using System;
using Microsoft.Extensions.CommandLineUtils;
using static System.Console;

namespace AutoGit.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[]
            {   "run",
                "-s", "\"E:\\dev\\gitauto-test-repo\""
            };

            var cmdLineApp = new CommandLineApplication();

            cmdLineApp.Command("run", cmd =>
            {
                var sourceOpt = cmd.Option("-s | --src",
                                           "Path to directory Git repository root",
                                           CommandOptionType.SingleValue);
                Func<int> fun = () =>
                {
                    if (sourceOpt.HasValue() == false)
                    {
                        cmd.Error.WriteLine("Path to repository is required.");
                    }

                    cmd.Out.WriteLine("Here i create cron job");
                    return 0;
                };

                cmd.OnExecute(fun);

            });

            cmdLineApp.Execute(args);
        }

        private static void ShowPrompt()
        {
            WriteLine("HangFire has started!");
            WriteLine("Press any key to exit.");
            ReadKey();
        }

        public static void SayHi() => WriteLine("Hi!");
    }
}
