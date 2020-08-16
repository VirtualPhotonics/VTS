using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for AdjointLineSourceBase.  Adjoint sources are ray traced
    /// back to tissue.
    /// </summary>
    public abstract class AdjointLineSourceBase : ISource
    {
        /// <summary>
        /// New source location
        /// </summary>
        protected Position _translationFromOrigin;
        /// <summary>
        /// The length of the line source
        /// </summary>
        protected double _lineLength;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;

        /// <summary>
        /// Defines LineSourceBase class
        /// </summary>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected AdjointLineSourceBase(
            double lineLength,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
           if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
            _lineLength = lineLength;
            _translationFromOrigin = translationFromOrigin.Clone();
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the line
            Position airPosition = SourceToolbox.GetPositionInALineRandomFlat(
                _translationFromOrigin, _lineLength, Rng);

            // determine final position on tissue
            Position finalPosition = GetFinalPosition(airPosition);

            // determine angular distribution
            Direction finalDirection = GetFinalDirection(finalPosition);

            var photon = new Photon(finalPosition, finalDirection, 1.0, tissue, _initialTissueRegionIndex, Rng);

            return photon;
        }

        /// <summary>
        /// Returns final position for a given air position
        /// </summary>
        /// <param name="position">Current position</param>
        /// <returns>new direction</returns>
        protected abstract Position GetFinalPosition(Position airPosition);

        /// <summary>
        /// Returns final direction for a given position
        /// </summary>
        /// <param name="position">Current position</param>
        /// <returns>new direction</returns>
        protected abstract Direction GetFinalDirection(Position position); 


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
