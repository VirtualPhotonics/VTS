using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to MultiLayerTissue class.
    /// </summary>
    public class MultiLayerTissueInput : TissueInput, ITissueInput
    {

        /// <summary>
        /// constructor for Multi-layer tissue input
        /// </summary>
        /// <param name="regions">list of tissue regions comprising tissue</param>
        public MultiLayerTissueInput(ITissueRegion[] regions)
        {
            TissueType = "MultiLayer";
            Regions = regions;
        }

        /// <summary>
        /// MultiLayerTissue default constructor provides homogeneous tissue
        /// </summary>
        public MultiLayerTissueInput()
            : this(
                new ITissueRegion[]
                { 
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                })
        {
        }

        /// <summary>
        /// list of tissue regions comprising tissue
        /// </summary>
        public ITissueRegion[] Regions { get; set; }

        /// <summary>
        /// Required factory method to create the corresponding 
        /// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="pft">Phase Function Type</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns>instantiated tissue class</returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, PhaseFunctionType pft, double russianRouletteWeightThreshold)
        {
            var t = new MultiLayerTissue(Regions);

            t.Initialize(awt, pft, russianRouletteWeightThreshold);

            return t;
        }
    }

    /// <summary>
    /// Implements ITissue.  Defines tissue geometries comprised of layers
    /// (including homogenous with air layers above and below).  Layers are infinite along
    /// x- and y- axes.
    /// </summary>
    public class MultiLayerTissue : TissueBase, ITissue
    {
        private readonly IList<LayerTissueRegion> _layerRegions;

        /// <summary>
        /// Creates an instance of a MultiLayerTissue
        /// </summary>
        /// <param name="regions">list of tissue regions comprising tissue</param>
        /// <remarks>air above and below tissue needs to be specified for a slab geometry</remarks>
        public MultiLayerTissue(
            IList<ITissueRegion> regions)
            : base(regions)
        {
            _layerRegions = regions.Select(region => (LayerTissueRegion) region).ToArray();
        }

        /// <summary>
        /// Creates a default instance of a MultiLayerTissue based on a homogeneous medium slab geometry
        /// and discrete absorption weighting
        /// </summary>
        public MultiLayerTissue() 
            : this(new MultiLayerTissueInput().Regions)
        {
        }

        /// <summary>
        /// method to determine region index of region photon is currently in
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>integer index of tissue region position is in</returns>
        public virtual int GetRegionIndex(Position position)
        {
            // use ITissueRegion interface method ContainsPosition for LayerTissueRegion to determine
            // which region photon resides

            var index = -1;
            for (var i = 0; i < _layerRegions.Count; i++)
            {
                if (_layerRegions[i].ContainsPosition(position))
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
        public virtual double GetDistanceToBoundary(Photon photon)
        {
            // moved code that used to be here to LayerTissueRegion.RayIntersectBoundary
            // where it should be

            // get current region index, could be index of inclusion
            var currentRegionIndex = photon.CurrentRegionIndex;
            // check if in embedded tissue region ckh fix 8/10/11
            var currentRegion = _layerRegions[1];
            if (currentRegionIndex < _layerRegions.Count)
            {
                currentRegion = _layerRegions[currentRegionIndex];
            }

            currentRegion.RayIntersectBoundary(photon, out var distanceToBoundary);

            return distanceToBoundary;
        }
        /// <summary>
        /// method to determine if on boundary of tissue, i.e. at tissue/air interface
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean indicating whether on boundary or not</returns>
        public virtual bool OnDomainBoundary(Position position)
        {
            // this code assumes that the first and last layer is air
            return 
                position.Z < 1e-10 ||
                (Math.Abs(position.Z - (_layerRegions.Last()).ZRange.Start) < 1e-10);
        }
        /// <summary>
        /// method to determine index of region photon is about to enter
        /// </summary>
        /// <param name="photon">photon info including position and direction</param>
        /// <returns>region index</returns>
        public virtual int GetNeighborRegionIndex(Photon photon)
        {
            if (photon.DP.Direction.Uz != 0.0)
            {
                return photon.DP.Direction.Uz > 0.0
                    ? Math.Min(photon.CurrentRegionIndex + 1, Regions.Count - 1)
                    : Math.Max(photon.CurrentRegionIndex - 1, 0);
            }

            throw new ArgumentException("GetNeighborRegionIndex called and Photon not on boundary");
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
        /// <returns>direction of reflected photon</returns>
        public virtual Direction GetReflectedDirection(
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
        /// <returns>direction</returns>
        public virtual Direction GetRefractedDirection(
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
        public virtual double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            return Math.Abs(photon.DP.Direction.Uz); // abs will work for upward normal and downward normal
        }
    }
}
