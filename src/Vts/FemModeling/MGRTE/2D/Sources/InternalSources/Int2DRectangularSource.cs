using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class Int2DRectangularSource : IntSourceBase
    {
        /// <summary>
        /// Length of the Rectangular source 
        /// </summary>
        private double _xLength;

        /// <summary>
        /// Height of the Rectangular source 
        /// </summary>
        private double _zHeight;

        /// <summary>
        /// Center cooordinates (x,z) of the geometry 
        /// </summary>
        private DoubleRange _center;

        /// <summary>
        /// Theta angle range
        /// </summary>
        private DoubleRange _thetaRange;


        public Int2DRectangularSource(
            double xLength,
            double zHeight,
            DoubleRange center,
            DoubleRange thetaRange)
        {
            _xLength = xLength;
            _zHeight = zHeight;
            _center = center;
            _thetaRange = thetaRange;
        }

        public Int2DRectangularSource()
            : this(
                0.5,
                0.5,
                new DoubleRange(0.5, 0.5),
                new DoubleRange(0, 2 * Math.PI)) { }

        public double XLength{ get; set; }
        public double ZHeight { get; set; }
        public DoubleRange Center { get; set; }
        public DoubleRange ThetaRange { get; set; }
    }
}
