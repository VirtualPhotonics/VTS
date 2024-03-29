using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Stores list of PhotonDataPoint that captures one photon's biography data. 
    /// </summary>
    public class PhotonHistory
    {

        /// <summary>
        /// constructor for photon history class
        /// </summary>
        /// <param name="numSubRegions">number of sub-regions in tissue</param>
        public PhotonHistory(int numSubRegions)
        {
            HistoryData = new List<PhotonDataPoint>();

            // this is performed here rather than in CollisionInfo constructor because
            // PhotonHistory needs to know this information in order to perform CAW
            // and other detector tallies that require path length and number of collisions
            // in each tissue sub-region detectors.
            SubRegionInfoList = new CollisionInfo(numSubRegions);
            for (var i = 0; i < numSubRegions; i++)
            {
                SubRegionInfoList.Add(new SubRegionCollisionInfo(0.0, 0)); 
            }
        }
        /// <summary>
        /// HistoryData contains the photon's biography within a list of PhotonDataPoints
        /// </summary>
        public IList<PhotonDataPoint> HistoryData{ get; private set; }

        /// <summary>
        /// SubRegionInfoList keeps track of number of collisions and pathlength in each tissue
        /// region
        /// </summary>
        public CollisionInfo SubRegionInfoList { get; set; }
        /// <summary>
        /// Identifies current PhotonDataPoint
        /// </summary>
        public PhotonDataPoint CurrentDP =>
            HistoryData.Count > 0 
                ? HistoryData[HistoryData.Count - 1] 
                : null;

        /// <summary>
        /// Identifies previous PhotonDataPoint
        /// </summary>
        public PhotonDataPoint PreviousDP => 
            HistoryData.Count > 1 
                ? HistoryData[HistoryData.Count - 2] 
                : null;

        /// <summary>
        /// Method to add PhotonDataPoint to History.  
        /// </summary>
        /// <param name="dp">PhotonD</param>
        public void AddDPToHistory(PhotonDataPoint dp)
        {
            HistoryData.Add(dp.Clone()); 
        }
    }
}
