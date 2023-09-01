namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of source-detector separation
/// and time for multiple sets of optical properties
/// </summary>
internal class Demo10ROfRhoAndTimeMulti : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 10: predict R(rho,t) at various OPs based on a standard diffusion approximation solution to the time-dependent RTE

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var ops = new[]
        {
            new OpticalProperties(mua:  0.1, musp: 1, g: 0.8, n: 1.4),
            new OpticalProperties(mua:  0.2, musp: 1, g: 0.8, n: 1.4)
        };
        var rho = 10; // radial detector location in mm
        var ts = new DoubleRange(start: 0, stop: 0.5, number: 501).ToArray(); // range of times in ns

        // predict the reflectance at each specified optical property and source-detector separation
        var allROfRho = solver.ROfRhoAndTime(ops, rho, ts).ToArray();

        // Plot log(reflectance) as a function of time at a range of source-detector separations
        // Create two plots - one for each set of optical properties
        var bothCharts = allROfRho.Chunk(ts.Length).Select((rOfTime, opIdx) => // pull out each R(t) (innermost loop is time)
            LineChart(ts, rOfTime.Select(r =>r).ToArray(),
                xLabel: "t [ns]", yLabel: $"R(t) [mm-2*ns-1])", title: $"R(t) @ mua={ops[opIdx].Mua:F3}"));

        var chartCombined = Chart.Combine(bothCharts);

        if (showPlots)
        {
            chartCombined.Show();
        }
    }
}