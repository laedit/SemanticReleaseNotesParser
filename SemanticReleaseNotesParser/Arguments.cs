using NDesk.Options;
using SemanticReleaseNotesParser.Core;

namespace SemanticReleaseNotesParser
{
    internal class Arguments : OptionSet
    {
        private const string DefaultReleaseNotesPath = "ReleaseNotes.md";

        public string ReleaseNotesPath { get; private set; }

        public string ResultFilePath { get; private set; }

        public OutputType OutputType { get; private set; }

        public string TemplatePath { get; private set; }

        public bool Debug { get; private set; }

        public bool Help { get; private set; }

        public OutputFormat OutputFormat { get; private set; }

        public Arguments()
        {
            Add("r|releasenotes=", "Release notes file path to parse (default: ReleaseNotes.md)", r => ReleaseNotesPath = r);
            Add("o|outputfile=", "Path of the resulting file (default: ReleaseNotes.html", o => ResultFilePath = o);
            Add<OutputType>("t|outputtype=", "Type of output [file|environment] (default: file)", t => OutputType = t);
            Add<OutputFormat>("f|outputformat=", "Format of the resulting file [Html|Markdown] (default: Html)", f => OutputFormat = f);
            Add("template=", "Path of the liquid template file to format the result", t => TemplatePath = t);
            Add("debug", "Debug mode, more messages are logged", d => Debug = true);
            Add("h|?|help", "Help", h => Help = true);

            ReleaseNotesPath = DefaultReleaseNotesPath;
            ResultFilePath = "ReleaseNotes.html";
        }

        public static Arguments ParseArguments(string[] args)
        {
            var arguments = new Arguments();
            var additionalArguments = arguments.Parse(args);

            if (arguments.ReleaseNotesPath == DefaultReleaseNotesPath && additionalArguments.Count > 0 && !string.IsNullOrEmpty(additionalArguments[0]))
            {
                arguments.ReleaseNotesPath = additionalArguments[0];
            }

            return arguments;
        }
    }
}
