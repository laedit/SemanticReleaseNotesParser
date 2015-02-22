using DotLiquid;
using System.Collections.Generic;

namespace SemanticReleaseNotesParser.Core
{
    /// <summary>
    /// Sections of a release notes
    /// </summary>
    public sealed class Section : Drop
    {
        /// <summary>
        /// Name of the section
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Summary of the section
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Items composing the section
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// Icon of the section
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Instantiates a new Section
        /// </summary>
        public Section()
        {
            Items = new List<Item>();
        }
    }
}
