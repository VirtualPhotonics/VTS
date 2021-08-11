using Moq;
using NUnit.Framework;
using System;
using Vts.Common.Logging;
using Vts.Common.Logging.NLogIntegration;

namespace Vts.Test.Common.Logging
{
    [TestFixture]
    public class AbstractLoggerFactoryTests
    {
        [Test]
        public void Test_create_logger_type_only()
        {
            var loggerMock = new Mock<AbstractLoggerFactory>()
            {
                CallBase = true
            };
            loggerMock.Setup(x => x.Create(It.IsAny<String>())).Returns(new NLogLogger());
            Assert.IsInstanceOf<NLogLogger>(loggerMock.Object.Create(typeof(AbstractLoggerFactoryTests)));
        }

        [Test]
        public void Test_create_logger_type_null_throws_argument_null_exception()
        {
            var loggerMock = new Mock<AbstractLoggerFactory>()
            {
                CallBase = true
            };
            Assert.Throws<ArgumentNullException>(() => loggerMock.Object.Create((Type)null));
        }

        [Test]
        public void Test_create_logger_type_null_level_throws_argument_null_exception()
        {
            var loggerMock = new Mock<AbstractLoggerFactory>()
            {
                CallBase = true
            };
            Assert.Throws<ArgumentNullException>(() => loggerMock.Object.Create((Type)null, LoggerLevel.Info));
        }

        [Test]
        public void Test_create_logger_type_and_level()
        {
            var loggerMock = new Mock<AbstractLoggerFactory>()
            {
                CallBase = true
            };
            loggerMock.Setup(x => x.Create(It.IsAny<String>(), It.IsAny<LoggerLevel>())).Returns(new NLogLogger());
            Assert.IsInstanceOf<NLogLogger>(loggerMock.Object.Create(typeof(AbstractLoggerFactoryTests), LoggerLevel.Error));
        }
    }
}
