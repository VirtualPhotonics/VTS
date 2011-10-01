using System;
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
        public SimulationOptions(
            int seed, 
            RandomNumberGeneratorType rngType, 
            AbsorptionWeightingType absWeightingType,
            PhaseFunctionType phaseFunctionType,
            IList<DatabaseType> writeDatabases,
            bool tallySecondMoment,
            bool trackStatistics,
            int simulationIndex)
        {
            RandomNumberGeneratorType = rngType;
            AbsorptionWeightingType = absWeightingType;
            PhaseFunctionType = phaseFunctionType;
            WriteDatabases = writeDatabases;
            Seed = seed;
            //if (Seed == -1) // handling of random seed moved to RNGFactory 10/01/11
            //{
            //    Seed = GetRandomSeed();
            //}
            SimulationIndex = simulationIndex;
            TallySecondMoment = tallySecondMoment;
            TrackStatistics = trackStatistics;
        }

        public SimulationOptions(
            int seed, 
            RandomNumberGeneratorType rngType, 
            AbsorptionWeightingType absWeightingType)
            : this(seed, 
                rngType, 
                absWeightingType, 
                PhaseFunctionType.HenyeyGreenstein,
                new List<DatabaseType>() { }, // databases to be written
                true, // tally 2nd moment
                false, // track statistics
                0) { }

        public SimulationOptions(int seed)
            : this(seed, 
                RandomNumberGeneratorType.MersenneTwister,  
                AbsorptionWeightingType.Discrete, 
                PhaseFunctionType.HenyeyGreenstein,
                null,
                true,
                false,
                0) { }

        public SimulationOptions()
            : this(-1, // default constructor needs -1 here to invoke GetRandomSeed
                RandomNumberGeneratorType.MersenneTwister, 
                AbsorptionWeightingType.Discrete, 
                PhaseFunctionType.HenyeyGreenstein,
                null,
                true,
                false,
                0) { }

        public RandomNumberGeneratorType RandomNumberGeneratorType { get; set; }
        public AbsorptionWeightingType AbsorptionWeightingType { get; set; }
        public PhaseFunctionType PhaseFunctionType { get; set; }
        public bool TallySecondMoment { get; set; }
        public bool TrackStatistics { get; set; }
        public int Seed { get; set; }
        public int SimulationIndex { get; set; }
        public IList<DatabaseType> WriteDatabases { get; set; }  // modified ckh 4/12/11



        //private static int GetRandomSeed()  // ckh 10/01/11 moved to RandomNumberGeneratorFactory
        //{
        //    return (int)DateTime.Now.Ticks % (1 << 15);
        //}
    }
}
