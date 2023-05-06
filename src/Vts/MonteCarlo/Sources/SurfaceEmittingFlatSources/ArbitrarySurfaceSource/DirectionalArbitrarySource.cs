using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for DirectionalArbitrarySurfaceSource implementation 
    /// including converging/diverging angle, width, height, source profile, direction, position, 
    /// inward normal beam rotation and initial tissue region index.
    /// </summary>
//    public class DirectionalArbitrarySourceInput : ISourceInput
//    {
//    /// <summary>
//    /// Initializes a new instance of the DirectionalArbitrarySourceInput class
//    /// </summary>
//    /// <param name="thetaConvOrDiv">Convergence or Divergence Angle {= 0, for a collimated beam}</param>
//    /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
//    /// <param name="translationFromOrigin">New source location</param>
//    /// <param name="beamRotationFromInwardNormal">beam rotation angle</param>
//    /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
//    public DirectionalArbitrarySourceInput(
//        double thetaConvOrDiv,
//        Direction newDirectionOfPrincipalSourceAxis,
//        Position translationFromOrigin,
//        PolarAzimuthalAngles beamRotationFromInwardNormal,
//        int initialTissueRegionIndex)
//    {
//        SourceType = "DirectionalArbitrary";
//        ThetaConvOrDiv = thetaConvOrDiv;
//        NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
//        TranslationFromOrigin = translationFromOrigin;
//        BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
//        InitialTissueRegionIndex = initialTissueRegionIndex;
//    }

//    /// <summary>
//    /// Initializes a new instance of the DirectionalArbitrarySourceInput class
//    /// </summary>
//    public DirectionalArbitrarySourceInput()
//        : this(
//            0.0,
//            SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
//            SourceDefaults.DefaultPosition.Clone(), 
//            SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
//            0)
//    { }

//    /// <summary>
//    /// Bitmap source type
//    /// </summary>
//    public string SourceType { get; set; }
   
//    /// <summary>
//    /// Source profile type
//    /// </summary>
//    [IgnoreDataMember]
//    public ISourceProfile SourceProfile { get; set; }
//    /// <summary>
//    /// Convergence or Divergence Angle {= 0, for a normal beam}
//    /// </summary>
//    public double ThetaConvOrDiv { get; set; }
//    /// <summary>
//    /// New source axis direction
//    /// </summary>
//    public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
//    /// <summary>
//    /// New source location
//    /// </summary>
//    public Position TranslationFromOrigin { get; set; }
//    /// <summary>
//    /// Beam rotation from inward normal
//    /// </summary>
//    public PolarAzimuthalAngles BeamRotationFromInwardNormal { get; set; }
//    /// <summary>
//    /// Initial tissue region index
//    /// </summary>
//    public int InitialTissueRegionIndex { get; set; }

//    /// <summary>
//    /// Required code to create a source based on the input values
//    /// </summary>
//    /// <param name="rng">random number generator</param>
//    /// <returns>instantiated source</returns>
//    public ISource CreateSource(Random rng = null)
//    {
//        rng = rng ?? new Random();

//        // instantiate image, pdf and cdf to use to select new Photon
//        SourceProfile = new ArbitrarySourceProfile();

//        return new DirectionalArbitrarySource(
//            this.ThetaConvOrDiv,
//            this.SourceProfile,
//            this.NewDirectionOfPrincipalSourceAxis,
//            this.TranslationFromOrigin,
//            this.BeamRotationFromInwardNormal,
//            this.InitialTissueRegionIndex);
//    }
//}

    /// <summary>
    /// Implements DirectionalArbitrarySurfaceSource with converging/diverging angle, length, width,
    /// source profile, direction, position, inward normal beam rotation and initial tissue 
    /// region index.
    /// </summary>
    public class DirectionalArbitrarySource 
    {
        private readonly double _thetaConvOrDiv;  //convergence:positive, divergence:negative, collimated:zero

        /// <summary>
        /// Defines DirectionalArbitrarySource
        /// </summary>
        /// <param name="thetaConvOrDiv">Convergence(negative angle in radians) or Divergence (positive angle in radians) angle {= 0, for a collimated beam}</param>
        /// <param name="sourceProfile">ISourceProfile type = Arbitrary</param>
        /// <param name="newDirectionOfPrincipalSourceAxis">New source axis direction</param>
        /// <param name="translationFromOrigin">New source location</param>
        /// <param name="beamRotationFromInwardNormal">null not used for this source</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public DirectionalArbitrarySource(
            double thetaConvOrDiv,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis = null,
            Position translationFromOrigin = null,
            PolarAzimuthalAngles beamRotationFromInwardNormal = null,
            int initialTissueRegionIndex = 0)
        {
            _thetaConvOrDiv = thetaConvOrDiv;
        }

    }
}
