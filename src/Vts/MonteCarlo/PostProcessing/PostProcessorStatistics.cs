using Vts.IO;

namespace Vts.MonteCarlo.PostProcessing
{
    /// <summary>
    /// This class captures informational statistics of the MC Post Processor executed
    /// </summary>
    public class PostProcessorStatistics
    {
        private int _numberOfTissueRegions;
        /// <summary>
        /// class captures statistics about the photon journey.
        /// </summary>
        /// <param name="tissueRegionsCount">number of tissue regions</param>
        public PostProcessorStatistics(
            int tissueRegionsCount)
        {
            _numberOfTissueRegions = tissueRegionsCount;
            // probably don't need both Average and Total but not sure right now which would be better
            AverageNumberOfCollisions = new double[tissueRegionsCount];
            AveragePathLength = new double[tissueRegionsCount];
            TotalNumberOfCollisions = new long[tissueRegionsCount];
            TotalPathLength = new double[tissueRegionsCount];
        }

        public PostProcessorStatistics() : this(3) { }

        /// <summary>
        /// number of collisions of each photon in each region averaged over all photons
        /// </summary>
        public double[] AverageNumberOfCollisions { get; set; }
        /// <summary>
        /// path length of each photon in each region averaged over all photons
        /// </summary>
        public double[] AveragePathLength { get; set; }
        /// <summary>
        /// total number of collisions of each photon in each region 
        /// </summary>
        public long[] TotalNumberOfCollisions { get; set; }
        /// <summary>
        /// total path length of each photon in each region
        /// </summary>
        public double[] TotalPathLength { get; set; }

        public void TallyStatistics(PhotonHistory history)
        {
            for (int i = 0; i < _numberOfTissueRegions; i++)
            {
                TotalNumberOfCollisions[i] += history.SubRegionInfoList[i].NumberOfCollisions;
                TotalPathLength[i] += history.SubRegionInfoList[i].PathLength;
            }
        }

        public void NormalizeStatistics(long N)
        {
            for (int i = 0; i < _numberOfTissueRegions; i++)
            {
                AverageNumberOfCollisions[i] = (double)TotalNumberOfCollisions[i] / N;
                AveragePathLength[i] = TotalPathLength[i] / N;
            }
        }


        /// <summary>
        /// method to write results to file
        /// </summary>
        /// <param name="filename"></param>
        public void ToFile(string filename)
        {
            FileIO.WriteToXML(this, filename);
        }
        /// <summary>
        /// method to read results from file
        /// </summary>
        /// <param name="filename">filename to be read</param>
        /// <returns>instance of simulation statistics</returns>
        public static PostProcessorStatistics FromFile(string filename)
        {
            return FileIO.ReadFromXML<PostProcessorStatistics>(filename);
        }
    }
}
