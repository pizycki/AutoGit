using System;

namespace AutoGit.Core
{
    public struct Unit
    {
        public static Unit Value => new Unit();

        public static Unit SideEffect(Action action)
        {
            action();
            return Unit.Value;
        }
    }
}
