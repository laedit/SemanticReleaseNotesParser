using System;
using System.IO;
using Xunit;

namespace SemanticReleaseNotesParser.Core.Tests
{
    public class SemanticReleaseNotesParserTest
    {
        [Fact]
        public void Parse_Null_Exception()
        {
            // act & assert
            var exception = Assert.Throws<ArgumentNullException>(() => SemanticReleaseNotesParser.Parse((string)null));
            Assert.Equal("rawReleaseNotes", exception.ParamName);
        }

        [Fact]
        public void Parse_TextReader_Null_Exception()
        {
            // act & assert
            var exception = Assert.Throws<ArgumentNullException>(() => SemanticReleaseNotesParser.Parse((TextReader)null));
            Assert.Equal("reader", exception.ParamName);
        }

        [Fact]
        public void Parse_Syntax_Summaries()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(Syntax_Summaries);

            // assert
            Assert.Equal("This is a _project_ summary with two paragraphs.Lorem ipsum dolor sit amet consectetuer **adipiscing** elit.Aliquam hendreritmi posuere lectus.\r\n\r\nVestibulum `enim wisi` viverra nec fringilla in laoreetvitae risus. Donec sit amet nisl. Aliquam [semper](?) ipsumsit amet velit.", releaseNote.Summary);
        }

        [Fact]
        public void Parse_TextReader_Syntax_Summaries()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(GetTextReader(Syntax_Summaries));

            // assert
            Assert.Equal("This is a _project_ summary with two paragraphs.Lorem ipsum dolor sit amet consectetuer **adipiscing** elit.Aliquam hendreritmi posuere lectus.\r\n\r\nVestibulum `enim wisi` viverra nec fringilla in laoreetvitae risus. Donec sit amet nisl. Aliquam [semper](?) ipsumsit amet velit.", releaseNote.Summary);
        }

        [Fact]
        public void Parse_Syntax_Items()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(Syntax_Items);

            // assert
            Assert.Equal(4, releaseNote.Items.Count);

            Assert.Equal("This is the _first_ *list* item.", releaseNote.Items[0].Summary);
            Assert.Equal("This is the **second** __list__ item.", releaseNote.Items[1].Summary);
            Assert.Equal("This is the `third` list item.", releaseNote.Items[2].Summary);
            Assert.Equal("This is the [forth](?) list item.", releaseNote.Items[3].Summary);
        }

        [Fact]
        public void Parse_Syntax_Sections()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(Syntax_Sections);

            // assert
            Assert.Equal(2, releaseNote.Sections.Count);

            Assert.Equal("Section", releaseNote.Sections[0].Name);
            Assert.Equal("This is the summary for Section.", releaseNote.Sections[0].Summary);

            Assert.Equal(2, releaseNote.Sections[0].Items.Count);
            Assert.Equal("This is a Section scoped first list item.", releaseNote.Sections[0].Items[0].Summary);
            Assert.Equal("This is a Section scoped second list item.", releaseNote.Sections[0].Items[1].Summary);

            Assert.Equal("Other Section", releaseNote.Sections[1].Name);
            Assert.Equal("This is the summary for Other Section.", releaseNote.Sections[1].Summary);

            Assert.Equal(2, releaseNote.Sections[1].Items.Count);
            Assert.Equal("This is a Other Section scoped first list item.", releaseNote.Sections[1].Items[0].Summary);
            Assert.Equal("This is a Other Section scoped second list item.", releaseNote.Sections[1].Items[1].Summary);
        }

        [Fact]
        public void Parse_Syntax_Priority()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(Syntax_Priority);

            // assert
            Assert.Equal(string.Empty, releaseNote.Summary);
            Assert.Equal(7, releaseNote.Items.Count);

            Assert.Equal(1, releaseNote.Items[0].Priority);
            Assert.Equal("This is a High priority list item.", releaseNote.Items[0].Summary);

            Assert.Equal(1, releaseNote.Items[1].Priority);
            Assert.Equal("This is a High priority list item.", releaseNote.Items[1].Summary);

            Assert.Equal(2, releaseNote.Items[2].Priority);
            Assert.Equal("This is a Normal priority list item.", releaseNote.Items[2].Summary);

            Assert.Equal(1, releaseNote.Items[3].Priority);
            Assert.Equal("This is a High priority list item.", releaseNote.Items[3].Summary);

            Assert.Equal(2, releaseNote.Items[4].Priority);
            Assert.Equal("This is a Normal priority list item.", releaseNote.Items[4].Summary);

            Assert.Equal(3, releaseNote.Items[5].Priority);
            Assert.Equal("This is a Minor priority list item.", releaseNote.Items[5].Summary);

            Assert.Equal(3, releaseNote.Items[6].Priority);
            Assert.Equal("This is a Minor priority list item.", releaseNote.Items[6].Summary);
        }

        [Fact]
        public void Parse_Syntax_Category()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(Syntax_Category);

            // assert
            Assert.Equal(string.Empty, releaseNote.Summary);
            Assert.Equal(8, releaseNote.Items.Count);

            Assert.Equal("New", releaseNote.Items[0].Categories[0]);
            Assert.Equal("This is a New list item.", releaseNote.Items[0].Summary);

            Assert.Equal("Fix", releaseNote.Items[1].Categories[0]);
            Assert.Equal("This is a Fix list item.", releaseNote.Items[1].Summary);

            Assert.Equal("Change", releaseNote.Items[2].Categories[0]);
            Assert.Equal("This is a Change list item.", releaseNote.Items[2].Summary);

            Assert.Equal("New", releaseNote.Items[3].Categories[0]);
            Assert.Equal("New features are everyone's favorites.", releaseNote.Items[3].Summary);

            Assert.Equal("Developer", releaseNote.Items[4].Categories[0]);
            Assert.Equal("This is a list item for a Developer.", releaseNote.Items[4].Summary);

            Assert.Equal("Super Special", releaseNote.Items[5].Categories[0]);
            Assert.Equal("This is a super-special custom list item.", releaseNote.Items[5].Summary);

            Assert.Equal("O", releaseNote.Items[6].Categories[0]);
            Assert.Equal("This is a o ne letter category.", releaseNote.Items[6].Summary);

            Assert.Equal("Developer", releaseNote.Items[7].Categories[0]);
            Assert.Equal("New", releaseNote.Items[7].Categories[1]);
            Assert.Equal("This is my last DEVELOPER list item.", releaseNote.Items[7].Summary);
        }

        [Fact]
        public void Parse_Example_A()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(ExampleA);

            // assert
            Assert.Equal("Incremental release designed to provide an update to some of the core plugins.", releaseNote.Summary);
            Assert.Equal(4, releaseNote.Items.Count);
            Assert.Equal("Release Checker: Now gives you a breakdown of exactly what you are missing.", releaseNote.Items[0].Summary);
            Assert.Equal("New", releaseNote.Items[0].Categories[0]);

            Assert.Equal("Structured Layout: An alternative layout engine that allows developers to control layout.", releaseNote.Items[1].Summary);
            Assert.Equal("New", releaseNote.Items[1].Categories[0]);

            Assert.Equal("Timeline: Comes with an additional grid view to show the same data.", releaseNote.Items[2].Summary);
            Assert.Equal("Changed", releaseNote.Items[2].Categories[0]);

            Assert.Equal("Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement.", releaseNote.Items[3].Summary);
            Assert.Equal("Fix", releaseNote.Items[3].Categories[0]);
        }

        [Fact]
        public void Parse_Example_B()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(ExampleB);

            // assert
            Assert.Equal("Incremental release designed to provide an update to some of the core plugins.", releaseNote.Summary);
            Assert.Equal(0, releaseNote.Items.Count);

            Assert.Equal(2, releaseNote.Sections.Count);

            Assert.Equal("System", releaseNote.Sections[0].Name);
            Assert.Equal(2, releaseNote.Sections[0].Items.Count);

            Assert.Equal("*Release Checker*: Now gives you a breakdown of exactly what you are missing.", releaseNote.Sections[0].Items[0].Summary);
            Assert.Equal("New", releaseNote.Sections[0].Items[0].Categories[0]);

            Assert.Equal("*Structured Layout*: An alternative layout engine that allows developers to control layout.", releaseNote.Sections[0].Items[1].Summary);
            Assert.Equal("New", releaseNote.Sections[0].Items[1].Categories[0]);

            Assert.Equal("Plugin", releaseNote.Sections[1].Name);
            Assert.Equal(2, releaseNote.Sections[1].Items.Count);

            Assert.Equal("*Timeline*: Comes with an additional grid view to show the same data.", releaseNote.Sections[1].Items[0].Summary);
            Assert.Equal("Changed", releaseNote.Sections[1].Items[0].Categories[0]);

            Assert.Equal("*Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement.", releaseNote.Sections[1].Items[1].Summary);
            Assert.Equal("Fix", releaseNote.Sections[1].Items[1].Categories[0]);
        }

        [Fact]
        public void Parse_Example_C()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(ExampleC);

            // assert
            Assert.Equal("Incremental release designed to provide an update to some of the core plugins.", releaseNote.Summary);

            Assert.Equal(1, releaseNote.Items.Count);
            Assert.Equal("*Example*: You can have global issues that aren't grouped to a section", releaseNote.Items[0].Summary);

            Assert.Equal(2, releaseNote.Sections.Count);

            Assert.Equal("System", releaseNote.Sections[0].Name);
            Assert.Equal("This description is specific to system section.", releaseNote.Sections[0].Summary);
            Assert.Equal(2, releaseNote.Sections[0].Items.Count);

            Assert.Equal("*Release Checker*: Now gives you a new breakdown of exactly what you are missing.", releaseNote.Sections[0].Items[0].Summary);
            Assert.Equal("New", releaseNote.Sections[0].Items[0].Categories[0]);

            Assert.Equal("*Structured Layout*: A new alternative layout engine that allows developers to control layout.", releaseNote.Sections[0].Items[1].Summary);
            Assert.Equal("New", releaseNote.Sections[0].Items[1].Categories[0]);

            Assert.Equal("Plugin", releaseNote.Sections[1].Name);
            Assert.Equal("This description is specific to plugin section.", releaseNote.Sections[1].Summary);
            Assert.Equal(2, releaseNote.Sections[1].Items.Count);

            Assert.Equal("*Timeline*: Comes with an additional grid view to show the same data.", releaseNote.Sections[1].Items[0].Summary);
            Assert.Equal("Changed", releaseNote.Sections[1].Items[0].Categories[0]);

            Assert.Equal("*Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement.", releaseNote.Sections[1].Items[1].Summary);
            Assert.Equal("Fix", releaseNote.Sections[1].Items[1].Categories[0]);
            Assert.Equal("i1234", releaseNote.Sections[1].Items[1].TaskId);
            Assert.Equal("http://getglimpse.com", releaseNote.Sections[1].Items[1].TaskLink);
        }

        [Fact]
        public void Parse_Example_D()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(ExampleD);

            // assert
            Assert.Equal("Incremental release designed to provide an update to some of the core plugins.", releaseNote.Summary);

            Assert.Equal(1, releaseNote.Items.Count);
            Assert.Equal(1, releaseNote.Items[0].Priority);
            Assert.Equal("*Example*: You can have global issues that aren't grouped to a section", releaseNote.Items[0].Summary);

            Assert.Equal(2, releaseNote.Sections.Count);

            Assert.Equal("System ", releaseNote.Sections[0].Name);
            Assert.Equal("This description is specific to system section.", releaseNote.Sections[0].Summary);
            Assert.Equal("http://getglimpse.com/release/icon/core.png", releaseNote.Sections[0].Icon);
            Assert.Equal(2, releaseNote.Sections[0].Items.Count);

            Assert.Equal("*Release Checker*: Now gives you a breakdown of exactly what you are missing.", releaseNote.Sections[0].Items[0].Summary);
            Assert.Equal("New", releaseNote.Sections[0].Items[0].Categories[0]);
            Assert.Equal(3, releaseNote.Sections[0].Items[0].Priority);

            Assert.Equal("*Structured Layout*: An alternative layout engine that allows developers to control layout.", releaseNote.Sections[0].Items[1].Summary);
            Assert.Equal("New", releaseNote.Sections[0].Items[1].Categories[0]);
            Assert.Equal(2, releaseNote.Sections[0].Items[1].Priority);

            Assert.Equal("Plugin ", releaseNote.Sections[1].Name);
            Assert.Equal("This description is specific to plugin section.", releaseNote.Sections[1].Summary);
            Assert.Equal("http://getglimpse.com/release/icon/mvc.png", releaseNote.Sections[1].Icon);
            Assert.Equal(2, releaseNote.Sections[1].Items.Count);

            Assert.Equal("*Timeline*: Comes with an additional grid view to show the same data.", releaseNote.Sections[1].Items[0].Summary);
            Assert.Equal("Changed", releaseNote.Sections[1].Items[0].Categories[0]);
            Assert.Equal(1, releaseNote.Sections[1].Items[1].Priority);

            Assert.Equal("*Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement.", releaseNote.Sections[1].Items[1].Summary);
            Assert.Equal("Fix", releaseNote.Sections[1].Items[1].Categories[0]);
            Assert.Equal(1, releaseNote.Sections[1].Items[1].Priority);
            Assert.Equal("i1234", releaseNote.Sections[1].Items[1].TaskId);
            Assert.Equal("http://getglimpse.com", releaseNote.Sections[1].Items[1].TaskLink);
        }

        [Fact]
        public void Parse_Real_NVika()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(NVikaReleaseNotes);

            // assert
            Assert.Equal(1, releaseNote.Metadata.Count);
            Assert.Equal("Commits", releaseNote.Metadata[0].Name);
            Assert.Equal("[19556f025b...0203ea9a43](https://github.com/laedit/vika/compare/19556f025b...0203ea9a43)", releaseNote.Metadata[0].Value);

            Assert.Equal(3, releaseNote.Items.Count);

            Assert.Equal("Enhancement", releaseNote.Items[0].Categories[0]);
            Assert.Equal("[#9](https://github.com/laedit/vika/issues/9) - Handle multiple report files", releaseNote.Items[0].Summary);

            Assert.Equal("Enhancement", releaseNote.Items[1].Categories[0]);
            Assert.Equal("[#2](https://github.com/laedit/vika/issues/2) - Support AppVeyor", releaseNote.Items[1].Summary);

            Assert.Equal("Enhancement", releaseNote.Items[2].Categories[0]);
            Assert.Equal("[#1](https://github.com/laedit/vika/issues/1) - Support InspectCode", releaseNote.Items[2].Summary);
        }

        [Fact]
        public void Parse_Example_A_WithOnlyLF()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(ExampleAWithOnlyLF);

            // assert
            Assert.Equal("Incremental release designed to provide an update to some of the core plugins.", releaseNote.Summary);
            Assert.Equal(4, releaseNote.Items.Count);
            Assert.Equal("Release Checker: Now gives you a breakdown of exactly what you are missing.", releaseNote.Items[0].Summary);
            Assert.Equal("New", releaseNote.Items[0].Categories[0]);

            Assert.Equal("Structured Layout: An alternative layout engine that allows developers to control layout.", releaseNote.Items[1].Summary);
            Assert.Equal("New", releaseNote.Items[1].Categories[0]);

            Assert.Equal("Timeline: Comes with an additional grid view to show the same data.", releaseNote.Items[2].Summary);
            Assert.Equal("Changed", releaseNote.Items[2].Categories[0]);

            Assert.Equal("Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement.", releaseNote.Items[3].Summary);
            Assert.Equal("Fix", releaseNote.Items[3].Categories[0]);
        }

        [Fact]
        public void Parse_Syntax_Metadata_Commits()
        {
            // act
            var releaseNote = SemanticReleaseNotesParser.Parse(Syntax_Metadata_Commits);

            // assert
            Assert.Equal(2, releaseNote.Metadata.Count);

            Assert.Equal("Commits", releaseNote.Metadata[0].Name);
            Assert.Equal("56af25a...d3fead4", releaseNote.Metadata[0].Value);
            Assert.Equal("Commits", releaseNote.Metadata[1].Name);
            Assert.Equal("[56af25a...d3fead4](https://github.com/Glimpse/Semantic-Release-Notes/compare/56af25a...d3fead4)", releaseNote.Metadata[1].Value);
        }

        private TextReader GetTextReader(string input)
        {
            return new StringReader(input);
        }

        private const string Syntax_Summaries = @"This is a _project_ summary with two paragraphs.
Lorem ipsum dolor sit amet consectetuer **adipiscing** elit.
Aliquam hendreritmi posuere lectus.

Vestibulum `enim wisi` viverra nec fringilla in laoreet
vitae risus. Donec sit amet nisl. Aliquam [semper](?) ipsum
sit amet velit.";

        private const string Syntax_Items = @" - This is the _first_ *list* item.
 - This is the **second** __list__ item.
 - This is the `third` list item.
 - This is the [forth](?) list item.";

        private const string Syntax_Sections = @"# Section
This is the summary for Section.
 - This is a Section scoped first list item.
 - This is a Section scoped second list item.

# Other Section
This is the summary for Other Section.
 - This is a Other Section scoped first list item.
 - This is a Other Section scoped second list item.
       ";

        private const string Syntax_Priority = @" 1. This is a High priority list item.
 1. This is a High priority list item.
 2. This is a Normal priority list item.
 1. This is a High priority list item.
 2. This is a Normal priority list item.
 3. This is a Minor priority list item.
 3. This is a Minor priority list item.
  ";

        private const string Syntax_Category = @" - This is a +New list item.
 - This is a +Fix list item.
 - This is a +Change list item.
 - +New features are everyone's favorites.
 - This is a list item for a +Developer.
 - This is a +super-special custom list item.
 - This is a +o ne letter category.
 - This is my last +DEVELOPER list item. +New
         ";

        private const string ExampleA = @"Incremental release designed to provide an update to some of the core plugins.

 - Release Checker: Now gives you a breakdown of exactly what you are missing. +New
 - Structured Layout: An alternative layout engine that allows developers to control layout. +New
 - Timeline: Comes with an additional grid view to show the same data. +Changed
 - Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement. +Fix";

        private const string ExampleAWithOnlyLF = "Incremental release designed to provide an update to some of the core plugins.\n\n - Release Checker: Now gives you a breakdown of exactly what you are missing. +New\n - Structured Layout: An alternative layout engine that allows developers to control layout. +New\n - Timeline: Comes with an additional grid view to show the same data. +Changed\n - Ajax: Fix that crashed poll in Chrome and IE due to log/trace statement. +Fix";

        private const string ExampleB = @"Incremental release designed to provide an update to some of the core plugins.

# System
 - *Release Checker*: Now gives you a breakdown of exactly what you are missing. +New
 - *Structured Layout*: An alternative layout engine that allows developers to control layout. +New

# Plugin
 - *Timeline*: Comes with an additional grid view to show the same data. +Changed
 - *Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement. +Fix";

        private const string ExampleC = @"Incremental release designed to provide an update to some of the core plugins.
 - *Example*: You can have global issues that aren't grouped to a section

# System
This description is specific to system section.
 - *Release Checker*: Now gives you a +new breakdown of exactly what you are missing.
 - *Structured Layout*: A +new alternative layout engine that allows developers to control layout.

# Plugin
This description is specific to plugin section.
 - *Timeline*: Comes with an additional grid view to show the same data. +Changed
 - *Ajax*: +Fix that crashed poll in Chrome and IE due to log/trace statement. [[i1234][http://getglimpse.com]]";

        private const string ExampleD = @"Incremental release designed to provide an update to some of the core plugins.
 1. *Example*: You can have global issues that aren't grouped to a section

# System [[icon][http://getglimpse.com/release/icon/core.png]]
This description is specific to system section.
 3. *Release Checker*: Now gives you a breakdown of exactly what you are missing. +New
 2. *Structured Layout*: An alternative layout engine that allows developers to control layout. +New

# Plugin [[icon][http://getglimpse.com/release/icon/mvc.png]]
This description is specific to plugin section.
 1. *Timeline*: Comes with an additional grid view to show the same data. +Changed
 1. *Ajax*: Fix that crashed poll in Chrome and IE due to log/trace statement. +Fix [[i1234][http://getglimpse.com]]";

        private const string NVikaReleaseNotes = @" - [#9](https://github.com/laedit/vika/issues/9) - Handle multiple report files +enhancement
 - [#2](https://github.com/laedit/vika/issues/2) - Support AppVeyor +enhancement
 - [#1](https://github.com/laedit/vika/issues/1) - Support InspectCode +enhancement

Commits: [19556f025b...0203ea9a43](https://github.com/laedit/vika/compare/19556f025b...0203ea9a43)
";

        private const string Syntax_Metadata_Commits = @"56af25a...d3fead4
Commits: [56af25a...d3fead4](https://github.com/Glimpse/Semantic-Release-Notes/compare/56af25a...d3fead4)";
    }
}