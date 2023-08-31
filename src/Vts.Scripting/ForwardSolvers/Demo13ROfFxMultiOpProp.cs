namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of spatial frequency while varying absorption linearly
/// </summary>
internal class Demo13ROfFxMultiOpProp : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 13: predict R(fx) based on a standard diffusion approximation solution to the time-dependent RTE
        //  while varying absorption linearly

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var fxs = new DoubleRange(start: 0, stop: 0.2, number: 201).ToArray(); // range of spatial frequencies in 1/mm
        var muas = new DoubleRange(start: 0, stop: 0.1, number: 11).ToArray(); // range of absorption values in 1/mm
        var ops = muas.Select(mua => new OpticalProperties(mua: mua, musp: 1.2, g: 0.8, n: 1.4)).ToArray();

        // predict the spatial frequency response at each specified optical properties
        var rOfFx = solver.ROfFx(ops, fxs);

        // Plot reflectance as a function of spatial frequencies at each set of optical properties
        var rOfFxAmplitude = rOfFx.Select(Math.Abs).ToArray();
        var chart = Chart.Combine(rOfFxAmplitude.Chunk(fxs.Length).Select((rOfFxSingleOpProp, opi) => // take and plot each R(fx) (outer loop is optical properties)
            LineChart(fxs, rOfFxSingleOpProp, xLabel: "fx [mm-1]", yLabel: $"R(fx) [unitless] (varying mua linearly)", 
                title: $"Reflectance @ mua={ops[opi].Mua:F3}, musp={ops[opi].Musp:F3}")));

        if (showPlots)
        {
            chart.Show();
        }
    }
}