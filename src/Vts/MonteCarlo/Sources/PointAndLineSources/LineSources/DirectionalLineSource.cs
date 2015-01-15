using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{

    /// <summary>
    /// Implements ISourceInput. Defines input data for DirectionalLineSource implementation 
    /// including converging/diverging angle, line length, source profile, direction, position, 
    /// inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class DirectionalLineSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of the DirectionalLineSourceInput class
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam} {= 0, for a collimated beam}</param>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalLineSourceInput(
            double thetaConvOrDiv,
            double lineLength,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            SourceType = "DirectionalLine";
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
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
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

        /// <summary>
        /// Covergence or Divergance Angle {= 0, for a collimated beam}
        /// </summary>
        public double ThetaConvOrDiv { get; set; }
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

            return new DirectionalLineSource(
                this.ThetaConvOrDiv,
                this.LineLength,
                this.SourceProfile,
                this.NewDirectionOfPrincipalSourceAxis,
                this.TranslationFromOrigin,
                this.BeamRotationFromInwardNormal,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements DirectionalLineSource. Returns DirectionalLineSource with converging/diverging angle,
    /// line length, source profile, direction, position, inward normal beam rotation and initial tissue
    /// region index.
    /// </summary>
    public class DirectionalLineSource : LineSourceBase
    {
        private double _thetaConvOrDiv;   //convergence:positive, divergence:negative  collimated:zero
               
        /// <summary>
        /// Initializes a new instance of the DirectionalLineSource class
        /// </summary>
        /// <param name="thetaConvOrDiv">Covergence or Divergance Angle {= 0, for a collimated beam}</param>
        /// <param name="lineLength">The length of the line source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">Ray rotation from inward normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalLineSource(
            double thetaConvOrDiv,
            double lineLength,
            ISourceProfile sourceProfile,
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
            _thetaConvOrDiv = thetaConvOrDiv;                     
        }
               
        /// <summary>
        /// Returns direction for a given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>new direction</returns>       
        protected override Direction GetFinalDirection(Position position)
        {
            if (_lineLength == 0.0)
                return (SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                            new DoubleRange(0.0, Math.Abs(_thetaConvOrDiv)),
                            SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                            Rng));
            else
            {   
                // sign is negative for diverging and positive positive for converging 
                var polarAngle = SourceToolbox.UpdatePolarAngleForDirectionalSources(
                    0.5 * _lineLength,
                    position.X,
                    _thetaConvOrDiv);           
                return (SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(polarAngle, position));
            }
        }


    }
}
