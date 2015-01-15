using System;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate source given a ISourceInput, ITissue
    /// </summary>
    public static class SourceFactory
    {
        /// <summary>
        /// Method to instantiate the correct source given ISourceInput
        /// </summary>
        /// <param name="input">ISourceInput</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="rng">random number generator instance</param>
        /// <returns>ISource</returns>
        public static ISource GetSource(ISourceInput input, Random rng)
        {
            var source = input.CreateSource(rng);

            return source;
        }
    }
}
