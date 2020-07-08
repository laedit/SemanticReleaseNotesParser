using SemanticReleaseNotesParser.Abstractions;
using SemanticReleaseNotesParser.Logging;

namespace SemanticReleaseNotesParser.BuildServers
{
    internal sealed class GitHubActions : IBuildServer
    {
        private readonly IEnvironment _environment;

        public GitHubActions(IEnvironment environment)
        {
            _environment = environment;
        }

        public bool CanApplyToCurrentContext()
        {
            return _environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
        }

        public void SetEnvironmentVariable(string variable, string value)
        {
            Logger.Info($"::set-env name={variable}::{value}");
            Logger.Info("Adding GitHub Actions environment variable: {0}.", variable);
        }
    }
}
