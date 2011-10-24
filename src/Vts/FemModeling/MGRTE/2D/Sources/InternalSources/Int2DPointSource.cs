using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class Int2DPointSource : IntSourceBase
    {
        /// <summary>
        /// Center cooordinates (x,z) of the geometry 
        /// </summary>
        private DoubleRange _center;

        /// <summary>
        /// Theta angle range
        /// </summary>
        private DoubleRange _thetaRange;


        public Int2DPointSource(
            DoubleRange center,
            DoubleRange thetaRange)
        {
            _center = center;
            _thetaRange = thetaRange;
        }

        public Int2DPointSource()
            : this(
             new DoubleRange(0.5, 0.5),
             new DoubleRange(0, 2 * Math.PI)) { }
        
        public DoubleRange Center { get; set; }
        public DoubleRange ThetaRange { get; set; }
    }
}
