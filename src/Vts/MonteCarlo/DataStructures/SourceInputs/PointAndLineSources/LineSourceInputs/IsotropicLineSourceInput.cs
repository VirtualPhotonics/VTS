using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for IsotropicLineSource implementation 
    /// including line length, source profile, direction, position, inward normal beam 
    /// rotation and initial tissue region index.
    /// </summary>
    public class IsotropicLineSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of IsotropicLineSourceInput class
        /// </summary>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicLineSourceInput(
            double lineLength,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            SourceType = SourceType.IsotropicLine;
            LineLength = lineLength;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of IsotropicLineSourceInput class
        /// </summary>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        public IsotropicLineSourceInput(
            double lineLength,
            ISourceProfile sourceProfile)
            : this(
                lineLength,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of IsotropicLineSourceInput class
        /// </summary>
        public IsotropicLineSourceInput()
            : this(
                1.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(), 
                0) { }

        /// <summary>
        /// Line source type
        /// </summary>
        public SourceType SourceType { get; set; }
        /// <summary>
        /// The length of the line source
        /// </summary>
        public double LineLength { get; set; }
        /// <summary>
        /// Source profile type
        /// </summary>
        public ISourceProfile SourceProfile { get; set; }
        /// <summary>
        /// New source axis direction
        /// </summary>
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        /// <summary>
        /// New source location
        /// </summary>
        public Position TranslationFromOrigin { get; set; }
        /// <summary>
        /// Beam rotation from inward normal
        /// </summary>
        public PolarAzimuthalAngles BeamRotationFromInwardNormal { get; set; }
        /// <summary>
        /// Initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }
    }
}