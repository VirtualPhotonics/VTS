using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class Int2DEllipticalSource : IntSourceBase
    {
        /// <summary>
        /// a Parameter of Elliptical source 
        /// </summary>
        private double _aParameter;

        /// <summary>
        /// b Parameter of Elliptical source 
        /// </summary>
        private double _bParameter;


        /// <summary>
        /// Center cooordinates (x,z) of the geometry 
        /// </summary>
        private DoubleRange _center;

        /// <summary>
        /// Theta angle range
        /// </summary>
        private DoubleRange _thetaRange;


        public Int2DEllipticalSource(
            double aParameter,
            double bParameter,
            DoubleRange center,
            DoubleRange thetaRange)
        {
            _center = center;
            _thetaRange = thetaRange;
        }

        public Int2DEllipticalSource()
            : this(
             0.5,
             0.25,
             new DoubleRange(0.5, 0.5),
             new DoubleRange(0, 2 * Math.PI)) { }

        public double AParameter { get; set; }
        public double BParameter { get; set; }
        public DoubleRange Center { get; set; }
        public DoubleRange ThetaRange { get; set; }
    }
}
