using NSubstitute;
using SemanticReleaseNotesParser.Abstractions;
using SemanticReleaseNotesParser.BuildServers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SemanticReleaseNotesParser.Tests.BuildServers
{
    public class AppVeyorTest
    {
        [Fact]
        public void CanApplyToCurrentContext_True()
        {
            // arrange
            var buildServer = new AppVeyor(GetEnvironment(), GetWebClientFactory());

            // act
            var canApply = buildServer.CanApplyToCurrentContext();

            // assert
            Assert.True(canApply);
        }

        [Fact]
        public void CanApplyToCurrentContext_False()
        {
            // arrange
            var buildServer = new AppVeyor(GetEnvironment(false), GetWebClientFactory());

            // act
            var canApply = buildServer.CanApplyToCurrentContext();

            // assert
            Assert.False(canApply);
        }

        [Fact]
        public void SetEnvironmentVariable()
        {
            // arrange
            var buildServer = new AppVeyor(GetEnvironment(), GetWebClientFactory());

            // act
            buildServer.SetEnvironmentVariable("name", "value\r\n\"value2\" \\o/");

            // assert
            Assert.False(_environmentVariables.ContainsKey("name"));
            Assert.Equal("api/build/variables", _address);
            Assert.Equal("POST", _method);
            Assert.Equal("{ \"name\": \"name\", \"value\": \"value\n\\\"value2\\\" \\\\o\\/\" }", _uploadedData);
            Assert.Equal("Adding AppVeyor environment variable: name.", _logs.ToString().Trim());
        }

        private StringBuilder _logs;

        public AppVeyorTest()
        {
            _logs = new StringBuilder();
            Logger.SetWriter(new StringWriter(_logs));
        }

        private Dictionary<string, string> _environmentVariables;

        private IEnvironment GetEnvironment(bool isOnAppVeyor = true)
        {
            _environmentVariables = new Dictionary<string, string>();

            var environment = Substitute.For<IEnvironment>();
            environment.When(e => e.SetEnvironmentVariable(Arg.Any<string>(), Arg.Any<string>())).Do(ci => _environmentVariables.Add((string)ci.Args()[0], (string)ci.Args()[1]));

            environment.GetEnvironmentVariable("APPVEYOR_API_URL").Returns("http://localhost:8080");

            if (isOnAppVeyor)
            {
                environment.GetEnvironmentVariable("APPVEYOR").Returns("TRUE");
            }

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
    }
}
