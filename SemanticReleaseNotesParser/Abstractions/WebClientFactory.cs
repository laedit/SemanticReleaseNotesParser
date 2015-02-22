using System.Diagnostics.CodeAnalysis;

namespace SemanticReleaseNotesParser.Abstractions
{
    [ExcludeFromCodeCoverage]
    internal class WebClientFactory : IWebClientFactory
    {
        public IWebClient Create(string baseAddress)
        {
            return new WebClientWrapper(baseAddress);
        }
    }
}
