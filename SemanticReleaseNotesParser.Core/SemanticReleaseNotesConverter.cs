using System.IO;

namespace SemanticReleaseNotesParser.Core
{
    /// <summary>
    /// Helper to convert a semantic release notes
    /// </summary>
    public static class SemanticReleaseNotesConverter
    {
        /// <summary>
        /// Convert a raw semantic release notes to a formatted semantic release notes
        /// </summary>
        /// <param name="rawReleaseNotes">raw semantic release notes</param>
        /// <param name="settings">Settings used for converting</param>
        /// <returns>A formatted release notes</returns>
        public static string Convert(string rawReleaseNotes, SemanticReleaseNotesConverterSettings settings = null)
        {
            return SemanticReleaseNotesFormatter.Format(SemanticReleaseNotesParser.Parse(rawReleaseNotes, settings), settings);
        }

        /// <summary>
        /// Convert a raw semantic release notes to a formatted semantic release notes
        /// </summary>
        /// <param name="reader">Reader of release notes</param>
        /// <param name="writer">TextWriter which will be used to writes the formatted release notes</param>
        /// <param name="settings">Settings used for converting</param>
        /// <returns>A formatted release notes</returns>
        public static void Convert(TextReader reader, TextWriter writer, SemanticReleaseNotesConverterSettings settings = null)
        {
            SemanticReleaseNotesFormatter.Format(writer, SemanticReleaseNotesParser.Parse(reader, settings), settings);
        }

        /// <summary>
        /// Parse a release notes from a stream
        /// </summary>
        /// <param name="reader">Reader of release notes</param>
        /// <param name="settings">Settings used for parsing</param>
        /// <returns>A parsed release notes</returns>
        public static ReleaseNotes Parse(TextReader reader, SemanticReleaseNotesConverterSettings settings = null)
        {
            return SemanticReleaseNotesParser.Parse(reader, settings);
        }

        /// <summary>
        /// Parse a release notes
        /// </summary>
        /// <param name="rawReleaseNotes">Raw release notes</param>
        /// <param name="settings">Settings used for parsing</param>
        /// <returns>A parsed release notes</returns>
        public static ReleaseNotes Parse(string rawReleaseNotes, SemanticReleaseNotesConverterSettings settings = null)
        {
            return SemanticReleaseNotesParser.Parse(rawReleaseNotes, settings);
        }

        /// <summary>
        /// Format a release notes
        /// </summary>
        /// <param name="writer">TextWriter which will be used to writes the formatted release notes</param>
        /// <param name="releaseNotes">Release notes to format</param>
        /// <param name="settings">Settings used for formatting</param>
        public static void Format(TextWriter writer, ReleaseNotes releaseNotes, SemanticReleaseNotesConverterSettings settings = null)
        {
            SemanticReleaseNotesFormatter.Format(writer, releaseNotes, settings);
        }

        /// <summary>
        ///Format a release notes
        /// </summary>
        /// <param name="releaseNotes">Release notes to format</param>
        /// <param name="settings">Settings used for formatting</param>
        /// <returns>Formatted release notes</returns>
        public static string Format(ReleaseNotes releaseNotes, SemanticReleaseNotesConverterSettings settings = null)
        {
            return SemanticReleaseNotesFormatter.Format(releaseNotes, settings);
        }
    }
}