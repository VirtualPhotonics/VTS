using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for LambertianRectangularSource implementation 
    /// including length, width, source profile, polar angle range, azimuthal angle 
    /// range, direction, position, inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class LambertianRectangularSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of LambertianRectangularSourceInput class
        /// </summary>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        /// <param name="lambertOrder">Lambert order of angular distribution</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianRectangularSourceInput(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            int lambertOrder,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            SourceType = "LambertianRectangular";
            RectLengthX = rectLengthX;
            RectWidthY = rectWidthY;
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
        /// Initializes a new instance of LambertianRectangularSourceInput class
        /// </summary>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle range</param>
        public LambertianRectangularSourceInput(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange)
            : this(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                polarAngleEmissionRange,
                azimuthalAngleEmissionRange,
                1,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                0) { }


        /// <summary>
        /// Initializes the default constructor of LambertianRectangularSourceInput class
        /// </summary>
        public LambertianRectangularSourceInput()
            : this(
                1.0,
                2.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                1,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Rectangular source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// The length of the Rectangular Source
        /// </summary>
        public double RectLengthX { get; set; }
        /// <summary>
        /// The width of the Rectangular Source
        /// </summary>
        public double RectWidthY { get; set; }
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
        /// Lambert order of angular distribution
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
        /// <returns>instantiated source</returns>
        public ISource CreateSource(Random rng = null)
        {
            rng ??= new Random();

            return new LambertianRectangularSource(
                RectLengthX,
                RectWidthY,
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
    /// Implements LambertianRectangularSource with length, width, source profile, polar angle range, 
    /// azimuthal angle range, direction, position, inward normal beam rotation and initial tissue
    /// region index.
    /// </summary>
    public class LambertianRectangularSource : RectangularSourceBase
    {
        private readonly DoubleRange _polarAngleEmissionRange;
        private readonly DoubleRange _azimuthalAngleEmissionRange;
        private readonly int _lambertOrder;

        /// <summary>
        /// Returns an instance of Lambertian Rectangular Source with specified length and width, source profile (Flat/Gaussian), 
        /// polar and azimuthal angle range, translation, inward normal rotation, and source axis rotation
        /// </summary>
        /// <param name="rectLengthX">The length of the Rectangular Source</param>
        /// <param name="rectWidthY">The width of the Rectangular Source</param>
        /// <param name="sourceProfile">Source Profile {Flat / Gaussian}</param>
        /// <param name="polarAngleEmissionRange">Polar angle emission range</param>
        /// <param name="azimuthalAngleEmissionRange">Azimuthal angle emission range</param>
        /// <param name="lambertOrder">Lambert order of angular distribution</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public LambertianRectangularSource(
            double rectLengthX,
            double rectWidthY,
            ISourceProfile sourceProfile,
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            int lambertOrder,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null,
            int initialTissueRegionIndex = 0)
            : base(
                rectLengthX,
                rectWidthY,
                sourceProfile,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal,
                initialTissueRegionIndex)
        {
            _lambertOrder = lambertOrder;
            _polarAngleEmissionRange = polarAngleEmissionRange.Clone();
            _azimuthalAngleEmissionRange = azimuthalAngleEmissionRange.Clone();
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

