// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Vts.Common.Logging.NLogIntegration
{
    /// <summary>
    ///   Implementation of <see cref = "ILoggerFactory" /> for NLog.
    /// </summary>
    public class NLogFactory : AbstractLoggerFactory
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "NLogFactory" /> class.
        /// </summary>
        public NLogFactory()
        {
#if SILVERLIGHT
            LogManager.Configuration = GetDefaultLoggingConfiguration();
#else
            LogManager.Configuration = GetDefaultDesktopLoggingConfiguration();
#endif
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NLogFactory" /> class.
        /// </summary>
        /// <param name = "configFile">The config file.</param>
        public NLogFactory(string configFile)
        {
            if( File.Exists(configFile))
            {
                var file = GetConfigFile(configFile);
                LogManager.Configuration = new XmlLoggingConfiguration(file.FullName);
            }
            else
            {
#if SILVERLIGHT
                LogManager.Configuration = GetDefaultLoggingConfiguration();
#else
                LogManager.Configuration = GetDefaultDesktopLoggingConfiguration();
#endif
            }
        }

        ///// <summary>
        ///// Destructor - added to try and fix an issue when using the logger in mono
        ///// </summary>
        //~ NLogFactory()
        //{
        //    LogManager.Configuration = null;
        //}

        private static LoggingConfiguration GetDefaultLoggingConfiguration()
        {
            var config = new LoggingConfiguration();

            var observableTarget = new ObservableTarget();

            config.AddTarget("observable", observableTarget);

            observableTarget.Layout = "${date:format=HH\\:mm\\:ss} | ${message}";

            LoggingRule rule1 = new LoggingRule("*", LogLevel.Info, observableTarget);

            config.LoggingRules.Add(rule1);

            return config;
        }
#if !SILVERLIGHT
        private static LoggingConfiguration GetDefaultDesktopLoggingConfiguration()
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget();

            config.AddTarget("console", consoleTarget);

            consoleTarget.Layout = "${date:format=HH\\:mm\\:ss} | ${message}";

            LoggingRule rule1 = new LoggingRule("*", LogLevel.Info, consoleTarget);

            config.LoggingRules.Add(rule1);

            return config;
        }
#endif

        /// <summary>
        ///   Creates a logger with specified <paramref name = "name" />.
        /// </summary>
        /// <param name = "name">The name.</param>
        /// <returns></returns>
        public override ILogger Create(String name)
        {
            //if (File.Exists(name))
            //{
                var nLogLogger = LogManager.GetLogger(name);
                return new NLogLogger(nLogLogger, this);
            //}
            //else
            //{
            //    var nullLogger = LogManager.CreateNullLogger();
            //    return new NLogLogger(nullLogger, this);
            //}
        }

        /// <summary>
        ///   Not implemented, NLog logger levels cannot be set at runtime.
        /// </summary>
        /// <param name = "name">The name.</param>
        /// <param name = "level">The level.</param>
        /// <returns></returns>
        /// <exception cref = "NotImplementedException" />
        public override ILogger Create(String name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}