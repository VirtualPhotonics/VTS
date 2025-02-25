using System;
using System.IO;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Vts.Common.Logging;
using Vts.Common.Logging.NLogIntegration;

namespace Vts.Test.Common.Logging
{
    [TestFixture]
    public class NLogFactoryTests
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private NLogFactory _loggerFactory;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _loggerFactory = new NLogFactory();
            var logger = LoggerFactoryLocator.GetNLogFactory("NLogUnitTest").Create(typeof(NLogFactoryTests));
            logger.Info("test message");
            _loggerFactory.Create(Logger.Name);
        }

        [Test]
        public void Test_create_nlog_factory_and_logger()
        {
            var nLogLogger = new NLogLogger(Logger, _loggerFactory);
            
            
        }

        [Test]
        public void Test_create_logger_with_level_throws_exception()
        {
            Assert.Throws<NotSupportedException>(() => _loggerFactory.Create(Logger.Name, LoggerLevel.Info));
        }

        [Test]
        public void Test_logger_factory_with_config()
        {
            var loggerFactoryWithConfig = new NLogFactory("NLogUnitTest.config");
            
        }

        [Test]
        public void Test_logger_factory_with_rooted_config()
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NLogUnitTest.config");
            var loggerFactoryWithConfig = new NLogFactory(fileName);
            
        }
    }
}
