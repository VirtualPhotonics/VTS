using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    ///// <summary>
    ///// This is the contract for perturbation Monte Carlo (pMC) surface detector
    ///// controllers.  The Tally method has a specific signature that includes
    ///// CollisionInfo in addition to PhotonDataPoint.  The CollisionInfo is
    ///// needed to determine the perturbed weight for the photon.
    ///// </summary>
    //public interface IpMCSurfaceDetectorController : IDetectorController
    //{
    //    // IList<IpMCSurfaceDetector> Detectors { get; }
    //    void Tally(PhotonDataPoint dp, CollisionInfo info);
    //}
}
