using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Tissues
{

    /// <summary>
    /// Implements ITissue.  Defines a tissue geometry comprised of a layered slab
    /// with multiple inclusions bounded laterally by a TissueRegion.
    /// </summary>
    public class BoundedWithMultiInclusionTissue : MultiLayerTissue, ITissue
    {
        private readonly ITissueRegion _boundingRegion;
        private readonly IList<ITissueRegion> _layerRegions;
        private readonly IList<ITissueRegion> _inclusionRegions;
        private readonly int _boundingRegionExteriorIndex;
        private readonly IList<int> _tissueLayersInsideBoundIndices;
        private readonly IList<int> _tissueInclusionsInsideBoundIndices;
        private readonly IList<int> _layerRegionIndicesOfInclusion;

        /// <summary>
        /// Creates an instance of a BoundedWithMultiInclusionTissue
        /// </summary>
        /// <param name="boundingRegion">Tissue region defining later extent of tissue (must span top to bottom of tissue layers)</param>
        /// <param name="layerRegions">The tissue layers</param>
        /// <param name="inclusions">The tissue inclusions within the layers</param>
        public BoundedWithMultiInclusionTissue(
            ITissueRegion boundingRegion,
            IList<ITissueRegion> layerRegions,
            IList<ITissueRegion> inclusions)
            : base(layerRegions)
        {
            // boundingRegionExteriorIndex is the area *outside* of the bounding region
            _boundingRegionExteriorIndex = layerRegions.Count + inclusions.Count; // index is, by convention, after the layer region and inclusion indices
            // overwrite the Regions property in the TissueBase class (will be called last in the most derived class)
            // the concat is with the outside of the bounding region by convention
            Regions = layerRegions.Concat(inclusions).Concat(boundingRegion).ToArray();
            _layerRegions = layerRegions;
            _inclusionRegions = inclusions;
            _boundingRegion = boundingRegion;
            // create list of tissue layers inside bounding region, assumes air-multilayer-air tissue
            _tissueLayersInsideBoundIndices = new List<int>();
            for (var i = 1; i < layerRegions.Count - 1; i++)
            {
                _tissueLayersInsideBoundIndices.Add(i);
            }
            // create list of inclusions inside bounding region
            _tissueInclusionsInsideBoundIndices = new List<int>();
            for (var j = 1; j < inclusions.Count - 1; j++)
            {
                _tissueInclusionsInsideBoundIndices.Add(j + _tissueLayersInsideBoundIndices.Count);
            }
            // determine which layers have inclusion
            _layerRegionIndicesOfInclusion = new List<int>();
            foreach (var inclusionRegion in _inclusionRegions)
            {
                for (var i = 0; i < _layerRegions.Count; i++)
                {
                    if (_layerRegions[i].ContainsPosition(inclusionRegion.Center))
                        _layerRegionIndicesOfInclusion.Add(i);
                }
            }
        }

        /// <summary>
        /// Creates a default instance of a BoundingTissue
        /// </summary>
        public BoundedWithMultiInclusionTissue()
            : this(
                new CaplessVoxelTissueRegion(),
                new List<ITissueRegion>
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 10.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(10.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                },
                new List<ITissueRegion>
                {
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 5), 1,
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 15), 1,
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4))
                })
        {
        }

        /// <summary>
        /// method to get tissue region index of photon's current position
        /// </summary>
        /// <param name="position">photon Position</param>
        /// <returns>integer tissue region index</returns>
        public override int GetRegionIndex(Position position)
        {
            // if it's in the bounding region, return bounding region index
            if (_boundingRegion.ContainsPosition(position))
                return _boundingRegionExteriorIndex;
            // if it's in an inclusion, return inclusion region index
            // Inclusions are indexed after the layer regions, so add _layerRegions.Count to index
            for (var j = 0; j < _inclusionRegions.Count; j++)
            {
                if (_inclusionRegions[j].ContainsPosition(position))
                {
                    return _layerRegions.Count + j;
                }
            }
            // else return index of layer
            return base.GetRegionIndex(position);  
        }

        /// <summary>
        /// Method to get distance from current photon position and direction to boundary of region
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>distance to boundary</returns>
        public override double GetDistanceToBoundary(Photon photon)
        {
            // smallest distance to bounding volume, layers or inclusions
            var smallestDistance = double.PositiveInfinity;
            // check that a projected track will hit bounding volume, if so, check layers and find minimum
            // if not, check distance to layers
            var projectedPhoton = new Photon
            {
                DP = new PhotonDataPoint(photon.DP.Position, photon.DP.Direction, photon.DP.Weight,
                    photon.DP.TotalTime, photon.DP.StateFlag),
                S = 100
            };
            if (_boundingRegion.RayIntersectBoundary(projectedPhoton, out var distanceToBoundingBoundary))
                smallestDistance = distanceToBoundingBoundary;
            
            // check if photon will hit inclusion
            foreach (var inclusion in _inclusionRegions)
            {
                if (!inclusion.RayIntersectBoundary(projectedPhoton, out var distanceToInclusion)) continue;
                if (distanceToInclusion < smallestDistance)
                    smallestDistance = distanceToInclusion;
            }

            // check if photon will hit layer boundary
            var distanceToLayerBoundary = base.GetDistanceToBoundary(photon);
            if (distanceToLayerBoundary < smallestDistance)
                smallestDistance = distanceToLayerBoundary;

            return smallestDistance;
        }

        /// <summary>
        /// Method to determine if on boundary of tissue, i.e. at tissue/air interface
        /// or bounding volume
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean indicating whether on boundary or not</returns>
        public override bool OnDomainBoundary(Position position)
        {
            // this code assumes that the first and last layer is air
            return _boundingRegion.OnBoundary(position) ||
                position.Z < 1e-10 ||
                Math.Abs(position.Z - ((LayerTissueRegion)_layerRegions.Last()).ZRange.Start) < 1e-10;
        }

        /// <summary>
        /// Method to get index of neighbor tissue region when photon on boundary of two regions
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>index of neighbor index</returns>
        public override int GetNeighborRegionIndex(Photon photon)
        {
            // if we're not on the boundary of the bounding region, check inclusions
            if (_boundingRegion.OnBoundary(photon.DP.Position)) return _boundingRegionExteriorIndex;

            // on some internal boundary at this point, possibilities include
            // 1) in layer of inclusion entering inclusion
            // 2) in inclusion entering layer of inclusion
            // 3) on layer region boundary
            // first, check what region the photon is in
            var currentRegionIndex = photon.CurrentRegionIndex;

            // check if we are in a layer region
            var inLayer = currentRegionIndex >= 0 && currentRegionIndex < _layerRegions.Count;

            // check if on boundary of layer, then neighbor is next layer region
            if (inLayer && Regions[currentRegionIndex].OnBoundary(photon.DP.Position))
            {
                return base.GetNeighborRegionIndex(photon);
            }

            // if we're in a layer region with an inclusion(s) and not on boundary of layer
            // then on boundary of one of the inclusions and could be entering or exiting region

            // determine which inclusion photon is on boundary of
            // use _inclusionRegion to determine if within one of inclusions
            for (var j = 0; j < _inclusionRegions.Count; j++)
            {
                if (!_inclusionRegions[j].ContainsPosition(photon.DP.Position)) continue;

                return currentRegionIndex == _layerRegionIndicesOfInclusion[j]
                    ? _layerRegions.Count + j
                    : // entering inclusion
                    _layerRegionIndicesOfInclusion[j]; // exiting into surrounding layer region
            }
            return _boundingRegionExteriorIndex;
        }

        /// <summary>
        /// Method to determine photon state type of photon exiting tissue boundary
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>PhotonStateType</returns>
        public new PhotonStateType GetPhotonDataPointStateOnExit(Position position)
        {
            if (position.Z < 1e-10)
            {
                return PhotonStateType.PseudoReflectedTissueBoundary;
            }
            return Math.Abs(position.Z - ((LayerTissueRegion)_layerRegions.Last()).ZRange.Start) < 1e-10
                ? PhotonStateType.PseudoTransmittedTissueBoundary
                : PhotonStateType.PseudoBoundingVolumeTissueBoundary;
        }

        /// <summary>
        /// Method that provides reflected direction when photon reflects off boundary
        /// </summary>
        /// <param name="currentPosition">Position</param>
        /// <param name="currentDirection">Direction</param>
        /// <returns>new Direction</returns>
        public override Direction GetReflectedDirection(
            Position currentPosition,
            Direction currentDirection)
        {
            // the following checks 1) top or bottom layer, 2) inclusions, 3) layers, then on bounding volume
            // note that inclusions checked before layers because ContainsPosition could be in both

            // call MultiLayerTissue if crossing top or bottom layer
            if (base.OnDomainBoundary(currentPosition))
            {
                return base.GetReflectedDirection(currentPosition, currentDirection);
            }
            // determine surfaceNormal based on if on inclusion, layer, or bounding volume
            Direction surfaceNormal = null;
            // if on boundary of an inclusion, check which one
            var inclusionIndex = -1;
            for (var i = 0; i < _inclusionRegions.Count; i++)
            {
                if (_inclusionRegions[i].ContainsPosition(currentPosition)) inclusionIndex = i;
            }
            // if on inclusion boundary set surface normal if refractive index mismatch
            if (inclusionIndex != -1)
            {
                if (Math.Abs(_inclusionRegions[inclusionIndex].RegionOP.N -
                             Regions[_layerRegionIndicesOfInclusion[inclusionIndex]].RegionOP.N) < 1e-6)
                {
                    return currentDirection; // no refractive index mismatch
                }

                surfaceNormal = _inclusionRegions[inclusionIndex].SurfaceNormal(currentPosition);
            }

            // if on boundary of a layer, check which one
            var layerIndex = -1;
            for (var i = 1; i < _layerRegions.Count - 1; i++)
            {
                if (_layerRegions[i].ContainsPosition(currentPosition)) layerIndex = i;
            }
            // if on inclusion boundary set surface normal if refractive index mismatch
            if (inclusionIndex != -1)
            {
                if (Math.Abs(_inclusionRegions[inclusionIndex].RegionOP.N -
                             Regions[_layerRegionIndicesOfInclusion[inclusionIndex]].RegionOP.N) < 1e-6)
                {
                    return currentDirection; // no refractive index mismatch
                }

                surfaceNormal = _layerRegions[layerIndex].SurfaceNormal(currentPosition);
            }
           
            if (surfaceNormal == null)  // must be on bounding volume
                surfaceNormal = _boundingRegion.SurfaceNormal(currentPosition);
            // reflection equation reflected = incident - 2(incident dot surfaceNormal)surfaceNormal
            var currentDirDotNormal = Direction.GetDotProduct(currentDirection, surfaceNormal);
            var newX = currentDirection.Ux - 2 * currentDirDotNormal * surfaceNormal.Ux;
            var newY = currentDirection.Uy - 2 * currentDirDotNormal * surfaceNormal.Uy;
            var newZ = currentDirection.Uz - 2 * currentDirDotNormal * surfaceNormal.Uz;
            var norm = Math.Sqrt(newX * newX + newY * newY + newZ * newZ);
            return new Direction(newX / norm, newY / norm, newZ / norm);
        }

        /// <summary>
        /// Method that provides refracted direction when photon refracts off boundary
        /// </summary>
        /// <param name="currentPosition">Position</param>
        /// <param name="currentDirection">Direction</param>
        /// <param name="currentN">refractive index N of current tissue region</param>
        /// <param name="nextN">refractive index N of next tissue region</param>
        /// <param name="cosThetaSnell">cosine of theta per Snell's</param>
        /// <returns>new Direction</returns>
        public override Direction GetRefractedDirection(
            Position currentPosition,
            Direction currentDirection,
            double currentN,
            double nextN,
            double cosThetaSnell)
        {
            // the following checks 1) top or bottom layer, 2) inclusions, 3) layers, then on bounding volume
            // note that inclusions checked before layers because ContainsPosition could be in both

            // needs to call MultiLayerTissue when crossing top and bottom layer
            if (base.OnDomainBoundary(currentPosition))
            {
                return base.GetRefractedDirection(currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            }
            // determine surfaceNormal based on if on inclusion, layer, or bounding volume
            Direction surfaceNormal = null;
            // if on boundary of an inclusion, check which one
            var inclusionIndex = -1;
            for (var i = 0; i < _inclusionRegions.Count; i++)
            {
                if (_inclusionRegions[i].ContainsPosition(currentPosition)) inclusionIndex = i;
            }
            // if on inclusion boundary set surface normal if refractive index mismatch
            if (inclusionIndex != -1)
            {
                if (Math.Abs(_inclusionRegions[inclusionIndex].RegionOP.N -
                             Regions[_layerRegionIndicesOfInclusion[inclusionIndex]].RegionOP.N) < 1e-6)
                {
                    return currentDirection; // no refractive index mismatch
                }

                surfaceNormal = _inclusionRegions[inclusionIndex].SurfaceNormal(currentPosition);
            }

            // if on boundary of a layer, check which one
            var layerIndex = -1;
            for (var i = 1; i < _layerRegions.Count - 1; i++)
            {
                if (_layerRegions[i].ContainsPosition(currentPosition)) layerIndex = i;
            }
            // if on inclusion boundary set surface normal if refractive index mismatch
            if (inclusionIndex != -1)
            {
                if (Math.Abs(_inclusionRegions[inclusionIndex].RegionOP.N -
                             Regions[_layerRegionIndicesOfInclusion[inclusionIndex]].RegionOP.N) < 1e-6)
                {
                    return currentDirection; // no refractive index mismatch
                }

                surfaceNormal = _layerRegions[layerIndex].SurfaceNormal(currentPosition);
            }

            if (surfaceNormal == null)  // must be on bounding volume
                surfaceNormal = _boundingRegion.SurfaceNormal(currentPosition);
            var cosTheta1 = Direction.GetDotProduct(currentDirection, surfaceNormal);
            var nRatio = currentN / nextN;
            var sinTheta2Squared = nRatio * nRatio * (1 - cosTheta1 * cosTheta1);
            var factor = nRatio * cosTheta1 - Math.Sqrt(1 - sinTheta2Squared);
            var newX = nRatio * currentDirection.Ux + factor * surfaceNormal.Ux;
            var newY = nRatio * currentDirection.Uy + factor * surfaceNormal.Uy;
            var newZ = nRatio * currentDirection.Uz + factor * surfaceNormal.Uz;
            var norm = Math.Sqrt(newX * newX + newY * newY + newZ * newZ);
            return new Direction(newX / norm, newY / norm, newZ / norm);
            
        }

        /// <summary>
        /// Method to get cosine of the angle between photons current direction and boundary normal.
        /// When this method is called photon is sitting on boundary of region and CurrentRegionIndex is Index
        /// of region photon had been in.
        /// </summary>
        /// <param name="photon"></param>
        /// <returns>Uz=cos(theta)</returns>
        public override double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            // needs to call MultiLayerTissue when crossing top and bottom layer
            if (base.OnDomainBoundary(photon.DP.Position))
            {
                return base.GetAngleRelativeToBoundaryNormal(photon);
            }

            return Math.Abs(Direction.GetDotProduct( // Abs consistent with SingleInclusionTissue
                photon.DP.Direction, _boundingRegion.SurfaceNormal(photon.DP.Position)));
        }
    }
}
