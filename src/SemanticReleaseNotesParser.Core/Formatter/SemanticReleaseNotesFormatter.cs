using CommonMark;
using DotLiquid;
using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SemanticReleaseNotesParser.Core.Formatter
{
    /// <summary>
    /// Semantic release notes formatter
    /// </summary>
    internal static class SemanticReleaseNotesFormatter
    {
        private const string HtmlEnvelope = @"<html>{0}
<body>
{1}
</body>
</html>";

        private const string HtmlHeader = @"
<header>
<style>
{0}
</style>
</header>";

        private readonly static CommonMarkSettings DefaultCommonMarkSettings;

        static SemanticReleaseNotesFormatter()
        {
            DefaultCommonMarkSettings = CommonMarkSettings.Default.Clone();
            DefaultCommonMarkSettings.AdditionalFeatures = CommonMarkAdditionalFeatures.StrikethroughTilde;
            DefaultCommonMarkSettings.OutputFormat = CommonMark.OutputFormat.Html;
        }

        /// <summary>
        /// Format a release notes
        /// </summary>
        /// <param name="writer">TextWriter which will be used to writes the formatted release notes</param>
        /// <param name="releaseNotes">Release notes to format</param>
        /// <param name="settings">Settings used for formatting</param>
        public static void Format(TextWriter writer, ReleaseNotes releaseNotes, SemanticReleaseNotesConverterSettings settings = null)
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
        public static string Format(ReleaseNotes releaseNotes, SemanticReleaseNotesConverterSettings settings = null)
        {
            if (releaseNotes == null)
            {
                throw new ArgumentNullException("releaseNotes");
            }

            if (settings == null)
            {
                settings = SemanticReleaseNotesConverterSettings.Default;
            }

            // template selection
            string template = settings.LiquidTemplate ?? GetLiquidTemplate(settings);

            // liquid rendering
            var liquidTemplate = Template.Parse(template);

            // process categories
            List<Category> processedCategories = GetCategories(releaseNotes, settings);

            var itemsWithoutCategory = new List<Item>(releaseNotes.Items.Where(i => !i.Categories.Any()));
            releaseNotes.Sections.ForEach(s => itemsWithoutCategory.AddRange(s.Items.Where(i => !i.Categories.Any())));

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

            var header = string.Empty;
            if (settings.IncludeStyle)
            {
                header = string.Format(HtmlHeader, string.IsNullOrEmpty(settings.CustomStyle) ? GetEmbeddedResource("DefaultStyle.css") : settings.CustomStyle);
            }

            return string.Format(HtmlEnvelope, header, CommonMarkConverter.Convert(result, DefaultCommonMarkSettings).Trim());
        }

        private static List<Category> GetCategories(ReleaseNotes releaseNotes, SemanticReleaseNotesConverterSettings settings)
        {
            var categories = new Dictionary<string, List<Item>>();
            ProcessCategories(categories, releaseNotes.Items, settings);

            foreach (var section in releaseNotes.Sections)
            {
                ProcessCategories(categories, section.Items, settings);
            }

            return categories.Select(x => new Category { Name = x.Key, Items = x.Value }).ToList();
        }

        private static void ProcessCategories(Dictionary<string, List<Item>> categories, IEnumerable<Item> items, SemanticReleaseNotesConverterSettings settings)
        {
            foreach (var item in items)
            {
                if (item.Categories.Any())
                {
                    foreach (var category in item.Categories)
                    {
                        var categoryName = category;
                        if (settings.PluralizeCategoriesTitle)
                        {
                            categoryName = categoryName.Pluralize(false);
                        }

                        if (!categories.ContainsKey(categoryName))
                        {
                            categories.Add(categoryName, new List<Item>());
                        }
                        categories[categoryName].Add(item);
                    }
                }
            }
        }

        private static string GetLiquidTemplate(SemanticReleaseNotesConverterSettings settings)
        {
            string templateName = "GroupBySections.liquid";
            if (settings.GroupBy == GroupBy.Categories)
            {
                templateName = "GroupByCategories.liquid";
            }

            return GetEmbeddedResource(templateName);
        }

        private static string GetEmbeddedResource(string resourceName)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("SemanticReleaseNotesParser.Core.Resources." + resourceName)))
            {
                return reader.ReadToEnd();
            }
        }
    }
}