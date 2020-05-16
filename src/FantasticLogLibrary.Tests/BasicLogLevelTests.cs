using Moq;
using NUnit.Framework;

namespace FantasticLogLibrary.Tests
{
    [TestFixture]
    public class BasicLogLevelTests
    {
        private Logger _sut;
        private ILogDestination _logDestination;
        private Mock<ILogDestination> _mock;

        [SetUp]
        public void SetUp()
        {
            _mock = new Mock<ILogDestination>();
            _logDestination = _mock.Object;
        }

        private void CreateLogger(LogLevel level)
        {
            _sut = new Logger(level, _logDestination);
        }

        [Test]
        public void Verify_debug_log_not_logged_if_level_is_info()
        {
            CreateLogger(LogLevel.Info);
            _sut.Log(LogLevel.Debug, "This should not be logged");
            _mock.Verify(x => x.AddLog(It.IsAny<LogMessage>()), Times.Never);
        }

        [Test]
        public void Verify_info_log_is_logged_if_level_is_info()
        {
            CreateLogger(LogLevel.Info);
            _sut.Log(LogLevel.Info, "This should be logged");
            _mock.Verify(x => x.AddLog(It.IsAny<LogMessage>()), Times.Once);
        }

        [Test]
        public void Verify_warn_log_is_logged_if_level_is_info()
        {
            CreateLogger(LogLevel.Info);
            _sut.Log(LogLevel.Warn, "This should be logged");
            _mock.Verify(x => x.AddLog(It.IsAny<LogMessage>()), Times.Once);
        }

        [Test]
        public void Verify_log_formatting()
        {
            CreateLogger(LogLevel.Info);
            _sut.Log(LogLevel.Info, "This have a parameter {0}", "value");
            _mock.Verify(x => x.AddLog(It.Is<LogMessage>(m => m.RenderedMessage == "This have a parameter value")), Times.Once);
        }
    }
}
