 using System.Collections.Generic;
 using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// Controller of virtual boundaries.  
    /// </summary>
    public class VirtualBoundaryController
    {
        private IList<IVirtualBoundary> _virtualBoundaries;
        /// <summary>
        /// virtual boundary controller
        /// </summary>
        /// <param name="virtualBoundaries">IList of virtual boundaries</param>
        public VirtualBoundaryController(
            IList<IVirtualBoundary> virtualBoundaries)
        {
            _virtualBoundaries = virtualBoundaries;
        }
        /// <summary>
        /// List of IVirtualBoundary.  All VBs handled by this controller.
        /// </summary>
        public IList<IVirtualBoundary> VirtualBoundaries { get { return _virtualBoundaries; } set { _virtualBoundaries = value; } }

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
            if (_virtualBoundaries != null && _virtualBoundaries.Count > 0)
            {
                foreach (var virtualBoundary in _virtualBoundaries)
                {
                    // each VB takes direction of VB into consideration when determining distance
                    var distanceToVB = virtualBoundary.GetDistanceToVirtualBoundary(dp);
                    if (distanceToVB < distance)
                    {
                        distance = distanceToVB;
                        vb = virtualBoundary;
                    }
                }
            }
            return vb;
        }

    }
}
