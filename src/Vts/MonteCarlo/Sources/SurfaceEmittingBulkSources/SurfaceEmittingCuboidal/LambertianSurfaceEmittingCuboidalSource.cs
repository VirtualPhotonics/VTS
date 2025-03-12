﻿using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianSurfaceEmittingCuboidalSource 
    /// implementation including length, width, height, source profile, direction, position, and
    /// initial tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingCuboidalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingCuboidalSourceInput class
        /// </summary>
        /// <param name="cubeLengthX">The length of the cube (along x axis)</param>
        /// <param name="cubeWidthY">The width of the cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of the cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="lambertOrder">Lambert order of angular distribution</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            int lambertOrder,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "LambertianSurfaceEmittingCuboidal";
            CubeLengthX = cubeLengthX;
            CubeWidthY = cubeWidthY;
            CubeHeightZ = cubeHeightZ;
            SourceProfile = sourceProfile;
            LambertOrder = lambertOrder;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingCuboidalSourceInput
        /// class assuming Lambert order=1
        /// </summary>
        /// <param name="cubeLengthX">The length of the cube (along x axis)</param>
        /// <param name="cubeWidthY">The width of the cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of the cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = "LambertianSurfaceEmittingCuboidal";
            CubeLengthX = cubeLengthX;
            CubeWidthY = cubeWidthY;
            CubeHeightZ = cubeHeightZ;
            SourceProfile = sourceProfile;
            LambertOrder = 1;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of LambertianSurfaceEmittingCuboidalSourceInput class
        /// </summary>
        /// <param name="cubeLengthX">length</param>
        /// <param name="cubeWidthY">width</param>
        /// <param name="cubeHeightZ">height</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        public LambertianSurfaceEmittingCuboidalSourceInput(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile)
            : this(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of LambertianSurfaceEmittingCuboidalSourceInput class
        /// </summary>
        public LambertianSurfaceEmittingCuboidalSourceInput()
            : this(
                1.0,
                1.0,
                1.0,
                new FlatSourceProfile(),
                1,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Surface Emitting Cuboidal source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The length of the cube (along x axis)
        /// </summary>
        public double CubeLengthX { get; set; }
        /// <summary>
        /// The width of the cube (along y axis)
        /// </summary>
        public double CubeWidthY { get; set; }
        /// <summary>
        /// The height of the cube (along z axis)
        /// </summary>
        public double CubeHeightZ { get; set; }
        /// <summary>
        /// Source profile type
        /// </summary>
        public ISourceProfile SourceProfile { get; set; }
        /// <summary>
        /// Lambert order of angular distribution
        /// </summary>
        public int LambertOrder { get; set; }
        /// <summary>
        /// New source axis direction
        /// </summary>
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        /// <summary>
        /// New source location
        /// </summary>
        public Position TranslationFromOrigin { get; set; }
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <returns>instantiated source</returns>
        public ISource CreateSource(Random rng = null)
        {
            rng ??= new Random();

            return new LambertianSurfaceEmittingCuboidalSource(
                CubeLengthX,
                CubeWidthY,
                CubeHeightZ,
                SourceProfile,
                LambertOrder,
                NewDirectionOfPrincipalSourceAxis,
                TranslationFromOrigin,
                InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements LambertianSurfaceEmittingCuboidalSource with length, width, height, source profile, 
    /// direction, position, and initial tissue region index.
    /// </summary>
    public class LambertianSurfaceEmittingCuboidalSource : SurfaceEmittingCuboidalSourceBase
    {
        private readonly int _lambertOrder;

        /// <summary>
        /// Returns an instance of Lambertian Surface Emitting Cuboidal Source with a given source profile
        /// new source axis direction, and translation,
        /// </summary>
        /// <param name="cubeLengthX">The length of the cube (along x axis)</param>
        /// <param name="cubeWidthY">The width of the cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of the cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="lambertOrder">Lambert order of angular distribution</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>       
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile, 
            int lambertOrder,
            Direction newDirectionOfPrincipalSourceAxis = null, 
            Position translationFromOrigin = null,
            int initialTissueRegionIndex = 0)
            : base(
            cubeLengthX,
            cubeWidthY,
            cubeHeightZ,
            sourceProfile,
            SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
            newDirectionOfPrincipalSourceAxis,
            translationFromOrigin,
            initialTissueRegionIndex)
        {
            _lambertOrder = lambertOrder;
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            //Sample angular distribution with full range of theta and phi
            var finalDirection = SourceToolbox.GetDirectionForLambertianRandom(_lambertOrder, Rng);

            return finalDirection;
        }
    }
}
