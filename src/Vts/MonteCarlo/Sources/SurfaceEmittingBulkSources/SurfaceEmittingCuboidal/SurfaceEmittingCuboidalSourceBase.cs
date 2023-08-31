using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Abstract class for SurfaceEmittingCuboidalSourceBase
    /// </summary>
    public abstract class SurfaceEmittingCuboidalSourceBase : ISource
    {
        /// <summary>
        /// Source profile type
        /// </summary>
        protected ISourceProfile _sourceProfile;
        /// <summary>
        /// Polar angle emission range
        /// </summary>
        protected DoubleRange _polarAngleEmissionRange;
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
        /// The length of the cube (along x axis)
        /// </summary>
        protected double _cubeLengthX;
        /// <summary>
        /// The width of the cube (along y axis)
        /// </summary>
        protected double _cubeWidthY;
        /// <summary>
        /// The height of the cube (along z axis)
        /// </summary>
        protected double _cubeHeightZ;
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        protected int _initialTissueRegionIndex;

        /// <summary>
        /// Defines SurfaceEmittingCuboidalSourceBase class
        /// </summary>
        /// <param name="cubeLengthX">The length of the cube (along x axis)</param>
        /// <param name="cubeWidthY">The width of the cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of the cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range {0 - 90degrees}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>  
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected SurfaceEmittingCuboidalSourceBase(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis,                     
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                 newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                 translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                 false);
            
            _cubeLengthX = cubeLengthX;
            _cubeWidthY = cubeWidthY;
            _cubeHeightZ = cubeHeightZ;
            _sourceProfile = sourceProfile;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();  
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
            //Sample emitting side
            var cSide = SelectEmittingSurface(
                _cubeLengthX, 
                _cubeWidthY, 
                _cubeHeightZ, 
                Rng);

            //sample angular distribution
            var finalDirection = SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                _polarAngleEmissionRange, 
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                Rng);

            Position tempPosition;
            var finalPosition = new Position();               
             
            switch (cSide)
            {
                case "xpos":
                    tempPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeHeightZ, _cubeWidthY, Rng);
                    finalPosition.X = 0.5 * _cubeLengthX;
                    finalPosition.Y = tempPosition.Y;
                    finalPosition.Z = tempPosition.X;
                    finalDirection = SourceToolbox.UpdateDirectionAfterRotatingAroundXAxis(0.5 * Math.PI, finalDirection);
                    break;
                case "xneg":
                    tempPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeHeightZ, _cubeWidthY, Rng);
                    finalPosition.X = -0.5 * _cubeLengthX;
                    finalPosition.Y = tempPosition.Y;
                    finalPosition.Z = tempPosition.X;
                    finalDirection = SourceToolbox.UpdateDirectionAfterRotatingAroundXAxis(-0.5 * Math.PI, finalDirection);
                    break;
                case "ypos":
                    tempPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeLengthX, _cubeHeightZ, Rng);
                    finalPosition.X = tempPosition.X;
                    finalPosition.Y = 0.5 * _cubeWidthY;
                    finalPosition.Z = tempPosition.Y;
                    finalDirection = SourceToolbox.UpdateDirectionAfterRotatingAroundYAxis(0.5 * Math.PI, finalDirection);
                    break;
                case "yneg":
                    tempPosition = GetFinalPositionFromProfileType(_sourceProfile, _cubeLengthX, _cubeHeightZ, Rng);
                    finalPosition.X = tempPosition.X;
                    finalPosition.Y = -0.5 * _cubeWidthY;
                    finalPosition.Z = tempPosition.Y;
                    finalDirection = SourceToolbox.UpdateDirectionAfterRotatingAroundYAxis(-0.5 * Math.PI, finalDirection);
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

            //Find the relevant polar and azimuthal pair for the direction
            var rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);
            
            //Translation and source rotation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
                _rotationAndTranslationFlags);

            var photon = new Photon(finalPosition, finalDirection, 1.0, tissue, _initialTissueRegionIndex, Rng);

            return photon;
        }


        private static Position GetFinalPositionFromProfileType(ISourceProfile sourceProfile, double rectLengthX, double rectWidthY, Random rng)
        {
            Position finalPosition = null;
            switch (sourceProfile.SourceProfileType)
            {
                case SourceProfileType.Flat:
                    finalPosition = SourceToolbox.GetPositionInARectangleRandomFlat(
                        SourceDefaults.DefaultPosition.Clone(),
                        rectLengthX,
                        rectWidthY,
                        rng);
                    break;
                case SourceProfileType.Gaussian:
                    if (sourceProfile is GaussianSourceProfile gaussianProfile)
                        finalPosition = SourceToolbox.GetPositionInARectangleRandomGaussian(
                            SourceDefaults.DefaultPosition.Clone(),
                            0.5 * rectLengthX,
                            0.5 * rectWidthY,
                            gaussianProfile.BeamDiaFWHM,
                            rng);
                    break;
                case SourceProfileType.Image:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(sourceProfile.SourceProfileType.ToString());
            }


            return finalPosition;
        }
        
        /// <summary>
        /// Select the cuboid surface after sampling
        /// </summary>
        /// <param name="lengthX">The length of the cube (along x axis)</param>
        /// <param name="widthY">The width of the cube (along y axis)</param>
        /// <param name="heightZ">The height of the cube (along z axis)</param>
        /// <param name="rng">random number generator</param>
        /// <returns>string indicating emitting surface</returns>
        public static string SelectEmittingSurface(            
            double lengthX,
            double widthY,
            double heightZ,
            Random rng)
        {
            var hw = widthY * heightZ;
            var lh = lengthX * heightZ;
            var lw = lengthX * widthY;
            var temp1 = 2 * (hw + lh + lw) * rng.NextDouble();

            if (temp1 < hw) {return "xpos"; }
            else if (temp1 < 2 * hw) { return "xneg"; }
            else if (temp1 < 2 * hw + lh) { return "ypos"; }
            else if (temp1 < 2 * (hw + lh)) { return "yneg"; }
            else if (temp1 < 2 * (hw + lh) + lw) { return "zpos"; }
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
