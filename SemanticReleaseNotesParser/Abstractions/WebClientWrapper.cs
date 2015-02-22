using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SemanticReleaseNotesParser.Abstractions
{
    [ExcludeFromCodeCoverage]
    internal sealed class WebClientWrapper : WebClient, IWebClient
    {
        public WebClientWrapper(string baseAddress)
        {
            this.BaseAddress = baseAddress;
            this.Headers["Accept"] = "application/json";
            this.Headers["Content-type"] = "application/json";
        }
    }
}
