namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of spatial frequency and time
/// </summary>
internal class Demo11ROfFxAndTime : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 11: predict R(fx, t) based on a standard diffusion approximation solution to the time-dependent RTE

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var op = new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4);
        var fxs = new DoubleRange(start: 0, stop: 0.5, number: 11).ToArray(); // range of spatial frequencies in 1/mm
        var ts = new DoubleRange(start: 0, stop: 0.5, number: 501).ToArray(); // range of times in ns

        // predict the temporal response at each specified optical property and spatial frequency
        var rOfFxAndTime = solver.ROfFxAndTime(op, fxs, ts).ToArray();

        // Plot reflectance as a function of time at the specified spatial frequencies
        var fxChart = Chart.Combine(rOfFxAndTime.Chunk(ts.Length).Select((rOfTime ,fxi) => // take and plot each R(t) (outer loop is spatial frequency)
            LineChart(ts, rOfTime, xLabel: "time [ns]", yLabel: $"R(t) [ns-1]", title: $"amp@fx={fxs[fxi]:F3}")));
        
        var (tIdx0p01, tIdx0p05) = (10, 50);
        var rOfFx0p01 = rOfFxAndTime.TakeEveryNth(ts.Length, skip: tIdx0p01).ToArray(); // t = 0.01 ns data
        var rOfFx0p05 = rOfFxAndTime.TakeEveryNth(ts.Length, skip: tIdx0p05).ToArray(); // t = 0.05 ns data

        // Plot reflectance as a function of time at the specified spatial frequencies
        var timeChart = Chart.Combine(new[]
        {
            LineChart(fxs, rOfFx0p01, xLabel: "fx [mm-1]", yLabel: $"R(fx) [unitless]", title: $"amp@t={ts[tIdx0p01]:F3}"),
            LineChart(fxs, rOfFx0p05, xLabel: "fx [mm-1]", yLabel: $"R(fx) [unitless]", title: $"amp@t={ts[tIdx0p05]:F3}")
        });

        if (showPlots)
        {
            fxChart.Show();
            timeChart.Show();
        }
    }
}