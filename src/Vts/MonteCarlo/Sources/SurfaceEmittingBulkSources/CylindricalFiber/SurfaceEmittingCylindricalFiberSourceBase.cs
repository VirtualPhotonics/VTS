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

            Direction finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                        SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                        SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                        Rng);
            Position finalPosition = SourceDefaults.DefaultPosition.Clone();

            if (_fiberRadius > 0.0)
            {   
                if (Rng.NextDouble() > bottom / (curved + bottom))   //Consider 
                {   /* Curved surface */
                    // To utilize the final direction given above, we can assume a tube 
                    // parallel to the y-axis. We can rotate it about the x-axis by pi/2 
                    // to compute the new direction.
                    SourceToolbox.UpdateDirectionAfterRotatingAroundXAxis(-0.5 * Math.PI, finalDirection);

                    //Sample tube perimeter first to compute x and y coordinates
                    finalPosition = SourceToolbox.GetPositionAtCirclePerimeter(finalPosition,
                        _fiberRadius, 
                        Rng);

                    //Sample tube height to compute z coordinate
                    finalPosition.Z = _fiberHeightZ * (Rng.NextDouble() - 0.5);
                }
                else
                {   /* Bottom Surface */
                    //Shift finalPosition by _fiberHeightZ / 2
                    finalPosition = new Position(0.0, 0.0, _fiberHeightZ * 0.5);

                    //Sample the bottom face to find  x, y coordinates of the emission
                    finalPosition = SourceToolbox.GetPositionInACircleRandomFlat(finalPosition,
                        0.0,
                        _fiberRadius,
                        Rng);                    
                }
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

            var photon = new Photon(finalPosition, finalDirection, 1.0, tissue, _initialTissueRegionIndex, Rng);

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
