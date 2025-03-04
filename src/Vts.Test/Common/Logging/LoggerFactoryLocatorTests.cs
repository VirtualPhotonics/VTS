using NUnit.Framework;
using Vts.Common.Logging;

namespace Vts.Test.Common.Logging
{
    [TestFixture]
    public class LoggerFactoryLocatorTests
    {
        [Test]
        public void Test_get_default_nlog_factory()
        {
            var loggerFactory = LoggerFactoryLocator.GetDefaultNLogFactory();
            Assert.That(loggerFactory, Is.InstanceOf<ILoggerFactory>());
        }

        [Test]
        public void Test_create_nlog_logger()
        {
            var logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(LoggerFactoryLocatorTests));
            Assert.That(logger, Is.InstanceOf<ILogger>());
        }
    }
}
