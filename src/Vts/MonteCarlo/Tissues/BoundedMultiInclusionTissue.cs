using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Tissues
{

    /// <summary>
    /// Implements ITissue.  All "InclusionTissue" classes define processing for those tissues that use
    /// this class to get created. Defines a tissue geometry comprised of a list of inclusions (e.g. ellipsoids,
    /// infinite cylinders, etc.) embedded within *multiple* (non-air) layers of a layered slab, bounded by a tissue
    /// region (ITissueRegion).  An example would be a two layer phantom with multiple inclusions in each layer.
    /// The layers in the x and y directions are bounded by the bounding region. The inclusions can be of any shape
    /// that implements ITissueRegion. The layers are assumed to be stacked in the z direction, with the first layer
    /// at the top (z=0) and the last layer at the bottom (z=Zmax). The bounding region "height" along the z-axis needs
    /// to be defined to be equal to the total height of the tissue layers. The inclusions can be located anywhere within
    /// the layers, but can not overlap with each other, or with the layer boundaries, or with the bounding region.
    /// The tissue is assumed to be surrounded by air above and below, which must be specified as the first and last
    /// layers in the list of LayerTissueRegion objects.
    /// Note that many of the methods in this class are invoked by Photon class and Photon masterminds their
    /// returns.  For example, when the photon on the boundary of the layers or the inclusions, Photon
    /// determines whether in the critical angle and if so whether to reflect or refract, then invokes the
    /// methods below accordingly.
    /// </summary>
    public class BoundedMultiInclusionTissue : MultiLayerTissue, ITissue
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
        /// <param name="inclusions">The tissue inclusions within the layers</param>
        /// <param name="layerRegions">The tissue layers</param>
        public BoundedMultiInclusionTissue(
            ITissueRegion boundingRegion,
            IList<ITissueRegion> inclusions,
            IList<ITissueRegion> layerRegions)
            : base(layerRegions)
        {
            // boundingRegionExteriorIndex is the area *outside* of the bounding region
            _boundingRegionExteriorIndex = layerRegions.Count; // index is, by convention, after the layer region
            // overwrite the Regions property in the TissueBase class (will be called last in the most derived class)
            // the concat is with the outside of the bounding region by convention
            Regions = layerRegions.Concat(boundingRegion).Concat(inclusions).ToArray();
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
                _tissueInclusionsInsideBoundIndices.Add(j + _tissueLayersInsideBoundIndices.Count + 1); // +1 for bounding region exterior index
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
        public BoundedMultiInclusionTissue()
            : this(
                new CaplessVoxelTissueRegion(),
                new List<ITissueRegion>
                {
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 5), 1,
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 15), 1,
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4))
                },
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
                }
               )
        {
        }

        /// <summary>
        /// method to get tissue region index of photon's current position
        /// </summary>
        /// <param name="position">photon Position</param>
        /// <returns>integer tissue region index</returns>
        public override int GetRegionIndex(Position position)
        {
            // if it's not inside bounding region, then outside which is bounding region index
            if (!_boundingRegion.ContainsPosition(position))
                return _boundingRegionExteriorIndex;
            // if it's in an inclusion, return inclusion region index
            // Inclusions are indexed after the layer regions, so add _layerRegions.Count to index
            for (var j = 0; j < _inclusionRegions.Count; j++)
            {
                if (_inclusionRegions[j].ContainsPosition(position))
                {
                    return _layerRegions.Count + j + 1; // +1 for bounding region exterior index
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
            // first check what region the photon is in 
            var currentRegionIndex = photon.CurrentRegionIndex;

            // if photon is on bounding region, then neighbor must be layer so call base
            if (currentRegionIndex == _boundingRegionExteriorIndex &&
                _boundingRegion.OnBoundary(photon.DP.Position)) return base.GetRegionIndex(photon.DP.Position);
            

            // if photon is in layer and on boundary of bounding region, then neighbor is bounding region
            if (currentRegionIndex < _layerRegions.Count &&
                _boundingRegion.OnBoundary(photon.DP.Position)) return _boundingRegionExteriorIndex;

            // at this point on some internal boundary, possibilities include
            // 1) on layer region boundary away from bounding region
            // 2) in layer on inclusion entering inclusion
            // 3) in inclusion exiting into layer

            // check if on layer boundary, if so call base to get neighbor region index
            if (currentRegionIndex < _layerRegions.Count && 
                _layerRegions[currentRegionIndex].OnBoundary(photon.DP.Position)) 
                return base.GetNeighborRegionIndex(photon);

            // check if in inclusion and on boundary, then neighbor is surrounding layer
            if (currentRegionIndex >= _layerRegions.Count &&
                _inclusionRegions[currentRegionIndex - _layerRegions.Count].OnBoundary(photon.DP.Position))
            {
                return _layerRegionIndicesOfInclusion[currentRegionIndex - _layerRegions.Count];
            }

            // check if in layer and on boundary of inclusion, then neighbor is inclusion
            if (currentRegionIndex < _layerRegions.Count)
            {
                for (var i = 0; i < _inclusionRegions.Count; i++)
                {
                    if (_inclusionRegions[i].ContainsPosition(photon.DP.Position))
                        return _layerRegions.Count + i + 1; // +1 for bounding region exterior index
                }
            }
            return -1; // should never get here, but just in case, return -1 to indicate no neighbor found

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
            // check if photon on cylinder 
            var inclusionIndex = -1;
            for (var i = 0; i < _inclusionRegions.Count; i++)
            {
                if (_inclusionRegions[i].ContainsPosition(photon.DP.Position)) 
                    inclusionIndex = i; // +1 for bounding region exterior index
            }

            // Since this method is called by Photon and used in Optics/Fresnel, definition used
            // there calls for cos(theta) of normal to surface interface (normal to both sides).
            // This is why the Abs is taken
            if (inclusionIndex != -1)
                return Math.Abs(Direction.GetDotProduct( // Abs consistent with SingleInclusionTissue
                    photon.DP.Direction, _inclusionRegions[inclusionIndex].SurfaceNormal(photon.DP.Position)));


            // if not on any inclusion, must be on bounding region       
                return Math.Abs(Direction.GetDotProduct(
                    photon.DP.Direction, _boundingRegion.SurfaceNormal(photon.DP.Position)));
        }
    }
}
