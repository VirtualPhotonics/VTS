﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Vts.MonteCarlo.IO
{
    /// <summary>
    /// The <see cref="IO"/> namespace contains the Monte Carlo input/output classes for databases
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Describes database of generic type allowing for various classes to be written to database.
    /// </summary>
    /// <typeparam name="T">type of database</typeparam>
    [Serializable]
    public class Database<T>// : IEnumerable<Time>
    {
        [IgnoreDataMember]
        private IEnumerable<T> _dataPoints;
        /// <summary>
        /// number of elements in database
        /// </summary>
        public long NumberOfElements { get; set; }
        /// <summary>
        /// method to set private _dataPoints to passed in dataPoints
        /// </summary>
        /// <param name="dataPoints">IEnumerable of generic type Time"</param>
        public void SetDataPoints(IEnumerable<T> dataPoints)
        {
            _dataPoints = dataPoints;
        }
        /// <summary>
        /// DataPoints is an IEnumerable of Time
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<T> DataPoints => _dataPoints;
    }
}
