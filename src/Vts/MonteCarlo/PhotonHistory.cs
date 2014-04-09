using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Stores list of PhotonDataPoint that captures one photon's biography data. 
    /// </summary>
    public class PhotonHistory
    {
       
        //private IList<SubRegionCollisionInfo> _SubRegionInfoList;
        private CollisionInfo _SubRegionInfoList;

        /// <summary>
        /// constructor for photon history class
        /// </summary>
        /// <param name="numSubRegions">number of subregions in tissue</param>
        public PhotonHistory(int numSubRegions)
        {
            HistoryData = new List<PhotonDataPoint>();

            //_SubRegionInfoList = Enumerable.Range(0, numSubRegions)
            //    .Select(i => new SubRegionCollisionInfo(0.0, 0));

            _SubRegionInfoList = new CollisionInfo(numSubRegions);

            // dc: why doesn't CollisionInfo do following in its constructor?
            for (int i = 0; i < numSubRegions; i++)
            {
                _SubRegionInfoList.Add(new SubRegionCollisionInfo(0.0, 0)); 
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
        public CollisionInfo SubRegionInfoList
        {
            get { return _SubRegionInfoList; } 
            set { _SubRegionInfoList = value; }
        }
        /// <summary>
        /// Identifies current PhotonDataPoint
        /// </summary>
        public PhotonDataPoint CurrentDP
        {
            get
            {
                if (HistoryData.Count > 0)
                {
                    return HistoryData[HistoryData.Count - 1];
                }

                return null;
            }
        }
        /// <summary>
        /// Identifies previous PhotonDataPoint
        /// </summary>
        public PhotonDataPoint PreviousDP
        {
            get
            {
                if (HistoryData.Count > 1)
                {
                    return HistoryData[HistoryData.Count - 2];
                }

                return null;
            }
        }

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
