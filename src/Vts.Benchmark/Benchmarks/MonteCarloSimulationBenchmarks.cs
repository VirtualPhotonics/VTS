using BenchmarkDotNet.Attributes;
using NLog;
using System.Collections.Generic;
using Vts.MonteCarlo;

namespace Vts.Benchmark.Benchmarks;

public class MonteCarloSimulationBenchmarks
{
    private MonteCarloSimulation _simulation;

    [ParamsSource(nameof(SimulationInputs))]
    public SimulationInput Input { get; set; }

    public IEnumerable<SimulationInput> SimulationInputs()
    {
        yield return new SimulationInput { N = 100 };
        // we can add more SimulationInput instances with different configurations if needed
    }

    [GlobalSetup]
    public void Setup()
    {
        LogManager.SuspendLogging(); // logging can interfere with benchmark results, it is also suspended in NLog config for benchmarks

        _simulation = new MonteCarloSimulation(Input);
    }

    [Benchmark]
    public SimulationOutput RunSimulation()
        => _simulation.Run();
}