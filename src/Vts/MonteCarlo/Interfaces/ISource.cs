using System;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Source classes for Monte Carlo simulation.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Method to initiate new photon.
        /// </summary>
        /// <param name="tissue">ti</param>
        /// <returns></returns>
        Photon GetNextPhoton(ITissue tissue);
        /// <summary>
        /// Random number generator
        /// </summary>
        Random Rng { get; }
    }
}
