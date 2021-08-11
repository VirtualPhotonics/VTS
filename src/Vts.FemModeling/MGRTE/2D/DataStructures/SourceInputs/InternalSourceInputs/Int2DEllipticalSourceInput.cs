using System;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D.SourceInputs
{
    /// <summary>
    /// Internal 2D Elliptical source
    /// </summary>
    public class Int2DEllipticalSourceInput : IIntFemSourceInput
    {   
        /// <summary>
        /// General constructor for 2D elliptical source
        /// </summary>
        /// <param name="aParameter">a Parameter</param>
        /// <param name="bParameter">b parameter</param>
        /// <param name="center">Center (x,z)</param>
        /// <param name="thetaRange">Theta Range</param>
        public Int2DEllipticalSourceInput(
            double aParameter,
            double bParameter,
            DoubleRange center,
            DoubleRange thetaRange)
        {
            SourceType = FemSourceType.Int2DEllipticalSource;
            AParameter = aParameter;
            BParameter = bParameter;
            Center = center;
            ThetaRange = thetaRange;
        }

        /// <summary>
        /// Default constructor for 2D elliptical source
        /// </summary>
        public Int2DEllipticalSourceInput()
            : this(
             0.25,
             0.125,
             new DoubleRange(0.0, 0.5),
             new DoubleRange(0, 2 * Math.PI)) { }
                
        /// <summary>
        /// Internal source type
        /// </summary>
        public FemSourceType SourceType { get; set; }
        /// <summary>
        /// a Parameter of the Elliptical source 
        /// </summary>
        public double AParameter { get; set; }
        /// <summary>
        /// b Parameter of the Elliptical source 
        /// </summary>
        public double BParameter { get; set; }
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
