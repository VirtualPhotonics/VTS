using System;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class SphericalSurfaceEmmitingSourceCustom : ISource
    {
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;
        private Position _translationFromOrigin;
        private PolarAzimuthalAngles _mappingOntoSpehere;
        private ThreeAxisRotation _rotationOfPrincipalSourceAxis;
        private double _radius = 1.0;
        private SourceFlags _rotationAndTranslationFlags;

        /// <summary>
        /// Returns an instance of Spherical Surface Emission Source with a specified translation and source axis rotation
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public SphericalSurfaceEmmitingSourceCustom(
            double radius,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin,            
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();           
            _rotationOfPrincipalSourceAxis = rotationOfPrincipalSourceAxis.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);            
        }

        /// <summary>
        /// Returns an instance of Spherical Surface Emission Source with a specified translation
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="translationFromOrigin"></param>
        public SphericalSurfaceEmmitingSourceCustom(
            double radius,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Position translationFromOrigin)
            : this(
                radius,     
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                translationFromOrigin,                
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);      
        }

        /// <summary>
        /// Returns an instance of Spherical Surface Emission Source with a source axis rotation
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public SphericalSurfaceEmmitingSourceCustom(
            double radius,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,  
            ThreeAxisRotation rotationOfPrincipalSourceAxis
            )
            : this(
                radius,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),              
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of Spherical Surface Emission Source with no translation or source axis rotation
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="polarAngleEmissionRange"></param>
        /// <param name="azimuthalAngleEmissionRange"></param>
        public SphericalSurfaceEmmitingSourceCustom(
            double radius,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange)
            : this(
                radius,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                new Position(0, 0, 0),               
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }

   
     
        
        

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in sphere
            Direction finalDirection = SourceToolbox.GetRandomDirectionForIsotropicDistribution(Rng);

            Position finalPosition = new Position(0, 0, 0);
            finalPosition.X = _radius* finalDirection.Ux;
            finalPosition.Y = _radius * finalDirection.Ux;
            finalPosition.Z = _radius * finalDirection.Ux;
             
             
            //Sample polar and azimuthal angle
            PolarAzimuthalAngles polarAzimuthal = SourceToolbox.GetRandomPolarAzimuthalForLambertianPolarAndAzimuthalAngleRange(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);

            //Avoid updating the finalDirection during following rotation
            Position dummyPosition = finalPosition;

            //Rotate polar azimutahl angle by tempdir vector
            SourceToolbox.DoSourceRotationByGivenPolarAndAzimuthalAngle(polarAzimuthal, ref finalDirection, ref dummyPosition);

                       //Rotation and translation
            SourceToolbox.DoRotationandTranslationForGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _translationFromOrigin,               
                _rotationOfPrincipalSourceAxis,
                _rotationAndTranslationFlags);   

            // the handling of specular needs work
            var weight = 1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N);

            var dataPoint = new PhotonDataPoint(
                finalPosition,
                finalDirection,
                weight,
                0.0,
                PhotonStateType.NotSet);

            var photon = new Photon { DP = dataPoint };

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
