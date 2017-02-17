using System;
using System.Collections.Generic;

namespace SemanticReleaseNotesParser.Logging
{
    internal static class ConsoleLogAction
    {
        private static readonly Dictionary<LogLevel, ConsoleColor> Colors = new Dictionary<LogLevel, ConsoleColor>
             {
                 { LogLevel.Error, ConsoleColor.DarkRed },
                 { LogLevel.Info, ConsoleColor.White },
                 { LogLevel.Debug, ConsoleColor.Gray }
             };

        internal static void Write(string message, LogLevel traceLevel)
        {
            ConsoleColor consoleColor;

            if (Colors.TryGetValue(traceLevel, out consoleColor))
            {
                var originalForground = Console.ForegroundColor;
                try
                {
                    Console.ForegroundColor = consoleColor;
                    Console.WriteLine(message);
                }
                finally
                {
                    Console.ForegroundColor = originalForground;
                }
            }
            else
            {
                Console.WriteLine(message);
            }

        }
    }
}
