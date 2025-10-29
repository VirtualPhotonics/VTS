using System;
using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for FromFileSourceBase
    /// </summary>
    public abstract class FromFileSourceBase : ISource
    {
        /// <summary>
        /// enumerator through database
        /// </summary>
        protected IEnumerator<PhotonDataPoint> Enumerator;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int InitialTissueRegionIndex;

        /// <summary>
        /// Defines FromFileSourceBase class
        /// </summary>
        protected FromFileSourceBase(
            IEnumerator<PhotonDataPoint> enumerator,
            int initialTissueRegionIndex)
        {
            Enumerator = enumerator;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            // read next source data point
            Enumerator.MoveNext();
            var dp = Enumerator.Current;

            var photon = new Photon(dp.Position, dp.Direction,
            dp.Weight, tissue, InitialTissueRegionIndex, Rng); // Rng not used here

            return photon;
        }

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