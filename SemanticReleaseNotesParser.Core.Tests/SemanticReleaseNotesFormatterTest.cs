using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SemanticReleaseNotesParser.Core.Tests
{
    public class SemanticReleaseNotesFormatterTest
    {
        [Fact]
        public void Format_Null_Exception()
        {
            // act & assert
            var exception = Assert.Throws<ArgumentNullException>(() => SemanticReleaseNotesFormatter.Format(null));
            Assert.Equal("releaseNotes", exception.ParamName);
        }

        [Fact]
        public void Format_TextWriter_Null_Exception()
        {
            // act & assert
            var exception = Assert.Throws<ArgumentNullException>(() => SemanticReleaseNotesFormatter.Format(null, new ReleaseNotes()));
            Assert.Equal("writer", exception.ParamName);
        }

        [Fact]
        public void Format_ExampleA_Default()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleAReleaseNotes());

            // assert
            Assert.Equal(ExampleAHtml, resultHtml.Trim());
        }

        [Fact]
        public void Format_TextWriter_ExampleA_Default()
        {
            // arrange
            var resultHtml = new StringBuilder();

            // act
            SemanticReleaseNotesFormatter.Format(new StringWriter(resultHtml), GetExempleAReleaseNotes());

            // assert
            Assert.Equal(ExampleAHtml, resultHtml.ToString().Trim());
        }

        [Fact]
        public void Format_ExampleA_Output_Html()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleAReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Html });

            // assert
            Assert.Equal(ExampleAHtml, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleA_Output_Markdown()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleAReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Markdown });

            // assert
            Assert.Equal(ExampleAMarkdown, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleA_Output_Markdown_TextWriter()
        {
            // arrange
            var resultHtml = new StringBuilder();

            // act
            SemanticReleaseNotesFormatter.Format(new StringWriter(resultHtml), GetExempleAReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Markdown });

            // assert
            Assert.Equal(ExampleAMarkdown, resultHtml.ToString().Trim());
        }

        [Fact]
        public void Format_ExampleA_Output_Html_GroupBy_Categories()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleAReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Html, GroupBy = GroupBy.Categories });

            // assert
            Assert.Equal(ExampleAHtmlCategories, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleA_Output_Markdown_GroupBy_Categories()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleAReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Markdown, GroupBy = GroupBy.Categories });

            // assert
            Assert.Equal(ExampleAMarkdownCategories, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleB_Output_Html()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleBReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Html });

            // assert
            Assert.Equal(ExampleBHtml, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleB_Output_Markdown()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleBReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Markdown });

            // assert
            Assert.Equal(ExampleBMarkdown, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleA_Output_Html_Custom_LiquidTemplate()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleAReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Html, LiquidTemplate = CustomLiquidTemplate });

            // assert
            Assert.Equal(CustomLiquidTemplateHtml, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleA_Output_Markdown_Custom_LiquidTemplate()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleAReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Markdown, LiquidTemplate = CustomLiquidTemplate });

            // assert
            Assert.Equal(CustomLiquidTemplateMarkdown, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleC_Output_Html()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleCReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Html });

            // assert
            Assert.Equal(ExampleCHtml, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleC_Output_Markdown()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleCReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Markdown });

            // assert
            Assert.Equal(ExampleCMarkdown, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleC_Output_Html_GroupBy_Categories()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleCReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Html, GroupBy = GroupBy.Categories });

            // assert
            Assert.Equal(ExampleCHtmlCategories, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleC_Output_Markdown_GroupBy_Categories()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleCReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Markdown, GroupBy = GroupBy.Categories });

            // assert
            Assert.Equal(ExampleCMarkdownCategories, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleD_Output_Html()
        {
            // act
            var exception = Assert.Throws<InvalidOperationException>(() => SemanticReleaseNotesFormatter.Format(GetExempleDReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Html }));

            // assert
            Assert.Equal("The priorities for items are not supported currently for Html output.", exception.Message);
            //Assert.Equal(ExampleDHtml, resultHtml.Trim());
        }

        [Fact]
        public void Format_ExampleD_Output_Markdown()
        {
            // act
            var resultHtml = SemanticReleaseNotesFormatter.Format(GetExempleDReleaseNotes(), new SemanticReleaseNotesFormatterSettings { OutputFormat = OutputFormat.Markdown });

            // assert
            Assert.Equal(ExampleDMarkdown, resultHtml.Trim());
        }

        private ReleaseNotes GetExempleAReleaseNotes()
        {
            return new ReleaseNotes
            {
                Summary = "Incremental release designed to provide an update to some of the core plugins.",
                Items = new List<Item>
                 {
                     new Item { Category = "New", Summary = "Release Checker: Now gives you a breakdown of exactly what you are missing." },
                     new Item { Category = "New", Summary = "Structured Layout: An alternative layout engine that allows developers to control layout." },
                     new Item { Category = "Changed", Summary = "Timeline: Comes with an additional grid view to show the same data." },
                     new Item { Category = "Fix", Summary = "Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement." }
                 }
            };
        }

        private ReleaseNotes GetExempleBReleaseNotes()
        {
            return new ReleaseNotes
            {
                Summary = "Incremental release designed to provide an update to some of the core plugins.",
                Sections = new List<Section>
                {
                    new Section { Name  = "System", Items = new List<Item>
                    {
                        new Item { Category = "New", Summary = "*Release Checker*: Now gives you a breakdown of exactly what you are missing." },
                        new Item { Category = "New", Summary = "*Structured Layout*: An alternative layout engine that allows developers to control layout." }
                    } },
                    new Section { Name  = "Plugin", Items = new List<Item>
                    {
                        new Item { Category = "Changed", Summary = "*Timeline*: Comes with an additional grid view to show the same data." },
                        new Item { Category = "Fix", Summary = "*Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement." }
                    } }
                }
            };
        }

        private ReleaseNotes GetExempleCReleaseNotes()
        {
            return new ReleaseNotes
            {
                Summary = "Incremental release designed to provide an update to some of the core plugins.",
                Items = new List<Item> { new Item { Summary = "*Example*: You can have global issues that aren't grouped to a section" } },
                Sections = new List<Section>
                {
                    new Section { Name  = "System", Summary = "This description is specific to system section.", Items = new List<Item>
                    {
                        new Item { Category = "new", Summary = "*Release Checker*: Now gives you a breakdown of exactly what you are missing." },
                        new Item { Category = "new", Summary = "*Structured Layout*: An alternative layout engine that allows developers to control layout." }
                    } },
                    new Section { Name  = "Plugin", Summary = "This description is specific to plugin section.", Items = new List<Item>
                    {
                        new Item { Category = "Changed", Summary = "*Timeline*: Comes with an additional grid view to show the same data." },
                        new Item { Category = "Fix", Summary = "*Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement.", TaskId = "i1234", TaskLink = "http://getglimpse.com" }
                    } }
                }
            };
        }

        private ReleaseNotes GetExempleDReleaseNotes()
        {
            return new ReleaseNotes
            {
                Summary = "Incremental release designed to provide an update to some of the core plugins.",
                Items = new List<Item> { new Item { Priority = 1, Summary = "*Example*: You can have global issues that aren't grouped to a section" } },
                Sections = new List<Section>
                {
                    new Section { Name  = "System", Summary = "This description is specific to system section.", Icon = "http://getglimpse.com/release/icon/core.png", Items = new List<Item>
                    {
                        new Item { Priority = 3, Category = "new", Summary = "*Release Checker*: Now gives you a breakdown of exactly what you are missing." },
                        new Item { Priority = 2, Category = "new", Summary = "*Structured Layout*: An alternative layout engine that allows developers to control layout." }
                    } },
                    new Section { Name  = "Plugin", Summary = "This description is specific to plugin section.", Icon= "http://getglimpse.com/release/icon/mvc.png", Items = new List<Item>
                    {
                        new Item { Priority = 1, Category = "Changed", Summary = "*Timeline*: Comes with an additional grid view to show the same data." },
                        new Item { Priority = 1, Category = "Fix", Summary = "*Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement.", TaskId = "i1234", TaskLink = "http://getglimpse.com" }
                    } }
                }
            };
        }

        private const string ExampleAHtml = @"<html>
<body>
<p>Incremental release designed to provide an update to some of the core plugins.</p>
<ul>
<li>{New} Release Checker: Now gives you a breakdown of exactly what you are missing.</li>
<li>{New} Structured Layout: An alternative layout engine that allows developers to control layout.</li>
<li>{Changed} Timeline: Comes with an additional grid view to show the same data.</li>
<li>{Fix} Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement.</li>
</ul>
</body>
</html>";

        private const string ExampleAMarkdown = @"Incremental release designed to provide an update to some of the core plugins.

 - {New} Release Checker: Now gives you a breakdown of exactly what you are missing.
 - {New} Structured Layout: An alternative layout engine that allows developers to control layout.
 - {Changed} Timeline: Comes with an additional grid view to show the same data.
 - {Fix} Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement.";

        private const string ExampleAHtmlCategories = @"<html>
<body>
<p>Incremental release designed to provide an update to some of the core plugins.</p>
<h1>New</h1>
<ul>
<li>Release Checker: Now gives you a breakdown of exactly what you are missing.</li>
<li>Structured Layout: An alternative layout engine that allows developers to control layout.</li>
</ul>
<h1>Changed</h1>
<ul>
<li>Timeline: Comes with an additional grid view to show the same data.</li>
</ul>
<h1>Fix</h1>
<ul>
<li>Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement.</li>
</ul>
</body>
</html>";

        private const string ExampleAMarkdownCategories = @"Incremental release designed to provide an update to some of the core plugins.

# New
 - Release Checker: Now gives you a breakdown of exactly what you are missing.
 - Structured Layout: An alternative layout engine that allows developers to control layout.

# Changed
 - Timeline: Comes with an additional grid view to show the same data.

# Fix
 - Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement.";

        private const string ExampleBHtml = @"<html>
<body>
<p>Incremental release designed to provide an update to some of the core plugins.</p>
<h1>System</h1>
<ul>
<li>{New} <em>Release Checker</em>: Now gives you a breakdown of exactly what you are missing.</li>
<li>{New} <em>Structured Layout</em>: An alternative layout engine that allows developers to control layout.</li>
</ul>
<h1>Plugin</h1>
<ul>
<li>{Changed} <em>Timeline</em>: Comes with an additional grid view to show the same data.</li>
<li>{Fix} <em>Ajax</em>: Fix that crashed poll in Chrome and IE due to log/trace statement.</li>
</ul>
</body>
</html>";

        private const string ExampleBMarkdown = @"Incremental release designed to provide an update to some of the core plugins.

# System
 - {New} *Release Checker*: Now gives you a breakdown of exactly what you are missing.
 - {New} *Structured Layout*: An alternative layout engine that allows developers to control layout.

# Plugin
 - {Changed} *Timeline*: Comes with an additional grid view to show the same data.
 - {Fix} *Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement.";

        private const string CustomLiquidTemplate = @"
{%- for category in categories -%}
# {{ category.name }}
{%- for item in category.items -%}
 - {{ item.summary }}
{%- endfor -%}
{%- endfor -%}

{{ release_notes.summary }}";

        private const string CustomLiquidTemplateHtml = @"<html>
<body>
<h1>New</h1>
<ul>
<li>Release Checker: Now gives you a breakdown of exactly what you are missing.</li>
<li>Structured Layout: An alternative layout engine that allows developers to control layout.</li>
</ul>
<h1>Changed</h1>
<ul>
<li>Timeline: Comes with an additional grid view to show the same data.</li>
</ul>
<h1>Fix</h1>
<ul>
<li>Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement.</li>
</ul>
<p>Incremental release designed to provide an update to some of the core plugins.</p>
</body>
</html>";

        private const string CustomLiquidTemplateMarkdown = @"# New
 - Release Checker: Now gives you a breakdown of exactly what you are missing.
 - Structured Layout: An alternative layout engine that allows developers to control layout.
# Changed
 - Timeline: Comes with an additional grid view to show the same data.
# Fix
 - Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement.

Incremental release designed to provide an update to some of the core plugins.";

        private const string ExampleCHtml = @"<html>
<body>
<p>Incremental release designed to provide an update to some of the core plugins.</p>
<ul>
<li><em>Example</em>: You can have global issues that aren't grouped to a section</li>
</ul>
<h1>System</h1>
<ul>
<li>{new} <em>Release Checker</em>: Now gives you a breakdown of exactly what you are missing.</li>
<li>{new} <em>Structured Layout</em>: An alternative layout engine that allows developers to control layout.</li>
</ul>
<h1>Plugin</h1>
<ul>
<li>{Changed} <em>Timeline</em>: Comes with an additional grid view to show the same data.</li>
<li>{Fix} <em>Ajax</em>: Fix that crashed poll in Chrome and IE due to log/trace statement. <a href=""http://getglimpse.com"">i1234</a></li>
</ul>
</body>
</html>";

        private const string ExampleCMarkdown = @"Incremental release designed to provide an update to some of the core plugins.

 - *Example*: You can have global issues that aren't grouped to a section

# System
 - {new} *Release Checker*: Now gives you a breakdown of exactly what you are missing.
 - {new} *Structured Layout*: An alternative layout engine that allows developers to control layout.

# Plugin
 - {Changed} *Timeline*: Comes with an additional grid view to show the same data.
 - {Fix} *Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement. [i1234](http://getglimpse.com)";

        private const string ExampleCHtmlCategories = @"<html>
<body>
<p>Incremental release designed to provide an update to some of the core plugins.</p>
<ul>
<li><em>Example</em>: You can have global issues that aren't grouped to a section</li>
</ul>
<h1>New</h1>
<ul>
<li><em>Release Checker</em>: Now gives you a breakdown of exactly what you are missing.</li>
<li><em>Structured Layout</em>: An alternative layout engine that allows developers to control layout.</li>
</ul>
<h1>Changed</h1>
<ul>
<li><em>Timeline</em>: Comes with an additional grid view to show the same data.</li>
</ul>
<h1>Fix</h1>
<ul>
<li><em>Ajax</em>: Fix that crashed poll in Chrome and IE due to log/trace statement. <a href=""http://getglimpse.com"">i1234</a></li>
</ul>
</body>
</html>";

        private const string ExampleCMarkdownCategories = @"Incremental release designed to provide an update to some of the core plugins.

 - *Example*: You can have global issues that aren't grouped to a section

# New
 - *Release Checker*: Now gives you a breakdown of exactly what you are missing.
 - *Structured Layout*: An alternative layout engine that allows developers to control layout.

# Changed
 - *Timeline*: Comes with an additional grid view to show the same data.

# Fix
 - *Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement. [i1234](http://getglimpse.com)";

        private const string ExampleDHtml = @"<html>
<body>
<p>Incremental release designed to provide an update to some of the core plugins.</p>
<ul class=""srn_priority"">
<li data-content=""1""><em>Example</em>: You can have global issues that aren't grouped to a section</li>
</ul>
<h1>System</h1>
<ul class=""srn_priority"">
<li data-content=""3"">{new} <em>Release Checker</em>: Now gives you a breakdown of exactly what you are missing.</li>
<li data-content=""2"">{new} <em>Structured Layout</em>: An alternative layout engine that allows developers to control layout.</li>
</ul>
<h1>Plugin</h1>
<ul class=""srn_priority"">
<li data-content=""1"">{Changed} <em>Timeline</em>: Comes with an additional grid view to show the same data.</li>
<li data-content=""1"">{Fix} <em>Ajax</em>: Fix that crashed poll in Chrome and IE due to log/trace statement. <a href=""http://getglimpse.com"">i1234</a></li>
</ul>
</body>
</html>";

        private const string ExampleDMarkdown = @"Incremental release designed to provide an update to some of the core plugins.

 1. *Example*: You can have global issues that aren't grouped to a section

# System
 3. {new} *Release Checker*: Now gives you a breakdown of exactly what you are missing.
 2. {new} *Structured Layout*: An alternative layout engine that allows developers to control layout.

# Plugin
 1. {Changed} *Timeline*: Comes with an additional grid view to show the same data.
 1. {Fix} *Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement. [i1234](http://getglimpse.com)";
    }
}
