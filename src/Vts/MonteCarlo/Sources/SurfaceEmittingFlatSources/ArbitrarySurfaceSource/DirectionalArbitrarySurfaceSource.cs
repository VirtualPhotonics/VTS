using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements DirectionalArbitrarySurfaceSource with converging/diverging angle, length, width,
    /// source profile, direction, position, inward normal beam rotation and initial tissue 
    /// region index.
    /// </summary>
    public class DirectionalArbitrarySurfaceSource : DirectionalRectangularSource
    {
        /// <summary>
        /// Defines DirectionalArbitrarySurfaceSource
        /// </summary>
        /// <param name="thetaConvOrDiv"></param>
        /// <param name="sourceLengthX">The length of the arbitrary (image-based) Source</param>
        /// <param name="sourceWidthY">The width of the arbitrary (image-based) Source</param>
        /// <param name="image"></param>
        /// <param name="pixelLengthX">pixel length X</param>
        /// <param name="pixelWidthY">pixel length Y</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        protected DirectionalArbitrarySurfaceSource(
            double thetaConvOrDiv,
            double sourceLengthX,
            double sourceWidthY,
            double[] image,
            int pixelLengthX,
            int pixelWidthY,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
            : base(
                thetaConvOrDiv,
                sourceLengthX,
                sourceWidthY,
                new ArbitrarySourceProfile(image, pixelWidthY, pixelLengthX),
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal,
                initialTissueRegionIndex)
        {
        }
    }
}
