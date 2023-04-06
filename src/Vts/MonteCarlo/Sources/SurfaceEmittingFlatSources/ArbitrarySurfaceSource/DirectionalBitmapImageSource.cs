using System;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for DirectionalBitmapImageSource implementation 
    /// including converging/diverging angle, length, width, source profile, direction, position, 
    /// inward normal beam rotation and initial tissue region index.
    /// </summary>
    public class DirectionalBitmapImageSourceInput : ISourceInput
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
        /// <param name="thetaConvOrDiv">Convergence or Divergence Angle {= 0, for a collimated beam}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalBitmapImageSourceInput(
            string inputFolder,
            string imageName,
            int numberOfPixelsX, 
            int numberOfPixelsY,
            double pixelLengthX,
            double pixelWidthY,
            double thetaConvOrDiv,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal,
            int initialTissueRegionIndex)
        {
            SourceType = "DirectionalBitmapImage";
            InputFolder = inputFolder;
            ImageName = imageName;
            NumberOfPixelsX = numberOfPixelsX;
            NumberOfPixelsY = numberOfPixelsY;
            PixelLengthX = pixelLengthX;
            PixelWidthY = pixelWidthY;
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
        public DirectionalBitmapImageSourceInput(
            string inputFolder,
            string imageName,
            int numberOfPixelsX,
            int numberOfPixelsY,
            double pixelLengthX,
            double pixelWidthY)
            : this (
                inputFolder,
                imageName,
                numberOfPixelsX,
                numberOfPixelsY, 
                pixelLengthX,
                pixelWidthY,
                0.0,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

        /// <summary>
        /// Initializes a new instance of the DirectionalBitmapSourceInput class
        /// </summary>
        public DirectionalBitmapImageSourceInput()
            : this(
                "",
                "",
                1280,
                1024,
                0.003,
                0.003,
                0.0,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0) { }

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

            return new DirectionalBitmapImageSource(
                this.InputFolder,
                this.ImageName,
                this.NumberOfPixelsX,
                this.NumberOfPixelsY,
                this.PixelLengthX,
                this.PixelWidthY,
                this.ThetaConvOrDiv,
                this.NewDirectionOfPrincipalSourceAxis,
                this.TranslationFromOrigin,
                this.BeamRotationFromInwardNormal,
                this.InitialTissueRegionIndex) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements DirectionalBitmapSource with converging/diverging angle, length, width,
    /// source profile, direction, position, inward normal beam rotation and initial tissue 
    /// region index.
    /// </summary>
    public class DirectionalBitmapImageSource : RectangularSourceBase
    {
      private readonly double _thetaConvOrDiv;  //convergence:positive, divergence:negative, collimated:zero

        /// <summary>
        /// Returns an instance of directional (diverging/converging/collimated) Bitmap Source with specified length and width, 
        /// polar and azimuthal angle range, new source axis direction, translation, and  inward normal ray rotation
        /// </summary>
        /// <param name="inputFolder">Folder where image resides</param>
        /// <param name="imageName">Name of image to be used as source</param>
        /// <param name="numberOfPixelsX">The number of pixels along x-axis of the image source e.g. 1280</param>
        /// <param name="numberOfPixelsY">The number of pixels along y-axis of the image source e.g. 1024</param>
        /// <param name="pixelLengthX">pixel length X</param>
        /// <param name="pixelWidthY">pixel length Y</param>
        /// <param name="thetaConvOrDiv">Convergence or Divergence Angle {= 0, for a collimated beam}</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>    
        /// <param name="beamRotationFromInwardNormal">Polar Azimuthal Rotational Angle of inward Normal</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalBitmapImageSource(
            string inputFolder,
            string imageName,
            int numberOfPixelsX,
            int numberOfPixelsY,
            double pixelLengthX,
            double pixelWidthY,
            double thetaConvOrDiv,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null,
            int initialTissueRegionIndex = 0)
            : base(
                inputFolder,
                imageName,
                numberOfPixelsX,
                numberOfPixelsY,
                pixelLengthX,
                pixelWidthY,
                thetaConvOrDiv,
                newDirectionOfPrincipalSourceAxis,
                translationFromOrigin,
                beamRotationFromInwardNormal,
                initialTissueRegionIndex)
        {
            Image = new BitmapImageLoader(inputFolder, imageName, numberOfPixelsX, numberOfPixelsY, pixelLengthX, pixelWidthY);
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
            return (SourceToolbox.GetDirectionForGiven2DPositionAndGivenPolarAngle(polarAngle, position));
        }
    }
}
