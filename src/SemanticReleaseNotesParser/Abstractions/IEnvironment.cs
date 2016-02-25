namespace SemanticReleaseNotesParser.Abstractions
{
    internal interface IEnvironment
    {
        string GetEnvironmentVariable(string variable);

        void SetEnvironmentVariable(string variable, string value);

        void Exit(int exitCode);
    }
}
