using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for PointSourceBase
    /// </summary>
    public abstract class PointSourceBase : ISource
    {      
        /// <summary>
        /// Polar angle range
        /// </summary>
        protected DoubleRange _polarAngleEmissionRange;
        /// <summary>
        /// Azimuthal angle range
        /// </summary>
        protected DoubleRange _azimuthalAngleEmissionRange;
        /// <summary>
        /// New position
        /// </summary>
        protected Position _pointLocation;
        /// <summary>
        /// Point source emitting direction
        /// </summary>
        protected Direction _direction;
        /// <summary>
        /// Rotation angles pair(polar/azimuthal) of emitting direction
        /// </summary>
        protected PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis;
        /// <summary>
        /// Source translation and rotation flags
        /// </summary>
        protected SourceFlags _rotationAndTranslationFlags;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;
        
        /// <summary>
        /// Defines PointSourceBase class
        /// </summary>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        /// <param name="direction">Point source emitting direction</param>
        /// <param name="pointLocation">New position</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected PointSourceBase( 
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction direction,
            Position pointLocation,
            int initialTissueRegionIndex)
        {
            if (direction == null)
                direction = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone();
            if (pointLocation == null)
                pointLocation = SourceDefaults.DefaultPosition.Clone();       

            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();    
            _pointLocation = pointLocation.Clone();
            _direction = direction.Clone();

            _rotationAndTranslationFlags = new SourceFlags(
                direction != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                pointLocation != SourceDefaults.DefaultPosition.Clone(),
                false);
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Implements Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns>photon</returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts at the origin 
            Position finalPosition = SourceDefaults.DefaultPosition.Clone();

            // sample angular distribution
            Direction finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);

            //Find the relevent polar and azimuthal pair for the direction
            _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_direction);

            //Rotation and translation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _pointLocation,
                _rotationAndTranslationFlags);

            var photon = new Photon(finalPosition, finalDirection, tissue, _initialTissueRegionIndex, Rng);

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
