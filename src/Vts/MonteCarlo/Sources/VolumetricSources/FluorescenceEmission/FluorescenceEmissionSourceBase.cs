using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for FluorescenceEmissionSourceBase
    /// </summary>
    public abstract class FluorescenceEmissionSourceBase : ISource
    {
        /// <summary>
        /// Input folder where AE(x,y,z) resides
        /// </summary>
        protected string _inputFolder;
        /// <summary>
        /// Infile that generated AOfXAndYAndZ
        /// </summary>
        protected string _infile;        
        /// <summary>
        /// Initial tissue region index = tissue region of fluorescence
        /// </summary>
        protected int _initialTissueRegionIndex;
        /// <summary>
        /// Photon weight as determined by GetNextPositionAndWeight
        /// </summary>
        protected double _weight;

        /// <summary>
        /// Defines FluorescenceEmissionSourceBase class
        /// </summary>
        /// <param name="inputFolder">folder where absorbed energy result resides</param>
        /// <param name="infile">infile that was used to generate absorbed energy result</param>
        /// <param name="fluorescenceTissueRegionIndex">tissue region index of fluorescent region</param>
        protected FluorescenceEmissionSourceBase(string inputFolder, string infile,
            int fluorescenceTissueRegionIndex)
        {
            _inputFolder = inputFolder;
            _infile = infile;
            _initialTissueRegionIndex = fluorescenceTissueRegionIndex;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the tissue region of fluorescence
            var finalPosition = GetFinalPositionAndWeight(Rng, out _weight);
            //Starting weight based on sampling method
            //Lambertian direction
            var finalDirection = GetFinalDirection(Rng);
            
            var photon = new Photon(finalPosition, finalDirection, _weight, tissue, _initialTissueRegionIndex, Rng);
            return photon;
        }

        /// <summary>
        /// Returns Lambertian direction - all fluorescence emission is Lambertian
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <returns>photon Direction</returns>
        private static Direction GetFinalDirection(Random rng)
        {
            //Lambertian distribution 
            return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                    SourceDefaults.DefaultFullPolarAngleRange.Clone(),
                    SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                    rng);
        }

        /// <summary>
        /// each inheritor generates own GetFinalPositionAndWeight
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <param name="weight">out: photon weight</param>
        /// <returns>photon Position</returns>
        protected abstract Position GetFinalPositionAndWeight(Random rng, out double weight);

        #region Random number generator code (copy-paste into all sources)
        /// <summary>
        /// The random number generator used to create photons. If not assigned externally,
        /// a Mersenne Twister (MathNet.Numerics.Random.MersenneTwister) will be created with
        /// a seed of zero.
        /// </summary>
        public Random Rng
        {
            get
            {
                if (_rng == null)
                {
                    _rng = new MathNet.Numerics.Random.MersenneTwister(0);
                }
                return _rng;
            }
            set { _rng = value; }
        }
        private Random _rng;
        #endregion
    }
}
