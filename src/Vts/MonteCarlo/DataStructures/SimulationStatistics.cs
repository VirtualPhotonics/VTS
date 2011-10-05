using System;
using System.Collections.Generic;
using Vts.IO;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This class captures informational statistics of the MC simulation executed
    /// </summary>
    public class SimulationStatistics
    {
        /// <summary>
        /// class captures statistics about where the photon ended up.
        /// </summary>
        /// <param name="numberOfPhotonsOutTopOfTissue"># photons out top of tissue</param>
        /// <param name="numberOfPhotonsOutBottomOfTissue"># photons out bottom of tissue</param>
        /// <param name="numberOfPhotonsAbsorbed"># photons absorbed</param>
        /// <param name="numberOfPhotonsKilledOverMaximumPathLength"># photons killed due to maximum path length</param>
        /// <param name="numberOfPhotonsKilledOverMaximumCollisions"># photons killed due to maxiumu collisions</param>
        /// <param name="numberOfPhotonsKilledByRussianRoulette"># photons killed by Russian Roulette
        /// 
        /// </param>
        public SimulationStatistics(
            long numberOfPhotonsOutTopOfTissue,
            long numberOfPhotonsOutBottomOfTissue,
            long numberOfPhotonsAbsorbed,
            long numberOfPhotonsKilledOverMaximumPathLength,
            long numberOfPhotonsKilledOverMaximumCollisions,
            long numberOfPhotonsKilledByRussianRoulette)
        {
            NumberOfPhotonsOutTopOfTissue = numberOfPhotonsOutTopOfTissue;
            NumberOfPhotonsOutBottomOfTissue = numberOfPhotonsOutBottomOfTissue;
            NumberOfPhotonsAbsorbed = numberOfPhotonsAbsorbed;
            NumberOfPhotonsKilledOverMaximumPathLength = numberOfPhotonsKilledOverMaximumPathLength;
            NumberOfPhotonsKilledOverMaximumCollisions = numberOfPhotonsKilledOverMaximumCollisions;
            NumberOfPhotonsKilledByRussianRoulette = numberOfPhotonsKilledByRussianRoulette;
        }

        public SimulationStatistics() : this(0, 0, 0, 0, 0, 0) { }

        /// <summary>
        /// number of photons out top of tissue
        /// </summary>
        public long NumberOfPhotonsOutTopOfTissue { get; set; }
        /// <summary>
        /// number of photons out bottom of tissue
        /// </summary>
        public long NumberOfPhotonsOutBottomOfTissue { get; set; }
        /// <summary>
        /// number of photons absorbed
        /// </summary>
        public long NumberOfPhotonsAbsorbed { get; set; }
        /// <summary>
        /// number of photons killed because of path length longer than maximum allowed
        /// </summary>
        public long NumberOfPhotonsKilledOverMaximumPathLength { get; set; }
        /// <summary>
        /// number of photons killed because number of collisions larger than maximum allowed
        /// </summary>
        public long NumberOfPhotonsKilledOverMaximumCollisions { get; set; }
        /// <summary>
        /// number of photons killed by Russian Roulette
        /// </summary>
        public long NumberOfPhotonsKilledByRussianRoulette { get; set; }

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
        public static SimulationStatistics FromFile(string filename)
        {
            return FileIO.ReadFromXML<SimulationStatistics>(filename);
        }
    }
}
