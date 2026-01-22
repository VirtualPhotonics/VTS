using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Vts.Benchmark.Benchmarks;

namespace Vts.Benchmark
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Set up execution of Monte Carlo Command Line application and run benchmark - We might need to figure this out later
            // collecting estimate of the Mean time of execution and Standard Deviation.
            // Output to CSV file.
            // Notes: 1) Build Vts.Benchmark in Release configuration
            //        2) Run with Debug tab -> Start Without Debugging
            var config = new ManualConfig()
                .AddJob(new Job("Benchmark"))
                .AddLogger(ConsoleLogger.Default)
                .AddColumn(TargetMethodColumn.Method,
                    StatisticColumn.Mean, 
                    StatisticColumn.Error, 
                    StatisticColumn.StdDev,
                    StatisticColumn.Median)
                .AddExporter(CsvExporter.Default, HtmlExporter.Default, MarkdownExporter.GitHub)
                .AddAnalyser(EnvironmentAnalyser.Default);
            config.UnionRule = ConfigUnionRule.Union;
            // This line is used to run the non-parallel benchmark, we could add a parameter to switch between them
            var summary = BenchmarkRunner.Run<MonteCarloSimulationBenchmarks>(config);
            // This line is used to run the parallel benchmark
            //var summary = BenchmarkRunner.Run<ParallelMonteCarloSimulationBenchmarks>(config);
            Console.WriteLine(summary);
            // Read CSV file for Mean and Standard Deviation (StDev) data
            var inputPath = Path.GetFullPath(Directory.GetCurrentDirectory());
            const string csvFile = "\\BenchmarkDotNet.Artifacts\\results\\Vts.Benchmark.Benchmarks.MonteCarloSimulationBenchmarks-report.csv";
            var filePath = Path.GetFullPath(inputPath + csvFile);
            using var reader = new StreamReader(filePath);
            // read header line
            reader.ReadLine();
            // read data line
            var line = reader.ReadLine();
            if (line == null) return;
            var values = line.Split(',');
            // excise double from string of type "###.## ms"
            var mean = double.Parse(Regex.Match(values[1], @"-?\d+(?:\.\d+)?").Value); // has 'ms' appended
            var standardDeviation = double.Parse(Regex.Match(values[3], @"-?\d+(?:\.\d+)?").Value); // has 'ms' appended
            // write out result of current run and standard deviation compared against prior mean 
            const double priorMean = 84.0; // ms
            // output 1 sigma results
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("SUMMARY: (Mean = {0:F} ms) +/- (SD = {1:F} ms)", mean, standardDeviation);
            // output if result is larger than established mean + 1-SD
            if (mean > standardDeviation + priorMean)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" is 1-SD > prior mean = {0:F}", priorMean);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" is within 1-SD of prior mean = {0:F}", priorMean);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
