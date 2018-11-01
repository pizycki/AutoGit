using System;
using System.Diagnostics;

namespace Process
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"E:\dev\gitauto-test-repo";

            var proc = new System.Diagnostics.Process()
            {
                StartInfo = new ProcessStartInfo("git", $"-C {path} log")
                {

                }
            };

            proc.Start();

            Console.ReadKey();
        }
    }
}
