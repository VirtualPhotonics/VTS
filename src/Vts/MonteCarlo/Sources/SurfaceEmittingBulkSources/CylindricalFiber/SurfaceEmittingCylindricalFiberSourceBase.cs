using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for SurfaceEmittingCylindricalFiberSourceBase
    /// </summary>
    public abstract class SurfaceEmittingCylindricalFiberSourceBase : ISource
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
        /// Fiber radius
        /// </summary>
        protected double _fiberRadius;
        /// <summary>
        /// Fiber height
        /// </summary>
        protected double _fiberHeightZ;
        /// <summary>
        /// Efficciency of the curved surface (0-1)
        /// </summary>
        protected double _curvedSurfaceEfficiency;
        /// <summary>
        /// Efficciency of the bottom surface (0-1)
        /// </summary>
        protected double _bottomSurfaceEfficiency;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;

        /// <summary>
        /// Defines SurfaceEmittingCylindricalFiberSourceBase class
        /// </summary>
        /// <param name="fiberRadius">Fiber radius</param>
        /// <param name="fiberHeightZ">Fiber height</param>
        /// <param name="curvedSurfaceEfficiency">Efficciency of the curved surface (0-1)</param>
        /// <param name="bottomSurfaceEfficiency">Efficciency of the bottom surface (0-1)</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected SurfaceEmittingCylindricalFiberSourceBase(
            double fiberRadius,
            double fiberHeightZ,  
            double curvedSurfaceEfficiency,
            double bottomSurfaceEfficiency,
            Direction newDirectionOfPrincipalSourceAxis,                  
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                 newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                 translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                 false);
            
            _fiberRadius = fiberRadius;
            _fiberHeightZ = fiberHeightZ;
            _curvedSurfaceEfficiency = curvedSurfaceEfficiency;
            _bottomSurfaceEfficiency = bottomSurfaceEfficiency;
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
            double curved = 2 * Math.PI * _fiberRadius * _fiberHeightZ * _curvedSurfaceEfficiency;
            double bottom = Math.PI * _fiberRadius * _fiberRadius * _bottomSurfaceEfficiency;

            Direction finalDirection;
            Position finalPosition;

            if (_fiberRadius > 0.0)
            {              
                if (Rng.NextDouble() > bottom / (curved + bottom))
                {
                    //sample angular distribution
                    finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                        SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                        SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                        Rng);

                    //Translate the photon to _tubeRadius length below the origin. Ring lies on yz plane.
                    finalPosition = new Position(0.0, 0.0, _fiberRadius);

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
                    finalPosition.Z = _fiberHeightZ * (2.0 * Rng.NextDouble() - 1.0);
                }
                else
                {
                    finalPosition = SourceToolbox.GetPositionInACircleRandomFlat(
                        SourceDefaults.DefaultPosition.Clone(),
                        0.0,
                        _fiberRadius,
                        Rng);

                    finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                        SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                        SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                        Rng);
                }
            }
            else                 
            {
                finalPosition = SourceToolbox.GetPositionInALineRandomFlat(
                        SourceDefaults.DefaultPosition.Clone(),
                        _fiberHeightZ,
                        Rng);

                finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                        SourceDefaults.DefaultFullPolarAngleRange.Clone(),
                        SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                        Rng);

                //Rotate 90degrees around y axis
                SourceToolbox.UpdateDirectionPositionAfterRotatingAroundYAxis(
                        0.5 * Math.PI,
                        ref finalDirection,
                        ref finalPosition);
            }


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
