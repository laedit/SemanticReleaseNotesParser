﻿using System;

namespace SemanticReleaseNotesParser.Logging
{
    internal static class Logger
    {
        private static Action<string, LogLevel> _trace;
        private static LogLevel _minLevel;

        static Logger()
        {
            // Do nothing by default
            _trace = (message, TraceLevel) => { };
            _minLevel = LogLevel.Info;
        }

        internal static void SetLogAction(Action<string, LogLevel> trace)
        {
            _trace = trace;
        }

        internal static void SetMinimalLevel(LogLevel level)
        {
            _minLevel = level;
        }

        /// <summary>
        /// Trace a debug message
        /// </summary>
        /// <param name="message">Message to trace</param>
        /// <param name="messageParameters">Format parameters for the message</param>
        public static void Debug(string message, params object[] messageParameters)
        {
            TraceMessage(message, messageParameters, LogLevel.Debug);
        }

        /// <summary>
        /// Trace an info message
        /// </summary>
        /// <param name="message">Message to trace</param>
        /// <param name="messageParameters">Format parameters for the message</param>
        public static void Info(string message, params object[] messageParameters)
        {
            TraceMessage(message, messageParameters, LogLevel.Info);
        }
        
        /// <summary>
        /// Trace an error message
        /// </summary>
        /// <param name="message">Message to trace</param>
        /// <param name="messageParameters">Format parameters for the message</param>
        public static void Error(string message, params object[] messageParameters)
        {
            TraceMessage(message, messageParameters, LogLevel.Error);
        }

        private static void TraceMessage(string message, object[] messageParameters, LogLevel messageLevel)
        {
            if (messageLevel >= _minLevel)
            {
                _trace(string.Format(message, messageParameters), messageLevel);
            }
        }
    }

    internal enum LogLevel
    {
        Debug,
        Info,
        Error
    }
}
