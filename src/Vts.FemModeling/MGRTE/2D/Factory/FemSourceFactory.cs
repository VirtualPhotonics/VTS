using System;
using Vts.FemModeling.MGRTE._2D.DataStructures;
using Vts.FemModeling.MGRTE._2D.SourceInputs;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Instantiates appropriate source
    /// </summary>
    public static class FemSourceFactory
    {
        /// <summary>
        /// Get External source
        /// </summary>
        /// <param name="input">External FEM source</param>
        /// <returns>selected source</returns>
        public static IExtSource GetExtSource(IExtFemSourceInput input)
        {
            switch (input.SourceType)
            {
                case FemSourceType.ExtPointSource:
                    var epsInput = (ExtPointSourceInput)input;
                    return new ExtPointSource(
                        epsInput.LaunchPoint,
                        epsInput.ThetaRange);

                case FemSourceType.ExtLineSource:
                    var elsInput = (ExtLineSourceInput)input;
                    return new ExtLineSource(
                        elsInput.Start,
                        elsInput.End,
                        elsInput.ThetaRange);

                default:
                    throw new NotImplementedException(
                        "Problem generating IExtSource instance. Check that IExtFemSourceInput has a matching IExtSource definition.");
            }             
        }

        /// <summary>
        /// Get Internal source
        /// </summary>
        /// <param name="input">Internal FEM source</param>
        /// <returns>selected source</returns>
        public static IIntSource GetIntSource(IIntFemSourceInput input)
        {
            switch (input.SourceType)
            {
                case FemSourceType.Int2DPointSource:
                    var ipsInput = (Int2DPointSourceInput)input;
                    return new Int2DPointSource(
                        ipsInput.Center,
                        ipsInput.ThetaRange);

                case FemSourceType.Int2DCircularSource:
                    var icsInput = (Int2DCircularSourceInput)input;
                    return new Int2DCircularSource(
                        icsInput.Radius,
                        icsInput.Center,
                        icsInput.ThetaRange);

                case FemSourceType.Int2DEllipticalSource:
                    var iesInput = (Int2DEllipticalSourceInput)input;
                    return new Int2DEllipticalSource(
                        iesInput.AParameter,
                        iesInput.BParameter,
                        iesInput.Center,
                        iesInput.ThetaRange);

                case FemSourceType.Int2DRectangularSource:
                    var irsInput = (Int2DRectangularSourceInput)input;
                    return new Int2DRectangularSource(
                        irsInput.XLength,
                        irsInput.ZHeight,
                        irsInput.Center,
                        irsInput.ThetaRange);

                default:
                    throw new NotImplementedException(
                        "Problem generating IIntSource instance. Check that IIntFemSourceInput has a matching IIntSource definition.");
            }
        }        
    }
}