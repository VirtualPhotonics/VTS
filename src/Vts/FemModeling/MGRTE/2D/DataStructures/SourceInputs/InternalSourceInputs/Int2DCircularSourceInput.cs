using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.SourceInputs
{
    public class Int2DCircularSourceInput : IIntFemSourceInput
    {        
        public Int2DCircularSourceInput(
            double radius,
            DoubleRange center,
            DoubleRange thetaRange)
        {
            SourceType = FemSourceType.Int2DCircularSource;
            Radius = radius;
            Center = center;
            ThetaRange = thetaRange;
        }

        public Int2DCircularSourceInput()
            : this(
             0.25,
             new DoubleRange(0.0, 0.5),
             new DoubleRange(0, 2 * Math.PI)) { }
                
        /// <summary>
        /// Internal source type
        /// </summary>
        public FemSourceType SourceType { get; set; }
        /// <summary>
        /// Radius of the circular source 
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// Center cooordinates (x,z) 
        /// </summary>
        public DoubleRange Center { get; set; }
        /// <summary>
        /// Theta angle range
        /// </summary>
        public DoubleRange ThetaRange { get; set; }
        
    }
}
