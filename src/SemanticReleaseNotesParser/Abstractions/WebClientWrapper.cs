using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SemanticReleaseNotesParser.Abstractions
{
    [ExcludeFromCodeCoverage]
    internal sealed class WebClientWrapper : WebClient, IWebClient
    {
        public WebClientWrapper(string baseAddress)
        {
            BaseAddress = baseAddress;
            Headers["Accept"] = "application/json";
            Headers["Content-type"] = "application/json";
        }
    }
}
