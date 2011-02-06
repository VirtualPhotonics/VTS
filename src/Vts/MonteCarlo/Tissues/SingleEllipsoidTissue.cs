using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissue.  Defines a tissue geometry comprised of an
    /// ellipsoid embedded within a voxel slab.
    /// </summary>
    public class SingleEllipsoidTissue : TissueBase
    {
        private EllipsoidRegion _ellipsoidRegion;

        public SingleEllipsoidTissue(VoxelRegion slab, EllipsoidRegion ellipsoid, AbsorptionWeightingType absorptionWeightingType)
            : base(new ITissueRegion[] { slab, ellipsoid }, absorptionWeightingType)
        {
            _ellipsoidRegion = ellipsoid;
        }

        public SingleEllipsoidTissue()
            : this(
                new VoxelRegion(),
                new EllipsoidRegion(),
                AbsorptionWeightingType.Discrete) { }

        public override int GetRegionIndex(Position position)
        {
            return _ellipsoidRegion.ContainsPosition(position) ? 1 : 0;
        }

        public override bool OnDomainBoundary(Photon photon)
        {
            throw new NotImplementedException();
        }

        public override int GetNeighborRegionIndex(Photon photon)
        {
            throw new NotImplementedException();
        }

        public override double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            throw new NotImplementedException();
        }

        public override PhotonStateType GetPhotonDataPointStateOnExit(Position position)
        {
            return position.Z < 1e-10 ? PhotonStateType.ExitedOutTop : PhotonStateType.ExitedOutBottom;
        }

        public override double GetDistanceToBoundary(Photon photon)
        {
            throw new NotImplementedException();
        }

        public override Direction GetReflectedDirection(
            Position currentPosition, 
            Direction currentDirection)
        {
            throw new NotImplementedException();
        }

        public override Direction GetRefractedDirection(
            Position currentPosition, 
            Direction currentDirection,
            double nCurrent, 
            double nNext, 
            double cosThetaSnell)
        {
            throw new NotImplementedException();
        }
    }
}
