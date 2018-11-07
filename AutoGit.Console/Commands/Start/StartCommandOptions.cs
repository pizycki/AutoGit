using Microsoft.Extensions.CommandLineUtils;

namespace AutoGit.DotNet
{
    public class StartCommandOptions
    {
        public readonly CommandOption Source;
        public readonly CommandOption CommitInterval;
        public readonly CommandOption Push;

        public StartCommandOptions(CommandOption source, CommandOption commitInterval, CommandOption push)
        {
            Source = source;
            CommitInterval = commitInterval;
            Push = push;
        }

        public static StartCommandOptions Create(CommandLineApplication cmd)
        {
            var source =
                cmd.Option("-r | --repository",
                           "Path to directory Git repository root",
                           CommandOptionType.SingleValue);

            var commitInterval =
                cmd.Option("-ci | --commit-interval",
                           "Committing interval in minutes",
                           CommandOptionType.SingleValue);

            var push =
                cmd.Option("-p | --push",
                           "Commit is followed by Push to remote repository.",
                           CommandOptionType.SingleValue);

            return new StartCommandOptions(source, commitInterval, push);
        }
    }
}