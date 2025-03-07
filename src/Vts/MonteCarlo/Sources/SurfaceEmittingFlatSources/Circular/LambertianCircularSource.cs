using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianCircularSource implementation 
    /// including inner and outer radius, source profile, polar angle range, azimuthal angle 
    /// range, direction, position, inward normal beam rotation and initial tissue region index. 
    /// </summary>
    public class LambertianCircularSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of LambertianCircularSourceInput class
        /// </summary>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        /// <param name="lambertOrder">Lambert order of angular distribution</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Beam rotation from inward normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianCircularSourceInput(
            double outerRadius,
            double innerRadius,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            int lambertOrder,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            SourceType = "LambertianCircular";
            OuterRadius = outerRadius;
            InnerRadius = innerRadius;
            SourceProfile = sourceProfile;
            PolarAngleEmissionRange = polarAngleEmissionRange;
            AzimuthalAngleEmissionRange = azimuthalAngleEmissionRange;
            LambertOrder = lambertOrder;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of LambertianCircularSourceInput class
        /// </summary>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        public LambertianCircularSourceInput(
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
                1,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of LambertianCircularSourceInput class
        /// </summary>
        public LambertianCircularSourceInput()
            : this(
                1.0,
                0.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                1,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
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
        /// Lambert order of the angular distribution
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
        /// <returns>source class that implements ISource</returns>
        public ISource CreateSource(Random rng = null)
        {
            rng ??= new Random();

            return new LambertianCircularSource(
                OuterRadius,
                InnerRadius,
                SourceProfile,
                PolarAngleEmissionRange,
                AzimuthalAngleEmissionRange,
                LambertOrder,
                NewDirectionOfPrincipalSourceAxis,
                TranslationFromOrigin,
                BeamRotationFromInwardNormal,
                InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements LambertianCircularSource with inner and outer radius, source profile, polar angle range, 
    /// azimuthal angle range, direction, position, inward normal beam rotation and initial tissue region
    /// index. 
    /// </summary>
    public class LambertianCircularSource : CircularSourceBase
    {
        private readonly DoubleRange _polarAngleEmissionRange;
        private readonly DoubleRange _azimuthalAngleEmissionRange;
        private readonly int _lambertOrder;

        /// <summary>
        /// Returns an instance of  Lambertian Circular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, new source axis direction, translation, and  inward normal ray rotation
        /// </summary>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="lambertOrder">Lambert order of angular distribution</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param> 
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianCircularSource(            
            double outerRadius,
            double innerRadius,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            int lambertOrder,
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
            _lambertOrder = lambertOrder;
        }
        
        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>  
        protected override Direction GetFinalDirection(Position position)
        {
            return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeLambertianRandom(
                _polarAngleEmissionRange,
                _azimuthalAngleEmissionRange,
                _lambertOrder,
                Rng);
        }
    }

}
