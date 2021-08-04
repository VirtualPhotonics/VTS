using System;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D.SourceInputs
{
    /// <summary>
    /// Internal 2D Point source
    /// </summary>
    public class Int2DPointSourceInput : IIntFemSourceInput
    {
        /// <summary>
        /// General constructor for 2D point source
        /// </summary>
        /// <param name="center">Center (x,z)</param>
        /// <param name="thetaRange">Theta Range</param>
        public Int2DPointSourceInput(            
            DoubleRange center,
            DoubleRange thetaRange)
        {
            SourceType = FemSourceType.Int2DPointSource;            
            Center = center;
            ThetaRange = thetaRange;
        }

        /// <summary>
        /// Default constructor for 2D point source
        /// </summary>
        public Int2DPointSourceInput()
            : this(
             new DoubleRange(0, 5.0),
             new DoubleRange(0, 2 * Math.PI)) { }
                
        /// <summary>
        /// Internal source type
        /// </summary>
        public FemSourceType SourceType { get; set; }        
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
