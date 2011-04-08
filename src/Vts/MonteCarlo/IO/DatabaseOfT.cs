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
    public class Database<T>// : IEnumerable<T>
    {
        [IgnoreDataMember]
        private IEnumerable<T> _dataPoints;

        public long NumberOfElements { get; set; }

        public void SetDataPoints(IEnumerable<T> dataPoints)
        {
            _dataPoints = dataPoints;
        }

        public IEnumerable<T> DataPoints
        {
            get { return _dataPoints; }
        }
    }
}
