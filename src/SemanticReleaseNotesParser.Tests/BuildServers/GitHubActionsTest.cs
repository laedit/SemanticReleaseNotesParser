using NSubstitute;
using SemanticReleaseNotesParser.Abstractions;
using SemanticReleaseNotesParser.BuildServers;
using SemanticReleaseNotesParser.Logging;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SemanticReleaseNotesParser.Tests.BuildServers
{
    public class GitHubActionsTest
    {
        [Fact]
        public void CanApplyToCurrentContext_True()
        {
            // arrange
            var buildServer = new GitHubActions(GetEnvironment());

            // act
            var canApply = buildServer.CanApplyToCurrentContext();

            // assert
            Assert.True(canApply);
        }

        [Fact]
        public void CanApplyToCurrentContext_False()
        {
            // arrange
            var buildServer = new GitHubActions(GetEnvironment(false));

            // act
            var canApply = buildServer.CanApplyToCurrentContext();

            // assert
            Assert.False(canApply);
        }

        [Fact]
        public void SetEnvironmentVariable()
        {
            // arrange
            var logs = new StringBuilder();
            Logger.SetLogAction((message, logLevel) => logs.AppendLine(message));
            var buildServer = new GitHubActions(GetEnvironment());

            // act
            buildServer.SetEnvironmentVariable("name", "value\r\n\"value2\" \\o/");

            // assert
            Assert.False(_environmentVariables.ContainsKey("name"));
            var trimmedLogs = logs.ToString().Trim();
            Assert.Contains("::set-env name=name::value%0D%0A\"value2\" \\o/", trimmedLogs);
            Assert.Contains("Adding GitHub Actions environment variable: name.", trimmedLogs);
        }

        private Dictionary<string, string> _environmentVariables;

        private IEnvironment GetEnvironment(bool isOnGitHubActions = true)
        {
            _environmentVariables = new Dictionary<string, string>();

            var environment = Substitute.For<IEnvironment>();
            if (isOnGitHubActions)
            {
                environment.GetEnvironmentVariable("GITHUB_ACTIONS").Returns("true");
            }

            return environment;
        }
    }
}
