using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class Int2DCircularSource : IntSourceBase
    {
        /// <summary>
        /// Radius of the circular source 
        /// </summary>
        private double _radius;

        /// <summary>
        /// Center cooordinates (x,z) of the geometry 
        /// </summary>
        private DoubleRange _center;

        /// <summary>
        /// Theta angle range
        /// </summary>
        private DoubleRange _thetaRange;


        public Int2DCircularSource(
            double radius,
            DoubleRange center,
            DoubleRange thetaRange)
        {
            _radius = radius;
            _center = center;
            _thetaRange = thetaRange;
        }

        public Int2DCircularSource()
            : this(
             0.5,
             new DoubleRange(0.5, 0.5),
             new DoubleRange(0, 2 * Math.PI)) { }

        public double Radius { get; set; }
        public DoubleRange Center { get; set; }
        public DoubleRange ThetaRange { get; set; }
    }
}
