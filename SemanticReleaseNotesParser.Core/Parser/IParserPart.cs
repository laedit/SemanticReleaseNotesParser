namespace SemanticReleaseNotesParser.Core.Parser
{
    internal interface IParserPart
    {
        bool Parse(string input, ReleaseNotes releaseNotes, string nextInput);
    }
}