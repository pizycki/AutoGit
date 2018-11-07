using System;
using System.Diagnostics;

namespace Process
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\dev\test-repo";

            var proc = new System.Diagnostics.Process()
            {
                StartInfo = new ProcessStartInfo("git", "status")
                {

                }
            };

            proc.Start();

            proc.Start();

            Console.WriteLine("bla");

            Console.ReadKey();
        }
    }
}
