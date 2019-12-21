using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{   
    /// <summary>
    /// Implements ITissue.  Defines a tissue geometry comprised of a layered slab bounded laterally by a TissueRegion.
    /// </summary>
    public class BoundedTissue : MultiLayerTissue, ITissue
    {
        private ITissueRegion _boundingRegion;
        private int _boundingRegionExteriorIndex;

        /// <summary>
        /// Creates an instance of a SingleInclusionTissue
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
            // the concatonation is with the outside of the bounding region by convention
            Regions = layerRegions.Concat(boundingRegion).ToArray();

            _boundingRegion = boundingRegion;
        }

        /// <summary>
        /// Creates a default instance of a BoundingTissue
        /// </summary>
        public BoundedTissue()
            : this(
                new CylinderTissueRegion(),
                new MultiLayerTissueInput().Regions) { }
        /// <summary>
        /// method to get tissue region index of photon's current position
        /// </summary>
        /// <param name="position">photon Position</param>
        /// <returns>integer tissue region index</returns>
        public override int GetRegionIndex(Position position)
        {
            // if it's in the inclusion, return "3", otherwise, call the layer method to determine
            return !_boundingRegion.ContainsPosition(position)  ? _boundingRegionExteriorIndex : base.GetRegionIndex(position);
        }
        
        /// <summary>
        /// method to get index of neighbor tissue region when photon on boundary of two regions
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>index of neighbor index</returns>
        public override int GetNeighborRegionIndex(Photon photon)
        {
            // first, check what region the photon is in
            int regionIndex = photon.CurrentRegionIndex;

            // if we're on the boundary of the bounding region
            if (_boundingRegion.OnBoundary(photon.DP.Position))
            {
                //  and outside bounding region then neighbor is tissue layer
                if (regionIndex == _boundingRegionExteriorIndex)
                {
                    return base.GetRegionIndex(photon.DP.Position);
                }

                // else inside bounding region so return outside bounding region index
                return _boundingRegionExteriorIndex;
            }
            // else on layer boundary so return layer neighbor
            return base.GetNeighborRegionIndex(photon);
        }
        /// <summary>
        /// method to get distance from current photon position and direction to boundary of region
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>distance to boundary</returns>
        public override double GetDistanceToBoundary(Photon photon)
        {
            // first, check what region the photon is in
            int regionIndex = photon.CurrentRegionIndex;

            // if we're outside the bounding region distance is either to bounding region or top or bottom of tissue
            if (regionIndex == _boundingRegionExteriorIndex)
            {
                // check if current track will hit the inclusion boundary, returning the correct distance
                double distanceToBoundingBoundary;
                if (_boundingRegion.RayIntersectBoundary(photon, out distanceToBoundingBoundary))
                {
                    double distanceToTissueBoundary = base.GetDistanceToBoundary(photon);
                    if (distanceToBoundingBoundary < distanceToTissueBoundary)
                    {
                        return distanceToBoundingBoundary;
                    }
                    return distanceToTissueBoundary;
                }
            }
            // if not hitting the inclusion, call the base (layer) method
            return base.GetDistanceToBoundary(photon);
        }
        /// <summary>
        /// method that provides reflected direction when phton reflects off boundary
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
            else // currently reflection/refraction not performed on bounding region
            {
                return currentDirection;
            }
            //throw new NotImplementedException(); // hopefully, this won't happen when the tissue inclusion is index-matched
        }
        /// <summary>
        /// method that provides refracted direction when phton refracts off boundary
        /// </summary>
        /// <param name="currentPosition">Position</param>
        /// <param name="currentDirection">Direction</param>
        /// <returns>new Direction</returns>
        public override Direction GetRefractedDirection(
            Position currentPosition,
            Direction currentDirection,
            double nCurrent,
            double nNext,
            double cosThetaSnell)
        {
            // needs to call MultiLayerTissue when crossing top and bottom layer
            if (base.OnDomainBoundary(currentPosition))
            {
                return base.GetRefractedDirection(currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            }
            else // currently reflection/refraction not performed on bounding region
            {
                return currentDirection;
            }
            //throw new NotImplementedException(); // hopefully, this won't happen when the tissue inclusion is index-matched
        }
    }
}
