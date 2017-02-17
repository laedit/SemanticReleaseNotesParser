using NSubstitute;
using SemanticReleaseNotesParser.Abstractions;
using SemanticReleaseNotesParser.BuildServers;
using SemanticReleaseNotesParser.Logging;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SemanticReleaseNotesParser.Tests.BuildServers
{
    public class LocalBuildServerTest
    {
        [Fact]
        public void CanApplyToCurrentContext()
        {
            // arrange
            var buildServer = new LocalBuildServer(GetEnvironment());

            // act
            var canApply = buildServer.CanApplyToCurrentContext();

            // assert
            Assert.True(canApply);
        }

        [Fact]
        public void SetEnvironmentVariable()
        {
            // arrange
            var logs = new StringBuilder();
            Logger.SetLogAction((message, logLevel) => logs.AppendLine(message));
            var buildServer = new LocalBuildServer(GetEnvironment());

            // act
            buildServer.SetEnvironmentVariable("name", "value");

            // assert
            Assert.Equal("value", _environmentVariables["name"]);
            Assert.Equal("Adding local environment variable: name.", logs.ToString().Trim());
        }
        
        private Dictionary<string, string> _environmentVariables;

        private IEnvironment GetEnvironment()
        {
            _environmentVariables = new Dictionary<string, string>();

            var environment = Substitute.For<IEnvironment>();
            environment.When(e => e.SetEnvironmentVariable(Arg.Any<string>(), Arg.Any<string>())).Do(ci => _environmentVariables.Add((string)ci.Args()[0], (string)ci.Args()[1]));

            return environment;
        }
    }
}
