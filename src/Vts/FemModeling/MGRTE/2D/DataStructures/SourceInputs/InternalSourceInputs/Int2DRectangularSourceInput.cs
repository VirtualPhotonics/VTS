using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.SourceInputs
{
    public class Int2DRectangularSourceInput : IIntFemSourceInput
    {
        public Int2DRectangularSourceInput(
            double xLength,
            double zHeight,
            DoubleRange center,
            DoubleRange thetaRange)
        {
            SourceType = FemSourceType.Int2DRectangularSource;
            XLength = xLength;
            ZHeight = zHeight;
            Center = center;
            ThetaRange = thetaRange;
        }

        public Int2DRectangularSourceInput()
            : this(
             0.25,
             0.125,
             new DoubleRange(0.0, 0.5),
             new DoubleRange(0, 2 * Math.PI)) { }
                
        /// <summary>
        /// External source type
        /// </summary>
        public FemSourceType SourceType { get; set; }
        /// <summary>
        /// Length of the rectangular source 
        /// </summary>
        public double XLength { get; set; }
        /// <summary>
        /// height of the rectangular source 
        /// </summary>
        public double ZHeight { get; set; }
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
