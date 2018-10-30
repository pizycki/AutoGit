using System;

namespace AutoGit.Core
{
    public struct Unit
    {
        public static Unit Value => new Unit();

    }

    public static class UnitHelper
    {
        public static Unit SideEffect(this Unit unit, Action action)
        {
            action();
            return unit;
        }
    }
}
