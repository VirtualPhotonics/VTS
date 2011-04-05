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
        public ROfXAndYDetectorInput(DoubleRange x, DoubleRange y, String name)
        {
            TallyType = TallyType.ROfXAndY;
            Name = name;
            X = x;
            Y = y;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="name"></param>
        public ROfXAndYDetectorInput(DoubleRange x, DoubleRange y) 
            : this (x, y, TallyType.ROfXAndY.ToString()) {}

        /// <summary>
        /// Default constructor uses default x and y bins
        /// </summary>
        public ROfXAndYDetectorInput() 
            : this (new DoubleRange(-200.0, 200.0, 401), 
                    new DoubleRange(-200.0, 200.0, 401), 
                    TallyType.ROfXAndY.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange X { get; set; }
        public DoubleRange Y { get; set; }
    }
}
