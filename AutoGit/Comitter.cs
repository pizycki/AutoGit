using System;
using System.Diagnostics;

namespace AutoGit.Core
{
    public static class Git
    {
        private static GitCommand StageAllCommand => CreateCommand("add --all");
        private static GitCommand CommitCommand => CreateCommand($"commit --message {CreateMessage(DateTime.Now)}");

        private static GitCommand CreateCommand(string args) => new GitCommand(
            new Process
            {
                StartInfo = new ProcessStartInfo("git", args)
            });

        private static string CreateMessage(DateTime commitTime) => $"AutoGit - {commitTime:dd/MM/yyyy hh:mm}";
    }

    public class GitCommand
    {
        private readonly Process _process;

        public GitCommand(Process process)
        {
            _process = process;
        }

        public Unit Go() => Unit.SideEffect(() => _process.Start());
    }
}
