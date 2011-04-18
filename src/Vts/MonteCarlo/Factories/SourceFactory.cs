using System;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate source given a ISourceInput, ITissue
    /// </summary>
    public static class SourceFactory
    {
        public static ISource GetSource(ISourceInput input, ITissue tissue, Random rng)
        {
            if (input is PointSourceCollimatedInput)
            {
                return new DirectionalPointSource() { Rng = rng }; // todo: update SourceFactory 
            }
            // else if...

            throw new ArgumentException(
                    "Problem generating ISource instance. Check that SourceInput, si, has a matching ISource definition.");
        }
    }
}
