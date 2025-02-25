using System;
using System.Runtime.CompilerServices;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// The <see cref="Sources"/> namespace contains the Monte Carlo source classes
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Implements ISourceInput. Defines input data for CustomLineSource implementation 
    /// including line length, source profile, polar angle range, azimuthal angle range, 
    /// direction, position, inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class CustomLineSourceInput : ISourceInput
    {
        /// <summary>
        /// Constructor with the fullest parameter list, no default parameter settings.
        /// </summary>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        /// <param name="sourceAngularDistribution">source angular distribution</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomLineSourceInput(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            SourceAngularDistributionType sourceAngularDistribution,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            SourceType = "CustomLine";
            LineLength = lineLength;
            SourceProfile = sourceProfile;
            PolarAngleEmissionRange = polarAngleEmissionRange;
            AzimuthalAngleEmissionRange = azimuthalAngleEmissionRange;
            SourceAngularDistribution = sourceAngularDistribution;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of CustomLineSourceInput class
        /// with the assumption of an Isotropic angular distribution.
        /// Default parameter settings:
        /// SourceAngularDistribution = Isotropic
        /// </summary>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomLineSourceInput(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            SourceType = "CustomLine";
            LineLength = lineLength;
            SourceProfile = sourceProfile;
            PolarAngleEmissionRange = polarAngleEmissionRange;
            AzimuthalAngleEmissionRange = azimuthalAngleEmissionRange;
            // default angular distribution is Isotropic
            SourceAngularDistribution = SourceAngularDistributionType.Isotropic;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of CustomLineSourceInput class with default
        /// settings for principal source axis direction, translation from origin, and
        /// beam rotation from inward normal
        /// Default parameter settings:
        /// SourceAngularDistribution = Isotropic
        /// TranslationFromOrigin = Position(0,0,0)
        /// BeamRotationFromInwardNorm = (theta=0, phi=0)
        /// InitialTissueRegionIndex = 0 (air above tissue)
        /// </summary>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        public CustomLineSourceInput(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange)
            : this(
                lineLength,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                SourceAngularDistributionType.Isotropic,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of CustomLineSourceInput class
        /// Default parameter settings:
        /// LineLength = 1.0mm
        /// SourceProfile = Flat
        /// PolarAngleEmissionRange = [0, pi]
        /// AzimuthalAngleEmissionRange = [0, 2pi]
        /// SourceAngularDistribution = Isotropic
        /// TranslationFromOrigin = Position(0,0,0)
        /// BeamRotationFromInwardNorm = (theta=0, phi=0)
        /// InitialTissueRegionIndex = 0 (air above tissue)
        /// </summary>
        public CustomLineSourceInput()
            : this(
                1.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultFullPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                SourceAngularDistributionType.Isotropic,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Line source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The length of the line source
        /// </summary>
        public double LineLength { get; set; }
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
        /// Source Angular Distribution
        /// </summary>
        public SourceAngularDistributionType SourceAngularDistribution { get; set; }
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

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <returns>instantiated source</returns>
        public ISource CreateSource(Random rng = null)
        {
            rng ??= new Random();

            return new CustomLineSource(
                        LineLength,
                        SourceProfile,
                        PolarAngleEmissionRange,
                        AzimuthalAngleEmissionRange,
                        SourceAngularDistribution,
                        NewDirectionOfPrincipalSourceAxis,
                        TranslationFromOrigin,
                        BeamRotationFromInwardNormal,
                        InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements CustomLineSource with line length, source profile, polar angle range, azimuthal 
    /// angle range, direction, position, inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class CustomLineSource : LineSourceBase
    {
        private readonly DoubleRange _polarAngleEmissionRange;
        private readonly DoubleRange _azimuthalAngleEmissionRange;
        private readonly SourceAngularDistributionType _sourceAngularDistribution;

        /// <summary>
        /// Initializes a new instance of the CustomLineSource class
        /// </summary>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Beam rotation from inward normal</param>
        /// <param name="sourceAngularDistribution">source angular distribution</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomLineSource(
            double lineLength,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            SourceAngularDistributionType sourceAngularDistribution,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null,
            int initialTissueRegionIndex = 0)
            : base(
                lineLength,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal,
                initialTissueRegionIndex)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
            _sourceAngularDistribution = sourceAngularDistribution;
        }

        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            // Isotropic
            if (_sourceAngularDistribution == SourceAngularDistributionType.Isotropic)
                return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                    _polarAngleEmissionRange,
                    _azimuthalAngleEmissionRange,
                    Rng);
            // Lambertian
            return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeLambertianRandom(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                1,
                Rng);
        }
    }

}
