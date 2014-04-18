using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for DirectionalellipticalSource implementation 
    /// including converging/diverging angle, a and b parameters, source profile, direction, 
    /// position, inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class DirectionalEllipticalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of DirectionalEllipticalSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="aParameter">"a" parameter of the ellipse source</param>
        /// <param name="bParameter">"b" parameter of the ellipse source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalEllipticalSourceInput(
            double thetaConvOrDiv,
            double aParameter,
            double bParameter,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex) 
        {
            SourceType = SourceType.DirectionalElliptical;
            ThetaConvOrDiv = thetaConvOrDiv;
            AParameter = aParameter;
            BParameter = bParameter;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of DirectionalEllipticalSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="aParameter">"a" parameter of the ellipse source</param>
        /// <param name="bParameter">"b" parameter of the ellipse source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        public DirectionalEllipticalSourceInput(
            double thetaConvOrDiv,
            double aParameter,
            double bParameter,
            ISourceProfile sourceProfile)
            : this(
                thetaConvOrDiv,
                aParameter,
                bParameter,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }
        
        /// <summary>
        /// Initializes the default constructor of DirectionalEllipticalSourceInput class
        /// </summary>
        public DirectionalEllipticalSourceInput()
            : this(
                0.0,
                1.0,
                2.0,
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
        /// Elliptical source type
        /// </summary>
        public SourceType SourceType { get; set; }
        /// <summary>
        /// "a" parameter of the ellipse source
        /// </summary>
        public double AParameter { get; set; }
        /// <summary>
        /// "b" parameter of the ellipse source
        /// </summary>
        public double BParameter { get; set; }
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
