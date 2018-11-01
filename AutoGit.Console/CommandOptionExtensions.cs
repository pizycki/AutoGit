using System;
using Microsoft.Extensions.CommandLineUtils;
using Optional;

namespace AutoGit.DotNet
{
    public static class CommandOptionExtensions
    {
        public static Option<T> GetValue<T>(this CommandOption cmdOpt, Func<string, T> converter) =>
            cmdOpt == null ? throw new ArgumentNullException(nameof(cmdOpt))
          : cmdOpt.HasValue() ? converter(cmdOpt.Value()).Some()
          : Option.None<T>();

        public static Option<string> GetValue(this CommandOption cmdOpt) =>
            cmdOpt == null ? throw new ArgumentNullException(nameof(cmdOpt))
          : cmdOpt.HasValue() ? cmdOpt.Value().Some()
          : Option.None<string>();

        public static CommandLineApplication AddHelp(this CommandLineApplication cmd)
        {
            cmd.HelpOption("-?|-h|--help");
            return cmd;
        }

    }
}