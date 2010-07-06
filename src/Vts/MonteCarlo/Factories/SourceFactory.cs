using System;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate source given a ISourceInput, ITissue
    /// </summary>
    public static class SourceFactory
    {
        public static ISource GetSource(ISourceInput si, ITissue tissue, bool tallyMomentumTransfer)
        {
            ISource s = null;
            if (si is PointSourceInput)
            {
                return new PointSource((PointSourceInput)si, tissue, tallyMomentumTransfer);
            }
            if (s == null)
                throw new ArgumentException(
                    "Problem generating ISource instance. Check that SourceInput, si, has a matching ISource definition.");

            return s;
        }
    }
}
