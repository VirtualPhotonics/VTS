using System;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Designates random number generator,
    /// absorption weighting type and flags input to the Monte
    /// Carlo simulation (e.g. to write the histories to file,
    /// specify seed for RNG).
    /// </summary>
    public class SimulationOptions
    {
        public SimulationOptions(
            int seed, 
            RandomNumberGeneratorType rngType, 
            AbsorptionWeightingType absWeightingType, 
            bool writeHistories,
            int simulationIndex)
        {
            RandomNumberGeneratorType = rngType;
            AbsorptionWeightingType = absWeightingType;
            Seed = seed;
            SimulationIndex = simulationIndex;
            WriteHistories = writeHistories;
        }

        public RandomNumberGeneratorType RandomNumberGeneratorType { get; set; }
        public AbsorptionWeightingType AbsorptionWeightingType { get; set; }
        public int Seed { get; set; }
        public int SimulationIndex { get; set; }

        public bool WriteHistories { get; set; }  // Added by DC 2009-08-01

        public SimulationOptions(
            int seed, 
            RandomNumberGeneratorType rngType, 
            AbsorptionWeightingType absWeightingType)
            : this(seed, rngType, absWeightingType, false, 0) { }

        public SimulationOptions(int seed)
            : this(seed, RandomNumberGeneratorType.MersenneTwister, AbsorptionWeightingType.Discrete, false, 0) { }

        public SimulationOptions()
            : this(GetRandomSeed(), RandomNumberGeneratorType.MersenneTwister, AbsorptionWeightingType.Discrete, false, 0) { }

        public static int GetRandomSeed()  // ckh 12/15/09 made this public so Photon can see
        {
            return (int)DateTime.Now.Ticks % (1 << 15);
        }
    }
}
