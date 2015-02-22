using System;
using System.Collections.Generic;
using System.IO;

namespace SemanticReleaseNotesParser
{
    internal static class Logger
    {
        private static readonly List<string> DefaultCategories = new List<string> { "info", "error" };
        private static List<string> categories = new List<string>(DefaultCategories);
        private static TextWriter writer;

        public static void SetWriter(TextWriter textWriter)
        {
            writer = textWriter;
        }

        public static void AddCategory(string category)
        {
            categories.Add(category);
        }

        public static void Debug(string message, params object[] args)
        {
            Write(message, "debug", args);
        }

        public static void Info(string message, params object[] args)
        {
            Write(message, "info", args);
        }

        public static void Error(string message, params object[] args)
        {
            Write(message, "error", args);
        }

        private static void Write(string message, string category, params object[] args)
        {
            if (writer == null)
            {
                throw new Exception("The writer must be set with the 'SetWriter' method first.");
            }

            if (categories.Contains(category))
            {
                writer.WriteLine(message, args);
            }
        }

        internal static void Reset()
        {
            writer = null;
            categories = new List<string>(DefaultCategories);
        }
    }
}
