using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace AutoGit.Tests
{
    public class GitConfigParser
    {
        public void Load(string gitConfig)
        {
            var lines = gitConfig.Split('\n').ToList();

            var sectionHeaders = new List<(string line, int number)>();
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (LooksLikeSection(line))
                {
                    sectionHeaders.Add((line, i + 1));
                }
            }

            var sections = new List<GitConfigSection>();
            foreach (var header in sectionHeaders)
            {
                var sectionValues =
                    lines.Skip(header.number)
                         .Take(sectionHeaders.IndexOf(header) + 1);

                sections.Add(new GitConfigSection(header.line, sectionValues));
            }

            var count = sections.First(x => x.Name == "[user]").SectionValues.Count();
        }

        private bool LooksLikeSection(string line)
        {
            return line.StartsWith("[") && line.EndsWith("]");
        }
    }

    public class GitConfigSection
    {
        public string Name { get; }
        public Dictionary<string, string> SectionValues { get; }

        public GitConfigSection(string name, IEnumerable<string> sectionValues)
        {
            Name = name;
            SectionValues = sectionValues.Select(ParseLine).ToDictionary(x => x.Key, x => x.Value);
        }

        private KeyValuePair<string, string> ParseLine(string line)
        {
            const string pattern = @"\s*(\w*)\s+=\s+(.*)";
            Match match = Regex.Match(line, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var key = match.Groups[1].Value;
            var value = match.Groups[2].Value;
            return new KeyValuePair<string, string>(key, value);
        }
    }

    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var config = await ReadGitConfig();
            Assert.NotEmpty(config);

            var parser = new GitConfigParser();
            parser.Load(config);
        }

        private static async Task<string> ReadGitConfig()
        {
            using (var file = File.OpenText(@"C:\Users\pawelizycki\Dropbox\EdmxConverterAF\EdmxConverter\.git\config"))
            {
                return await file.ReadToEndAsync();
            }
        }
    }
}
