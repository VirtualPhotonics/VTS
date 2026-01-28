using BenchmarkDotNet.Attributes;
using NLog;
using System.Collections.Generic;
using Vts.MonteCarlo;

namespace Vts.Benchmark.Benchmarks;

public class ParallelMonteCarloSimulationBenchmarks
{
    private ParallelMonteCarloSimulation _simulation;

    [ParamsSource(nameof(SimulationInputs))]
    public SimulationInput Input { get; set; }

    public static IEnumerable<SimulationInput> SimulationInputs()
    {
        yield return new SimulationInput { N = 10000 };
        // we can add more SimulationInput instances with different configurations if needed
    }

    [Params(4)] // we can add more parameters separated by commas for different CPU counts
    public int NumberOfCpUs { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        LogManager.SuspendLogging();

        _simulation = new ParallelMonteCarloSimulation(Input, NumberOfCpUs);
    }

    [Benchmark]
    public SimulationOutput RunSingleInParallel()
        => _simulation.RunSingleInParallel();
}