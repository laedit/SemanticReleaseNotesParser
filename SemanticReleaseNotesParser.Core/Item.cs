using DotLiquid;
using System.Collections.Generic;

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
        public List<string> Categories { get; set; }

        /// <summary>
        /// Priority
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Instantiates a new Item
        /// </summary>
        public Item()
        {
            Categories = new List<string>();
        }
    }
}