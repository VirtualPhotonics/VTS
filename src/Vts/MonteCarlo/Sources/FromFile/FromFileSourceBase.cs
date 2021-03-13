using System;
using System.Collections.Generic;
using System.Linq;
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
        protected ZRDRayDatabase _sourceDatabase;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;
        /// <summary>
        /// SourceDatabase changed into list
        /// </summary>
        private IList<ZRDRayDataInUFD.ZRDRayDataPoint> listOfSourceDataPoints;
        private static int index;
        
        /// <summary>
        /// Defines FromFileSourceBase class
        /// </summary>
        protected FromFileSourceBase( 
            string sourceFileName,
            int initialTissueRegionIndex)
        {
            _sourceDatabase = ReadFile(sourceFileName);
            _initialTissueRegionIndex = initialTissueRegionIndex;
            //listOfSourceDataPoints = _sourceDatabase.ZRDRayDataPoints.ToList();
            index = 0;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            //// read next source data point
            //var _sourceDataPoint = listOfSourceDataPoints[index];
            //_sourceDataPoint.Weight = 1.0;
            //++index;

            //var photon = new Photon(_sourceDataPoint.Position, _sourceDataPoint.Direction,
            //    _sourceDataPoint.Weight, tissue, _initialTissueRegionIndex, Rng);
            var photon = new Photon();
            return photon;
        }
        /// <summary>
        /// Each source that inherits this base class returns SourceDatabase after reading file 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected abstract ZRDRayDatabase ReadFile(string filename);

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
