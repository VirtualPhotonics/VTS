using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.SourceInputs
{
    /// Implements ISourceInput. Defines input data for DirectionalellipticalSource implementation 
    /// including converging/diverging angle, a and b parameters, source profile, direction, 
    /// position, inward normal beam rotation and initial tissue region index.
    public class DirectionalEllipticalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of the DirectionalEllipticalSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">Converging/diverging angle {= 0, for a collimated beam}</param>
        /// <param name="aParameter">"a" parameter of the ellipse source</param>
        /// <param name="bParameter">"b" parameter of the ellipse source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Tissue region index</param>
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
        /// Initializes a new instance of the DirectionalEllipticalSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">converging/diverging angle {= 0, for a collimated beam}</param>
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
        /// Initializes a new instance of the DirectionalEllipticalSourceInput class
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

        public SourceType SourceType { get; set; }
        public double ThetaConvOrDiv { get; set; }
        public double AParameter { get; set; }
        public double BParameter { get; set; }
        public ISourceProfile SourceProfile { get; set; }
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
        public PolarAzimuthalAngles BeamRotationFromInwardNormal { get; set; }
        public int InitialTissueRegionIndex { get; set; }
    }
}
