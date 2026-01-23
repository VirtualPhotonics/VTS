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
        /// <summary>
        /// Set up execution of Monte Carlo Command Line application and run benchmark
        /// collecting estimate of the Mean time of execution and Standard Deviation.
        /// Output to CSV file.
        /// Notes: 1) Build Vts.Benchmark in Release configuration
        ///        2) Pull down enables running BenchmarkMonteCarlo or BenchmarkMonteCarloParallel
        ///        3) Run with Debug tab -> Start Without Debugging
        /// </summary>
        /// <param name="args">command line parameters</param>
        public static void Main(string[] args)
        {
            // check for -p or --parallel argument to run parallel benchmark
            var runInParallel = args.Length > 0 && (args[0] == "-p" || args[0] == "--parallel");

            // configure BenchmarkDotNet
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

            // run the benchmark
            var summary = runInParallel ?
                // This line is used to run the parallel benchmark
                BenchmarkRunner.Run<ParallelMonteCarloSimulationBenchmarks>(config) :
                // This line is used to run the non-parallel benchmark
                BenchmarkRunner.Run<MonteCarloSimulationBenchmarks>(config);

            Console.WriteLine(summary);
            // Read CSV file for Mean and Standard Deviation (StDev) data
            var inputPath = Path.GetFullPath(Directory.GetCurrentDirectory());
            const string filePath = "\\BenchmarkDotNet.Artifacts\\results\\";
            var csvFile = runInParallel ?
                $"{typeof(ParallelMonteCarloSimulationBenchmarks).FullName}-report.csv" :
                $"{typeof(MonteCarloSimulationBenchmarks).FullName}-report.csv";
            var fullPath = Path.GetFullPath(inputPath + filePath + csvFile);
            using var reader = new StreamReader(fullPath);
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
            var priorMean = runInParallel ? 8.25 : 84.0; // ms
            // output 1 sigma results
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("SUMMARY: (Mean = {0:F} ms) +/- (3*SD = {1:F} ms)", mean, 3 *standardDeviation);
            // check if mean is within 3 standard deviation about the prior mean
            if (mean < priorMean - 3 * standardDeviation || mean > priorMean + 3* standardDeviation)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" is not within 3-SD of prior mean = {0:F}", priorMean);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" is within 3-SD of prior mean = {0:F}", priorMean);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
