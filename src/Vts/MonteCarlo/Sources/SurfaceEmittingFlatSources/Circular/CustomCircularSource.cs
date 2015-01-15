using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
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
            SourceType = "CustomCircular";
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
        public string SourceType { get; set; }
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

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        public ISource CreateSource(Random rng = null)
        {
            rng = rng ?? new Random();

            return new CustomCircularSource(
                this.OuterRadius,
                this.InnerRadius,
                this.SourceProfile,
                this.PolarAngleEmissionRange,
                this.AzimuthalAngleEmissionRange,
                this.NewDirectionOfPrincipalSourceAxis,
                this.TranslationFromOrigin,
                this.BeamRotationFromInwardNormal,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements CustomCircularSource with inner and outer radius, source profile, polar angle range, 
    /// azimuthal angle range, direction, position, inward normal beam rotation and initial tissue region
    /// index. 
    /// </summary>
    public class CustomCircularSource : CircularSourceBase
    {
        private DoubleRange _polarAngleEmissionRange;
        private DoubleRange _azimuthalAngleEmissionRange;

        /// <summary>
        /// Returns an instance of  Custom Circular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, new source axis direction, translation, and  inward normal ray rotation
        /// </summary>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param> 
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public CustomCircularSource(            
            double outerRadius,
            double innerRadius,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null,
            int initialTissueRegionIndex = 0)
            : base(                
                outerRadius,
                innerRadius,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal,
                initialTissueRegionIndex)
        {
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();

            if (newDirectionOfPrincipalSourceAxis == null)
                newDirectionOfPrincipalSourceAxis = SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone();
            if (translationFromOrigin == null)
                translationFromOrigin = SourceDefaults.DefaultPosition.Clone();
            if (beamRotationFromInwardNormal == null)
                beamRotationFromInwardNormal = SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone();
        }
        
        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                Rng);
        }
    }

}
