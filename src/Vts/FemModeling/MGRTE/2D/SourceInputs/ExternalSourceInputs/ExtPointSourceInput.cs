using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.SourceInputs
{
    public class ExtPointSourceInput : IExtFemSourceInput
    {   
        public ExtPointSourceInput(
            DoubleRange launchPoint,
            DoubleRange thetaRange)
        {
            SourceType = FemSourceType.ExtPointSource;
            LaunchPoint = launchPoint;
            ThetaRange = thetaRange;
        }

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
