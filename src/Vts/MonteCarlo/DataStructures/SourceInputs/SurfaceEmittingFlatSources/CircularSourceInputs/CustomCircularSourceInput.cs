using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for CustomCircularSource implementation 
    /// including inner and outer radius, source profile, polar angle range, azimuthal angle 
    /// range, direction, position, inward normal beam rotation and initial tissue region index. 
    /// </summary>
    public class CustomCircularSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of CustomCircularSourceInput class
        /// </summary>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Beam rotation from inward normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomCircularSourceInput(
            double outerRadius,
            double innerRadius,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            SourceType = SourceType.CustomCircular;
            OuterRadius = outerRadius;
            InnerRadius = innerRadius;
            SourceProfile = sourceProfile;
            PolarAngleEmissionRange = polarAngleEmissionRange;
            AzimuthalAngleEmissionRange = azimuthalAngleEmissionRange;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of CustomCircularSourceInput class
        /// </summary>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        public CustomCircularSourceInput(
            double outerRadius,
            double innerRadius,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange)
            : this(
                outerRadius,
                innerRadius,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of CustomCircularSourceInput class
        /// </summary>
        public CustomCircularSourceInput()            
            : this(
                1.0,
                0.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Circular source type
        /// </summary>
        public SourceType SourceType { get; set; }
        /// <summary>
        /// The outer radius of the circular source
        /// </summary>
        public double OuterRadius { get; set; }
        /// <summary>
        /// The inner radius of the circular source
        /// </summary>
        public double InnerRadius { get; set; }
        /// <summary>
        /// Source profile type
        /// </summary>
        public ISourceProfile SourceProfile { get; set; }
        /// <summary>
        /// Polar angle range
        /// </summary>
        public DoubleRange PolarAngleEmissionRange { get; set; }
        /// <summary>
        /// Azimuthal angle range
        /// </summary>
        public DoubleRange AzimuthalAngleEmissionRange { get; set; }
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