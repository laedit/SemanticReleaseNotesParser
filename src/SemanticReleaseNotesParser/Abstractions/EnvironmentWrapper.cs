using System;
using System.Diagnostics.CodeAnalysis;

namespace SemanticReleaseNotesParser.Abstractions
{
    [ExcludeFromCodeCoverage]
    internal class EnvironmentWrapper : IEnvironment
    {
        public string GetEnvironmentVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }

        public void SetEnvironmentVariable(string variable, string value)
        {
            Environment.SetEnvironmentVariable(variable, value);
        }

        public void Exit(int exitCode)
        {
            Environment.Exit(exitCode);
        }
    }
}
