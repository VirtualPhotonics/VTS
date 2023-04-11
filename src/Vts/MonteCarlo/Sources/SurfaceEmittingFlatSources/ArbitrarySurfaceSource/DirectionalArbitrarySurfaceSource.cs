using System;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for DirectionalArbitrarySurfaceSource implementation 
    /// including converging/diverging angle, length, width, source profile, direction, position, 
    /// inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class DirectionalArbitrarySurfaceSourceInput : ISourceInput
    {
    /// <summary>
    /// Initializes a new instance of the DirectionalBitmapSourceInput class
    /// </summary>
    /// <param name="inputFolder">Folder where image resides</param>
    /// <param name="imageName">Name of image to be used as source</param>
    /// <param name="numberOfPixelsX">The number of pixels along x-axis of the image source e.g. 1280</param>
    /// <param name="numberOfPixelsY">The number of pixels along y-axis of the image source e.g. 1024</param>
    /// <param name="pixelLengthX">pixel length X</param>
    /// <param name="pixelWidthY">pixel length Y</param>
    /// <param name="sourceProfile">source profile = Arbitrary since image</param>
    /// <param name="thetaConvOrDiv">Convergence or Divergence Angle {= 0, for a collimated beam}</param>
    /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
    /// <param name="translationFromOrigin">New source location</param>
    /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
    /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
    public DirectionalArbitrarySurfaceSourceInput(
        string inputFolder,
        string imageName,
        int numberOfPixelsX,
        int numberOfPixelsY,
        double pixelLengthX,
        double pixelWidthY,
        ISourceProfile sourceProfile,
        double thetaConvOrDiv,
        Direction newDirectionOfPrincipalSourceAxis,
        Position translationFromOrigin,
        PolarAzimuthalAngles beamRotationFromInwardNormal,
        int initialTissueRegionIndex)
    {
        SourceType = "DirectionalArbitrary";
        InputFolder = inputFolder;
        ImageName = imageName;
        NumberOfPixelsX = numberOfPixelsX;
        NumberOfPixelsY = numberOfPixelsY;
        PixelLengthX = pixelLengthX;
        PixelWidthY = pixelWidthY;
        SourceProfile = sourceProfile;
        ThetaConvOrDiv = thetaConvOrDiv;
        NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
        TranslationFromOrigin = translationFromOrigin;
        BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
        InitialTissueRegionIndex = initialTissueRegionIndex;
    }

    /// <summary>
    /// Initializes a new instance of the DirectionalBitmapSourceInput class
    /// </summary>
    /// <param name="inputFolder">Folder where image resides</param>
    /// <param name="imageName">name of image file</param>
    /// <param name="numberOfPixelsX">number of pixels along x-axis (columns) of source image</param>
    /// <param name="numberOfPixelsY">number of pixels along y-axis (rows) </param>
    /// <param name="pixelLengthX"></param>
    /// <param name="pixelWidthY"></param>
    /// <param name="sourceProfile">Source profile = Arbitrary</param>
    public DirectionalArbitrarySurfaceSourceInput(
        string inputFolder,
        string imageName,
        int numberOfPixelsX,
        int numberOfPixelsY,
        double pixelLengthX,
        double pixelWidthY,
        ISourceProfile sourceProfile)
        : this(
            inputFolder,
            imageName,
            numberOfPixelsX,
            numberOfPixelsY,
            pixelLengthX,
            pixelWidthY,
            sourceProfile,
            0.0,
            SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
            SourceDefaults.DefaultPosition.Clone(),
            SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
            0)
    { }

    /// <summary>
    /// Initializes a new instance of the DirectionalBitmapSourceInput class
    /// </summary>
    public DirectionalArbitrarySurfaceSourceInput()
        : this(
            "",
            "",
            1280,
            1024,
            0.003,
            0.003,
            new ArbitrarySourceProfile(),
            0.0,
            SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
            SourceDefaults.DefaultPosition.Clone(),
            SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
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
    public double PixelLengthX { get; set; }
    /// <summary>
    /// The width of the Bitmap Source
    /// </summary>
    public double PixelWidthY { get; set; }
    /// <summary>
    /// Flattened image
    /// </summary>
    public double[] Image { get; set; }
    /// <summary>
    /// Source profile type
    /// </summary>
    public ISourceProfile SourceProfile { get; set; }
    /// <summary>
    /// Convergence or Divergence Angle {= 0, for a collimated beam}
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

            Image = BitmapImageLoader.LinearizeBitmap( // LM: should this be in SourceToolbox or FileIO?
                InputFolder,
                ImageName,
                NumberOfPixelsX,
                NumberOfPixelsY);

            return new DirectionalArbitrarySurfaceSource(
            this.ThetaConvOrDiv,
            this.NumberOfPixelsX * this.PixelLengthX,
            this.NumberOfPixelsY * this.PixelWidthY,
            this.Image,
            this.NumberOfPixelsX,
            this.NumberOfPixelsY,
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
    /// Implements DirectionalArbitrarySurfaceSource with converging/diverging angle, length, width,
    /// source profile, direction, position, inward normal beam rotation and initial tissue 
    /// region index.
    /// </summary>
    // LM: I did now change parameter order from before, let's discuss order
    public class DirectionalArbitrarySurfaceSource : DirectionalRectangularSource
    {
        /// <summary>
        /// Defines DirectionalArbitrarySurfaceSource
        /// </summary>
        /// <param name="thetaConvOrDiv">Convergence(negative angle in radians) or Divergence (positive angle in radians) angle {= 0, for a collimated beam}</param>
        /// <param name="sourceLengthX">The length of the arbitrary (image-based) Source</param>
        /// <param name="sourceWidthY">The width of the arbitrary (image-based) Source</param>
        /// <param name="image">image to be used as source</param>
        /// <param name="pixelLengthX">pixel length X</param>
        /// <param name="pixelWidthY">pixel length Y</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalArbitrarySurfaceSource(
            double thetaConvOrDiv,
            double sourceLengthX,
            double sourceWidthY,
            double[] image,
            int pixelLengthX,
            int pixelWidthY,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
            : base(
                thetaConvOrDiv,
                sourceLengthX,
                sourceWidthY,
                new ArbitrarySourceProfile(image, pixelWidthY, pixelLengthX),
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal,
                initialTissueRegionIndex)
        {
        }
    }
}
