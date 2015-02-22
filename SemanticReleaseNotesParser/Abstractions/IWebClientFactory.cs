namespace SemanticReleaseNotesParser.Abstractions
{
    internal interface IWebClientFactory
    {
        IWebClient Create(string baseAddress);
    }
}
