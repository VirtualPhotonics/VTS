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
            var finalPosition = SourceDefaults.DefaultPosition.Clone();

            // sample angular distribution
            var finalDirection = GetFinalDirection(finalPosition);

            if (_tubeRadius > 0.0)
            {
                // To utilize the final direction given above, we can assume a tube 
                // parallel to the y-axis. We can rotate it about the x-axis by pi/2 
                // to compute the new direction.
                SourceToolbox.UpdateDirectionAfterRotatingAroundXAxis(0.5 * Math.PI, finalDirection);

                //Sample tube perimeter first to compute x and y coordinates
                finalPosition = SourceToolbox.GetPositionAtCirclePerimeter(finalPosition,
                    _tubeRadius,
                    Rng);

                //Sample tube height to compute z coordinate
                finalPosition.Z = _tubeHeightZ * (Rng.NextDouble() - 0.5);
            }

            //Find the relevant polar and azimuthal pair for the direction
            var _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);
            
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

        /// <summary>
        /// Returns final direction for a given position
        /// </summary>
        /// <param name="position">Current position</param>
        /// <returns>new direction</returns>
        protected abstract Direction GetFinalDirection(Position position); // angular distribution may be function of position


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
