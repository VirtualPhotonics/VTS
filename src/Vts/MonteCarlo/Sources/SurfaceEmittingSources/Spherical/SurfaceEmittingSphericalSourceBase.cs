using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for SurfaceEmittingSphericalSourceBase
    /// </summary>
    public abstract class SurfaceEmittingSphericalSourceBase : ISource
    {
        protected DoubleRange _polarAngleRangeToDefineSphericalSurface;  
        protected DoubleRange _azimuthalAngleRangeToDefineSphericalSurface;
        protected Direction _newDirectionOfPrincipalSourceAxis;
        protected Position _translationFromOrigin;
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _radius;
        protected int _initialTissueRegionIndex;

        protected SurfaceEmittingSphericalSourceBase(
            double radius,
            DoubleRange polarAngleRangeToDefineSphericalSurface,  
            DoubleRange azimuthalAngleRangeToDefineSphericalSurface,            
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            _radius = radius;
            _polarAngleRangeToDefineSphericalSurface = polarAngleRangeToDefineSphericalSurface.Clone();
            _azimuthalAngleRangeToDefineSphericalSurface = azimuthalAngleRangeToDefineSphericalSurface.Clone();
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone(); 
            _translationFromOrigin = translationFromOrigin.Clone();
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Implement Get next photon
        /// </summary>
        /// <param name="tissue">tissue</param>
        /// <returns></returns>
        public Photon GetNextPhoton(ITissue tissue)
        {
            // sample angular distribution
            Direction finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(_polarAngleRangeToDefineSphericalSurface, _azimuthalAngleRangeToDefineSphericalSurface, Rng);
                        
            //Source starts from anywhere in the sphere
            Position finalPosition = GetFinalPositionFromProfileType(finalDirection, _radius, Rng);

            //Lambertian distribution (uniform hemispherical distribution)
            PolarAzimuthalAngles polarAzimuthalPair = SourceToolbox.GetPolarAzimuthalPairForGivenAngleRangeRandom(
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                Rng);

            //Avoid updating the finalDirection during following rotation
            Position dummyPosition = finalPosition;

            //Rotate polar azimutahl angle by polarAzimuthalPair vector
            SourceToolbox.UpdateDirectionPositionAfterRotatingByGivenAnglePair(polarAzimuthalPair, ref finalDirection, ref dummyPosition);

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
               

        private static Position GetFinalPositionFromProfileType(Direction finalDirection, double radius, Random rng)
        {
            if (radius == 0.0)
                return new Position(0, 0, 0);
            else
                return new Position(
                    radius * finalDirection.Ux,
                    radius * finalDirection.Uy,
                    radius * finalDirection.Uz);           
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
