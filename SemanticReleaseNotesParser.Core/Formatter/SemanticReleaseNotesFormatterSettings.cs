namespace SemanticReleaseNotesParser.Core
{
    /// <summary>
    /// Settings for semantic release notes formatter
    /// </summary>
    public sealed class SemanticReleaseNotesFormatterSettings
    {
        private readonly static SemanticReleaseNotesFormatterSettings _default = new SemanticReleaseNotesFormatterSettings();

        /// <summary>
        /// Defines the output format
        /// </summary>
        public OutputFormat OutputFormat { get; set; }

        /// <summary>
        /// Defines how items will be grouped
        /// </summary>
        public GroupBy GroupBy { get; set; }

        /// <summary>
        /// Path to a liquid template to use for the conversion. Overrides OutputFormat and GroupBy.
        /// </summary>
        public string LiquidTemplate { get; set; }

        internal static SemanticReleaseNotesFormatterSettings Default
        {
            get { return _default; }
        }
    }
}
