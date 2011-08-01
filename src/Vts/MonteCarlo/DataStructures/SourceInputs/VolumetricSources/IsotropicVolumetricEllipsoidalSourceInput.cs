using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.SourceInputs
{
    /// Implements ISourceInput. Defines input data for IsotropicVolumetricEllipsoidalSource
    /// implementation including a,b and c parameters, source profile, direction, position, 
    /// and initial tissue region index.
    public class IsotropicVolumetricEllipsoidalSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of the IsotropicVolumetricEllipsoidalSourceInput class
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipsoid source</param>
        /// <param name="bParameter">"b" parameter of the ellipsoid source</param>
        /// <param name="cParameter">"c" parameter of the ellipsoid source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public IsotropicVolumetricEllipsoidalSourceInput(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            SourceType = SourceType.IsotropicVolumetricEllipsoidal;
            AParameter = aParameter;
            BParameter = bParameter;
            CParameter = cParameter;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            InitialTissueRegionIndex = initialTissueRegionIndex;
        }

        /// <summary>
        /// Initializes a new instance of the IsotropicVolumetricEllipsoidalSourceInput class
        /// </summary>
        /// <param name="aParameter">"a" parameter of the ellipsoid source</param>
        /// <param name="bParameter">"b" parameter of the ellipsoid source</param>
        /// <param name="cParameter">"c" parameter of the ellipsoid source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        public IsotropicVolumetricEllipsoidalSourceInput(
            double aParameter,
            double bParameter,
            double cParameter,
            ISourceProfile sourceProfile)
            : this(
                aParameter,
                bParameter,
                cParameter,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }
        
        /// <summary>
        /// Initializes a new instance of the IsotropicVolumetricEllipsoidalSourceInput class
        /// </summary>
        public IsotropicVolumetricEllipsoidalSourceInput()
            : this(
                1.0,
                1.0,
                2.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0) { }

        /// <summary>
        /// Ellipsoidal source type
        /// </summary>
        public SourceType SourceType { get; set; }
        /// <summary>
        /// "a" parameter of the ellipsoid source
        /// </summary>
        public double AParameter { get; set; }
        /// <summary>
        /// "b" parameter of the ellipsoid source
        /// </summary>
        public double BParameter { get; set; }
        /// <summary>
        /// "c" parameter of the ellipsoid source
        /// </summary>
        public double CParameter { get; set; }
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
        /// Initial tissue region index
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }
    }
}