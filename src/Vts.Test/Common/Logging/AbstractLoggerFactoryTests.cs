using NSubstitute;
using NUnit.Framework;
using System;
using Vts.Common.Logging;
using Vts.Common.Logging.NLogIntegration;

namespace Vts.Test.Common.Logging
{
    [TestFixture]
    public class AbstractLoggerFactoryTests
    {
        private AbstractLoggerFactory _loggerMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _loggerMock = Substitute.ForPartsOf<AbstractLoggerFactory>();
        }

        [Test]
        public void Test_create_logger_type_only()
        {
            _loggerMock.Create(Arg.Any<string>()).Returns(new NLogLogger());
            Assert.IsInstanceOf<NLogLogger>(_loggerMock.Create(typeof(AbstractLoggerFactoryTests)));
        }

        [Test]
        public void Test_create_logger_type_null_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _loggerMock.Create((Type)null));
        }

        [Test]
        public void Test_create_logger_type_null_level_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _loggerMock.Create((Type)null, LoggerLevel.Info));
        }

        [Test]
        public void Test_create_logger_type_and_level()
        {
            _loggerMock.Create(Arg.Any<string>(), Arg.Any<LoggerLevel>()).Returns(new NLogLogger());
            Assert.IsInstanceOf<NLogLogger>(_loggerMock.Create(typeof(AbstractLoggerFactoryTests), LoggerLevel.Error));
        }
    }
}
