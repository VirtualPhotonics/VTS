using System;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Source classes for Monte Carlo simulation.
    /// </summary>
    public interface ISource
    {
        Photon GetNextPhoton(ITissue tissue);

        Random Rng { get; set; }
    }
}
