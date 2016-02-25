using System.Collections.Generic;

namespace SemanticReleaseNotesParser.Core.Parser
{
    internal sealed class MetadataParserPart : IParserPart
    {
        private static readonly List<IMetadataDefinition> MetadataDefinitions = new List<IMetadataDefinition>
        {
            new CommitsMetadataDefinition()
        };

        public bool Parse(string input, ReleaseNotes releaseNotes, string nextInput)
        {
            foreach (var metadataDefinition in MetadataDefinitions)
            {
                var match = metadataDefinition.Regex.Match(input);
                if (match.Success)
                {
                    releaseNotes.Metadata.Add(new Metadata { Name = metadataDefinition.Name, Value = metadataDefinition.GetValue(match) });
                    return true;
                }
            }
            return false;
        }
    }
}