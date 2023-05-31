using System;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Vts.MonteCarlo;

namespace Vts.Benchmark
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var config = new ManualConfig()
                .AddJob(new Job("Benchmark").WithCustomBuildConfiguration("Benchmark"))
                .AddLogger(ConsoleLogger.Default)
                .AddColumn(TargetMethodColumn.Method,
                    StatisticColumn.Mean, 
                    StatisticColumn.Error, 
                    StatisticColumn.StdDev,
                    StatisticColumn.Median)
                .AddExporter(CsvExporter.Default, HtmlExporter.Default, MarkdownExporter.GitHub)
                .AddAnalyser(EnvironmentAnalyser.Default);
            config.UnionRule = ConfigUnionRule.Union;
            var summary = BenchmarkRunner.Run<MonteCarloSimulation>(config);
            Console.WriteLine(summary);
        }
    }
}
