using SemanticReleaseNotesParser.Abstractions;
using System.Text;

namespace SemanticReleaseNotesParser.BuildServers
{
    internal sealed class AppVeyor : IBuildServer
    {
        private const string SetEnvironmentVariableRequest = "{{ \"name\": \"{0}\", \"value\": \"{1}\" }}";

        private readonly IEnvironment _environment;
        private readonly IWebClientFactory _webClientFactory;
        private readonly string _appVeyorApiUrl;

        public AppVeyor(IEnvironment environment, IWebClientFactory webClientFactory)
        {
            _environment = environment;
            _webClientFactory = webClientFactory;
            _appVeyorApiUrl = _environment.GetEnvironmentVariable("APPVEYOR_API_URL");
        }

        public bool CanApplyToCurrentContext()
        {
            return !string.IsNullOrEmpty(_environment.GetEnvironmentVariable("APPVEYOR"));
        }

        public void SetEnvironmentVariable(string variable, string value)
        {
            using (var webClient = _webClientFactory.Create(_appVeyorApiUrl))
            {
                webClient.UploadData("api/build/variables", "POST", Encoding.UTF8.GetBytes(string.Format(SetEnvironmentVariableRequest, variable, value)));
                Logger.Info("Adding AppVeyor environment variable: {0}.", variable);
            }
        }
    }
}
