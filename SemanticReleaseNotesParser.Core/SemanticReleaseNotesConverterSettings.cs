namespace SemanticReleaseNotesParser.Core
{
    /// <summary>
    /// Settings for Semantic Release Notes Converter
    /// </summary>
    public sealed class SemanticReleaseNotesConverterSettings
    {
        private readonly static SemanticReleaseNotesConverterSettings _default = new SemanticReleaseNotesConverterSettings();

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

        internal static SemanticReleaseNotesConverterSettings Default
        {
            get { return _default; }
        }
    }
}