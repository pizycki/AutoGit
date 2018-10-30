using Microsoft.Extensions.CommandLineUtils;
using static System.Console;

// ReSharper disable MemberCanBePrivate.Global

namespace AutoGit.DotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdLineApp = new CommandLineApplication();
            cmdLineApp.AddHelp();
            cmdLineApp.Command("start", cmd => new StartCommand(cmd));
            cmdLineApp.Execute(args);

            ShowPrompt();
        }

        private static void ShowPrompt()
        {
            WriteLine("======================");
            WriteLine("Press any key to exit.");
            WriteLine("======================");
            ReadKey();
        }
    }
}
