using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SemanticReleaseNotesParser.Core.Parser
{
    internal sealed class PrimaryParserPart : IParserPart
    {
        private static readonly Regex SummaryRegex = new Regex(@"^[a-zA-Z0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool Parse(string input, ReleaseNotes releaseNotes, string nextInput)
        {
            input = input.Trim();
            if (string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(nextInput) && SummaryRegex.IsMatch(nextInput))
            {
                input = Environment.NewLine + Environment.NewLine;
            }

            if (releaseNotes.Sections.Count > 0)
            {
                var lastSection = releaseNotes.Sections.Last();
                lastSection.Summary = (lastSection.Summary ?? string.Empty) + input;
            }
            else
            {
                releaseNotes.Summary = (releaseNotes.Summary ?? string.Empty) + input;
            }
            return true;
        }
    }
}
