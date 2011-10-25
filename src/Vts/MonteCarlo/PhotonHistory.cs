using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.PhotonData;
using Vts.Common;

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

        public IList<PhotonDataPoint> HistoryData{ get; private set; }

        //public IList<SubRegionCollisionInfo> SubRegionInfoList // modified from IList<SubRegionCollisionInfo>
        public CollisionInfo SubRegionInfoList
        {
            get { return _SubRegionInfoList; } 
            set { _SubRegionInfoList = value; }
        }

        /// <summary>
        /// Method to add PhotonDataPoint to History.  
        /// </summary>
        /// <param name="dp"></param>
        public void AddDPToHistory(PhotonDataPoint dp)
        {
            HistoryData.Add(dp.Clone()); 
        }
    }
}
