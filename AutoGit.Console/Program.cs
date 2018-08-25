using Hangfire;
using Hangfire.MemoryStorage;

namespace AutoGit.Console
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();

            using (var server = new BackgroundJobServer())
            {
                RecurringJob.AddOrUpdate(
                    methodCall: () => SayHi(),
                    cronExpression: Cron.Minutely);

                Console.WriteLine("Hangfire Server started. Press any key to exit...");
                Console.ReadKey();
            }
        }

        public static void SayHi() => Console.WriteLine("Hi!");
    }
}
