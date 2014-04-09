using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for DirectionalCircularSource implementation 
    /// including converging/diverging angle, inner and outer radius, source profile, direction,
    /// position, inward normal beam rotation and initial tissue region index.
    /// </summary>
    [KnownType(typeof(FlatSourceProfile))]
    [KnownType(typeof(GaussianSourceProfile))]
    public class DirectionalCircularSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of DirectionalCircularSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalCircularSourceInput(
            double thetaConvOrDiv,            
            double outerRadius,
            double innerRadius,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex) 
        {
            SourceType = SourceType.DirectionalCircular;
            ThetaConvOrDiv = thetaConvOrDiv;
            OuterRadius = outerRadius;
            InnerRadius = innerRadius;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of DirectionalCircularSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="outerRadius">The outer radius of the circular source</param>
        /// <param name="innerRadius">The inner radius of the circular source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        public DirectionalCircularSourceInput(
            double thetaConvOrDiv,
            double outerRadius,
            double innerRadius,
            ISourceProfile sourceProfile)
            : this(
                thetaConvOrDiv,
                outerRadius,
                innerRadius,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Initializes the default constructor of DirectionalCircularSourceInput class
        /// </summary>
        public DirectionalCircularSourceInput()            
            : this(
                0.0,
                1.0,
                0.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Covergence or Divergance Angle {= 0, for a collimated beam}
        /// </summary>
        public double ThetaConvOrDiv { get; set; }
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
