using System;
using System.Text.RegularExpressions;

namespace SemanticReleaseNotesParser.Core.Parser
{
    internal sealed class SectionParserPart : IParserPart
    {
        private static readonly Regex SectionRegex = new Regex(@"^# ([\w\s*]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool Parse(string input, ReleaseNotes releaseNotes, string nextInput)
        {
            var match = SectionRegex.Match(input);
            if (match.Success)
            {
                var section = new Section
                {
                    Name = match.Groups[1].Value
                };

                var link = LinkParser.GetLink(input);
                if (!string.IsNullOrEmpty(link.Item1) && link.Item1.Equals("icon", StringComparison.OrdinalIgnoreCase))
                {
                    section.Icon = link.Item2;
                }

                releaseNotes.Sections.Add(section);
                return true;
            }

            return false;
        }
    }
}