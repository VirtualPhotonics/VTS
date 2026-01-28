using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using System;
using Vts.Benchmark.Benchmarks;

namespace Vts.Benchmark;

public static class Program
{
    /// <summary>
    /// Set up execution of Monte Carlo Command Line application and run benchmark
    /// collecting estimate of the Mean time of execution and Standard Deviation.
    /// Output to CSV file.
    /// Notes: 1) Build Vts.Benchmark in Release configuration
    ///        2) Pull down enables running BenchmarkMonteCarlo or BenchmarkMonteCarloParallel
    ///        3) Run with Debug tab -> Start Without Debugging
    ///        4) Prior run mean data (in BenchmarkDotNet.Artifacts) is used as validation mean.
    ///           If no prior run data, prior mean set in code is used as validation mean.
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

        // If the previous CSV file exists, get the prior mean using helper methods
        var fullPath = Helpers.CsvTools.GetCsvFullPath(runInParallel);
        var values = Helpers.CsvTools.ReadValuesFromCsv(fullPath);
        // write out result of current run and standard deviation compared against prior mean 
        var priorMean = runInParallel ? 4.35 : 7.76; // [ms] put default values here in case there is no file
        if (values != null)
        {
            var meanValue = Helpers.CsvTools.ReadValue(values, 1); // the mean is the 2nd value
            if (meanValue == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"Could not read results from file");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                priorMean = (double)meanValue;
                Console.WriteLine(@"Prior mean is {0:F}", priorMean);
            }
        }

        // run the benchmark
        var summary = runInParallel ?
            // This line is used to run the parallel benchmark
            BenchmarkRunner.Run<ParallelMonteCarloSimulationBenchmarks>(config) :
            // This line is used to run the non-parallel benchmark
            BenchmarkRunner.Run<MonteCarloSimulationBenchmarks>(config);

        Console.WriteLine(summary);

        // Read CSV file for Mean and Standard Deviation (StDev) data using helper methods
        fullPath = Helpers.CsvTools.GetCsvFullPath(runInParallel);
        values = Helpers.CsvTools.ReadValuesFromCsv(fullPath);
        if (values == null) return;
        var mean = Helpers.CsvTools.ReadValue(values, 1); // the mean is the 2nd value
        var standardDeviation = Helpers.CsvTools.ReadValue(values, 3); //the standard deviation is the 4th value

        // output 1 sigma results
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(@"SUMMARY: (Mean = {0:F} ms) +/- (2-SD = {1:F} ms)", mean, 2 * standardDeviation);
        // check if mean is within 1 standard deviation about the prior mean
        if (mean < priorMean - 2* standardDeviation || mean > priorMean + 2 * standardDeviation)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(@" is not within 2-SD (95.4%) of prior mean = {0:F}", priorMean);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@" is within 2-SD (95.4%) of prior mean = {0:F}", priorMean);
        }
        Console.ForegroundColor = ConsoleColor.White;
    }
}