using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(x,y)
    /// </summary>
    public class ROfXAndYDetectorInput : IDetectorInput
    {
        public ROfXAndYDetectorInput(DoubleRange x, DoubleRange y)
        {
            TallyType = TallyType.ROfXAndY;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Default constructor uses default x and y bins
        /// </summary>
        public ROfXAndYDetectorInput()
            : this(new DoubleRange(-200.0, 200.0, 401), new DoubleRange(-200.0, 200.0, 401)) 
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange X { get; set; }
        public DoubleRange Y { get; set; }
    }
}
