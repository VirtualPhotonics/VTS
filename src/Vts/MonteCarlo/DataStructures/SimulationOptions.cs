using System;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ISimulationOptions.  Designates random number generator,
    /// absorption weighting type and flags input to the Monte
    /// Carlo simulation (e.g. to write the histories to file, to generate
    /// the P(V and D) results).
    /// </summary>
    public class SimulationOptions : ISimulationOptions
    {
        public SimulationOptions(
            int seed, 
            RandomNumberGeneratorType rngType, 
            AbsorptionWeightingType absWeightingType, 
            bool doTimeResolvedFluence, 
            bool doPofVandD, 
            bool writeHistories,
            int simulationIndex)
        {
            RandomNumberGeneratorType = rngType;
            AbsorptionWeightingType = absWeightingType;
            Seed = seed;
            DoTimeResolvedFluence = doTimeResolvedFluence;
            DoPofVandD = doPofVandD;
            SimulationIndex = simulationIndex;
            WriteHistories = writeHistories;
        }

        public RandomNumberGeneratorType RandomNumberGeneratorType { get; set; }
        public AbsorptionWeightingType AbsorptionWeightingType { get; set; }
        public int Seed { get; set; }
        public int SimulationIndex { get; set; }
        public bool DoTimeResolvedFluence { get; set; }
        public bool DoPofVandD { get; set; }

        public bool WriteHistories { get; set; }  // Added by DC 2009-08-01

        public SimulationOptions(
            int seed, 
            RandomNumberGeneratorType rngType, 
            AbsorptionWeightingType absWeightingType)
            : this(seed, rngType, absWeightingType, false, false, false, 0) { }

        public SimulationOptions(int seed)
            : this(seed, RandomNumberGeneratorType.MersenneTwister, AbsorptionWeightingType.Discrete, false, false, false, 0) { }

        public SimulationOptions()
            : this(GetRandomSeed(), RandomNumberGeneratorType.MersenneTwister, AbsorptionWeightingType.Discrete, false, false, false, 0) { }

        public static int GetRandomSeed()  // ckh 12/15/09 made this public so Photon can see
        {
            return (int)DateTime.Now.Ticks % (1 << 15);
        }
    }
}
