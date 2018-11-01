using Microsoft.Extensions.CommandLineUtils;

namespace AutoGit.DotNet
{
    public class StartCommandOptions
    {
        public readonly CommandOption Source;
        public readonly CommandOption UserName;
        public readonly CommandOption Email;
        public readonly CommandOption CommitInterval;
        public readonly CommandOption Push;

        public StartCommandOptions(CommandOption source, CommandOption userName, CommandOption email, CommandOption commitInterval, CommandOption push)
        {
            Source = source;
            UserName = userName;
            Email = email;
            CommitInterval = commitInterval;
            Push = push;
        }

        public static StartCommandOptions Create(CommandLineApplication cmd)
        {
            var source =
                cmd.Option("-s | --src",
                           "Path to directory Git repository root",
                           CommandOptionType.SingleValue);

            var username =
                cmd.Option("-un | --username",
                           "Name of author which will pasted into commit",
                           CommandOptionType.SingleValue);

            var email =
                cmd.Option("-ue | --email",
                           "Author email",
                           CommandOptionType.SingleValue);

            var commitInterval =
                cmd.Option("-ci | --commit-interval",
                           "Committing interval in minutes",
                           CommandOptionType.SingleValue);

            var push =
                cmd.Option("-p | --push",
                           "Commit is followed by Push to remote repository.",
                           CommandOptionType.SingleValue);

            return new StartCommandOptions(source, username, email, commitInterval, push);
        }
    }
}