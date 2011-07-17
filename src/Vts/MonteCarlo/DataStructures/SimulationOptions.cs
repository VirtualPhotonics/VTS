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
            bool tallySecondMoment,
            bool trackStatistics,
            int simulationIndex)
        {
            RandomNumberGeneratorType = rngType;
            AbsorptionWeightingType = absWeightingType;
            PhaseFunctionType = phaseFunctionType;
            Seed = seed;
            if (Seed == -1)
            {
                Seed = GetRandomSeed();
            }
            SimulationIndex = simulationIndex;
            TallySecondMoment = tallySecondMoment;
            TrackStatistics = trackStatistics;
        }

        public RandomNumberGeneratorType RandomNumberGeneratorType { get; set; }
        public AbsorptionWeightingType AbsorptionWeightingType { get; set; }
        public PhaseFunctionType PhaseFunctionType { get; set; }
        public bool TallySecondMoment { get; set; }
        public bool TrackStatistics { get; set; }
        public int Seed { get; set; }
        public int SimulationIndex { get; set; }

        public SimulationOptions(
            int seed, 
            RandomNumberGeneratorType rngType, 
            AbsorptionWeightingType absWeightingType)
            : this(seed, 
                rngType, 
                absWeightingType, 
                PhaseFunctionType.HenyeyGreenstein, 
                true, // tally 2nd moment
                false, // track statistics
                0) { }

        public SimulationOptions(int seed)
            : this(seed, 
                RandomNumberGeneratorType.MersenneTwister,  
                AbsorptionWeightingType.Discrete, 
                PhaseFunctionType.HenyeyGreenstein, 
                true,
                false,
                0) { }

        public SimulationOptions()
            : this(GetRandomSeed(), 
                RandomNumberGeneratorType.MersenneTwister, 
                AbsorptionWeightingType.Discrete, 
                PhaseFunctionType.HenyeyGreenstein, 
                true,
                false,
                0) { }

        public static int GetRandomSeed()  // ckh 12/15/09 made this public so Photon can see
        {
            return (int)DateTime.Now.Ticks % (1 << 15);
        }
    }
}
