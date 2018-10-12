using System.Collections.Generic;
using Vts.Common.Logging.NLogIntegration;

namespace Vts.Common.Logging
{
    /// <summary>
    /// Represents a global service locator for singleton instances of ILoggerFactory
    /// </summary>
    public class LoggerFactoryLocator
    {
        /// <summary>
        /// Internal dictionary to store singleton instances of ILoggerFactory
        /// </summary>
        private static IDictionary<string, ILoggerFactory> _loggerFactories = new Dictionary<string, ILoggerFactory>(1);

        /// <summary>
        /// Returns a singleton instance of an NLogFactcory
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        public static ILoggerFactory GetNLogFactory(string loggerName)
        {
            ILoggerFactory factory;
            if (!_loggerFactories.TryGetValue(loggerName, out factory) || factory == null)
            {
                factory = new NLogFactory(loggerName + ".config");
                _loggerFactories.Add(loggerName,factory);
            }
            return factory;
        }


        /// <summary>
        /// Returns a default singleton instance of an NLogFactcory
        /// </summary>
        /// <returns></returns>
        public static ILoggerFactory GetDefaultNLogFactory()
        {
            return GetNLogFactory("NLog");
        }
    }
}
