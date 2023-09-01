namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of source-detector separation
/// for multiple sets of optical properties
/// </summary>
internal class Demo09ROfRhoMulti : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 08: predict R(rho) for multiple OPs based on a standard diffusion approximation solution to the time-dependent RTE

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var ops = new[]
        {
            new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4),
            new OpticalProperties(mua:  0.1, musp: 1, g: 0.8, n: 1.4),
            new OpticalProperties(mua:    1, musp: 1, g: 0.8, n: 1.4)
        };
        var rhos = new DoubleRange(start: 0.5, stop: 9.5, number: 19).ToArray(); // range of radial detector locations in mm

        // predict the reflectance at each specified optical property and source-detector separation
        var allROfRho = solver.ROfRho(ops, rhos).ToArray();

        // Plot log(reflectance) as a function of radial distance at the specified spatial frequencies
        var allCharts = allROfRho.Chunk(rhos.Length).Select((rOfRho, rIdx) => // pull out each R(ρ) (outer loop is optical properties)
            LineChart(rhos, rOfRho.Select(r => Math.Log(r)).ToArray(),
                xLabel: "ρ [mm]", yLabel: $"log(R(ρ) [mm-2])", title: $"log(R(ρ)) @ mua={ops[rIdx].Mua:F3}"));
        var chartCombined = Chart.Combine(allCharts);

        if (showPlots)
        {
            chartCombined.Show();
        }
    }
}