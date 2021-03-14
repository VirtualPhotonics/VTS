using System;
using System.Collections.Generic;
using Vts.MonteCarlo.RayData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for FromFileSourceBase
    /// </summary>
    public abstract class FromFileSourceBase : ISource
    {  
        /// <summary>
        /// Database of SourceDataPoint
        /// </summary>
        protected IList<RayDataPoint> _sourceDatabase;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;
        private static int index;

        /// <summary>
        /// Defines FromFileSourceBase class
        /// </summary>
        protected FromFileSourceBase(
            //string sourceFileName,  // CKH:should base read in database or implementor
            int initialTissueRegionIndex)
        {
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
            var _sourceDataPoint = _sourceDatabase[index];
            _sourceDataPoint.Weight = 1.0;
            ++index;

            var photon = new Photon(_sourceDataPoint.Position, _sourceDataPoint.Direction,
                _sourceDataPoint.Weight, tissue, _initialTissueRegionIndex, Rng);

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
