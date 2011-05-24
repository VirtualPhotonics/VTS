using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class SurfaceEmittingCuboidalSourceBase : ISource
    {
        protected ISourceProfile _sourceProfile;
        protected DoubleRange _polarAngleEmissionRange;
        protected Direction _newDirectionOfPrincipalSourceAxis;
        protected Position _translationFromOrigin;
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _cubeLengthX;
        protected double _cubeWidthY;
        protected double _cubeHeightZ;

        protected SurfaceEmittingCuboidalSourceBase(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis,                     
            Position translationFromOrigin)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                 newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis,
                 translationFromOrigin != SourceDefaults.DefaultPosition,
                 false);
            
            _cubeLengthX = cubeLengthX;
            _cubeWidthY = cubeWidthY;
            _cubeHeightZ = cubeHeightZ;
            _sourceProfile = sourceProfile;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();  
            _translationFromOrigin = translationFromOrigin.Clone();           
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Sample emitting side
            String cSide = SelectEmittingSurface(
                _cubeLengthX, 
                _cubeWidthY, 
                _cubeHeightZ, 
                Rng);

            //sample angular distribution
            Direction finalDirection = SourceToolbox.GetRandomDirectionForPolarAndAzimuthalAngleRange(
                _polarAngleEmissionRange, 
                SourceDefaults.DefaultAzimuthalAngleRange,
                Rng);

            Position tempPosition = null;               
            Position finalPosition = null;               
             
            switch (cSide)
            {
                case "xpos":
                    tempPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeHeightZ, _cubeWidthY, Rng);
                    finalPosition.X = 0.5 * _cubeLengthX;
                    finalPosition.Y = tempPosition.Y;
                    finalPosition.Z = tempPosition.X;
                    finalDirection = SourceToolbox.GetDirectionAfterRotationAroundXAxis(0.5 * Math.PI, finalDirection);
                    break;
                case "xneg":
                    tempPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeHeightZ, _cubeWidthY, Rng);
                    finalPosition.X = -0.5 * _cubeLengthX;
                    finalPosition.Y = tempPosition.Y;
                    finalPosition.Z = tempPosition.X;
                    finalDirection = SourceToolbox.GetDirectionAfterRotationAroundXAxis(-0.5 * Math.PI, finalDirection);
                    break;
                case "ypos":
                    tempPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeLengthX, _cubeHeightZ, Rng);
                    finalPosition.X = tempPosition.X;
                    finalPosition.Y = 0.5 * _cubeWidthY;
                    finalPosition.Z = tempPosition.Y;
                    finalDirection = SourceToolbox.GetDirectionAfterRotationAroundYAxis(0.5 * Math.PI, finalDirection);
                    break;
                case "yneg":
                    tempPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeLengthX, _cubeHeightZ, Rng);
                    finalPosition.X = tempPosition.X;
                    finalPosition.Y = -0.5 * _cubeWidthY;
                    finalPosition.Z = tempPosition.Y;
                    finalDirection = SourceToolbox.GetDirectionAfterRotationAroundYAxis(-0.5 * Math.PI, finalDirection);
                    break;
                case "zpos":
                    tempPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeLengthX, _cubeWidthY, Rng);
                    finalPosition.X = tempPosition.X;
                    finalPosition.Y = tempPosition.Y;
                    finalPosition.Z = 0.5 * _cubeHeightZ;                    
                    break;
                case "zneg":
                    tempPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeLengthX, _cubeWidthY, Rng);
                    finalPosition.X = tempPosition.X;
                    finalPosition.Y = tempPosition.Y;
                    finalPosition.Z = -0.5 * _cubeHeightZ;
                    //reverse signs                    
                    finalDirection.Uz = -finalDirection.Uz;
                    break;
            }

            //Find the relevent polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAndAzimuthalAnglesFromDirection(_newDirectionOfPrincipalSourceAxis);
            
            //Translation and source rotation
            SourceToolbox.UpdateDirectionAndPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
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


        private static Position GetFinalPositionFromProfileType(ISourceProfile sourceProfile, double rectLengthX, double rectWidthY, Random rng)
        {
            Position finalPosition = null;
            switch (sourceProfile.ProfileType)
            {
                case SourceProfileType.Flat:
                    // var flatProfile = sourceProfile as FlatSourceProfile;
                    SourceToolbox.GetRandomFlatRectangulePosition(
                        SourceDefaults.DefaultPosition,
                        rectLengthX,
                        rectWidthY,
                        rng);
                    break;
                case SourceProfileType.Gaussian:
                    var gaussianProfile = sourceProfile as GaussianSourceProfile;
                    finalPosition = SourceToolbox.GetRandomGaussianRectangulePosition(
                        SourceDefaults.DefaultPosition,
                        rectLengthX,
                        rectWidthY,
                        gaussianProfile.BeamDiaFWHM,
                        rng);
                    break;
            }


            return finalPosition;
        }


        /// <summary>
        /// Select the cuboid surface after sampling
        /// </summary>
        /// <param name="lengthX"></param>
        /// <param name="widthY"></param>
        /// <param name="heightZ"></param>
        /// <param name="rng"></param>
        public static String SelectEmittingSurface(            
            double lengthX,
            double widthY,
            double heightZ,
            Random rng)
        {
            double hw = widthY * heightZ;
            double lh = lengthX * heightZ;
            double lw = lengthX * widthY;
            double temp1 = 2 * (hw + lh + lw) * rng.NextDouble();

            if (temp1 < hw) {return "xpos"; }
            else if (temp1 < (2 * hw)) { return "xneg"; }
            else if (temp1 < (2 * hw + lh)) { return "ypos"; }
            else if (temp1 < (2 * (hw + lh))) { return "yneg"; }
            else if (temp1 < (2 * (hw + lh) + lw)) { return "zpos"; }
            else { return "zneg"; }
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
