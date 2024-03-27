using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// The <see cref="Tissues"/> namespace contains the Monte Carlo tissue classes
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Implements ITissue.  Defines a tissue geometry comprised of a layered slab bounded laterally by a TissueRegion.
    /// </summary>
    public class BoundedTissue : MultiLayerTissue, ITissue
    {
        private readonly ITissueRegion _boundingRegion;
        private readonly IList<ITissueRegion> _layers;
        private readonly int _boundingRegionExteriorIndex;
        private readonly IList<int> _tissueLayersInsideBoundIndices;

        /// <summary>
        /// Creates an instance of a BoundedTissue
        /// </summary>
        /// <param name="boundingRegion">Tissue region defining later extent of tissue (must span top to bottom of tissue layers)</param>
        /// <param name="layerRegions">The tissue layers</param>
        public BoundedTissue(
            ITissueRegion boundingRegion,
            IList<ITissueRegion> layerRegions)
            : base(layerRegions)
        {
            // boundingRegionExteriorIndex is the area *outside* of the bounding region
            _boundingRegionExteriorIndex = layerRegions.Count; // index is, by convention, after the layer region indices
            // overwrite the Regions property in the TissueBase class (will be called last in the most derived class)
            // the concat is with the outside of the bounding region by convention
            Regions = layerRegions.Concat(boundingRegion).ToArray();
            _layers = layerRegions;
            _boundingRegion = boundingRegion;
            // create list of tissue layers inside bounding region, assumes air-multilayer-air tissue
            _tissueLayersInsideBoundIndices = new List<int>();
            for (var i = 1; i < layerRegions.Count - 1; i++)
            {
                _tissueLayersInsideBoundIndices.Add(i);
            }
        }

        /// <summary>
        /// Creates a default instance of a BoundingTissue
        /// </summary>
        public BoundedTissue()
            : this(
                new CaplessCylinderTissueRegion(),
                new MultiLayerTissueInput().Regions) { }
        /// <summary>
        /// method to get tissue region index of photon's current position
        /// </summary>
        /// <param name="position">photon Position</param>
        /// <returns>integer tissue region index</returns>
        public override int GetRegionIndex(Position position)
        {
            // if it's in the bounding region, return bounding region index,
            // otherwise, call the layer method to determine
            return !_boundingRegion.ContainsPosition(position)  ? _boundingRegionExteriorIndex : base.GetRegionIndex(position);
        }

        /// <summary>
        /// Method to get distance from current photon position and direction to boundary of region
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>distance to boundary</returns>
        public override double GetDistanceToBoundary(Photon photon)
        {
            // if we're inside or outside the bounding region, distance is either to bounding region or
            // edge of layer

            // check that a projected track will hit bounding volume, if so, check layers and find minimum
            // if not, check distance to layers
            var projectedPhoton = new Photon
            {
                DP = new PhotonDataPoint(photon.DP.Position, photon.DP.Direction, photon.DP.Weight,
                    photon.DP.TotalTime, photon.DP.StateFlag),
                S = 100
            };
            if (!_boundingRegion.RayIntersectBoundary(projectedPhoton, out var distanceToBoundingBoundary))
                return base.GetDistanceToBoundary(photon);

            // check if will hit layer boundary
            var distanceToLayerBoundary = base.GetDistanceToBoundary(photon);
            return !(distanceToBoundingBoundary < distanceToLayerBoundary) 
                ? distanceToLayerBoundary 
                : distanceToBoundingBoundary;
        }

        /// <summary>
        /// Method to determine if on boundary of tissue, i.e. at tissue/air interface
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean indicating whether on boundary or not</returns>
        public override bool OnDomainBoundary(Position position)
        {
            // this code assumes that the first and last layer is air
            return _boundingRegion.OnBoundary(position) ||
                position.Z < 1e-10 ||
                (Math.Abs(position.Z - ((LayerTissueRegion)_layers.Last()).ZRange.Start) < 1e-10);
        }

        /// <summary>
        /// Method to get index of neighbor tissue region when photon on boundary of two regions
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>index of neighbor index</returns>
        public override int GetNeighborRegionIndex(Photon photon)
        {
            // first, check what region the photon is in
            var regionIndex = photon.CurrentRegionIndex;

            // if we're not on the boundary of the bounding region, get layer neighbor
            if (!_boundingRegion.OnBoundary(photon.DP.Position)) return base.GetNeighborRegionIndex(photon);

            // if we are on boundary of bounding region, determine neighbor index
            return regionIndex != _boundingRegionExteriorIndex
                ? _boundingRegionExteriorIndex
                : base.GetRegionIndex(photon.DP.Position);

            // else inside bounding region so return outside bounding region index
            // else on layer boundary so return layer neighbor
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
            return Math.Abs(position.Z - ((LayerTissueRegion)_layers.Last()).ZRange.Start) < 1e-10
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
            // needs to call MultiLayerTissue when crossing top and bottom layer
            if (base.OnDomainBoundary(currentPosition))
            {
                return base.GetReflectedDirection(currentPosition, currentDirection);
            }

            // on tissue layer and bounding region border
            // determine which layer 
            var layerIndex = base.GetRegionIndex(currentPosition);
            // the following assumes air-multilayers-air tissue
            if (_boundingRegion.RegionOP.N == Regions[layerIndex].RegionOP.N)
            {
                return currentDirection;  // no refractive index mismatch
            }

            // reflection equation reflected = incident - 2(incident dot surfaceNormal)surfaceNormal
            var surfaceNormal = _boundingRegion.SurfaceNormal(currentPosition);
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
            // needs to call MultiLayerTissue when crossing top and bottom layer
            if (base.OnDomainBoundary(currentPosition))
            {
                return base.GetRefractedDirection(currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            }
            {
                if (currentN == nextN)
                {
                    return currentDirection;  // no refractive index mismatch
                }

                // refraction equations in ref
                // where theta1 and theta2 are angles relative to normal
                var normal = _boundingRegion.SurfaceNormal(currentPosition);
                var cosTheta1 = Direction.GetDotProduct(currentDirection, normal);
                var nRatio = currentN / nextN;
                var sinTheta2Squared = nRatio * nRatio * (1 - cosTheta1 * cosTheta1);
                var factor = nRatio * cosTheta1 - Math.Sqrt(1 - sinTheta2Squared);
                var newX = nRatio * currentDirection.Ux + factor * normal.Ux;
                var newY = nRatio * currentDirection.Uy + factor * normal.Uy;
                var newZ = nRatio * currentDirection.Uz + factor * normal.Uz;
                var norm = Math.Sqrt(newX * newX + newY * newY + newZ * newZ);
                return new Direction(newX / norm, newY / norm, newZ / norm);
            }
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
