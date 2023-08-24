using NLog;
using NSubstitute;
using NUnit.Framework;
using System;
using Vts.Common.Logging.NLogIntegration;
using Logger = NLog.Logger;

namespace Vts.Test.Common.Logging
{
    [TestFixture]
    public class NLogLoggerTests
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private NLogLogger _nLogLogger;
        private IFormatProvider _formatProviderMock;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _nLogLogger = new NLogLogger(Logger, new NLogFactory());
            _formatProviderMock = Substitute.For<IFormatProvider>();
            _formatProviderMock.GetFormat(Arg.Any<Type>()).Returns((ICustomFormatter)null);
        }

        [Test]
        public void Test_create_new_nlog_logger()
        {
            Assert.IsInstanceOf<NLogLogger>(_nLogLogger);
        }

        [Test]
        public void Test_default_constructor()
        {
            var logger = new NLogLogger();
            Assert.IsInstanceOf<NLogLogger>(logger);
        }

        [Test]
        public void Test_is_info_enabled()
        {
            Assert.AreEqual(Logger.IsInfoEnabled, _nLogLogger.IsInfoEnabled);
        }

        [Test]
        public void Test_is_warn_enabled()
        {
            Assert.AreEqual(Logger.IsWarnEnabled, _nLogLogger.IsWarnEnabled);
        }

        [Test]
        public void Test_is_debug_enabled()
        {
            Assert.AreEqual(Logger.IsDebugEnabled, _nLogLogger.IsDebugEnabled);
        }

        [Test]
        public void Test_is_fatal_enabled()
        {
            Assert.AreEqual(Logger.IsFatalEnabled, _nLogLogger.IsFatalEnabled);
        }

        [Test]
        public void Test_is_error_enabled()
        {
            Assert.AreEqual(Logger.IsErrorEnabled, _nLogLogger.IsErrorEnabled);
        }

        [Test]
        public void Test_to_string()
        {
            Assert.AreEqual(Logger.ToString(), _nLogLogger.ToString());
        }

        [Test]
        public void Test_create_child_logger()
        {
            var childLogger = _nLogLogger.CreateChildLogger("Child");
            Assert.IsInstanceOf<NLogLogger>(childLogger);
            Assert.IsInstanceOf<Logger>(LogManager.GetLogger("NLog.Child"));
        }

        [Test]
        public void Test_debug_logger_calls_do_not_error()
        {
            Assert.DoesNotThrow(() => _nLogLogger.Debug("message"));
            Assert.DoesNotThrow(() => _nLogLogger.Debug("message", new NullReferenceException("Object not set to an instance of an object")));
            Assert.DoesNotThrow(() => _nLogLogger.Debug(() => "message"));
            Assert.DoesNotThrow(() => _nLogLogger.DebugFormat("format"));
            Assert.DoesNotThrow(() => _nLogLogger.DebugFormat(new NullReferenceException("Object not set to an instance of an object"), "format"));
            Assert.DoesNotThrow(() => _nLogLogger.DebugFormat(_formatProviderMock, "format"));
            Assert.DoesNotThrow(() => _nLogLogger.DebugFormat(new NullReferenceException("Object not set to an instance of an object"), _formatProviderMock, "format"));
        }

        [Test]
        public void Test_info_logger_calls_do_not_error()
        {
            Assert.DoesNotThrow(() => _nLogLogger.Info("message"));
            Assert.DoesNotThrow(() => _nLogLogger.Info("message", new NullReferenceException("Object not set to an instance of an object")));
            Assert.DoesNotThrow(() => _nLogLogger.Info(() => "message"));
            Assert.DoesNotThrow(() => _nLogLogger.InfoFormat("format"));
            Assert.DoesNotThrow(() => _nLogLogger.InfoFormat(new NullReferenceException("Object not set to an instance of an object"), "format"));
            Assert.DoesNotThrow(() => _nLogLogger.InfoFormat(_formatProviderMock, "format"));
            Assert.DoesNotThrow(() => _nLogLogger.InfoFormat(new NullReferenceException("Object not set to an instance of an object"), _formatProviderMock, "format"));
        }

        [Test]
        public void Test_warn_logger_calls_do_not_error()
        {
            Assert.DoesNotThrow(() => _nLogLogger.Warn("message"));
            Assert.DoesNotThrow(() => _nLogLogger.Warn("message", new NullReferenceException("Object not set to an instance of an object")));
            Assert.DoesNotThrow(() => _nLogLogger.Warn(() => "message"));
            Assert.DoesNotThrow(() => _nLogLogger.WarnFormat("format"));
            Assert.DoesNotThrow(() => _nLogLogger.WarnFormat(new NullReferenceException("Object not set to an instance of an object"), "format"));
            Assert.DoesNotThrow(() => _nLogLogger.WarnFormat(_formatProviderMock, "format"));
            Assert.DoesNotThrow(() => _nLogLogger.WarnFormat(new NullReferenceException("Object not set to an instance of an object"), _formatProviderMock, "format"));
        }

        [Test]
        public void Test_error_logger_calls_do_not_error()
        {
            Assert.DoesNotThrow(() => _nLogLogger.Error("message"));
            Assert.DoesNotThrow(() => _nLogLogger.Error("message", new NullReferenceException("Object not set to an instance of an object")));
            Assert.DoesNotThrow(() => _nLogLogger.Error(() => "message"));
            Assert.DoesNotThrow(() => _nLogLogger.ErrorFormat("format"));
            Assert.DoesNotThrow(() => _nLogLogger.ErrorFormat(new NullReferenceException("Object not set to an instance of an object"), "format"));
            Assert.DoesNotThrow(() => _nLogLogger.ErrorFormat(_formatProviderMock, "format"));
            Assert.DoesNotThrow(() => _nLogLogger.ErrorFormat(new NullReferenceException("Object not set to an instance of an object"), _formatProviderMock, "format"));
        }

        [Test]
        public void Test_fatal_logger_calls_do_not_error()
        {
            Assert.DoesNotThrow(() => _nLogLogger.Fatal("message"));
            Assert.DoesNotThrow(() => _nLogLogger.Fatal("message", new NullReferenceException("Object not set to an instance of an object")));
            Assert.DoesNotThrow(() => _nLogLogger.Fatal(() => "message"));
            Assert.DoesNotThrow(() => _nLogLogger.FatalFormat("format"));
            Assert.DoesNotThrow(() => _nLogLogger.FatalFormat(new NullReferenceException("Object not set to an instance of an object"), "format"));
            Assert.DoesNotThrow(() => _nLogLogger.FatalFormat(_formatProviderMock, "format"));
            Assert.DoesNotThrow(() => _nLogLogger.FatalFormat(new NullReferenceException("Object not set to an instance of an object"), _formatProviderMock, "format"));
        }
    }
}
