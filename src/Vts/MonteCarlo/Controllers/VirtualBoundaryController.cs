 using System.Collections.Generic;
 using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// Controller of virtual boundaries.  
    /// </summary>
    public class VirtualBoundaryController
    {
        /// <summary>
        /// virtual boundary controller
        /// </summary>
        /// <param name="virtualBoundaries">IList of virtual boundaries</param>
        public VirtualBoundaryController(
            IList<IVirtualBoundary> virtualBoundaries)
        {
            VirtualBoundaries = virtualBoundaries;
        }
        /// <summary>
        /// List of IVirtualBoundary.  All VBs handled by this controller.
        /// </summary>
        public IList<IVirtualBoundary> VirtualBoundaries { get; set; }

        /// <summary>
        /// Method to determine the distance to the closest VB in VirtualBoundaries list.
        /// </summary>
        /// <param name="dp">current PhotonDataPoint</param>
        /// <param name="distance">return: distance to closest VB</param>
        /// <returns>closest VB</returns>
        public IVirtualBoundary GetClosestVirtualBoundary(PhotonDataPoint dp, out double distance)
        {
            IVirtualBoundary vb = null;
            distance = double.PositiveInfinity;
            if (VirtualBoundaries == null || VirtualBoundaries.Count <= 0) return vb;
            foreach (var virtualBoundary in VirtualBoundaries)
            {
                // each VB takes direction of VB into consideration when determining distance
                var distanceToVb = virtualBoundary.GetDistanceToVirtualBoundary(dp);
                if (distanceToVb >= distance) continue;
                distance = distanceToVb;
                vb = virtualBoundary;
            }
            return vb;
        }

    }
}
