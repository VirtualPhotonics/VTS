using System;
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
        private IList<ITissueRegion> _layers;
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
            // the concat is with the outside of the bounding region by convention
            Regions = layerRegions.Concat(boundingRegion).ToArray();
            _layers = layerRegions;
            _boundingRegion = boundingRegion;
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
            // if it's in the bounding region, return "3", otherwise, call the layer method to determine
            return !_boundingRegion.ContainsPosition(position)  ? _boundingRegionExteriorIndex : base.GetRegionIndex(position);
        }

        /// <summary>
        /// method to get distance from current photon position and direction to boundary of region
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>distance to boundary</returns>
        public override double GetDistanceToBoundary(Photon photon)
        {
            // if we're inside or outside the bounding region, distance is either to bounding region or
            // edge of layer

            // check if current track will hit the bounding boundary
            double distanceToBoundingBoundary;
            if (_boundingRegion.RayIntersectBoundary(photon, out distanceToBoundingBoundary))
            {
                // check if will hit layer boundary
                double distanceToLayerBoundary = base.GetDistanceToBoundary(photon);
                if (distanceToBoundingBoundary < distanceToLayerBoundary)
                {
                    return distanceToBoundingBoundary;
                }
                return distanceToLayerBoundary;
            }
            
            // if not hitting the inclusion, call the base (layer) method
            return base.GetDistanceToBoundary(photon);
        }
        /// <summary>
        /// method to determine if on boundary of tissue, i.e. at tissue/air interface
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns></returns>
        public virtual bool OnDomainBoundary(Position position)
        {
            // this code assumes that the first and last layer is air
            return _boundingRegion.OnBoundary(position) ||
                position.Z < 1e-10 ||
                (Math.Abs(position.Z - ((LayerTissueRegion)_layers.Last()).ZRange.Start) < 1e-10);
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
            if (Math.Abs(position.Z - ((LayerTissueRegion)_layers.Last()).ZRange.Start) < 1e-10)
            {
                return PhotonStateType.PseudoTransmittedTissueBoundary;
            }
            return PhotonStateType.PseudoBoundingVolumeTissueBoundary;
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
