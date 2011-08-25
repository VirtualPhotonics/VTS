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

        public long NumberOfElements { get; set; }

        public void SetDataPoints(IEnumerable<T> dataPoints)
        {
            _dataPoints = dataPoints;
        }

        [IgnoreDataMember]
        public IEnumerable<T> DataPoints
        {
            get { return _dataPoints; }
        }
    }
}
