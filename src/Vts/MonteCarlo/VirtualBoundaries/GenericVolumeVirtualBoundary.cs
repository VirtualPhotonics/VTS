using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.VirtualBoundaries
{
    /// <summary>
    /// Implements IVirtualBoundary.  Used to capture all internal volume detectors
    /// </summary>
    public class GenericVolumeVirtualBoundary : IVirtualBoundary
    {
        private IDetectorController _detectorController;
        /// <summary>
        /// Creates an instance of a volume virtual boundary
        /// </summary>
        public GenericVolumeVirtualBoundary(
            VirtualBoundaryType type,
            string name)
        {
            _detectorController = new DetectorController(new List<IDetector>());
            Name = name;
            VirtualBoundaryType = type;
        }       

        /// <summary>
        /// Creates a default instance of a GenericVolumeVB 
        /// </summary>
        public GenericVolumeVirtualBoundary() 
            : this(
            VirtualBoundaryType.GenericVolumeBoundary,
            VirtualBoundaryType.GenericVolumeBoundary.ToString())
        {
        }

        public IDetectorController DetectorController { get { return _detectorController; } set { _detectorController = value; } }
        public string Name { get; set; }
        public VirtualBoundaryType VirtualBoundaryType { get; set; }

        /// <summary>
        /// Finds the distance to the virtual boundary 
        /// </summary>
        /// <param name="photon"></param>
        public double GetDistanceToVirtualBoundary(Photon photon)
        {
            // not sure of following
            return double.PositiveInfinity;
        }

    }
}
