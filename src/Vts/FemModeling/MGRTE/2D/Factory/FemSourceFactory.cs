using System;
using Vts.Common;
using Vts.FemModeling.MGRTE._2D;
using Vts.FemModeling.MGRTE._2D.DataStructures;
using Vts.FemModeling.MGRTE._2D.SourceInputs;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Instantiates appropriate source given a ISourceInput, ITissue
    /// </summary>
    public static class FemSourceFactory
    {
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


        public static IIntSource GetExtSource(IIntFemSourceInput input)
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
                        "Problem generating IExtSource instance. Check that IExtFemSourceInput has a matching IExtSource definition.");
            }
        }
    }
}