using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for DirectionalImageSurfaceSource implementation 
    /// including converging/diverging angle, width, height, source profile, direction, position, 
    /// inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class DirectionalImageSourceInput : ISourceInput
    {
    /// <summary>
    /// Initializes a new instance of the DirectionalImageSourceInput class
    /// </summary>
    /// <param name="inputFolder">Folder where image resides</param>
    /// <param name="imageName">Name of image to be used as source</param>
    /// <param name="numberOfPixelsX">The number of pixels along x-axis of the image source e.g. 1280</param>
    /// <param name="numberOfPixelsY">The number of pixels along y-axis of the image source e.g. 1024</param>
    /// <param name="pixelWidthX">pixel length X-axis</param>
    /// <param name="pixelHeightY">pixel length Y-axis</param>
    /// <param name="thetaConvOrDiv">Convergence or Divergence Angle {= 0, for a collimated beam}</param>
    /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
    /// <param name="translationFromOrigin">New source location</param>
    /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
    /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
    public DirectionalImageSourceInput(
        string inputFolder,
        string imageName,
        int numberOfPixelsX,
        int numberOfPixelsY,
        double pixelWidthX,
        double pixelHeightY,
        double thetaConvOrDiv,
        Direction newDirectionOfPrincipalSourceAxis,
        Position translationFromOrigin,
        PolarAzimuthalAngles beamRotationFromInwardNormal,
        int initialTissueRegionIndex)
    {
        SourceType = "DirectionalImage";
        InputFolder = inputFolder;
        ImageName = imageName;
        NumberOfPixelsX = numberOfPixelsX;
        NumberOfPixelsY = numberOfPixelsY;
        PixelWidthX = pixelWidthX;
        PixelHeightY = pixelHeightY;
        ThetaConvOrDiv = thetaConvOrDiv;
        NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
        TranslationFromOrigin = translationFromOrigin;
        BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
        InitialTissueRegionIndex = initialTissueRegionIndex;
    }

    /// <summary>
    /// Initializes a new instance of the DirectionalImageSourceInput class
    /// </summary>
    /// <param name="inputFolder">Folder where image resides</param>
    /// <param name="imageName">name of image file</param>
    /// <param name="numberOfPixelsX">number of pixels along x-axis (columns) of source image</param>
    /// <param name="numberOfPixelsY">number of pixels along y-axis (rows) </param>
    /// <param name="pixelWidthX"></param>
    /// <param name="pixelHeightY"></param>
    public DirectionalImageSourceInput(
        string inputFolder,
        string imageName,
        int numberOfPixelsX,
        int numberOfPixelsY,
        double pixelWidthX,
        double pixelHeightY)
        : this(
            inputFolder,
            imageName,
            numberOfPixelsX,
            numberOfPixelsY,
            pixelWidthX,
            pixelHeightY,
            0.0,
            SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
            SourceDefaults.DefaultPosition.Clone(), 
            SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
            0)
    { }

    /// <summary>
    /// Initializes a new instance of the DirectionalImageSourceInput class
    /// </summary>
    public DirectionalImageSourceInput()
        : this(
            "",
            "",
            1280,
            1024,
            0.1,
            0.1,
            0.0,
            SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
            SourceDefaults.DefaultPosition.Clone(),
            SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
              0)
    { }

    /// <summary>
    /// Bitmap source type
    /// </summary>
    public string SourceType { get; set; }
    /// <summary>
    /// Input folder of bit map
    /// </summary>
    public string InputFolder { get; set; }
    /// <summary>
    /// Name of bitmap image file
    /// </summary>
    public string ImageName { get; set; }
    /// <summary>
    /// The length of the Bitmap Source
    /// </summary>
    public int NumberOfPixelsX { get; set; }
    /// <summary>
    /// The width of the Bitmap Source
    /// </summary>
    public int NumberOfPixelsY { get; set; }
    /// <summary>
    /// The length of the Bitmap Source
    /// </summary>
    public double PixelWidthX { get; set; }
    /// <summary>
    /// The width of the Bitmap Source
    /// </summary>
    public double PixelHeightY { get; set; }
    /// <summary>
    /// Source profile type
    /// </summary>
    [IgnoreDataMember]
    public ISourceProfile SourceProfile { get; set; }
    /// <summary>
    /// Convergence or Divergence Angle {= 0, for a normal beam}
    /// </summary>
    public double ThetaConvOrDiv { get; set; }
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
        rng = rng ?? new Random();

        var image = ImageDataLoader.ReadAndFlattenCsv(  
            InputFolder,
            ImageName,
            NumberOfPixelsX,
            NumberOfPixelsY);

        // instantiate image, pdf and cdf to use to select new Photon
        SourceProfile = new ImageSourceProfile(
            image, 
            NumberOfPixelsX, 
            NumberOfPixelsY,
            PixelWidthX,
            PixelHeightY,
            TranslationFromOrigin
        );

        return new DirectionalImageSource(
            this.ThetaConvOrDiv,
            this.NumberOfPixelsX * this.PixelWidthX,
            this.NumberOfPixelsY * this.PixelHeightY,
            this.SourceProfile,
            this.NewDirectionOfPrincipalSourceAxis,
            this.TranslationFromOrigin,
            this.BeamRotationFromInwardNormal,
            this.InitialTissueRegionIndex)
            {
                Rng = rng
            };
    }
}

    /// <summary>
    /// Implements DirectionalImageSurfaceSource with converging/diverging angle, length, width,
    /// source profile, direction, position, inward normal beam rotation and initial tissue 
    /// region index.
    /// </summary>
    public class DirectionalImageSource : RectangularSourceBase
    {
        private readonly double _thetaConvOrDiv;  //convergence:positive, divergence:negative, collimated:zero

        /// <summary>
        /// Defines DirectionalImageSource
        /// </summary>
        /// <param name="thetaConvOrDiv">Convergence(negative angle in radians) or Divergence (positive angle in radians) angle {= 0, for a collimated beam}</param>
        /// <param name="sourceWidthX">The width of image source</param>
        /// <param name="sourceHeightY">The height of image source</param>
        /// <param name="sourceProfile">ISourceProfile type = Image</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">null not used for this source</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalImageSource(
            double thetaConvOrDiv,
            double sourceWidthX,
            double sourceHeightY,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null,
            int initialTissueRegionIndex = 0)
            : base(
                sourceWidthX,
                sourceHeightY,
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
            if (_rectLengthX == 0.0 && _rectWidthY == 0.0)
            {
                return SourceToolbox.GetDirectionForGivenPolarAzimuthalAngleRangeRandom(
                    new DoubleRange(0.0, Math.Abs(_thetaConvOrDiv)),
                    SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                    Rng);
            }

            // sign is negative for diverging and positive positive for converging 
            var polarAngle = SourceToolbox.UpdatePolarAngleForDirectionalSources(
                _rectLengthX,
                Math.Sqrt(position.X * position.X + position.Y * position.Y),
                _thetaConvOrDiv);
            return SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(polarAngle, position);
        }
    }
}
