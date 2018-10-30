using System;

namespace AutoGit.DotNet
{
    public static class FuncExtensions
    {
        public static Func<T> func<T>(Func<T> f) => f;
    }
}