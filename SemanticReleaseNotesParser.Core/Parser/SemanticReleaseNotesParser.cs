using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SemanticReleaseNotesParser.Core
{
    /// <summary>
    /// Semantic Release Notes Parser
    /// </summary>
    internal static class SemanticReleaseNotesParser
    {
        private static readonly Regex LinkRegex = new Regex(@"\[\[(\S+)\]\[(\S+)\]\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SectionRegex = new Regex(@"^# ([\w\s*]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex PriorityRegex = new Regex(@"^ [\-\+\*]|([123])\. ", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CategoryRegex = new Regex(@"\+([\w-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SummaryRegex = new Regex(@"^[a-zA-Z0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly List<MetadataDefinition> MetadataDefinitions = new List<MetadataDefinition>
        {
            { new MetadataDefinition {
                 Name = "Commits",
                Regex = new Regex(@"^(?:commits:)?[ ]*(?:([0-9a-f]{5,40}\.{3}[0-9a-f]{5,40})|(\[[0-9a-f]{5,40}\.{3}[0-9a-f]{5,40}\]\(https?:\/\/\S+\)))$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                GetValue = match => match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value
            } }
        };

        /// <summary>
        /// Parse a release notes from a stream
        /// </summary>
        /// <param name="reader">Reader of release notes</param>
        /// <param name="settings">Settings used for converting</param>
        /// <returns>A parsed release notes</returns>
        public static ReleaseNotes Parse(TextReader reader, SemanticReleaseNotesConverterSettings settings = null)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return Parse(reader.ReadToEnd());
        }

        /// <summary>
        /// Parse a release notes
        /// </summary>
        /// <param name="rawReleaseNotes">Raw release notes</param>
        /// <param name="settings">Settings used for converting</param>
        /// <returns>A parsed release notes</returns>
        public static ReleaseNotes Parse(string rawReleaseNotes, SemanticReleaseNotesConverterSettings settings = null)
        {
            if (string.IsNullOrEmpty(rawReleaseNotes))
            {
                throw new ArgumentNullException("rawReleaseNotes");
            }

            var releaseNotes = new ReleaseNotes();

            var rawLines = rawReleaseNotes.Replace("\r", string.Empty).Split('\n');

            for (int i = 0; i < rawLines.Length; i++)
            {
                var rawLine = rawLines[i];
                bool matched;

                // Process the line
                matched = ProcessSection(rawLine, releaseNotes);

                if (!matched)
                {
                    matched = ProcessItem(rawLine, releaseNotes);
                }

                if (!matched)
                {
                    matched = ProcessMetadata(rawLine, releaseNotes);
                }

                if (!matched)
                {
                    string nextInput = string.Empty;
                    if (i + 1 < rawLines.Length)
                    {
                        nextInput = rawLines[i + 1];
                    }
                    ProcessPrimary(rawLine, releaseNotes, nextInput);
                }
            }

            return releaseNotes;
        }

        private static bool ProcessMetadata(string input, ReleaseNotes releaseNotes)
        {
            foreach (var metadataDefinition in MetadataDefinitions)
            {
                var match = metadataDefinition.Regex.Match(input);
                if (match.Success)
                {
                    releaseNotes.Metadata.Add(new Metadata { Name = metadataDefinition.Name, Value = metadataDefinition.GetValue(match) });
                    return true;
                }
            }
            return false;
        }

        private static void ProcessPrimary(string input, ReleaseNotes releaseNotes, string nextInput)
        {
            input = input.Trim();
            if (string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(nextInput) && SummaryRegex.IsMatch(nextInput))
            {
                input = Environment.NewLine + Environment.NewLine;
            }

            if (releaseNotes.Sections.Count > 0)
            {
                var lastSection = releaseNotes.Sections.Last();
                lastSection.Summary = (lastSection.Summary ?? string.Empty) + input;
            }
            else
            {
                releaseNotes.Summary = (releaseNotes.Summary ?? string.Empty) + input;
            }
        }

        private static bool ProcessItem(string input, ReleaseNotes releaseNotes)
        {
            var match = PriorityRegex.Match(input);
            if (match.Success)
            {
                var item = new Item();

                // Priority
                int priority;
                if (!string.IsNullOrEmpty(match.Groups[1].Value) && Int32.TryParse(match.Groups[1].Value, out priority))
                {
                    item.Priority = priority;
                }
                input = PriorityRegex.Replace(input, string.Empty);

                // link
                var link = GetLink(input);
                if (!string.IsNullOrEmpty(link.Item1))
                {
                    item.TaskId = link.Item1;
                    item.TaskLink = link.Item2;
                    input = LinkRegex.Replace(input, string.Empty).Trim();
                }

                // category
                var categories = CategoryRegex.Matches(input);
                foreach (Match category in categories)
                {
                    if (category.Success && !string.IsNullOrEmpty(category.Groups[1].Value))
                    {
                        var categoryName = category.Groups[1].Value.ToLowerInvariant().Titleize();
                        if (!item.Categories.Contains(categoryName))
                        {
                            item.Categories.Add(categoryName);
                        }
                        var replacement = category.Groups[1].Value;
                        if (input.EndsWith(category.Groups[1].Value))
                        {
                            replacement = string.Empty;
                        }
                        input = input.Replace(category.Groups[0].Value, replacement);
                    }
                }

                // summary
                item.Summary = input.Trim();

                if (releaseNotes.Sections.Count == 0)
                {
                    releaseNotes.Items.Add(item);
                }
                else
                {
                    releaseNotes.Sections.Last().Items.Add(item);
                }

                return true;
            }
            return false;
        }

        private static bool ProcessSection(string input, ReleaseNotes releaseNotes)
        {
            var match = SectionRegex.Match(input);
            if (match.Success)
            {
                var section = new Section
                {
                    Name = match.Groups[1].Value
                };

                var link = GetLink(input);
                if (!string.IsNullOrEmpty(link.Item1) && link.Item1.Equals("icon", StringComparison.OrdinalIgnoreCase))
                {
                    section.Icon = link.Item2;
                }

                releaseNotes.Sections.Add(section);
                return true;
            }

            return false;
        }

        private static Tuple<string, string> GetLink(string input)
        {
            var match = LinkRegex.Match(input);
            if (match.Success)
            {
                return Tuple.Create(match.Groups[1].Value, match.Groups[2].Value);
            }
            return Tuple.Create(string.Empty, string.Empty);
        }
    }
}