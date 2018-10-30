namespace AutoGit.DotNet
{
    public class Failure
    {
        private Failure(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public static implicit operator Failure(string message) => new Failure(message);
    }
}