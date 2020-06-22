using System.Collections.Generic;

namespace SemanticReleaseNotesParser.Core
{
    /// <summary>
    /// Category of a release notes
    /// </summary>
    public sealed class Category
    {
        /// <summary>
        /// Name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Items composing the category
        /// </summary>
        public List<Item> Items { get; set; }
    }
}
