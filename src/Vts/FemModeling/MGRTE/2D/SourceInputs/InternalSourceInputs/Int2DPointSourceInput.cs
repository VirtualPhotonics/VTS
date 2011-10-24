using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.SourceInputs
{
    public class Int2DPointSourceInput : IIntFemSourceInput
    {
        public Int2DPointSourceInput(            
            DoubleRange center,
            DoubleRange thetaRange)
        {
            SourceType = FemSourceType.Int2DPointSource;            
            Center = center;
            ThetaRange = thetaRange;
        }

        public Int2DPointSourceInput()
            : this(
             new DoubleRange(0.0, 0.5),
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
