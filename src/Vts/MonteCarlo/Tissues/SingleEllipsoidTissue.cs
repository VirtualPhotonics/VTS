using System;
using System.Linq;
using System.Collections.Generic;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissue.  Defines a tissue geometry comprised of an
    /// ellipsoid embedded within a layered slab.
    /// </summary>
    public class SingleEllipsoidTissue : MultiLayerTissue
    {
        private EllipsoidRegion _ellipsoidRegion;
        private int _ellipsoidRegionIndex;
        private int _layerRegionIndexOfElipsoid;

        /// <summary>
        /// Creates an instance of a SingleEllipsoidTissue
        /// </summary>
        /// <param name="ellipsoidRegion">The single ellipsoid (must be contained completely within a layer region)</param>
        /// <param name="layerReions">The tissue layers</param>
        /// <param name="absorptionWeightingType">The type of absorption weighting</param>
        /// <param name="phaseFunctionType">The type of phase function</param>
        public SingleEllipsoidTissue(
            ITissueRegion ellipsoidRegion,
            IList<ITissueRegion> layerRegions,
            AbsorptionWeightingType absorptionWeightingType,
            PhaseFunctionType phaseFunctionType)
            : base(layerRegions, 
                   absorptionWeightingType, 
                   phaseFunctionType)
        {
            // overwrite the Regions property in the TissueBase class (will be called last in the most derived class)
            Regions = layerRegions.Concat(ellipsoidRegion).ToArray();
            RegionScatterLengths = Regions.Select(region => region.RegionOP.GetScatterLength(absorptionWeightingType)).ToArray();

            _ellipsoidRegion = (EllipsoidRegion)ellipsoidRegion;
            _ellipsoidRegionIndex = layerRegions.Count; // index is, by convention, after the layer region indices
            _layerRegionIndexOfElipsoid = Enumerable.Range(0, layerRegions.Count)
                .FirstOrDefault(i => ((LayerRegion) layerRegions[i]).ContainsPosition(_ellipsoidRegion.Center));
        }

        /// <summary>
        /// Creates a default instance of a SingleEllipsoidTissue
        /// </summary>
        public SingleEllipsoidTissue()
            : this(
                new EllipsoidRegion(),
                new MultiLayerTissueInput().Regions,
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein) { }

        public override int GetRegionIndex(Position position)
        {
            // if it's in the ellipse, return "3", otherwise, call the layer method to determine
            return _ellipsoidRegion.ContainsPosition(position) ? _ellipsoidRegionIndex : base.GetRegionIndex(position);
        }

        // todo: DC - worried that this is "uncombined" with GetDistanceToBoundary() from an efficiency standpoint
        // note, however that there are two overloads currently for RayIntersectEllipse, one that does extra work to calc distances
        public override int GetNeighborRegionIndex(Photon photon)
        {
            // first, check what layer the photon is in
            int layerRegionIndex = photon.CurrentRegionIndex;
            
            // if we're outside the layer containing the ellipsoid, then just call the base method
            if (layerRegionIndex != _layerRegionIndexOfElipsoid)
            {
                return base.GetNeighborRegionIndex(photon);
            }

            // if we're actually inside the ellipsoid
            if (_ellipsoidRegion.ContainsPosition(photon.DP.Position))
            {
                // then the neighbor region is the layer containing the current photon position
                return layerRegionIndex;
            }

            // otherwise, it depends on which direction the photon's pointing from within the layer

            // if the ray intersects the ellipsoid, the neighbor is the ellipsoid
            if( _ellipsoidRegion.RayIntersectEllipsoid(photon) )
            {
                return _ellipsoidRegionIndex;
            }

            // otherwise we can do this with the base class method
            return base.GetNeighborRegionIndex(photon);
        }

        public override double GetDistanceToBoundary(Photon photon)
        {
            // first, check what layer the photon is in
            int layerRegionIndex = photon.CurrentRegionIndex;

            // if we're outside the layer containing the ellipsoid, then just call the base (layer) method
            if (layerRegionIndex != _layerRegionIndexOfElipsoid)
            {
                return base.GetDistanceToBoundary(photon);
            }

            // otherwise, check if we'll hit the ellipsoid, returning the correct distance
            double distanceToBoundary;
            if (_ellipsoidRegion.RayIntersectEllipsoid(photon, out distanceToBoundary))
            {
                return distanceToBoundary;
            }

            // if not hitting the ellipsoid, call the base (layer) method
            return base.GetDistanceToBoundary(photon);
        }

        public override Direction GetReflectedDirection(
            Position currentPosition,
            Direction currentDirection)
        {
            // needs to call MultiLayerTissue when crossing top and bottom layer
            if (base.OnDomainBoundary(currentPosition))
            {
                return base.GetReflectedDirection(currentPosition, currentDirection);
            }
            else
            {
                return currentDirection;
            }
            //throw new NotImplementedException(); // hopefully, this won't happen when the tissue ellipsoid is index-matched
        }

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
            else
            {
                return currentDirection;
            }
                //throw new NotImplementedException(); // hopefully, this won't happen when the tissue ellipsoid is index-matched
        }
    }
}
