using NDesk.Options;
using SemanticReleaseNotesParser.Core;
using System.Collections.Generic;

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

        public GroupBy GroupBy { get; set; }

        private Arguments()
        {
            Add("r|releasenotes=", "Release notes file path to parse (default: ReleaseNotes.md)", r => ReleaseNotesPath = r);
            Add("o|outputfile=", "Path of the resulting file (default: ReleaseNotes.html", o => ResultFilePath = o);
            Add<OutputType>("t|outputtype=", "Type of output [file|environment|fileandenvironment] (default: file)", t => OutputType = t);
            Add<OutputFormat>("f|outputformat=", "Format of the resulting file [html|markdown] (default: Html)", f => OutputFormat = f);
            Add<GroupBy>("g|groupby=", "Defines the grouping of items [sections|categories] (default: Sections)", g => GroupBy = g);
            Add("template=", "Path of the liquid template file to format the result", t => TemplatePath = t);
            Add("debug", "Debug mode, more messages are logged", d => Debug = true);
            Add("h|?|help", "Help", h => Help = true);

            ReleaseNotesPath = DefaultReleaseNotesPath;
            ResultFilePath = "ReleaseNotes.html";
            OutputType = OutputType.File;
        }

        public static Arguments ParseArguments(IEnumerable<string> args)
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
