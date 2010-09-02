using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissue.  Defines a tissue geometry comprised of an
    /// ellipsoid embedded within a voxel slab.
    /// </summary>
    public class SingleEllipsoidTissue : ITissue
    {
        private EllipsoidRegion _ellipsoidRegion;

        public SingleEllipsoidTissue(VoxelRegion slab, EllipsoidRegion ellipsoid)
        {
            Regions = new List<ITissueRegion>()
            { 
                slab, 
                ellipsoid,
            };
            _ellipsoidRegion = ellipsoid;
        }
        public SingleEllipsoidTissue() : this(
            new VoxelRegion(),
            new EllipsoidRegion()) {}

        public IList<ITissueRegion> Regions { get; set; }

        # region ISource Members
        public int GetRegionIndex(Position position)
        {
            if (_ellipsoidRegion.ContainsPosition(position))
                return 1;
            else
                return 0;
        }
        public bool OnDomainBoundary(Photon photon)
        {
            throw new NotImplementedException();
        }
        public int GetNeighborRegionIndex(Photon photon)
        {
            throw new NotImplementedException();
        }
        public double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ITissue Members


        public double GetDistanceToBoundary(Photon photon)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
