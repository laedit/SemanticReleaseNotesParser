using SemanticReleaseNotesParser.Core;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace SemanticReleaseNotesParser.Tests
{
    public class ArgumentsTest
    {
        [Fact]
        public void WriteHelp_All()
        {
            // arrange
            var arguments = Arguments.ParseArguments(Enumerable.Empty<string>());
            var output = new StringBuilder();
            var writer = new StringWriter(output);

            // act
            arguments.WriteOptionDescriptions(writer);

            // assert
            Assert.Contains("-r, --releasenotes=VALUE", output.ToString());
            Assert.Contains("-o, --outputfile=VALUE", output.ToString());
            Assert.Contains("-t, --outputtype=VALUE", output.ToString());
            Assert.Contains("--template=VALUE", output.ToString());
            Assert.Contains("--debug", output.ToString());
            Assert.Contains("-h, -?, --help", output.ToString());
            Assert.Contains("-f, --outputformat=VALUE", output.ToString());
        }

        [Fact]
        public void ParseArguments_NoArgument()
        {
            // act
            var arguments = Arguments.ParseArguments(new string[0]);

            // assert
            Assert.False(arguments.Debug);
            Assert.False(arguments.Help);
            Assert.Equal(OutputFormat.Html, arguments.OutputFormat);
            Assert.Equal(OutputType.File, arguments.OutputType);
            Assert.Equal("ReleaseNotes.md", arguments.ReleaseNotesPath);
            Assert.Equal("ReleaseNotes.html", arguments.ResultFilePath);
            Assert.Null(arguments.TemplatePath);
        }

        [Fact]
        public void ParseArguments_Debug()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "--debug" });

            // assert
            Assert.True(arguments.Debug);
        }

        [Fact]
        public void ParseArguments_Help_h()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "-h" });

            // assert
            Assert.True(arguments.Help);
        }

        [Fact]
        public void ParseArguments_Help_QuestionMark()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "-?" });

            // assert
            Assert.True(arguments.Help);
        }

        [Fact]
        public void ParseArguments_Help_Help()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "--help" });

            // assert
            Assert.True(arguments.Help);
        }

        [Fact]
        public void ParseArguments_ReleaseNotesPath_r()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "-r=myReleases.md" });

            // assert
            Assert.Equal("myReleases.md", arguments.ReleaseNotesPath);
        }

        [Fact]
        public void ParseArguments_ReleaseNotesPath_releasenotes()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "--releasenotes=myReleases.md" });

            // assert
            Assert.Equal("myReleases.md", arguments.ReleaseNotesPath);
        }

        [Fact]
        public void ParseArguments_ResultFilePath_o()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "-o=myReleases.html" });

            // assert
            Assert.Equal("myReleases.html", arguments.ResultFilePath);
        }

        [Fact]
        public void ParseArguments_ResultFilePath_outputfile()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "--outputfile=myReleases.html" });

            // assert
            Assert.Equal("myReleases.html", arguments.ResultFilePath);
        }

        [Fact]
        public void ParseArguments_OutputTypeh_t()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "-t=environment" });

            // assert
            Assert.Equal(OutputType.Environment, arguments.OutputType);
        }

        [Fact]
        public void ParseArguments_OutputType_outputfile()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "--outputtype=environment" });

            // assert
            Assert.Equal(OutputType.Environment, arguments.OutputType);
        }

        [Fact]
        public void ParseArguments_OutputFormat_f()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "-f=markdown" });

            // assert
            Assert.Equal(OutputFormat.Markdown, arguments.OutputFormat);
        }

        [Fact]
        public void ParseArguments_OutputFormat_outputformat()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "--outputformat=markdown" });

            // assert
            Assert.Equal(OutputFormat.Markdown, arguments.OutputFormat);
        }

        [Fact]
        public void ParseArguments_Template()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "--template=template.liquid" });

            // assert
            Assert.Equal("template.liquid", arguments.TemplatePath);
        }

        [Fact]
        public void ParseArguments_FirstAdditionalArgument_IsReleaseNotesPath()
        {
            // act
            var arguments = Arguments.ParseArguments(new[] { "myReleases.md" });

            // assert
            Assert.Equal("myReleases.md", arguments.ReleaseNotesPath);
        }
    }
}
