using System;
using System.Diagnostics;
using System.Text;

namespace AutoGit.Core
{
    public static class Git
    {
        public static GitCommand StageAll => CreateCommand("add --all");
        public static GitCommand Commit => CreateCommand($"commit --message {CreateMessage(DateTime.Now)}");
        public static GitCommand Push => CreateCommand("push");

        private static GitCommand CreateCommand(GitCommandArgs args) => new GitCommand(args.Value);

        private static string CreateMessage(DateTime commitTime) => $"AutoGit - {commitTime:dd/MM/yyyy hh:mm}";
    }

    public class GitCommandArgs
    {
        private readonly string Arguments;
        private readonly string RepositoryPath;

        public GitCommandArgs(string arguments, string repositoryPath = null)
        {
            Arguments = arguments;
            RepositoryPath = repositoryPath;
        }

        public string Value
        {
            get
            {
                var sb = new StringBuilder(Arguments);

                if (!string.IsNullOrWhiteSpace(RepositoryPath))
                {
                    sb.Append(" -C ");
                    sb.Append(RepositoryPath);
                }

                return sb.ToString();
            }
        }

        public static implicit operator GitCommandArgs(string args) => new GitCommandArgs(args);

    }

    public class GitCommand
    {
        private readonly Lazy<Process> _process;
        private readonly string _args;

        public GitCommand(GitCommandArgs args)
        {
            _args = args.Value;
            _process = new Lazy<Process>(() => new Process
            {
                StartInfo = new ProcessStartInfo("git", _args)
            });
        }

        public Unit Go() => Unit.SideEffect(() => _process.Value.Start());

        public GitCommand WithRepository(string repository) =>
            string.IsNullOrWhiteSpace(repository)
          ? throw new ArgumentNullException(nameof(repository))
          : new GitCommand($"{_args} -C {repository}");
    }
}
