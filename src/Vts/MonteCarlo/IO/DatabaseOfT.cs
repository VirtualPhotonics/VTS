using System;
using Vts.IO;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Vts.MonteCarlo.IO
{
#if !SILVERLIGHT
    [Serializable]
#endif
    /// <summary>
    /// Describes database of generic type allowing for various classes to be written to database.
    /// </summary>
    /// <typeparam name="T">type of database</typeparam>
    public class Database<T>// : IEnumerable<T>
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
        /// <param name="dataPoints">IEnumerable of generic type T"</param>
        public void SetDataPoints(IEnumerable<T> dataPoints)
        {
            _dataPoints = dataPoints;
        }
        /// <summary>
        /// DataPoints is an IEnumerable of T
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<T> DataPoints
        {
            get { return _dataPoints; }
        }
    }
}
