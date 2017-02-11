using SemanticReleaseNotesParser.Abstractions;
using SemanticReleaseNotesParser.BuildServers;
using SemanticReleaseNotesParser.Core;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;

namespace SemanticReleaseNotesParser
{
    internal class Program
    {
        internal static IFileSystem FileSystem { get; set; }

        internal static IEnvironment Environment { get; set; }

        internal static IWebClientFactory WebClientFactory { get; set; }

        internal static void Main(string[] args)
        {
            var exitCode = Run(args);
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.ReadKey();
            }
#endif
            Environment.Exit(exitCode);
        }

        private static int Run(string[] args)
        {
            try
            {
                SetDefaults();

                var output = Console.Out;
                Logger.SetWriter(output);

                Logger.Info("SemanticReleaseNotesParser V{0}", Assembly.GetExecutingAssembly().GetName().Version);

                // Arguments parsing
                var arguments = Arguments.ParseArguments(args);
                if (arguments.Help)
                {
                    arguments.WriteOptionDescriptions(output);
                    return 1;
                }

                // Handle debug
                if (arguments.Debug)
                {
                    Logger.AddCategory("debug");
                }

                if (!FileSystem.File.Exists(arguments.ReleaseNotesPath))
                {
                    Logger.Error("Release notes file '{0}' does not exists", arguments.ReleaseNotesPath);
                    return 1;
                }

                // Settings
                string template = null;
                if (!string.IsNullOrEmpty(arguments.TemplatePath))
                {
                    if (!FileSystem.File.Exists(arguments.TemplatePath))
                    {
                        Logger.Error("Template file '{0}' does not exists", arguments.TemplatePath);
                        return 1;
                    }

                    template = FileSystem.File.ReadAllText(arguments.TemplatePath);
                }

                var settings = new SemanticReleaseNotesConverterSettings
                {
                    OutputFormat = arguments.OutputFormat,
                    LiquidTemplate = template,
                    GroupBy = arguments.GroupBy,
                    PluralizeCategoriesTitle = arguments.PluralizeCategoriesTitle,
                    IncludeStyle = arguments.IncludeStyle != null,
                    CustomStyle = GetCustomStyle(arguments.IncludeStyle)
                };

                // Parsing
                Logger.Debug("Parsing release notes '{0}'", arguments.ReleaseNotesPath);

                var releaseNotes = SemanticReleaseNotesConverter.Parse(FileSystem.File.OpenText(arguments.ReleaseNotesPath), settings);

                // Formatting
                string formattedReleaseNotes = SemanticReleaseNotesConverter.Format(releaseNotes, settings);

                Logger.Debug("Formatted release notes: {0}", formattedReleaseNotes);

                // Select output
                if (arguments.OutputType.HasFlag(OutputType.File))
                {
                    Logger.Debug("File output");
                    FileSystem.File.WriteAllText(arguments.ResultFilePath, formattedReleaseNotes);
                    Logger.Info("File '{0}' generated", arguments.ResultFilePath);
                }

                if (arguments.OutputType.HasFlag(OutputType.Environment))
                {
                    Logger.Debug("Environment output");
                    var buildServer = GetApplicableBuildServer();
                    Logger.Debug("Build server selected: {0}", buildServer.GetType().Name);
                    buildServer.SetEnvironmentVariable("SemanticReleaseNotes", formattedReleaseNotes);
                }

                return 0;
            }
            catch (Exception exception)
            {
                var error = string.Format("An unexpected error occurred:\r\n{0}", exception);
                Logger.Error(error);
                return 1;
            }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private static void SetDefaults()
        {
            if (FileSystem == null)
            {
                FileSystem = new FileSystem();
            }

            if (Environment == null)
            {
                Environment = new EnvironmentWrapper();
            }

            if (WebClientFactory == null)
            {
                WebClientFactory = new WebClientFactory();
            }
        }

        private static IBuildServer GetApplicableBuildServer()
        {
            return new List<IBuildServer>
            {
                new AppVeyor(Environment, WebClientFactory),
                new LocalBuildServer(Environment)
            }.First(bs => bs.CanApplyToCurrentContext());
        }

        private static string GetCustomStyle(string customStyle)
        {
            if (customStyle != null)
            {
                if (customStyle.StartsWith("\""))
                {
                    customStyle = customStyle.Substring(1);
                }

                if (customStyle.EndsWith("\""))
                {
                    customStyle = customStyle.Substring(0, customStyle.Length - 1);
                }
            }

            return customStyle;
        }
    }
}