using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Designates random number generator,
    /// absorption weighting type and flags input to the Monte
    /// Carlo simulation (e.g. tally second moment and
    /// specify seed for RNG).
    /// </summary>
    public class SimulationOptions
    {
        /// <summary>
        /// constructor for simulation options, a class sub to SimulationInput
        /// </summary>
        /// <param name="seed">random number generator seed (-1=randomly chosen seed, >=0 reproducible sequence)</param>
        /// <param name="rngType">random number generator type</param>
        /// <param name="absWeightingType">absorption weighting type</param>
        /// <param name="phaseFunctionType">phase function type</param>
        /// <param name="databases">list of DatabaseType indicating data to be written database for post-processing</param>
        /// <param name="trackStatistics">flag indicating whether to track statistics about where photons end up</param>
        /// <param name="russianRouletteWeightThreshold">weight threshold to perform RR (default=0, no RR)</param>
        /// <param name="simulationIndex">index of simulation</param>
        public SimulationOptions(
            int seed, 
            RandomNumberGeneratorType rngType, 
            AbsorptionWeightingType absWeightingType,
            PhaseFunctionType phaseFunctionType,
            IList<DatabaseType> databases,
            bool trackStatistics,
            double russianRouletteWeightThreshold,
            int simulationIndex)
        {
            RandomNumberGeneratorType = rngType;
            AbsorptionWeightingType = absWeightingType;
            PhaseFunctionType = phaseFunctionType;
            Databases = databases;
            // check if databases list is null and if so make empty
            if (Databases == null)
            {
                Databases = new List<DatabaseType>() { };
            }
            Seed = seed;
            //if (Seed == -1) // handling of random seed moved to RNGFactory 10/01/11
            //{
            //    Seed = GetRandomSeed();
            //}
            SimulationIndex = simulationIndex;
            TrackStatistics = trackStatistics;
            RussianRouletteWeightThreshold = russianRouletteWeightThreshold;
        }
        /// <summary>
        /// constructor that uses Henyey-Greenstein phase function, does not save photon data to database,
        /// tallies 2nd moment information, does not track statistics and designates simulation index to 0
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="rngType"></param>
        /// <param name="absWeightingType"></param>
        public SimulationOptions(
            int seed, 
            RandomNumberGeneratorType rngType, 
            AbsorptionWeightingType absWeightingType)
            : this(seed, 
                rngType, 
                absWeightingType, 
                PhaseFunctionType.HenyeyGreenstein,
                new List<DatabaseType>() { }, // databases to be written
                false, // track statistics
                0.0, // Russian Roulette weight threshold: =0.0 -> no RR performed
                0) { }

        /// <summary>
        /// constructor that takes in seed but uses default values for all other parameters
        /// </summary>
        /// <param name="seed"></param>
        public SimulationOptions(int seed)
            : this(seed, 
                RandomNumberGeneratorType.MersenneTwister,  
                AbsorptionWeightingType.Discrete, 
                PhaseFunctionType.HenyeyGreenstein,
                null,
                false,
                0.0,
                0) { }

        /// <summary>
        /// default constructor
        /// </summary>
        public SimulationOptions()
            : this(-1, // default constructor needs -1 here to invoke GetRandomSeed
                RandomNumberGeneratorType.MersenneTwister, 
                AbsorptionWeightingType.Discrete, 
                PhaseFunctionType.HenyeyGreenstein,
                null,
                false,
                0.0,
                0) { }

        /// <summary>
        /// seed of random number generator (-1=randomly selected seed, >=0 reproducible sequence)
        /// </summary>
        public int Seed { get; set; }
        /// <summary>
        /// random number generator type
        /// </summary>
        public RandomNumberGeneratorType RandomNumberGeneratorType { get; set; }
        /// <summary>
        /// absorption weighting type
        /// </summary>
        public AbsorptionWeightingType AbsorptionWeightingType { get; set; }
        /// <summary>
        /// phase function type
        /// </summary>
        public PhaseFunctionType PhaseFunctionType { get; set; }
        /// <summary>
        /// list of databases to be written
        /// </summary>
        public IList<DatabaseType> Databases { get; set; }  
        /// <summary>
        /// flag indicating whether to track statistics about where photon ends up
        /// </summary>
        public bool TrackStatistics { get; set; }
        /// <summary>
        /// photon weight threshold to perform Russian Roulette.  Default = 0 means no RR performed.
        /// </summary>
        public double RussianRouletteWeightThreshold { get; set; }
        /// <summary>
        /// simulation index 
        /// </summary>
        public int SimulationIndex { get; set; }



        //private static int GetRandomSeed()  // ckh 10/01/11 moved to RandomNumberGeneratorFactory
        //{
        //    return (int)DateTime.Now.Ticks % (1 << 15);
        //}
    }
}
