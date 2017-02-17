
namespace SemanticReleaseNotesParser.Core
{
    /// <summary>
    /// Metadata of a release note
    /// </summary>
    public sealed class Metadata
    {
        /// <summary>
        /// Name of the metadata
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of the metadata
        /// </summary>
        public string Value { get; set; }
    }
}