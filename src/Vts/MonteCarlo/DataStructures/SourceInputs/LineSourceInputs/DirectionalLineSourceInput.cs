using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.SourceInputs
{
    /// Implements ISourceInput. Defines input data for DirectionalLineSource implementation 
    /// including converging/diverging angle, line length, source profile, direction, position, 
    /// inward normal beam rotation and initial tissue region index.
    public class DirectionalLineSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of the DirectionalLineSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">converging/diverging angle {= 0, for a collimated beam}</param>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Tissue region index</param>
        public DirectionalLineSourceInput(
            double thetaConvOrDiv,
            double lineLength,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex) 
        {
            SourceType = SourceType.DirectionalLine;
            ThetaConvOrDiv = thetaConvOrDiv;
            LineLength = lineLength;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of the DirectionalLineSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">converging/diverging angle {= 0, for a collimated beam}</param>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        public DirectionalLineSourceInput(
            double thetaConvOrDiv,
            double lineLength,
            ISourceProfile sourceProfile)
            : this(
                thetaConvOrDiv,
                lineLength,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Initializes a new instance of the DirectionalLineSourceInput class
        /// </summary>
        public DirectionalLineSourceInput()
            : this(
                0.0,
                1.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        public SourceType SourceType { get; set; }
        public double ThetaConvOrDiv { get; set; }
        public double LineLength { get; set; }
        public ISourceProfile SourceProfile { get; set; }
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
        public PolarAzimuthalAngles BeamRotationFromInwardNormal { get; set; }
        public int InitialTissueRegionIndex { get; set; }
    }
}
