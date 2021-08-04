using System;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D.SourceInputs
{
    /// <summary>
    /// External line source
    /// </summary>
    public class ExtLineSourceInput : IExtFemSourceInput
    {
        /// <summary>
        /// General constructor for external line source
        /// </summary>
        /// <param name="start">starting point (x,z)</param>
        /// <param name="end">end point (x,z)</param>
        /// <param name="thetaRange">Theta Range</param>
        public ExtLineSourceInput(
            DoubleRange start,
            DoubleRange end,
            DoubleRange thetaRange)
        {
            SourceType = FemSourceType.ExtPointSource;
            Start = start;
            End = end;
            ThetaRange = thetaRange;
        }

        /// <summary>
        /// Default constructor for external line source
        /// </summary>
        public ExtLineSourceInput()
            : this(
             new DoubleRange(0.0, 0.0),
             new DoubleRange(0.0, 0.0),
             new DoubleRange(0, 2 * Math.PI)) { }
                
        /// <summary>
        /// Ext source type
        /// </summary>
        public FemSourceType SourceType { get; set; }
        /// <summary>
        /// Starting cooordinates (x,z)  
        /// </summary>
        public DoubleRange Start { get; set; }
        /// <summary>
        /// Ending cooordinates (x,z) 
        /// </summary>
        public DoubleRange End { get; set; }
        /// <summary>
        /// Theta angle range
        /// </summary>
        public DoubleRange ThetaRange { get; set; }
        
    }
}
