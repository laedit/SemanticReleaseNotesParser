using System;
using System.Text.RegularExpressions;

namespace SemanticReleaseNotesParser.Core.Parser
{
    internal static class LinkParser
    {
        internal static readonly Regex LinkRegex = new Regex(@"\[\[(\S+)\]\[(\S+)\]\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static Tuple<string, string> GetLink(string input)
        {
            var match = LinkRegex.Match(input);
            if (match.Success)
            {
                return Tuple.Create(match.Groups[1].Value, match.Groups[2].Value);
            }
            return Tuple.Create(string.Empty, string.Empty);
        }
    }
}