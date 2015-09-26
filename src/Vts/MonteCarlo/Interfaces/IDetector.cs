
using System;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Properties and methods that all IDetectors must implement
    /// </summary>
    public interface IDetector
    {
        /// <summary>
        /// TallyType enum specification
        /// </summary>
        string TallyType { get; }

        /// <summary>
        /// Name string of IDetector.  Default = TallyType.ToString().
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicates if 2nd moment is tallied
        /// </summary>
        bool TallySecondMoment { get; }

        /// <summary>
        /// Details of the tally - booleans that specify when they should be tallied
        /// </summary>
        TallyDetails TallyDetails { get; set; }

        /// <summary>
        /// Initialize the detector, using tissue and random number generator information if necessary
        /// </summary>
        /// <param name="tissue">tissue definition</param>
        /// <param name="rng">random number generator</param>
        void Initialize(ITissue tissue = null, Random rng = null);

        /// <summary>
        /// Method to tally to detector using information in Photon
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        void Tally(Photon photon);

        /// <summary>
        /// Method to normalize the tally to get Mean and Second Moment estimates
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        void Normalize(long numPhotons);

        /// <summary>
        /// Method that returns info for each large binary data array
        /// </summary>
        /// <returns></returns>
        BinaryArraySerializer[] GetBinarySerializers();
    }
}