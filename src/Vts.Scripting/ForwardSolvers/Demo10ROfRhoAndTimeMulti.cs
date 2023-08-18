using Vts.Common;
using Vts.Modeling.ForwardSolvers;
using Plotly.NET.CSharp;

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
        var rhos = new DoubleRange(start: 0.5, stop: 9.5, number: 19).AsEnumerable().ToArray(); // range of radial detector locations in mm
        var ts = new DoubleRange(start: 0, stop: 0.5, number: 501).AsEnumerable().ToArray(); // range of times in ns

        // predict the reflectance at each specified optical property and source-detector separation
        var allROfRho = solver.ROfRhoAndTime(ops, rhos, ts).ToArray();

        // Plot log(reflectance) as a function of time at a range of source-detector separations
        // Create two plots - one for each set of optical properties
        var chartsForOP1 = allROfRho.Chunk(ts.Length).Skip(0).Take(rhos.Length).Select((rOfTime, ridx) => // pull out each R(t) (innermost loop is time)
            LineChart(ts, rOfTime.Select(r => Math.Log(r)).ToArray(),
                xLabel: "t [ns]", yLabel: $"log(R(t) [mm-2*s-1]) @ mua={ops[0].Mua:F3}", title: $"log(R(t)) @ ρ={rhos[ridx]:F3}"));
        var chartsForOP2 = allROfRho.Chunk(ts.Length).Skip(rhos.Length).Take(rhos.Length).Select((rOfTime, ridx) => 
            LineChart(ts, rOfTime.Select(r => Math.Log(r)).ToArray(),
                xLabel: "t [ns]", yLabel: $"log(R(t) [mm-2*s-1]) @ mua={ops[1].Mua:F3}", title: $"log(R(t)) @ ρ={rhos[ridx]:F3}"));

        var chartCombined = Chart.Grid(new[] { Chart.Combine(chartsForOP1), Chart.Combine(chartsForOP2) }, 
            nRows: 1, nCols: 2, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);

        if (showPlots)
        {
            chartCombined.Show();
        }
    }
}