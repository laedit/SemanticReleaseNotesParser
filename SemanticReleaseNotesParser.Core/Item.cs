using DotLiquid;

namespace SemanticReleaseNotesParser.Core
{
    /// <summary>
    /// Item of a release notes
    /// </summary>
    public sealed class Item : Drop
    {
        /// <summary>
        /// Task id
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// Task link
        /// </summary>
        public string TaskLink { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Priority
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; }
    }
}
