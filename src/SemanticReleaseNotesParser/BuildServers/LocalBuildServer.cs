using SemanticReleaseNotesParser.Abstractions;
using SemanticReleaseNotesParser.Logging;

namespace SemanticReleaseNotesParser.BuildServers
{
    internal sealed class LocalBuildServer : IBuildServer
    {
        private readonly IEnvironment _environment;

        public LocalBuildServer(IEnvironment environment)
        {
            _environment = environment;
        }

        public bool CanApplyToCurrentContext()
        {
            return true;
        }

        public void SetEnvironmentVariable(string variable, string value)
        {
            _environment.SetEnvironmentVariable(variable, value);
            Logger.Info("Adding local environment variable: {0}.", variable);
        }
    }
}
