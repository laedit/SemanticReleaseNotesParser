using CommonMark;
using DotLiquid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SemanticReleaseNotesParser.Core
{
    /// <summary>
    /// Semantic release notes formatter
    /// </summary>
    public sealed class SemanticReleaseNotesFormatter
    {
        private const string HtmlEnvelope = @"<html>
<body>
{0}
</body>
</html>";

        private static CommonMarkSettings DefaultCommonMarkSettings = new CommonMarkSettings
        {
            AdditionalFeatures = CommonMarkAdditionalFeatures.StrikethroughTilde,
            OutputFormat = CommonMark.OutputFormat.Html
        };

        /// <summary>
        /// Format a release notes
        /// </summary>
        /// <param name="writer">TextWriter which will be used to writes the formatted release notes</param>
        /// <param name="releaseNotes">Release notes to format</param>
        /// <param name="settings">Settings used for formatting</param>
        public static void Format(TextWriter writer, ReleaseNotes releaseNotes, SemanticReleaseNotesFormatterSettings settings = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            writer.Write(Format(releaseNotes, settings));
        }

        /// <summary>
        ///Format a release notes
        /// </summary>
        /// <param name="releaseNotes">Release notes to format</param>
        /// <param name="settings">Settings used for formatting</param>
        /// <returns>Formatted release notes</returns>
        public static string Format(ReleaseNotes releaseNotes, SemanticReleaseNotesFormatterSettings settings = null)
        {
            if (releaseNotes == null)
            {
                throw new ArgumentNullException("releaseNotes");
            }

            if (settings == null)
            {
                settings = SemanticReleaseNotesFormatterSettings.Default;
            }

            // template selection
            string template = settings.LiquidTemplate ?? GetLiquidTemplate(settings);

            // liquid rendering
            var liquidTemplate = Template.Parse(template);

            // process categories
            var categories = new Dictionary<string, List<Item>>();
            ProcessCategories(categories, releaseNotes.Items);

            foreach (var section in releaseNotes.Sections)
            {
                ProcessCategories(categories, section.Items);
            }

            var processedCategories = categories.Select(x => new Category { Name = x.Key, Items = x.Value }).ToList();

            var itemsWithoutCategory = new List<Item>(releaseNotes.Items.Where(i => string.IsNullOrEmpty(i.Category)));
            releaseNotes.Sections.ForEach(s => itemsWithoutCategory.AddRange(s.Items.Where(i => string.IsNullOrEmpty(i.Category))));

            string result = liquidTemplate.Render(Hash.FromAnonymousObject(new { release_notes = releaseNotes, lcb = "{", rcb = "}", categories = processedCategories, items_without_categories = itemsWithoutCategory })).Trim();

            if (settings.OutputFormat == OutputFormat.Markdown)
            {
                return result;
            }

            // convert to HTML
            if (releaseNotes.Items.Any(i => i.Priority > 0) || releaseNotes.Sections.Any(s => s.Items.Any(i => i.Priority > 0)))
            {
                throw new InvalidOperationException("The priorities for items are not supported currently for Html output.");
            }
            return string.Format(HtmlEnvelope, CommonMarkConverter.Convert(result, DefaultCommonMarkSettings).Trim());
        }

        private static void ProcessCategories(Dictionary<string, List<Item>> categories, List<Item> items)
        {
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Category))
                {
                    var categoryName = FirstLetterToUpper(item.Category);
                    if (!categories.ContainsKey(categoryName))
                    {
                        categories.Add(categoryName, new List<Item>());
                    }
                    categories[categoryName].Add(item);
                }
            }
        }

        private static string FirstLetterToUpper(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        private static string GetLiquidTemplate(SemanticReleaseNotesFormatterSettings settings)
        {
            string templateName = "SemanticReleaseNotesParser.Core.Resources.GroupBySections.liquid";
            if (settings.GroupBy == GroupBy.Categories)
            {
                templateName = "SemanticReleaseNotesParser.Core.Resources.GroupByCategories.liquid";
            }

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(templateName)))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
