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
            Logger.Debug("AppVeyor API Url: {0}", _appVeyorApiUrl);
            var request = string.Format(SetEnvironmentVariableRequest, variable, EscapeStringValue(value));
            Logger.Debug("Request body: {0}", request);

            using (var webClient = _webClientFactory.Create(_appVeyorApiUrl))
            {
                webClient.UploadData("api/build/variables", "POST", Encoding.UTF8.GetBytes(request));
                Logger.Info("Adding AppVeyor environment variable: {0}.", variable);
            }
        }

        private static string EscapeStringValue(string value)
        {
            const char BACK_SLASH = '\\';
            const char SLASH = '/';
            const char DBL_QUOTE = '"';

            var output = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                switch (c)
                {
                    case SLASH:
                        output.AppendFormat("{0}{1}", BACK_SLASH, SLASH);
                        break;

                    case BACK_SLASH:
                        output.AppendFormat("{0}{0}", BACK_SLASH);
                        break;

                    case DBL_QUOTE:
                        output.AppendFormat("{0}{1}", BACK_SLASH, DBL_QUOTE);
                        break;

                    default:
                        output.Append(c);
                        break;
                }
            }

            return output.ToString();
        }
    }
}
