using System.Text.RegularExpressions;

namespace SemanticReleaseNotesParser.Core.Parser
{
    internal interface IMetadataDefinition
    {
        string Name { get; }

        Regex Regex { get; }

        string GetValue(Match metadataMatch);
    }
}