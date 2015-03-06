using Humanizer;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SemanticReleaseNotesParser.Core.Parser
{
    internal sealed class ItemParserPart : IParserPart
    {
        private static readonly Regex PriorityRegex = new Regex(@"^ [\-\+\*]|([123])\. ", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CategoryRegex = new Regex(@"\+([\w-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool Parse(string input, ReleaseNotes releaseNotes, string nextInput)
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
                var link = LinkParser.GetLink(input);
                if (!string.IsNullOrEmpty(link.Item1))
                {
                    item.TaskId = link.Item1;
                    item.TaskLink = link.Item2;
                    input = LinkParser.LinkRegex.Replace(input, string.Empty).Trim();
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
    }
}