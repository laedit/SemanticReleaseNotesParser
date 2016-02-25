using DotLiquid;
using System.Collections.Generic;

namespace SemanticReleaseNotesParser.Core
{
    /// <summary>
    /// Release notes
    /// </summary>
    public sealed class ReleaseNotes : Drop
    {
        /// <summary>
        /// Summary of the release notes
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Sections of the release notes
        /// </summary>
        public List<Section> Sections { get; set; }

        /// <summary>
        /// Items of the release notes
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// Metadata of the release notes
        /// </summary>
        public List<Metadata> Metadata { get; set; }

        /// <summary>
        /// Instantiates a new ReleaseNotes
        /// </summary>
        public ReleaseNotes()
        {
            Sections = new List<Section>();
            Items = new List<Item>();
            Metadata = new List<Metadata>();
        }
    }
}