using SemanticReleaseNotesParser.Logging;
using System.Text;
using Xunit;

namespace SemanticReleaseNotesParser.Tests
{
    public class LoggerTest
    {
        [Fact]
        public void DefaultLogger_NoExceptionWhenLogActionNotSet()
        {
            // act
            Logger.Info("test info");
        }

        [Fact]
        public void Info()
        {
            // arrange
            var logs = new StringBuilder();
            Logger.SetLogAction((message, logLevel) => logs.AppendLine(message));

            // act
            Logger.Info("test '{0}'", "info");

            // assert
            Assert.Equal("test 'info'", logs.ToString().Trim());
        }

        [Fact]
        public void Error()
        {
            // arrange
            var logs = new StringBuilder();
            Logger.SetLogAction((message, logLevel) => logs.AppendLine(message));

            // act
            Logger.Error("test '{0}'", "error");

            // assert
            Assert.Equal("test 'error'", logs.ToString().Trim());
        }

        [Fact]
        public void Debug_WithoutSettingProperMinimalLevel_ShouldNotLog()
        {
            // arrange
            var logs = new StringBuilder();
            Logger.SetLogAction((message, logLevel) => logs.AppendLine(message));
            Logger.SetMinimalLevel(LogLevel.Info);

            // act
            Logger.Debug("test '{0}'", "debug");

            // assert
            Assert.Equal(string.Empty, logs.ToString());
        }

        [Fact]
        public void Debug()
        {
            // arrange
            var logs = new StringBuilder();
            Logger.SetLogAction((message, logLevel) => logs.AppendLine(message));
            Logger.SetMinimalLevel(LogLevel.Debug);

            // act
            Logger.Debug("test '{0}'", "debug");

            // assert
            Assert.Equal("test 'debug'", logs.ToString().Trim());
        }
    }
}
