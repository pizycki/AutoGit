using System;
using AutoGit.Contracts;
using LibGit2Sharp;
using static System.Console;

namespace AutoGit
{
    public class RepositorySettings
    {
        public RepositorySettings(string repositoryPath, GitUser user)
        {
            User = user;
            RepositoryPath = repositoryPath;
        }

        public string RepositoryPath { get; }
        public GitUser User { get; }
    }

    public class GitUser
    {
        public string Name { get; }
        public string Email { get; }

        public GitUser(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }

    public class Comitter : IComitter
    {
        private readonly RepositorySettings _repositorySettings;

        public Comitter(RepositorySettings repositorySettings)
        {
            _repositorySettings = repositorySettings;
        }

        public void CommitChanges()
        {
            using (var repo = new Repository(_repositorySettings.RepositoryPath))
            {
                Commands.Stage(repo, "*");
                var message = CreateMessage();
                var signature = CreateSignature();
                try
                {
                    repo.Commit(message, signature, signature);
                    WriteLine("Changes comitted successfully.");
                }
                catch (EmptyCommitException)
                {
                    WriteLine("Nothing to commit, carry on.");
                }
            }
        }

        private static string CreateMessage() => $"AutoGit - {DateTime.Now:dd/MM/yyyy hh:mm}";

        private Signature CreateSignature() =>
            new Signature(_repositorySettings.User.Name,
                          _repositorySettings.User.Email,
                          DateTimeOffset.Now);
    }
}
