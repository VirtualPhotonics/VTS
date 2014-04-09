using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for SurfaceEmittingTubularSourceBase
    /// </summary>
    public abstract class SurfaceEmittingTubularSourceBase : ISource
    {
        /// <summary>
        /// New source axis direction
        /// </summary>
        protected Direction _newDirectionOfPrincipalSourceAxis;
        /// <summary>
        /// New source location
        /// </summary>
        protected Position _translationFromOrigin;
        /// <summary>
        /// Source rotation and translation flags
        /// </summary>
        protected SourceFlags _rotationAndTranslationFlags;
        /// <summary>
        /// Tube radius
        /// </summary>
        protected double _tubeRadius;
        /// <summary>
        /// Tube height
        /// </summary>
        protected double _tubeHeightZ;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;

        /// <summary>
        /// Defines SurfaceEmittingTubularSourceBase class
        /// </summary>
        /// <param name="tubeRadius">Tube radius</param>
        /// <param name="tubeHeightZ">Tube height</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected SurfaceEmittingTubularSourceBase(
            double tubeRadius,
            double tubeHeightZ,  
            Direction newDirectionOfPrincipalSourceAxis,                  
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                 newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                 translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                 false);
            
            _tubeRadius = tubeRadius;
            _tubeHeightZ = tubeHeightZ;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
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
            //sample angular distribution
            Direction finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(), 
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                Rng);

            //Translate the photon to _tubeRadius length below the origin. Ring lies on yz plane.
            Position finalPosition = new Position(0.0, 0.0, _tubeRadius);

            //Sample a ring that emits photons outside.
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundXAxis(
                2.0 * Math.PI * Rng.NextDouble(),
                ref finalDirection,
                ref finalPosition);

            //Ring lies on xy plane. z= 0;
            SourceToolbox.UpdateDirectionPositionAfterRotatingAroundYAxis(
                0.5 * Math.PI,
                ref finalDirection,
                ref finalPosition);

            //Sample tube height
            finalPosition.Z = _tubeHeightZ * (2.0 * Rng.NextDouble() -1.0);

            //Find the relevent polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);
            
            //Translation and source rotation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,                
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
