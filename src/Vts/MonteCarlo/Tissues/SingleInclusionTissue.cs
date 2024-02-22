using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Tissues
{   
    /// <summary>
    /// Implements ITissue.  Defines a tissue geometry comprised of an inclusion embedded within a layered slab.
    /// Note that many of the methods in this class are invoked by Photon class and Photon masterminds their returns.
    /// For example, when the photon is on the boundary of the layers or the inclusion, Photon determines whether
    /// in the critical angle and if so whether to reflect or refract, then invokes the methods below accordingly.
    /// </summary>
    public class SingleInclusionTissue : MultiLayerTissue, ITissue
    {
        private readonly ITissueRegion _inclusionRegion;
        private readonly int _inclusionRegionIndex;
        private readonly int _layerRegionIndexOfInclusion;

        /// <summary>
        /// Creates an instance of a SingleInclusionTissue
        /// </summary>
        /// <param name="inclusionRegion">The single inclusion (must be contained completely within a layer region)</param>
        /// <param name="layerRegions">The tissue layers</param>
        public SingleInclusionTissue(
            ITissueRegion inclusionRegion,
            IList<ITissueRegion> layerRegions)
            : base(layerRegions)
        {
            // overwrite the Regions property in the TissueBase class (will be called last in the most derived class)
            Regions = layerRegions.Concat(inclusionRegion).ToArray();

            _inclusionRegion = inclusionRegion;
            _inclusionRegionIndex = layerRegions.Count; // index is, by convention, after the layer region indices
            _layerRegionIndexOfInclusion = Enumerable.Range(0, layerRegions.Count)
                .FirstOrDefault(i => ((LayerTissueRegion)layerRegions[i]).ContainsPosition(_inclusionRegion.Center));
        }

        /// <summary>
        /// Creates a default instance of a SingleInclusionTissue
        /// </summary>
        public SingleInclusionTissue()
            : this(
                new EllipsoidTissueRegion(),
                new MultiLayerTissueInput().Regions) { }
        /// <summary>
        /// Method to get tissue region index of photon's current position
        /// </summary>
        /// <param name="position">photon Position</param>
        /// <returns>integer tissue region index</returns>
        public override int GetRegionIndex(Position position)
        {
            // if it's in the inclusion, return index of inclusion,
            // otherwise, call the layer method to determine
            return _inclusionRegion.ContainsPosition(position) ? _inclusionRegionIndex : base.GetRegionIndex(position);
        }

        // DC - worried that this is "uncombined" with GetDistanceToBoundary() from an efficiency standpoint
        // note, however that there are two overloads currently for RayIntersectBoundary, one that does extra work to calc distances
        /// <summary>
        /// method to get index of neighbor tissue region when photon on boundary of two regions
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>index of neighbor index</returns>
        public override int GetNeighborRegionIndex(Photon photon)
        {
            // first, check what region the photon is in
            var regionIndex = photon.CurrentRegionIndex;

            // if we're in the layer region of the inclusion, could be on boundary of layer
            if (regionIndex == _layerRegionIndexOfInclusion && 
                !Regions[_layerRegionIndexOfInclusion].OnBoundary(photon.DP.Position) )
            {
                return _inclusionRegionIndex;
            }

            return regionIndex == _inclusionRegionIndex ? _layerRegionIndexOfInclusion :
                // otherwise we can do this with the base class method
                base.GetNeighborRegionIndex(photon);
        }
        /// <summary>
        /// Method to get distance from current photon position and direction to boundary of region
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>distance to boundary</returns>
        public override double GetDistanceToBoundary(Photon photon)
        {
            // first, check what region the photon is in
            var regionIndex = photon.CurrentRegionIndex;

            if ((regionIndex != _layerRegionIndexOfInclusion) && (regionIndex != _inclusionRegionIndex))
                return base.GetDistanceToBoundary(photon);
            // check if current track will hit the inclusion boundary, returning the correct distance
            if (_inclusionRegion.RayIntersectBoundary(photon, out var distanceToBoundary))
            {
                return distanceToBoundary;
            }

            // otherwise, check that a projected track will hit the inclusion boundary
            var projectedPhoton = new Photon
            {
                DP = new PhotonDataPoint(photon.DP.Position, photon.DP.Direction, photon.DP.Weight,
                    photon.DP.TotalTime, photon.DP.StateFlag),
                S = 100
            };
            return _inclusionRegion.RayIntersectBoundary(projectedPhoton, out distanceToBoundary) ? distanceToBoundary :
                // if not hitting the inclusion, call the base (layer) method
                base.GetDistanceToBoundary(photon);
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
            // note that inner layer reflections handled by Photon.CrossRegionOrReflect by calling
            // _tissue.GetRefractedDirection
            if (base.OnDomainBoundary(currentPosition)) // OnDomainBoundary checks if on tissue boundary
            {
                return base.GetReflectedDirection(currentPosition, currentDirection);
            }

            // on boundary of inclusion
            if (_inclusionRegion.RegionOP.N == Regions[_layerRegionIndexOfInclusion].RegionOP.N)
            {
                return currentDirection;  // no refractive index mismatch
            }

            // reflection equation reflected = incident - 2(incident dot surfaceNormal)surfaceNormal
            var surfaceNormal = _inclusionRegion.SurfaceNormal(currentPosition);
            var currentDirDotNormal = Direction.GetDotProduct(currentDirection, surfaceNormal);
            var newX = currentDirection.Ux - 2 * currentDirDotNormal * surfaceNormal.Ux;
            var newY = currentDirection.Uy - 2 * currentDirDotNormal * surfaceNormal.Uy;
            var newZ = currentDirection.Uz - 2 * currentDirDotNormal * surfaceNormal.Uz;
            var norm = Math.Sqrt(newX * newX + newY * newY + newZ * newZ);
            return new Direction(newX / norm, newY / norm, newZ / norm);
        }

        /// <summary>
        /// Method that provides refracted direction when photon refracts through boundary.
        /// ref: Bram de Greve "Reflections and Refractions in Ray Tracing" dated 11/13/2006, off web not published
        /// </summary>
        /// <param name="currentPosition">Position</param>
        /// <param name="currentDirection">Direction</param>
        /// <param name="currentN">N of tissue photon is exiting</param>
        /// <param name="nextN">N of tissue photon is entering</param>
        /// <param name="cosThetaSnell">cosine theta=normal dot exiting direction due to Snell's law</param>
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

            if (currentN == nextN) return currentDirection; // no refractive index mismatch

            var normal = _inclusionRegion.SurfaceNormal(currentPosition);
            var cosTheta1 = Direction.GetDotProduct(currentDirection, normal);
            var nRatio = currentN / nextN;
            var sinTheta2Squared = nRatio * nRatio * (1 - cosTheta1 * cosTheta1);
            var factor = nRatio * cosTheta1 - Math.Sqrt(1 - sinTheta2Squared);
            var newX = nRatio * currentDirection.Ux + factor * normal.Ux;
            var newY = nRatio * currentDirection.Uy + factor * normal.Uy;
            var newZ = nRatio * currentDirection.Uz + factor * normal.Uz;
            var norm = Math.Sqrt(newX * newX + newY * newY + newZ * newZ);
            return new Direction(newX / norm, newY / norm, newZ / norm);

            // refraction equations in ref
            // where theta1 and theta2 are angles relative to normal
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
            return base.OnDomainBoundary(photon.DP.Position)
                ? base.GetAngleRelativeToBoundaryNormal(photon)
                : Math.Abs(Direction.GetDotProduct( // need Abs here for unit tests but not sure correct
                    photon.DP.Direction, _inclusionRegion.SurfaceNormal(photon.DP.Position)));
        }
    }
}
