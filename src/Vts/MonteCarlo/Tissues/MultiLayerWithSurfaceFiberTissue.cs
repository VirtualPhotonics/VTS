using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to MultiLayerTissueWithSurfaceFiber class.
    /// This class is not created using SingleInclusionTissue.  Instead it is designed like
    /// MultiLayerTissue so that Photon CrossRegionOrReflect can ask
    /// int neighborIndex = _tissue.GetNeighborRegionIndex(this);
    /// double nNext = _tissue.Regions[neighborIndex].RegionOP.N;
    /// and get the detector N when the photon is residing in the fiber circle.
    /// </summary>
    public class MultiLayerWithSurfaceFiberTissueInput : TissueInput, ITissueInput
    {

        /// <summary>
        /// constructor for Multi-layer tissue with surface fiber circle input
        /// </summary>
        /// <param name="surfaceFiberRegion">surface fiber region defining area and characteristics of detector fiber</param>
        /// <param name="layerRegions">list of tissue regions comprising tissue</param>
        public MultiLayerWithSurfaceFiberTissueInput(ITissueRegion surfaceFiberRegion, ITissueRegion[] layerRegions)
        {
            TissueType = "MultiLayerWithSurfaceFiber";
            LayerRegions = layerRegions;
            SurfaceFiberRegion = surfaceFiberRegion;
            RegionPhaseFunctionInputs = new Dictionary<string, IPhaseFunctionInput>();
        }

        /// <summary>
        /// MultiLayerTissue default constructor provides homogeneous tissue
        /// </summary>
        public MultiLayerWithSurfaceFiberTissueInput()
            : this(
                new SurfaceFiberTissueRegion(
                    new Position(0, 0, 0),
                    0.3,
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                    "HenyeyGreensteinKey4"
                ),
                new ITissueRegion[]
                { 
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                })
        {
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
        }


        /// <summary>
        /// regions of tissue (layers and surface fiber circle)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return LayerRegions.Concat(SurfaceFiberRegion).ToArray(); } }
        /// <summary>
        /// surface fiber region
        /// </summary>
        public ITissueRegion SurfaceFiberRegion { get; set; }
        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions { get; set; }
        /// <summary>
        /// dictionary of region phase function inputs
        /// </summary>
        public IDictionary<string, IPhaseFunctionInput> RegionPhaseFunctionInputs { get; set; }

        /// <summary>
        /// Required factory method to create the corresponding 
        /// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="regionPhaseFunctions">Phase Functions for each tissue region</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns>instantiated tissue</returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, IDictionary<string, IPhaseFunction> regionPhaseFunctions, double russianRouletteWeightThreshold)
        {
            var t = new MultiLayerWithSurfaceFiberTissue(SurfaceFiberRegion, LayerRegions);

            t.Initialize(awt, regionPhaseFunctions, russianRouletteWeightThreshold);

            return t;

        }
    }

    /// <summary>
    /// Implements ITissue.  Defines tissue geometries comprised of layers
    /// (including homogenous with air layers above and below).  Layers are infinite along
    /// x- and y- axes.
    /// </summary>
    public class MultiLayerWithSurfaceFiberTissue : TissueBase, ITissue
    {
        private readonly IList<LayerTissueRegion> _layerRegions;
        private readonly ITissueRegion _surfaceFiberRegion;

        /// <summary>
        /// Creates an instance of a MultiLayerSurfaceFiberTissue
        /// </summary>
        /// <param name="surfaceFiberRegion">circular surface fiber region and characteristics</param>
        /// <param name="layerRegions">list of tissue regions comprising tissue</param>
        /// <remarks>air above and below tissue needs to be specified for a slab geometry</remarks>
        public MultiLayerWithSurfaceFiberTissue(ITissueRegion surfaceFiberRegion,
            IList<ITissueRegion> layerRegions)
            : base(layerRegions.Concat(surfaceFiberRegion).ToArray())
        {
            _layerRegions = layerRegions.Select(region => (LayerTissueRegion)region).ToArray();
            _surfaceFiberRegion = surfaceFiberRegion;
        }

        /// <summary>
        /// Creates a default instance of a MultiLayerTissue based on a homogeneous medium slab geometry
        /// and discrete absorption weighting
        /// </summary>
        public MultiLayerWithSurfaceFiberTissue()
            : this(new SurfaceFiberTissueRegion(), new MultiLayerTissueInput().Regions)
        {
        }

        /// <summary>
        /// method to determine region index of region photon is currently in
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>integer index of tissue region position is in</returns>
        public int GetRegionIndex(Position position)
        {
            // use ITissueRegion interface method ContainsPosition for TissueRegions to determine
            // which region photon resides
            var index = -1;
            for (var i = 0; i < Regions.Count; i++) // catch if in surface fiber AFTER layer 1
            {
                if (Regions[i].ContainsPosition(position))
                {
                    index = i;
                }
            }
            return index;
        }
        
        /// <summary>
        /// Finds the distance to the next boundary and independent of hitting it
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>distance to boundary</returns>
        public double GetDistanceToBoundary(Photon photon)
        {
            if (photon.DP.Direction.Uz == 0.0) return double.PositiveInfinity;

            // going "up" in negative z-direction
            var goingUp = photon.DP.Direction.Uz < 0.0;

            // get current and adjacent regions
            var currentRegionIndex = photon.CurrentRegionIndex; 
            // check if in embedded tissue region ckh fix 8/10/11
            var currentRegion = _layerRegions[1];
            if (currentRegionIndex < _layerRegions.Count)
            {
                currentRegion = _layerRegions[currentRegionIndex];
            }

            // calculate distance to boundary based on z-projection of photon trajectory
            var distanceToBoundary =
                goingUp
                    ? (currentRegion.ZRange.Start - photon.DP.Position.Z) / photon.DP.Direction.Uz
                    : (currentRegion.ZRange.Stop - photon.DP.Position.Z) / photon.DP.Direction.Uz;


            return distanceToBoundary;
        }
        /// <summary>
        /// method to determine if on boundary of tissue, i.e. at tissue/air interface
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean indicating whether position on domain boundary or not</returns>
        public bool OnDomainBoundary(Position position)
        {
            // this code assumes that the first and last layer is air
            return position.Z < 1e-10 ||
                Math.Abs(position.Z - (_layerRegions.Last()).ZRange.Start) < 1e-10;
        }
        /// <summary>
        /// method to determine index of region photon is about to enter
        /// </summary>
        /// <param name="photon">photon info including position and direction</param>
        /// <returns>region index</returns>
        public int GetNeighborRegionIndex(Photon photon)
        {
            if (photon.DP.Direction.Uz == 0.0)
            {
                throw new ArgumentException("GetNeighborRegionIndex called and Photon not on boundary");
            }

            if (photon.DP.Direction.Uz > 0.0) // move to layer below if pointed down
            {
                return Math.Min(photon.CurrentRegionIndex + 1, Regions.Count - 1);
            }

            // pointed up
            if (photon.CurrentRegionIndex != 1) // check if at surface
            {
                if (photon.CurrentRegionIndex == 3) return 0; // in surfaceFiberRegion
            }
            else
            {
                if (_surfaceFiberRegion.ContainsPosition(photon.DP.Position))
                {
                    return Regions.Count - 1; // return index of surfaceFiberRegion
                }

                return 0; // return air
            }

            return photon.CurrentRegionIndex - 1;  // must be layer above
        }
        /// <summary>
        /// method to determine photon state type of photon exiting tissue boundary
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>PhotonStateType</returns>
        public PhotonStateType GetPhotonDataPointStateOnExit(Position position)
        {
            return position.Z < 1e-10
                ? PhotonStateType.PseudoReflectedTissueBoundary
                : PhotonStateType.PseudoTransmittedTissueBoundary;
        }
        /// <summary>
        /// method to determine direction of reflected photon
        /// </summary>
        /// <param name="currentPosition">current photon position</param>
        /// <param name="currentDirection">current photon direction</param>
        /// <returns>direction of reflected input direction</returns>
        public Direction GetReflectedDirection(
            Position currentPosition, 
            Direction currentDirection)
        {
            return new Direction(
                currentDirection.Ux,
                currentDirection.Uy,
                -currentDirection.Uz);
        }
        /// <summary>
        /// method to determine refracted direction of photon
        /// </summary>
        /// <param name="currentPosition">current photon position</param>
        /// <param name="currentDirection">current photon direction</param>
        /// <param name="currentN">refractive index of current region</param>
        /// <param name="nextN">refractive index of next region</param>
        /// <param name="cosThetaSnell">cos(theta) resulting from Snell's law</param>
        /// <returns>direction of refracted photon</returns>
        public Direction GetRefractedDirection(
            Position currentPosition, 
            Direction currentDirection, 
            double currentN, 
            double nextN, 
            double cosThetaSnell)
        {
            if (currentDirection.Uz > 0)
                return new Direction(
                    currentDirection.Ux * currentN / nextN,
                    currentDirection.Uy * currentN / nextN,
                    cosThetaSnell);
            else
                return new Direction(
                    currentDirection.Ux * currentN / nextN,
                    currentDirection.Uy * currentN / nextN,
                    -cosThetaSnell);
        }
        /// <summary>
        /// method to get cosine of the angle between photons current direction and boundary normal
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>Uz=cos(theta)</returns>
        public double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            return Math.Abs(photon.DP.Direction.Uz); // abs will work for upward normal and downward normal
        }
    }
}
