using System;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate source given a ISourceInput, ITissue
    /// </summary>
    public static class SourceFactory
    {
        public static ISource GetSource(ISourceInput si, ITissue tissue, Random rng)
        {
            if (si is CustomPointSourceInputOld)
            {
                return new CustomPointSourceOld((CustomPointSourceInputOld) si) {Rng = rng};
            }
            // else if...
            
            throw new ArgumentException(
                    "Problem generating ISource instance. Check that SourceInput, si, has a matching ISource definition.");
        }
    }
}
