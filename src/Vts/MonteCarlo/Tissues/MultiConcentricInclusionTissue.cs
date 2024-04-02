using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissue.  All "InclusionTissue" classes define processing for those tissues that use
    /// this class to get created. Defines a tissue geometry comprised of a list of concentric inclusions
    /// (of same type) embedded within a layered slab.  Note that many of the methods in this class are
    /// invoked by Photon class and Photon masterminds their returns.  For example, when the photon is
    /// on the boundary of the layers or the inclusions, Photon determines whether in the critical angle
    /// and if so whether to reflect or refract, then invokes the methods below accordingly.
    /// This assumes inclusions are concentric and lie entirely within a single layer of tissue.
    /// </summary>
    public class MultiConcentricInclusionTissue : MultiLayerTissue, ITissue
    {
        private readonly IList<LayerTissueRegion> _layerRegions;
        private readonly IList<ITissueRegion> _inclusionRegions;
        private readonly int _layerRegionIndexOfInclusion;

        /// <summary>
        /// Creates an instance of a MultiConcentricInclusionTissue
        /// </summary>
        /// <param name="inclusionRegions">list of concentric inclusion regions ordered largest to smallest</param>
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
            _inclusionRegions = inclusionRegions.Select(r => r).ToList();
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
        /// Method to determine region index of region photon is currently in
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
                    index = i; // this gets set but could get overwritten below if also in inclusion
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

            // check that a projected track will hit one of the inclusions
            var projectedPhoton = new Photon
            {
                DP = new PhotonDataPoint(photon.DP.Position, photon.DP.Direction, photon.DP.Weight,
                    photon.DP.TotalTime, photon.DP.StateFlag),
                S = 100 
            };
            foreach (var inclusionRegion in _inclusionRegions)
            {
                inclusionRegion.RayIntersectBoundary(projectedPhoton, out var distToInclusion);
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
                Math.Abs(position.Z - _layerRegions[^1].ZRange.Start) < 1e-10;
        }

        /// <summary>
        /// method to determine index of region photon is about to enter
        /// </summary>
        /// <param name="photon">photon info including position and direction</param>
        /// <returns>region index</returns>
        public override int GetNeighborRegionIndex(Photon photon)
        {
            // on some boundary at this point, possibilities include
            // 1) in layer of inclusion entering inclusion
            // 2) in outer inclusion entering layer of inclusion
            // 3) in inner inclusion entering neighbor inclusion
            // 4) on layer region boundary
            // first, check what region the photon is in
            var regionIndex = photon.CurrentRegionIndex;

            // if we're in the layer region of the outermost inclusion and not on boundary of layer
            // then on boundary of outermost inclusion
            if (regionIndex == _layerRegionIndexOfInclusion &&
                !Regions[_layerRegionIndexOfInclusion].OnBoundary(photon.DP.Position))
            {
                return _layerRegions.Count;  // index of outer inclusion
            }

            // check if in outermost inclusion
            Direction surfaceNormal;
            if (regionIndex == _layerRegions.Count) // index of outer inclusion
            {
                surfaceNormal = Regions[regionIndex].SurfaceNormal(photon.DP.Position);
                if (Direction.GetDotProduct(photon.DP.Direction, surfaceNormal) > 0)
                    return _layerRegionIndexOfInclusion;
                else
                    return regionIndex + 1;
            }

            // check if in innermost inclusion
            if (regionIndex == Regions.Count - 1) return regionIndex - 1;

            // else if in an inner inclusion but not outermost or innermost
            if (regionIndex <= _layerRegions.Count - 1) return base.GetNeighborRegionIndex(photon); // photon on one of inclusions
            // dot product with surface normal will tell if outgoing or incoming
            surfaceNormal = Regions[regionIndex].SurfaceNormal(photon.DP.Position);
            if (Direction.GetDotProduct(photon.DP.Direction, surfaceNormal) > 0)
                return regionIndex - 1;
            return regionIndex + 1;

            // otherwise return neighbor layer index
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
                         Regions[_layerRegionIndexOfInclusion].RegionOP.N) < 1e-6)
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
