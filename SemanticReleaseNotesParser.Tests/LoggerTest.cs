using System;
using System.IO;
using System.Text;
using Xunit;

namespace SemanticReleaseNotesParser.Tests
{
    public class LoggerTest : IDisposable
    {
        private StringBuilder _logs;
        
        public LoggerTest()
        {
            Logger.Reset();
        }

        [Fact]
        public void Info_TextWriter_Null_ThrowException()
        {
            // act
            var exception = Assert.Throws<Exception>(() => Logger.Info("test info"));

            // assert
            Assert.Equal("The writer must be set with the 'SetWriter' method first.", exception.Message);
        }

        [Fact]
        public void Info()
        {
            // arrange
            Logger.SetWriter(GetLogOutput());

            // act
            Logger.Info("test '{0}'", "info");

            // assert
            Assert.Equal("test 'info'", _logs.ToString().Trim());
        }

        [Fact]
        public void Error()
        {
            // arrange
            Logger.SetWriter(GetLogOutput());

            // act
            Logger.Error("test '{0}'", "error");

            // assert
            Assert.Equal("test 'error'", _logs.ToString().Trim());
        }

        [Fact]
        public void Debug_WithoutAddingProperCategory_ShouldNotLog()
        {
            // arrange
            Logger.SetWriter(GetLogOutput());

            // act
            Logger.Debug("test '{0}'", "debug");

            // assert
            Assert.Equal(string.Empty, _logs.ToString());
        }

        [Fact]
        public void Debug()
        {
            // arrange
            Logger.SetWriter(GetLogOutput());
            Logger.AddCategory("debug");

            // act
            Logger.Debug("test '{0}'", "debug");

            // assert
            Assert.Equal("test 'debug'", _logs.ToString().Trim());
        }

        private TextWriter GetLogOutput()
        {
            _logs = new StringBuilder();
            return new StringWriter(_logs);
        }

        public void Dispose()
        {
            Logger.Reset();
        }
    }
}
