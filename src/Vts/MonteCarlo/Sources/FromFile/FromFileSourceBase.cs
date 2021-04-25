using System;
using System.Collections.Generic;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for FromFileSourceBase
    /// </summary>
    public abstract class FromFileSourceBase : ISource
    {
        protected IEnumerator<PhotonDataPoint> _enumerator;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;
        private static int index;

        /// <summary>
        /// Defines FromFileSourceBase class
        /// </summary>
        protected FromFileSourceBase(
            IEnumerator<PhotonDataPoint> enumerator,
            int initialTissueRegionIndex)
        {
            _enumerator = enumerator;
            _initialTissueRegionIndex = initialTissueRegionIndex;
            index = 0;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            // read next source data point
            _enumerator.MoveNext();
            var dp = _enumerator.Current;

            var photon = new Photon(dp.Position, dp.Direction,
            dp.Weight, tissue, _initialTissueRegionIndex, Rng); // Rng not used here

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
