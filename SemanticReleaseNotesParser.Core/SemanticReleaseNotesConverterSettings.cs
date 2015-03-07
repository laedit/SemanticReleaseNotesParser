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

        /// <summary>
        /// True to pluralize categories title
        /// </summary>
        public bool PluralizeCategoriesTitle { get; set; }

        /// <summary>
        /// True to include style int the HTML output
        /// </summary>
        public bool IncludeStyle { get; set; }

        /// <summary>
        /// Custom style to overrides the default style
        /// </summary>
        public string CustomStyle { get; set; }

        internal static SemanticReleaseNotesConverterSettings Default
        {
            get { return _default; }
        }
    }
}