using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
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
        /// PhaseFunctionType enum specifier indicating Henyey-Greenstein, Birdirectional, etc.
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
        /// Required method to initialiize the corresponding ITissue
        /// </summary>
        /// <param name="tissue"></param>
        void Initialize(
            AbsorptionWeightingType awt = AbsorptionWeightingType.Discrete,
            PhaseFunctionType pft = PhaseFunctionType.HenyeyGreenstein,
            double russianRouletteWeightThreshold = 0.0);

        /// <summary>
        /// Method that gives the current region index within Regions list (above) at the current
        /// position of the photon.
        /// </summary>
        /// <param name="position">current location of photon</param>
        /// <returns></returns>
        int GetRegionIndex(Position position);

        /// <summary>
        /// Method that provides the distance to the closest tissue boundary.
        /// </summary>
        /// <param name="photon">Photon information (e.g. current position, direction,
        /// and current track length, S)</param>
        /// <returns>distance, includes double.PositiveInfinity and double.NegativeInfinity</returns>
        double GetDistanceToBoundary(Photon photon);  

        /// <summary>
        /// Method to provide the angle relative to a tissue boundary.
        /// </summary>
        /// <param name="photon">Photon information (e.g. direction and position).</param>
        /// <returns></returns>
        double GetAngleRelativeToBoundaryNormal(Photon photon);

        /// <summary>
        /// Method that gives the region the photon is about to enter.
        /// </summary>
        /// <param name="photon">Photon information (e.g. direction and position)</param>
        /// <returns></returns>
        int GetNeighborRegionIndex(Photon photon);

        /// <summary>
        /// Method to determine whether on tissue domain boundary.  This method helps to
        /// determine when photon leaves the phase space and enters the air for example.
        /// </summary>
        /// <param name="position">Photon position</param>
        /// <returns></returns>
        bool OnDomainBoundary(Position position); 

        /// <summary>
        /// Method to return updated PhotonStateType enum indicating type of photon exit
        /// from domain (e.g. reflectance boundary, transmittance boundary)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        PhotonStateType GetPhotonDataPointStateOnExit(Position position);

        /// <summary>
        /// Method to provide reflected direction of photon given current position and direction.
        /// </summary>
        /// <param name="currentPosition">current position of photon</param>
        /// <param name="currentDirection">current direction of photon</param>
        /// <returns></returns>
        Direction GetReflectedDirection(Position currentPosition, Direction currentDirection);

        /// <summary>
        /// Method to provide refracted direction of photon.
        /// </summary>
        /// <param name="currentPosition">current position of photon</param>
        /// <param name="currentDirection">current direction of photon</param>
        /// <param name="currentN">refractive index of current tissue type</param>
        /// <param name="nextN">refractive index of next tissue type</param>
        /// <param name="cosThetaSnell">cos(theta) of the refracted direction according to Snell's law</param>
        /// <returns></returns>
        Direction GetRefractedDirection(Position currentPosition, Direction currentDirection, 
            double currentN, double nextN, double cosThetaSnell);
    }
}
