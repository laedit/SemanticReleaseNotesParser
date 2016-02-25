﻿using NSubstitute;
using SemanticReleaseNotesParser.Abstractions;
using SemanticReleaseNotesParser.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text;
using Xunit;

namespace SemanticReleaseNotesParser.Tests
{
    public class ProgramTest : IDisposable
    {
        [Fact]
        public void Run_Help()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "-?" });

            // assert
            Assert.Equal(1, _exitCode);
            Assert.Contains("-r", _output.ToString());
            Assert.Contains("--releasenotes", _output.ToString());
        }

        [Fact]
        public void Run_Default()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new string[0]);

            // assert
            Assert.Equal(0, _exitCode);
            Assert.Equal(ExpectedHtml, Program.FileSystem.File.ReadAllText("ReleaseNotes.html").Trim());
            Assert.DoesNotContain("File output", _output.ToString());
            Assert.Contains("File 'ReleaseNotes.html' generated", _output.ToString());
        }

        [Fact]
        public void Run_Default_Debug()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "--debug" });

            // assert
            Assert.Equal(0, _exitCode);
            Assert.Equal(ExpectedHtml, Program.FileSystem.File.ReadAllText("ReleaseNotes.html").Trim());
            Assert.Contains("File output", _output.ToString());
        }

        [Fact]
        public void Run_Default_FileNotExists_Exception()
        {
            // arrange
            Program.FileSystem = GetFileSystem(false);
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new string[0]);

            // assert
            Assert.Equal(1, _exitCode);
            Assert.Contains("Release notes file 'ReleaseNotes.md' does not exists", _output.ToString());
        }

        [Fact]
        public void Run_Custom_ReleaseNotes_And_OutputFile()
        {
            // arrange
            Program.FileSystem = GetFileSystem(true, "myreleansenotes.md");
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "-r=myreleansenotes.md", "-o=MyReleaseNotes.html" });

            // assert
            Assert.Equal(0, _exitCode);
            Assert.Equal(ExpectedHtml, Program.FileSystem.File.ReadAllText("MyReleaseNotes.html").Trim());
        }

        [Fact]
        public void Run_Custom_Template()
        {
            // arrange
            Program.FileSystem = GetFileSystem(true, "ReleaseNotes.md", "template.liquid", CustomTemplate);
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "--template=template.liquid" });

            // assert
            Assert.Equal(0, _exitCode);
            Assert.Equal(ExpectedCustomHtml, Program.FileSystem.File.ReadAllText("ReleaseNotes.html").Trim());
        }

        [Fact]
        public void Run_Custom_Template_Does_Not_Exists()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "--template=template.liquid" });

            // assert
            Assert.Equal(1, _exitCode);
            Assert.Contains("Template file 'template.liquid' does not exists", _output.ToString());
        }

        [Fact]
        public void Run_Default_Output_Format_Markdown()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "-f=markdown" });

            // assert
            Assert.Equal(0, _exitCode);
            Assert.Equal(ExpectedMarkdow, Program.FileSystem.File.ReadAllText("ReleaseNotes.html").Trim());
        }

        [Fact]
        public void Run_Handle_Global_Exception()
        {
            // arrange
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.File.Returns(ci => { throw new InvalidOperationException("Boom when accessing File"); });
            Program.FileSystem = fileSystem;
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new string[0]);

            // assert
            Assert.Equal(1, _exitCode);
            Assert.Contains("An unexpected error occurred:", _output.ToString());
            Assert.Contains("InvalidOperationException", _output.ToString());
            Assert.Contains("Boom when accessing File", _output.ToString());
        }

        [Fact]
        public void Run_Environment()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "-t=environment" });

            // assert
            Assert.Equal(0, _exitCode);
            Assert.False(Program.FileSystem.File.Exists("ReleaseNotes.html"));
            Assert.Equal(ExpectedHtml, _environmentVariables["SemanticReleaseNotes"].Trim());
        }

        [Fact]
        public void Run_FileAndEnvironment()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment();
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "-t=fileandenvironment" });

            // assert
            Assert.Equal(0, _exitCode);
            Assert.Equal(ExpectedHtml, Program.FileSystem.File.ReadAllText("ReleaseNotes.html").Trim());
            Assert.Equal(ExpectedHtml, _environmentVariables["SemanticReleaseNotes"].Trim());
        }

        [Fact]
        public void Run_Environment_AppVeyor()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment(true);
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "-t=environment" });

            // assert
            Assert.Equal(0, _exitCode);
            Assert.False(Program.FileSystem.File.Exists("ReleaseNotes.html"));
            Assert.False(_environmentVariables.ContainsKey("SemanticReleaseNotes"));
            Assert.Equal(ExpectedAppVeyorData, _uploadedData);
        }

        [Fact]
        public void Run_IncludeStyle_Default()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment(true);
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "-includestyle" });

            // assert
            Assert.Equal(0, _exitCode);
            Assert.Equal(GetExampleAHtmlWithStyle(), Program.FileSystem.File.ReadAllText("ReleaseNotes.html").Trim());
        }

        [Fact]
        public void Run_IncludeStyle_Custom()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment(true);
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "-includestyle=\"body { color: black; width: 500px; }\"" });

            // assert
            Assert.Equal(0, _exitCode);
            Assert.Equal(GetExampleAHtmlWithStyle("body { color: black; width: 500px; }"), Program.FileSystem.File.ReadAllText("ReleaseNotes.html").Trim());
        }

        [Fact]
        public void Run_PluralizeCategoriesTitle()
        {
            // arrange
            Program.FileSystem = GetFileSystem();
            Program.Environment = GetEnvironment(true);
            Program.WebClientFactory = GetWebClientFactory();

            // act
            Program.Main(new[] { "--pluralizecategoriestitle", "-g=categories" });

            // assert
            Assert.Equal(0, _exitCode);
            Assert.True(Program.FileSystem.File.Exists("ReleaseNotes.html"));
            Assert.Equal(ExpectedHtmlCategories, Program.FileSystem.File.ReadAllText("ReleaseNotes.html"));
        }

        private StringBuilder _output;

        public ProgramTest()
        {
            _output = new StringBuilder();
            Console.SetOut(new StringWriter(_output));
        }

        private Dictionary<string, string> _environmentVariables;
        private int _exitCode;

        private IEnvironment GetEnvironment(bool isOnAppVeyor = false)
        {
            _environmentVariables = new Dictionary<string, string>();

            var environment = Substitute.For<IEnvironment>();
            environment.When(e => e.SetEnvironmentVariable(Arg.Any<string>(), Arg.Any<string>())).Do(ci => _environmentVariables.Add((string)ci.Args()[0], (string)ci.Args()[1]));

            environment.GetEnvironmentVariable("APPVEYOR_API_URL").Returns("http://localhost:8080");

            if (isOnAppVeyor)
            {
                environment.GetEnvironmentVariable("APPVEYOR").Returns("TRUE");
            }

            environment.When(e => e.Exit(Arg.Any<int>())).Do(ci => _exitCode = ci.Arg<int>());

            return environment;
        }

        private string _uploadedData;
        private string _address;
        private string _method;

        private IWebClientFactory GetWebClientFactory()
        {
            var webClient = Substitute.For<IWebClient>();
            webClient.When(wc => wc.UploadData(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<byte[]>()))
                .Do(ci =>
                {
                    _address = (string)ci.Args()[0];
                    _method = (string)ci.Args()[1];
                    _uploadedData = Encoding.UTF8.GetString((byte[])ci.Args()[2]);
                });

            var factory = Substitute.For<IWebClientFactory>();
            factory.Create(Arg.Any<string>()).Returns(webClient);

            return factory;
        }

        private IFileSystem GetFileSystem(bool addFile = true, string fileName = "ReleaseNotes.md", string secondFileName = null, string secondFileContent = null)
        {
            var fileSystem = new MockFileSystem();

            if (addFile)
            {
                fileSystem.AddFile(fileName, new MockFileData(DefaultMarkdown));
            }

            if (secondFileName != null)
            {
                fileSystem.AddFile(secondFileName, new MockFileData(secondFileContent));
            }

            return fileSystem;
        }

        private string GetExampleAHtmlWithStyle(string style = null)
        {
            if (style == null)
            {
                using (var reader = new StreamReader(Assembly.GetAssembly(typeof(SemanticReleaseNotesConverter)).GetManifestResourceStream("SemanticReleaseNotesParser.Core.Resources.DefaultStyle.css")))
                {
                    style = reader.ReadToEnd();
                }
            }
            return string.Format(ExpectedHtmlWithHeader, style);
        }

        public void Dispose()
        {
            Logger.Reset();
        }

        private const string DefaultMarkdown = @"A little summary
# System
 - This is the **second** __list__ item. +new
 - This is the `third` list item. +fix";

        private const string ExpectedHtml = @"<html>
<body>
<p>A little summary</p>
<h1>System</h1>
<ul>
<li>{New} This is the <strong>second</strong> <strong>list</strong> item.</li>
<li>{Fix} This is the <code>third</code> list item.</li>
</ul>
</body>
</html>";

        private const string ExpectedMarkdow = @"A little summary

# System
 - {New} This is the **second** __list__ item.
 - {Fix} This is the `third` list item.";

        private const string CustomTemplate = @"{%- for section in release_notes.sections -%}
# {{ section.name }}
{%- for item in section.items -%}
 - {% if item.categories -%}{{ lcb }}{{ item.categories[0] }}{{ rcb }} {% endif %}{{ item.summary }} {%- if item.task_id %} [{{ item.task_id }}]({{ item.task_link }}) {%- endif -%}

{%- endfor -%}
{%- endfor -%}

{{ release_notes.summary }}";

        private const string ExpectedCustomHtml = @"<html>
<body>
<h1>System</h1>
<ul>
<li>{New} This is the <strong>second</strong> <strong>list</strong> item.</li>
<li>{Fix} This is the <code>third</code> list item.</li>
</ul>
<p>A little summary</p>
</body>
</html>";

        private const string ExpectedAppVeyorData = "{ \"name\": \"SemanticReleaseNotes\", \"value\": \"<html>\n<body>\n<p>A little summary<\\/p>\n<h1>System<\\/h1>\n<ul>\n<li>{New} This is the <strong>second<\\/strong> <strong>list<\\/strong> item.<\\/li>\n<li>{Fix} This is the <code>third<\\/code> list item.<\\/li>\n<\\/ul>\n<\\/body>\n<\\/html>\" }";

        private const string ExpectedHtmlCategories = @"<html>
<body>
<p>A little summary</p>
<h1>News</h1>
<ul>
<li>This is the <strong>second</strong> <strong>list</strong> item.</li>
</ul>
<h1>Fixes</h1>
<ul>
<li>This is the <code>third</code> list item.</li>
</ul>
</body>
</html>";

        private const string ExpectedHtmlWithHeader = @"<html>
<header>
<style>
{0}
</style>
</header>
<body>
<p>A little summary</p>
<h1>System</h1>
<ul>
<li>{{New}} This is the <strong>second</strong> <strong>list</strong> item.</li>
<li>{{Fix}} This is the <code>third</code> list item.</li>
</ul>
</body>
</html>";
    }
}