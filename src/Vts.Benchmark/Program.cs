using System;
using BenchmarkDotNet.Running;
using Vts.MonteCarlo;

namespace Vts.Benchmark
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MonteCarloSimulation>();
            Console.WriteLine(summary);
            summary = BenchmarkRunner.Run<ParallelMonteCarloSimulation>();
            Console.WriteLine(summary);
        }
    }
}
