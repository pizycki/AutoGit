using static System.Console;

namespace AutoGit.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (var scheduler = Scheduler.Create())
            //{
            //    scheduler.AddCronJob(() => SayHi(), Cron.Minutely);

            //    ShowPrompt();
            //}

            var repoSettings = new GitRepositorySettings("E:\\dev\\gitauto-test-repo", new GitUser("LeMua", "le.mua@domain.com"));
            var comitter = new Comitter(repoSettings);
            comitter.CommitChanges();

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
