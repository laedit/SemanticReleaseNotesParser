using System.Text.RegularExpressions;

namespace SemanticReleaseNotesParser.Core.Parser
{
    internal sealed class CommitsMetadataDefinition : IMetadataDefinition
    {
        private static readonly Regex _regex = new Regex(@"^(?:commits:)?[ ]*(?:([0-9a-f]{5,40}\.{3}[0-9a-f]{5,40})|(\[[0-9a-f]{5,40}\.{3}[0-9a-f]{5,40}\]\(https?:\/\/\S+\)))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Name
        {
            get
            {
                return "Commits";
            }
        }

        public Regex Regex
        {
            get
            {
                return _regex;
            }
        }

        public string GetValue(Match metadataMatch)
        {
            return metadataMatch.Groups[1].Success ? metadataMatch.Groups[1].Value : metadataMatch.Groups[2].Value;
        }
    }
}