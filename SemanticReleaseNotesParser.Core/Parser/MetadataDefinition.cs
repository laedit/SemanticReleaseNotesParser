using System;
using System.Text.RegularExpressions;

namespace SemanticReleaseNotesParser.Core
{
    internal sealed class MetadataDefinition
    {
        public string Name { get; set; }

        public Regex Regex { get; set; }

        public Func<Match, string> GetValue { get; set; }
    }
}