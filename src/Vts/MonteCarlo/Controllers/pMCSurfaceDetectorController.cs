using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Controllers
{
    public class pMCSurfaceDetectorController : IpMCSurfaceDetectorController  
    {
        private ITissue _tissue;
        private bool _tallySecondMoment;
        private IList<IDetector> _detectors;

        public pMCSurfaceDetectorController(
            IList<IpMCSurfaceDetector> detectors,
            ITissue tissue,
            bool tallySecondMoment)
        {
            _tissue = tissue;
            _tallySecondMoment = tallySecondMoment;
            _detectors = detectors.Select(d => (IDetector)d).ToList();
            ReferenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
        }

        //public IList<IpMCSurfaceDetector> Detectors { get { return _detectors; } }
        public IList<IDetector> Detectors { get { return _detectors; } }
        public IList<OpticalProperties> ReferenceOps { get; set; }
        public IList<int> PerturbedRegionsIndices { get; set; }
        
        public void Tally(PhotonDataPoint dp, CollisionInfo collisionInfo)
        {
            foreach (var detector in _detectors)
            {
                // only set up reflectance tallies for now, NEED TO FIX
                if (dp.StateFlag.Has(PhotonStateType.PseudoReflectedTissueBoundary) &&
                    detector.TallyType.IsSurfaceTally()) 
                    ((IpMCSurfaceDetector)detector).Tally(dp, collisionInfo);
            }
        }

        public void NormalizeDetectors(long N)
        {
            foreach (var detector in _detectors)
            {
                detector.Normalize(N);
            }
        }
    }
}
