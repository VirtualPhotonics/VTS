using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to MultiConcentricInclusionTissue class.
    /// This assumes inclusions are concentric and lie entirely within a single layer of tissue.
    /// </summary>
    public class MultiConcentricInclusionTissueInput : TissueInput, ITissueInput
    {
        private ITissueRegion[] _inclusionRegions;
        private ITissueRegion[] _layerRegions;

        /// <summary>
        /// constructor for MultiConcentricInclusion tissue input
        /// </summary>
        /// <param name="inclusionRegions">concentric inclusion regions, larger first</param>
        /// <param name="layerRegions">layer regions</param>
        public MultiConcentricInclusionTissueInput(
            ITissueRegion[] inclusionRegions,
            ITissueRegion[] layerRegions)
        {
            TissueType = "MultiConcentricInclusion";
            LayerRegions = layerRegions;
            InclusionRegions = inclusionRegions;
            Regions = LayerRegions.Concat(InclusionRegions).ToArray();
        }

        /// <summary>
        /// MultiConcentricInclusionTissue default constructor provides homogeneous tissue
        /// </summary>
        public MultiConcentricInclusionTissueInput()
            : this(
                new ITissueRegion[]
                {
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1),
                        0.75,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    ),
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1),
                        0.5,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                    )
                },
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
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get; private set; }

        /// <summary>
        /// tissue outer inclusion region
        /// </summary>
        public ITissueRegion[] InclusionRegions
        {
            get => _inclusionRegions;
            set
            {
                _inclusionRegions = value;
                if (LayerRegions != null) Regions = LayerRegions.Concat(_inclusionRegions).ToArray();
            }
        }

        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions
        {
            get => _layerRegions;
            set
            {
                _layerRegions = value;
                if (InclusionRegions != null) Regions = _layerRegions.Concat(InclusionRegions).ToArray();
            }
        }

        /// <summary>
        /// Required factory method to create the corresponding 
        /// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="pft">Phase Function Type</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns>instantiated tissue</returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, PhaseFunctionType pft, double russianRouletteWeightThreshold)
        {
            var t = new MultiConcentricInclusionTissue(InclusionRegions, LayerRegions);

            t.Initialize(awt, pft, russianRouletteWeightThreshold);

            return t;
        }
    }

    /// <summary>
    /// Implements ITissue.  Defines tissue geometries comprised of layers
    /// (including homogenous with air layers above and below).  Layers are infinite along
    /// x- and y- axes.
    /// </summary>
    public class MultiConcentricInclusionTissue : MultiLayerTissue, ITissue
    {
        private readonly IList<LayerTissueRegion> _layerRegions;
        private readonly IList<ITissueRegion> _inclusionRegions;
        private readonly int _layerRegionIndexOfInclusion;

        /// <summary>
        /// Creates an instance of a MultiConcentricInclusionTissue
        /// </summary>
        /// <param name="inclusionRegions">list of concentric inclusion regions</param>
        /// <param name="layerRegions">list of layer regions</param>
        /// <remarks>air above and below tissue needs to be specified for a slab geometry</remarks>
        public MultiConcentricInclusionTissue(
            IList<ITissueRegion> inclusionRegions,
            IList<ITissueRegion> layerRegions)
            : base(layerRegions)
        {
            // overwrite the Regions property in the TissueBase class (will be called last in the most derived class)
            Regions = layerRegions.Concat(inclusionRegions).ToArray();

            _layerRegions = layerRegions.Select(r => (LayerTissueRegion)r).ToList();
            _inclusionRegions = inclusionRegions.Select(r => (ITissueRegion)r).ToList();
            // also by convention larger radius inclusion is first
            _layerRegionIndexOfInclusion = Enumerable.Range(0, _layerRegions.Count)
                .FirstOrDefault(i => _layerRegions[i]
                    .ContainsPosition(_inclusionRegions[0].Center)); // if outer inclusion in layer, inner is
        }

        /// <summary>
        /// Creates a default instance of a MultiConcentricInclusionTissue based on a homogeneous medium slab geometry
        /// and discrete absorption weighting
        /// </summary>
        public MultiConcentricInclusionTissue()
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
        /// <param name="position">photon position</param>
        /// <returns>integer index of region position is in</returns>
        public override int GetRegionIndex(Position position)
        {
            var index = -1;
            // use LayerTissueRegion to determine which region photon resides
            for (var i = 0; i < _layerRegions.Count; i++)
            {
                if (_layerRegions[i].ContainsPosition(position))
                {
                    index = i;
                }
            }
            // use InclusionTissueRegion to determine if within one of inclusions
            // check goes from largest to smallest so index that contains point will be returned
            for (var j = 0; j < _inclusionRegions.Count; j++)
            {
                if (_inclusionRegions[j].ContainsPosition(position))
                {
                    index = _layerRegions.Count + j;
                }
            }
            return index;
        }

        /// <summary>
        /// Finds the distance to the next boundary and independent of hitting it
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>double distance to boundary</returns>
        public override double GetDistanceToBoundary(Photon photon)
        {
            // first check if closest boundary is layer

            // going "up" in negative z-direction
            var goingUp = photon.DP.Direction.Uz < 0.0;

            // get current and adjacent regions
            var currentRegionIndex = photon.CurrentRegionIndex;
            // check if not in embedded tissue region ckh fix 8/10/11
            var currentRegion = _layerRegions[1];
            if (currentRegionIndex < _layerRegions.Count)
            {
                currentRegion = _layerRegions[currentRegionIndex];
            }
            // calculate distance to boundary based on z-projection of photon trajectory
            var distanceToLayer =
                goingUp
                    ? (currentRegion.ZRange.Start - photon.DP.Position.Z) / photon.DP.Direction.Uz
                    : (currentRegion.ZRange.Stop - photon.DP.Position.Z) / photon.DP.Direction.Uz;

            // then check if inclusion boundaries are closer
            var smallestInclusionDistance = double.PositiveInfinity;

            foreach (var inclusionRegion in _inclusionRegions)
            {
                inclusionRegion.RayIntersectBoundary(photon, out var distToInclusion);
                // first check that photon isn't sitting on boundary of one of the inclusions
                // note 1e-9 was found by trial and error using unit tests to verify selection
                // if you change value, need to update InclusionTissueRegion.ContainsPosition eps
                if (distToInclusion > 1e-9 && distToInclusion < smallestInclusionDistance)
                {
                    smallestInclusionDistance = distToInclusion;
                }
            }

            return smallestInclusionDistance < distanceToLayer ? smallestInclusionDistance : distanceToLayer;
        }

        /// <summary>
        /// method to determine if on boundary of tissue, i.e. at tissue/air interface
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean indicating whether on boundary of domain or not</returns>
        public override bool OnDomainBoundary(Position position)
        {
            // Domain boundary: so check layer boundary
            // this code assumes that the first and last layer is air
            return
                position.Z < 1e-10 ||
                Math.Abs(position.Z - _layerRegions.Last().ZRange.Start) < 1e-10;
        }

        /// <summary>
        /// method to determine index of region photon is about to enter
        /// </summary>
        /// <param name="photon">photon info including position and direction</param>
        /// <returns>region index</returns>
        public override int GetNeighborRegionIndex(Photon photon)
        {
            // check if coming from innermost inclusion
            if (photon.CurrentRegionIndex == _layerRegions.Count + 1)
            {
                return _layerRegions.Count; // inner inclusion is inside outer so always neighbor
            }
            // if coming from outermost inclusion => layer of inclusion or inner inclusion
            if (photon.CurrentRegionIndex == _layerRegions.Count)
            {
                if (_inclusionRegions[1].ContainsPosition(photon.DP.Position)) // must be entering inner
                {
                    return _layerRegions.Count + 1;
                }
                return _layerRegionIndexOfInclusion;
            }
            // could be on layer boundary
            if (photon.CurrentRegionIndex != _layerRegionIndexOfInclusion) return base.GetNeighborRegionIndex(photon);

            // if not on layer boundary that contains inclusion, then return inclusion index
            // otherwise return neighbor layer index
            return !_layerRegions[_layerRegionIndexOfInclusion].OnBoundary(photon.DP.Position)
                ? _layerRegions.Count
                : base.GetNeighborRegionIndex(photon);
        }
        /// <summary>
        /// method to determine photon state type of photon exiting tissue boundary
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>PhotonStateType class</returns>
        public new PhotonStateType GetPhotonDataPointStateOnExit(Position position)
        {
            return position.Z < 1e-10
                ? PhotonStateType.PseudoReflectedTissueBoundary
                : PhotonStateType.PseudoTransmittedTissueBoundary;
        }
        /// <summary>
        /// method to determine direction of reflected photon
        /// </summary>
        /// <param name="currentPosition">current position of photon</param>
        /// <param name="currentDirection">current direction of photon</param>
        /// <returns>direction of reflected input direction</returns>
        public override Direction GetReflectedDirection(
            Position currentPosition,
            Direction currentDirection)
        {
            // check if crossing top and bottom layer
            if (currentPosition.Z < 1e-10 ||
                Math.Abs(currentPosition.Z - _layerRegions.Last().ZRange.Start) < 1e-10)
            {
                return base.GetReflectedDirection(currentPosition, currentDirection);
            }
            // must be on inclusions for now no reflection NOTE: when refractive index mismatch branch merged
            // change code to call inclusionTissueRegion.GetReflectedDirection
            return currentDirection;
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
        public override Direction GetRefractedDirection(
            Position currentPosition,
            Direction currentDirection,
            double currentN,
            double nextN,
            double cosThetaSnell)
        {
            // check if crossing top and bottom layer
            if (currentPosition.Z < 1e-10 ||
                Math.Abs(currentPosition.Z - _layerRegions.Last().ZRange.Start) < 1e-10)
            {
                return base.GetRefractedDirection(currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            }
            // must be on inclusions for now no reflection NOTE: when refractive index mismatch branch merged
            // change code to call inclusionTissueRegion.GetRefractedDirection
            return currentDirection;
        }
        /// <summary>
        /// method to get cosine of the angle between photons current direction and boundary normal
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>Uz=cos(theta)</returns>
        public new double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            return Math.Abs(photon.DP.Direction.Uz); // abs will work for upward normal and downward normal
        }
    }
}
