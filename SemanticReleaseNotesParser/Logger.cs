using System;
using System.Collections.Generic;
using System.IO;

namespace SemanticReleaseNotesParser
{
    internal static class Logger
    {
        private static readonly List<string> DefaultCategories = new List<string> { "info", "error" };
        private static List<string> _categories = new List<string>(DefaultCategories);
        private static TextWriter _writer;

        public static void SetWriter(TextWriter textWriter)
        {
            _writer = textWriter;
        }

        public static void AddCategory(string category)
        {
            _categories.Add(category);
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
            if (_writer == null)
            {
                throw new Exception("The writer must be set with the 'SetWriter' method first.");
            }

            if (_categories.Contains(category))
            {
                _writer.WriteLine(message, args);
            }
        }

        internal static void Reset()
        {
            _writer = null;
            _categories = new List<string>(DefaultCategories);
        }
    }
}
