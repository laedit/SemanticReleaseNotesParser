using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SemanticReleaseNotesParser.Core.Tests
{
    public class SemanticReleaseNotesConverterTest
    {
        [Fact]
        public void Parse_Syntax_Summaries()
        {
            // act
            var releaseNote = SemanticReleaseNotesConverter.Parse(Syntax_Summaries);

            // assert
            Assert.Equal(string.Format("This is a _project_ summary with two paragraphs.Lorem ipsum dolor sit amet consectetuer **adipiscing** elit.Aliquam hendreritmi posuere lectus.{0}{0}Vestibulum `enim wisi` viverra nec fringilla in laoreetvitae risus. Donec sit amet nisl. Aliquam [semper](?) ipsumsit amet velit.", Environment.NewLine), releaseNote.Summary);
        }

        [Fact]
        public void Parse_TextReader_Syntax_Summaries()
        {
            // act
            var releaseNote = SemanticReleaseNotesConverter.Parse(GetTextReader(Syntax_Summaries));

            // assert
            Assert.Equal(string.Format("This is a _project_ summary with two paragraphs.Lorem ipsum dolor sit amet consectetuer **adipiscing** elit.Aliquam hendreritmi posuere lectus.{0}{0}Vestibulum `enim wisi` viverra nec fringilla in laoreetvitae risus. Donec sit amet nisl. Aliquam [semper](?) ipsumsit amet velit.", Environment.NewLine), releaseNote.Summary);
        }

        [Fact]
        public void Format_ExampleA_Default()
        {
            // act
            var resultHtml = SemanticReleaseNotesConverter.Format(GetExempleAReleaseNotes());

            // assert
            Assert.Equal(ExampleAHtml, resultHtml.Trim());
        }

        [Fact]
        public void Format_TextWriter_ExampleA_Default()
        {
            // arrange
            var resultHtml = new StringBuilder();

            // act
            SemanticReleaseNotesConverter.Format(new StringWriter(resultHtml), GetExempleAReleaseNotes());

            // assert
            Assert.Equal(ExampleAHtml, resultHtml.ToString().Trim());
        }

        [Fact]
        public void Convert_ExampleA_Default()
        {
            // act
            var resultHtml = SemanticReleaseNotesConverter.Convert(ExampleA);

            // assert
            Assert.Equal(ExampleAHtml, resultHtml.Trim());
        }

        [Fact]
        public void Convert_TextWriter_ExampleA_Default()
        {
            // arrange
            var resultHtml = new StringBuilder();

            // act
            SemanticReleaseNotesConverter.Convert(GetTextReader(ExampleA), new StringWriter(resultHtml));

            // assert
            Assert.Equal(ExampleAHtml, resultHtml.ToString().Trim());
        }

        private TextReader GetTextReader(string input)
        {
            return new StringReader(input);
        }

        private ReleaseNotes GetExempleAReleaseNotes()
        {
            return new ReleaseNotes
            {
                Summary = "Incremental release designed to provide an update to some of the core plugins.",
                Items = new List<Item>
                 {
                     new Item { Categories = { { "New" } }, Summary = "Release Checker: Now gives you a breakdown of exactly what you are missing." },
                     new Item { Categories = { { "New" } }, Summary = "Structured Layout: An alternative layout engine that allows developers to control layout." },
                     new Item { Categories = { { "Changed" } }, Summary = "Timeline: Comes with an additional grid view to show the same data." },
                     new Item { Categories = { { "Fix" } }, Summary = "Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement." }
                 }
            };
        }

        private const string Syntax_Summaries = @"This is a _project_ summary with two paragraphs.
Lorem ipsum dolor sit amet consectetuer **adipiscing** elit.
Aliquam hendreritmi posuere lectus.

Vestibulum `enim wisi` viverra nec fringilla in laoreet
vitae risus. Donec sit amet nisl. Aliquam [semper](?) ipsum
sit amet velit.";

        private const string ExampleA = @"Incremental release designed to provide an update to some of the core plugins.

 - Release Checker: Now gives you a breakdown of exactly what you are missing. +New
 - Structured Layout: An alternative layout engine that allows developers to control layout. +New
 - Timeline: Comes with an additional grid view to show the same data. +Changed
 - Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement. +Fix";

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
    }
}
