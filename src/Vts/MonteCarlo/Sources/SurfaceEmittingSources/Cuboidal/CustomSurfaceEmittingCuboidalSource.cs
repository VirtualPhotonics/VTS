using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomSurfaceEmittingCuboidalSource : SurfaceEmittingCuboidalSourceBase
    {        
        /// <summary>
        /// Returns an instance of Custom Surface Emitting Cuboidal Source with a given source profile, polar angle range,
        /// new source axis direction, and translation,
        /// </summary>
        /// <param name="cubeLengthX">The length of cube (along x axis)</param>
        /// <param name="cubeWidthY">The  width of cube (along y axis)</param>
        /// <param name="cubeHeightZ">The height of cube (along z axis)</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range {0 - 90degrees}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>  
        public CustomSurfaceEmittingCuboidalSource(
            double cubeLengthX,
            double cubeWidthY,
            double cubeHeightZ,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null)
            : base(
            cubeLengthX,
            cubeWidthY, 
            cubeHeightZ,
            sourceProfile, 
            polarAngleEmissionRange,
            newDirectionOfPrincipalSourceAxis,
            translationFromOrigin)
        {
            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis;
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition;
        }        
    }
}
