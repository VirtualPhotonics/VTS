using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissue.  All "InclusionTissue" classes define processing for those tissues that use
    /// this class to get created. Defines a tissue geometry comprised of a list of inclusions
    /// embedded within *different* layers of a layered slab.  Note that many of the methods in this class are
    /// invoked by Photon class and Photon masterminds their returns.  For example, when the photon is
    /// on the boundary of the layers or the inclusions, Photon determines whether in the critical angle
    /// and if so whether to reflect or refract, then invokes the methods below accordingly.
    /// </summary>
    public class MultiInclusionTissue : MultiLayerTissue, ITissue
    {
        private readonly IList<LayerTissueRegion> _layerRegions;
        // the following two will have the same list length and map to each other, i.e. for inclusionRegion[i],
        // the corresponding layer region is _layerRegions[_layerRegionIndicesOfInclusion[i]]
        private readonly IList<ITissueRegion> _inclusionRegions;
        private readonly IList<int> _layerRegionIndicesOfInclusion;

        /// <summary>
        /// Creates an instance of a MultiInclusionTissue
        /// </summary>
        /// <param name="inclusionRegions">list of inclusion regions</param>
        /// <param name="layerRegions">list of layer regions</param>
        /// <remarks>air above and below tissue needs to be specified for a slab geometry</remarks>
        public MultiInclusionTissue(
            IList<ITissueRegion> inclusionRegions,
            IList<ITissueRegion> layerRegions)
            : base(layerRegions)
        {
            // overwrite the Regions property in the TissueBase class (will be called last in the most derived class)
            Regions = layerRegions.Concat(inclusionRegions).ToArray();

            _layerRegions = layerRegions.Select(r => (LayerTissueRegion)r).ToList();
            _inclusionRegions = inclusionRegions.Select(r => r).ToList();
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
        /// Creates a default instance of a MultiInclusionTissue based on a homogeneous medium slab geometry
        /// and discrete absorption weighting
        /// </summary>
        public MultiInclusionTissue()
            : this(
                [
                    new InfiniteCylinderTissueRegion(),
                    new InfiniteCylinderTissueRegion()
                ],
        new MultiLayerTissueInput().Regions)
        {
        }

        /// <summary>
        /// Method to determine region index of region photon is currently in
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>integer index of region position is in</returns>
        public override int GetRegionIndex(Position position)
        {
            // check layers first because inclusions are embedded in layers and could be in both
            var index = -1;
            // use LayerTissueRegion to determine which region photon resides
            for (var i = 0; i < _layerRegions.Count; i++)
            {
                if (_layerRegions[i].ContainsPosition(position))
                {
                    index = i; // this gets set but could get overwritten below if also in inclusion
                }
            }
            // use InclusionTissueRegion to determine if within one of the inclusions.
            // Inclusions are indexed after the layer regions, so add _layerRegions.Count to index
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
            // first check if in layer or inclusion

            // going "up" in negative z-direction
            var goingUp = photon.DP.Direction.Uz < 0.0;

            var distanceToLayer = double.PositiveInfinity;
            var distanceToInclusion = double.PositiveInfinity;

            // get layer index of photon (could be in inclusion in layer)
            if (photon.CurrentRegionIndex < _layerRegions.Count) // photon in layer
            {
                // calculate distance to boundary based on z-projection of photon trajectory
                distanceToLayer =
                    goingUp
                        ? (_layerRegions[photon.CurrentRegionIndex].ZRange.Start - photon.DP.Position.Z) /
                          photon.DP.Direction.Uz
                        : (_layerRegions[photon.CurrentRegionIndex].ZRange.Stop - photon.DP.Position.Z) /
                          photon.DP.Direction.Uz;
            }
            else // photon in some inclusion 
            {
                // check distance to boundary of inclusion photon is currently in
                var inclusionRegionIndex = photon.CurrentRegionIndex - _layerRegions.Count;

                // check that a projected track will hit one of the inclusions
                var projectedPhoton = new Photon
                {
                    DP = new PhotonDataPoint(photon.DP.Position, photon.DP.Direction, photon.DP.Weight,
                        photon.DP.TotalTime, photon.DP.StateFlag),
                    S = 100
                };
                _inclusionRegions[inclusionRegionIndex].RayIntersectBoundary(projectedPhoton, out var distToInclusion);
                distanceToInclusion = distToInclusion;
            }

            return distanceToInclusion < distanceToLayer ? distanceToInclusion : distanceToLayer;
        }

        /// <summary>
        /// Method to determine if on boundary of tissue, i.e. at tissue/air interface
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean indicating whether on boundary of domain or not</returns>
        public override bool OnDomainBoundary(Position position)
        {
            // Domain boundary: so check layer boundary
            // this code assumes that the first and last layer is air
            return
                position.Z < 1e-10 ||
                Math.Abs(position.Z - _layerRegions[^1].ZRange.Start) < 1e-10;
        }

        /// <summary>
        /// Method to determine index of region photon is about to enter
        /// </summary>
        /// <param name="photon">photon info including position and direction</param>
        /// <returns>region index</returns>
        public override int GetNeighborRegionIndex(Photon photon)
        {
            // on some boundary at this point, possibilities include
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

                return currentRegionIndex == _layerRegionIndicesOfInclusion[j] ? _layerRegions.Count + j :  // entering inclusion
                    _layerRegionIndicesOfInclusion[j]; // exiting into surrounding layer region
            }

            return -1; // should never get here, but just in case
        }

        /// <summary>
        /// Method to determine photon state type of photon exiting tissue boundary
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
        /// Method to determine direction of reflected photon
        /// ref: Bram de Greve "Reflections and Refractions in Ray Tracing" dated 11/13/2006, off web not published
        /// </summary>
        /// <param name="currentPosition">current position of photon</param>
        /// <param name="currentDirection">current direction of photon</param>
        /// <returns>direction of reflected input direction</returns>
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

            var inclusionIndex = 0;
            // on boundary of an inclusion, check which one
            for (var i = 0; i < _inclusionRegions.Count; i++)
            {
                if (_inclusionRegions[i].ContainsPosition(currentPosition)) inclusionIndex = i;
            }
            if (Math.Abs(_inclusionRegions[inclusionIndex].RegionOP.N - 
                         Regions[_layerRegionIndicesOfInclusion[inclusionIndex]].RegionOP.N) < 1e-6)
            {
                return currentDirection;  // no refractive index mismatch
            }

            // reflection equation reflected = incident - 2(incident dot surfaceNormal)surfaceNormal
            var surfaceNormal = _inclusionRegions[inclusionIndex].SurfaceNormal(currentPosition);

            var currentDirDotNormal = Direction.GetDotProduct(currentDirection, surfaceNormal);
            var newX = currentDirection.Ux - 2 * currentDirDotNormal * surfaceNormal.Ux;
            var newY = currentDirection.Uy - 2 * currentDirDotNormal * surfaceNormal.Uy;
            var newZ = currentDirection.Uz - 2 * currentDirDotNormal * surfaceNormal.Uz;
            var norm = Math.Sqrt(newX * newX + newY * newY + newZ * newZ);
            return new Direction(newX / norm, newY / norm, newZ / norm);
        }

        /// <summary>
        /// Method to determine refracted direction of photon
        /// ref: Bram de Greve "Reflections and Refractions in Ray Tracing" dated 11/13/2006, off web not published
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
            // needs to call MultiLayerTissue when crossing top and bottom layer
            if (base.OnDomainBoundary(currentPosition))
            {
                return base.GetRefractedDirection(currentPosition, currentDirection, currentN, nextN, cosThetaSnell);
            }

            if (Math.Abs(currentN - nextN) < 1e-6) return currentDirection; // no refractive index mismatch

            var inclusionIndex = 0;
            // on boundary of an inclusion, check which one
            for (var i = 0; i < _inclusionRegions.Count; i++)
            {
                if (_inclusionRegions[i].ContainsPosition(currentPosition)) inclusionIndex = i;
            }
            // must be on inclusions for now no reflection 
            // Theta1 = incident, Theta2 = transmitted relative to normal
            var normal = _inclusionRegions[inclusionIndex].SurfaceNormal(currentPosition);
            var cosTheta1 = Direction.GetDotProduct(currentDirection, normal);
            // the following code follows de Greve fairly closely but needed following 2 lines
            // from https://stackoverflow.com/questions/26087106/refraction-in-raytracing
            if (cosTheta1 > 0.0) normal = new Direction(-normal.Ux, -normal.Uy, -normal.Uz);
            else cosTheta1 = -cosTheta1;
            var nRatio = currentN / nextN;
            var sinTheta2Squared = nRatio * nRatio * (1 - cosTheta1 * cosTheta1);
            // check for internal reflection
            if (currentN > nextN && sinTheta2Squared > 1.0) return GetReflectedDirection(currentPosition, currentDirection);
            var factor = nRatio * cosTheta1 - Math.Sqrt(1 - sinTheta2Squared);
            // following is Eq.(21) of de Greve
            var newX = nRatio * currentDirection.Ux + factor * normal.Ux;
            var newY = nRatio * currentDirection.Uy + factor * normal.Uy;
            var newZ = nRatio * currentDirection.Uz + factor * normal.Uz;
            var norm = Math.Sqrt(newX * newX + newY * newY + newZ * newZ);
            return new Direction(newX / norm, newY / norm, newZ / norm);
        }

        /// <summary>
        /// This gets called by Photon/CrossOrReflect upon crossing any tissue region (not
        /// just domain boundaries).  Method determines cosine of the photon direction and
        /// surface normal. 
        /// </summary>
        /// <param name="photon">photon</param>
        /// <returns>Uz=cos(theta)</returns>
        public new double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            // needs to call MultiLayerTissue when crossing top and bottom layer
            if (base.OnDomainBoundary(photon.DP.Position))
            {
                return base.GetAngleRelativeToBoundaryNormal(photon);
            }
            // otherwise determine which cylinder photon is on
            var inclusionIndex = 0;
            for (var i = 0; i < _inclusionRegions.Count; i++)
            {
                if (_inclusionRegions[i].OnBoundary(photon.DP.Position)) inclusionIndex = i;
            }
            // Since this method is called by Photon and used in Optics/Fresnel, definition used
            // there calls for cos(theta) of normal to surface interface (normal to both sides).
            // This is why the Abs is taken.
            return Math.Abs(Direction.GetDotProduct( // Abs consistent with SingleInclusionTissue
                photon.DP.Direction, _inclusionRegions[inclusionIndex].SurfaceNormal(photon.DP.Position)));

        }
    }
}
