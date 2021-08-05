using System;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D.SourceInputs
{
    /// <summary>
    /// Internal 2D Circular source
    /// </summary>
    public class Int2DCircularSourceInput : IIntFemSourceInput
    {    
        /// <summary>
        /// General constructor for 2D circular source
        /// </summary>
        /// <param name="radius">Radius</param>
        /// <param name="center">Center (x,z)</param>
        /// <param name="thetaRange">Theta Range</param>
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

        /// <summary>
        /// Default constructor for 2D circular source
        /// </summary>
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
