using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Defines a contract for Tissue classes in Monte Carlo simulation.
    /// </summary>
    public interface ITissue
    {
        /// <summary>
        /// AbsorptionWeightingType enum specifier indicating Analog, Discrete Absorption weighting, etc.
        /// </summary>
        AbsorptionWeightingType AbsorptionWeightingType { get; }

        /// <summary>
        /// photon weight threshold, below which turns on Russian Roulette
        /// </summary>
        double RussianRouletteWeightThreshold { get; }

        /// <summary>
        /// PhaseFunctionType enum specifier indicating Henyey-Greenstein, Bidirectional, etc.
        /// </summary>
        PhaseFunctionType PhaseFunctionType { get; }

        /// <summary>
        /// A list of ITissueRegions that describes the entire system.
        /// </summary>
        IList<ITissueRegion> Regions { get; }

        /// <summary>
        /// The scattering lengths for each tissue region.  For discrete absorption weighting and
        /// analog, this is based on 1/mut, for continuous it is based on 1/mus.
        /// </summary>
        IList<double> RegionScatterLengths { get; }

        /// <summary>
        /// Method that gives the current region index within Regions list (above) at the current
        /// position of the photon.
        /// </summary>
        /// <param name="position">current location of photon</param>
        /// <returns></returns>
        int GetRegionIndex(Position position);


    }
}
