using System;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D.SourceInputs
{
    /// <summary>
    /// External point source
    /// </summary>
    public class ExtPointSourceInput : IExtFemSourceInput
    {   
        /// <summary>
        /// General constructor for external point source
        /// </summary>
        /// <param name="launchPoint">Launch point (x,z)</param>
        /// <param name="thetaRange">Theta Range</param>
        public ExtPointSourceInput(
            DoubleRange launchPoint,
            DoubleRange thetaRange)
        {
            SourceType = FemSourceType.ExtPointSource;
            LaunchPoint = launchPoint;
            ThetaRange = thetaRange;
        }

        /// <summary>
        /// Default constructor for external point source
        /// </summary>
        public ExtPointSourceInput()
            : this(             
             new DoubleRange(0.0, 0.0),
             new DoubleRange(0, 2 * Math.PI)) { }
                
        /// <summary>
        /// Ext source type
        /// </summary>
        public FemSourceType SourceType { get; set; }
        /// <summary>
        /// Launching cooordinates (x,z)  
        /// </summary>
        public DoubleRange LaunchPoint { get; set; }        
        /// <summary>
        /// Theta angle range
        /// </summary>
        public DoubleRange ThetaRange { get; set; }
        
    }
}
