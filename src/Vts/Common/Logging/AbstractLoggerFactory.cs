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

namespace Vts.Common.Logging
{
    /// <summary>
    /// factory methods for logger
    /// </summary>
    [Serializable]
    public abstract class AbstractLoggerFactory : MarshalByRefObject, ILoggerFactory
    {
        /// <summary>
        /// method to create logger given type
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>The ILogger that was created</returns>
        public virtual ILogger Create(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return Create(type.FullName);
        }
        /// <summary>
        /// method to create logger given type and logger level
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="level">logger level</param>
        /// <returns>The ILogger that was created</returns>
        public virtual ILogger Create(Type type, LoggerLevel level)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return Create(type.FullName, level);
        }
        /// <summary>
        /// method to create logger given name
        /// </summary>
        /// <param name="name">name string</param>
        /// <returns>The ILogger that was created</returns>
        public abstract ILogger Create(String name);
        /// <summary>
        /// method to create logger given name and logger level
        /// </summary>
        /// <param name="name">name string</param>
        /// <param name="level">LoggerLevel</param>
        /// <returns>The ILogger that was created</returns>

        public abstract ILogger Create(String name, LoggerLevel level);

        /// <summary>
        ///   Gets the configuration file.
        /// </summary>
        /// <param name="fileName">The filename i.e. log.config</param>
        /// <returns>The configuration file</returns>
        protected static FileInfo GetConfigFile(string fileName)
        {
            FileInfo result;

            if (Path.IsPathRooted(fileName))
            {
                result = new FileInfo(fileName);
            }
            else
            {
                result = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
            }

            return result;
        }
    }
}