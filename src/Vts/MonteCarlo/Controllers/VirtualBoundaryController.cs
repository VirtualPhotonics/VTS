using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.VirtualBoundaries;

namespace Vts.MonteCarlo.Controllers
{
    public class VirtualBoundaryController 
    {
        private static IList<IVirtualBoundary> _virtualBoundaries;

        public VirtualBoundaryController(
            IList<IVirtualBoundary> virtualBoundaries)
        {
            _virtualBoundaries = virtualBoundaries;
        }

        public IList<IVirtualBoundary> VirtualBoundaries { get { return _virtualBoundaries; } }

        // the following handles a list of VBs
        public static double GetDistanceToClosestVirtualBoundary(Photon photon)
        {
            var distance = double.PositiveInfinity;

            if (_virtualBoundaries != null && _virtualBoundaries.Count > 0)
            {
                foreach (var virtualBoundary in _virtualBoundaries)
                {
                    var distanceToVB = virtualBoundary.GetDistanceToVirtualBoundary(photon);

                    if (distanceToVB <= distance)
                    {
                        distance = distanceToVB;
                    }
                }
            }

            return distance;
        }

    }
}
