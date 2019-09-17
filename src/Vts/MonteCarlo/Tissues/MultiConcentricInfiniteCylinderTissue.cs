using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to MultiConcentricInfiniteCylinderTissue class.
    /// This assumes infinite cylinders are concentric and lie entirely within a layer.
    /// </summary>
    public class MultiConcentricInfiniteCylinderTissueInput : TissueInput, ITissueInput
    {
        private ITissueRegion[] _regions;
        private ITissueRegion[] _layerRegions;
        private ITissueRegion[] _infiniteCylinderRegions;

        /// <summary>
        /// constructor for Multi-ConcentricInfiniteCylinder tissue input
        /// </summary>
        /// <param name="infiniteCylinderRegions">concentric cylinder regions, larger radius first</param>
        /// <param name="layerRegions">layer regions</param>
        public MultiConcentricInfiniteCylinderTissueInput(
            ITissueRegion[] infiniteCylinderRegions,
            ITissueRegion[] layerRegions)
        {
            TissueType = "MultiConcentricInfiniteCylinder";
            _layerRegions = layerRegions;
            _infiniteCylinderRegions = infiniteCylinderRegions;
            RegionPhaseFunctionInputs = new Dictionary<string, IPhaseFunctionInput>();
        }

        /// <summary>
        /// MultiConcentricInfiniteCylinderTissue default constructor provides homogeneous tissue
        /// </summary>
        public MultiConcentricInfiniteCylinderTissueInput()
            : this(
                new ITissueRegion[]
                {
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1),
                        0.75,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey4"
                    ),
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1),
                        0.5,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey5"
                    )
                },
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
            RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey5", new HenyeyGreensteinPhaseFunctionInput());
        }


        /// <summary>
        /// list of tissue regions comprising tissue
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return _layerRegions.Concat(_infiniteCylinderRegions).ToArray(); } }
        /// <summary>
        /// tissue outer infinite cylinder region
        /// </summary>
        public ITissueRegion[] InfiniteCylinderRegions { get { return _infiniteCylinderRegions; } set { _infiniteCylinderRegions = value; } }
         /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions { get { return _layerRegions; } set { _layerRegions = value; } }
        /// <summary>
        /// dictionary of region phase function inputs
        /// </summary>
        public IDictionary<string, IPhaseFunctionInput> RegionPhaseFunctionInputs { get; set; }

        /// <summary>
        /// Required factory method to create the corresponding 
        /// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="regionPhaseFunctions">Phase Function dictionary</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns></returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, IDictionary<string, IPhaseFunction> regionPhaseFunctions, double russianRouletteWeightThreshold)
        {
            var t = new MultiConcentricInfiniteCylinderTissue(InfiniteCylinderRegions, LayerRegions);

            t.Initialize(awt, regionPhaseFunctions, russianRouletteWeightThreshold);

            return t;
        }
    }

    /// <summary>
    /// Implements ITissue.  Defines tissue geometries comprised of layers
    /// (including homogenous with air layers above and below).  Layers are infinite along
    /// x- and y- axes.
    /// </summary>
    public class MultiConcentricInfiniteCylinderTissue : MultiLayerTissue, ITissue
    {
        private IList<LayerTissueRegion> _layerRegions;
        private IList<InfiniteCylinderTissueRegion> _infiniteCylinderRegions;
        private int _inclusionRegionIndex;
        private int _layerRegionIndexOfInclusion;

        /// <summary>
        /// Creates an instance of a MultiConcentricInfiniteCylinderTissue
        /// </summary>
        /// <param name="regions">list of tissue regions comprising tissue</param>
        /// <remarks>air above and below tissue needs to be specified for a slab geometry</remarks>
        public MultiConcentricInfiniteCylinderTissue(
            IList<ITissueRegion> infiniteCylinderRegions,
            IList<ITissueRegion> layerRegions)
            : base(layerRegions)
        {
            // overwrite the Regions property in the TissueBase class (will be called last in the most derived class)
            Regions = layerRegions.Concat(infiniteCylinderRegions).ToArray();

            _layerRegions = layerRegions.Select(r => (LayerTissueRegion)r).ToList();
            _infiniteCylinderRegions = infiniteCylinderRegions.Select(r => (InfiniteCylinderTissueRegion) r).ToList();
            _inclusionRegionIndex = _layerRegions.Count; // index is, by convention, after the layer region indices
            // also by convention larger radius infinite cylinder is first
            _layerRegionIndexOfInclusion = Enumerable.Range(0, _layerRegions.Count)
                .FirstOrDefault(i => ((LayerTissueRegion)_layerRegions[i])
                    .ContainsPosition(_infiniteCylinderRegions[0].Center)); // if outer cyl in layer, inner is
        }

        /// <summary>
        /// Creates a default instance of a MultiConcentricInfiniteCylinderTissue based on a homogeneous medium slab geometry
        /// and discrete absorption weighting
        /// </summary>
        public MultiConcentricInfiniteCylinderTissue() 
            : this(
                new ITissueRegion[]
                    {
                        new InfiniteCylinderTissueRegion(),
                        new InfiniteCylinderTissueRegion()
                    },
        new MultiLayerTissueInput().Regions)
        {
        }

        /// <summary>
        /// method to determine region index of region photon is currently in
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual int GetRegionIndex(Position position)
        {
            int index = -1;
            // use LayerTissueRegion to determine which region photon resides
            for (int i = 0; i < _layerRegions.Count(); i++)
            {
                if (_layerRegions[i].ContainsPosition(position))
                {
                    index = i;
                }
            }
            // use InfiniteTissueRegion to determine if within one of cylinders
            // check goes from largest to smallest so index that contains point will be returned
            for (int j = 0; j < _infiniteCylinderRegions.Count(); j++)
            {
                if (_infiniteCylinderRegions[j].ContainsPosition(position))
                {
                    index = _layerRegions.Count + j;
                }
            }
            return index;
        }
        
        /// <summary>
        /// Finds the distance to the next boundary and independent of hitting it
        /// </summary>
        /// <param name="photon"></param>
        public virtual double GetDistanceToBoundary(Photon photon)
        {
            // first check if closest boundary is layer

            // going "up" in negative z-direction
            bool goingUp = photon.DP.Direction.Uz < 0.0;

            // get current and adjacent regions
            int currentRegionIndex = photon.CurrentRegionIndex; 
            // check if not in embedded tissue region ckh fix 8/10/11
            LayerTissueRegion currentRegion = _layerRegions[1];
            if (currentRegionIndex < _layerRegions.Count)
            {
                currentRegion = _layerRegions[currentRegionIndex];
            }
            // calculate distance to boundary based on z-projection of photon trajectory
            double distanceToLayer =
                goingUp
                    ? (currentRegion.ZRange.Start - photon.DP.Position.Z) / photon.DP.Direction.Uz
                    : (currentRegion.ZRange.Stop - photon.DP.Position.Z) / photon.DP.Direction.Uz;

            // then check if infinite cylinder boundaries are closer
            double smallestInfCylDistance = double.PositiveInfinity;
            double distToInfiniteCylinder;
            for (int i = 0; i < _infiniteCylinderRegions.Count; i++)
            {
                _infiniteCylinderRegions[i].RayIntersectBoundary(photon, out distToInfiniteCylinder);
                // first check that photon isn't sitting on boundary of one of the cylinders
                // note 1e-12 was found by trial and error using unit tests to verify selection
                if ((distToInfiniteCylinder > 1e-12) && (distToInfiniteCylinder < smallestInfCylDistance))
                {
                    smallestInfCylDistance = distToInfiniteCylinder;
                }
            }

            if (smallestInfCylDistance < distanceToLayer)
            {
                return smallestInfCylDistance;
            }
            return distanceToLayer;
        }

        /// <summary>
        /// method to determine if on boundary of tissue, i.e. at tissue/air interface
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns></returns>
        public virtual bool OnDomainBoundary(Position position)
        {
            // Domain boundary: so check layer boundary
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
            // check if coming from inner infinite cylinder
            if (photon.CurrentRegionIndex == _layerRegions.Count + 1)
            {
                return _layerRegions.Count; // inner inf cylinder is inside outer so always neighbor
            }
            // if coming from outer infinite cylinder => layer of inclusion or inner cylinder
            //double distanceToBoundary = double.PositiveInfinity;
            if (photon.CurrentRegionIndex == _layerRegions.Count)
            {
                if (_infiniteCylinderRegions[1].ContainsPosition(photon.DP.Position)) // must be entering inner
                //if (double.IsPositiveInfinity(distanceToBoundary))
                {
                    return _layerRegions.Count + 1;
                }
                return _layerRegionIndexOfInclusion;
            }
            // could be on layer boundary
            if (photon.CurrentRegionIndex == _layerRegionIndexOfInclusion)
            {
                if (!_layerRegions[_layerRegionIndexOfInclusion].OnBoundary(photon.DP.Position))
                {
                    return _layerRegions.Count; // return outer cylinder index
                }
            }
            // finally must be on layer boundary
            return base.GetNeighborRegionIndex(photon);
        }
        /// <summary>
        /// method to determine photon state type of photon exiting tissue boundary
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public PhotonStateType GetPhotonDataPointStateOnExit(Position position)
        {
            if (position.Z < 1e-10)
            {
                return PhotonStateType.PseudoReflectedTissueBoundary;
            }
            
            return PhotonStateType.PseudoTransmittedTissueBoundary;
        }
        /// <summary>
        /// method to determine direction of reflected photon
        /// </summary>
        /// <param name="positionCurrent"></param>
        /// <param name="directionCurrent"></param>
        /// <returns></returns>
        public virtual Direction GetReflectedDirection(
            Position positionCurrent, 
            Direction directionCurrent)
        {
            // check if crossing top and bottom layer
            if (positionCurrent.Z < 1e-10 ||
                (Math.Abs(positionCurrent.Z - (_layerRegions.Last()).ZRange.Start) < 1e-10))
            {
                return base.GetReflectedDirection(positionCurrent, directionCurrent);
            }
            // must be on cylinders for now no reflection NOTE: when refractive index mismatch branch merged
            // change code to call infiniteCylinderTissueRegion.GetReflectedDirection
            return directionCurrent;
        }
        /// <summary>
        /// method to determine refracted direction of photon
        /// </summary>
        /// <param name="positionCurrent">current photon position</param>
        /// <param name="directionCurrent">current photon direction</param>
        /// <param name="nCurrent">refractive index of current region</param>
        /// <param name="nNext">refractive index of next region</param>
        /// <param name="cosThetaSnell">cos(theta) resulting from Snell's law</param>
        /// <returns>direction</returns>
        public virtual Direction GetRefractedDirection(
            Position positionCurrent, 
            Direction directionCurrent, 
            double nCurrent, 
            double nNext, 
            double cosThetaSnell)
        {            
            // check if crossing top and bottom layer
            if (positionCurrent.Z < 1e-10 ||
                (Math.Abs(positionCurrent.Z - (_layerRegions.Last()).ZRange.Start) < 1e-10))
            {
                return base.GetRefractedDirection(positionCurrent, directionCurrent, nCurrent, nNext, cosThetaSnell);
            }
            // must be on cylinders for now no reflection NOTE: when refractive index mismatch branch merged
            // change code to call infiniteCylinderTissueRegion.GetRefractedDirection
            return directionCurrent;
        }
        /// <summary>
        /// method to get cosine of the angle between photons current direction and boundary normal
        /// </summary>
        /// <param name="photon"></param>
        /// <returns>Uz=cos(theta)</returns>
        public double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            return Math.Abs(photon.DP.Direction.Uz); // abs will work for upward normal and downward normal
        }
    }
}
