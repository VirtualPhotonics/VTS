﻿// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
            if (Environment.UserInteractive) // if current process in running *with* a user interface
            {
                LogManager.Configuration = GetDefaultLoggingConfiguration();
            }
            else
            {
                LogManager.Configuration = GetDefaultDesktopLoggingConfiguration();
            }
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
                if (Environment.UserInteractive) // if current process in running *with* a user interface
                {
                    LogManager.Configuration = GetDefaultLoggingConfiguration();
                }
                else
                {
                    LogManager.Configuration = GetDefaultDesktopLoggingConfiguration();
                }
            }
        }

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

        /// <summary>
        ///   Creates a logger with specified <paramref name = "name" />.
        /// </summary>
        /// <param name = "name">logger string name</param>
        /// <returns>The ILogger that was created</returns>
        public override ILogger Create(String name)
        {
            var nLogLogger = LogManager.GetLogger(name);
            return new NLogLogger(nLogLogger, this);
        }

        /// <summary>
        ///   Not implemented, NLog logger levels cannot be set at runtime.
        /// </summary>
        /// <param name = "name">logger sting name</param>
        /// <param name = "level">The logger level</param>
        /// <returns>The ILogger that was created</returns>
        /// <exception cref = "NotImplementedException" />
        public override ILogger Create(String name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}