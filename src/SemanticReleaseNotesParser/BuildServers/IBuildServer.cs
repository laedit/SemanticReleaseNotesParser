namespace SemanticReleaseNotesParser.BuildServers
{
    internal interface IBuildServer
    {
        bool CanApplyToCurrentContext();

        void SetEnvironmentVariable(string variable, string value);
    }
}
