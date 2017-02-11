using System;
using System.Collections.Generic;
using System.IO;

namespace SemanticReleaseNotesParser.Core.Parser
{
    /// <summary>
    /// Semantic Release Notes Parser
    /// </summary>
    internal static class SemanticReleaseNotesParser
    {
        private static readonly List<IParserPart> ParserParts = new List<IParserPart>
        {
            new SectionParserPart(),
            new ItemParserPart(),
            new MetadataParserPart(),
            new PrimaryParserPart()
        };

        /// <summary>
        /// Parse a release notes from a stream
        /// </summary>
        /// <param name="reader">Reader of release notes</param>
        /// <param name="settings">Settings used for converting</param>
        /// <returns>A parsed release notes</returns>
        public static ReleaseNotes Parse(TextReader reader, SemanticReleaseNotesConverterSettings settings = null)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return Parse(reader.ReadToEnd());
        }

        /// <summary>
        /// Parse a release notes
        /// </summary>
        /// <param name="rawReleaseNotes">Raw release notes</param>
        /// <param name="settings">Settings used for converting</param>
        /// <returns>A parsed release notes</returns>
        public static ReleaseNotes Parse(string rawReleaseNotes, SemanticReleaseNotesConverterSettings settings = null)
        {
            if (string.IsNullOrWhiteSpace(rawReleaseNotes))
            {
                throw new ArgumentNullException("rawReleaseNotes");
            }

            var releaseNotes = new ReleaseNotes();

            var rawLines = rawReleaseNotes.Replace("\r", string.Empty).Split('\n');

            for (int i = 0; i < rawLines.Length; i++)
            {
                var rawLine = rawLines[i];
                var nextInput = string.Empty;
                if (i + 1 < rawLines.Length)
                {
                    nextInput = rawLines[i + 1];
                }

                // Process the line
                for (int j = 0; j < ParserParts.Count; j++)
                {
                    if (ParserParts[j].Parse(rawLine, releaseNotes, nextInput))
                    {
                        break;
                    }
                }
            }

            return releaseNotes;
        }
    }
}