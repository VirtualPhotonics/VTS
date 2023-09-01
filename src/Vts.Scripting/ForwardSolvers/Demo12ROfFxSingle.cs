namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of spatial frequency at a single set of optical properties
/// </summary>
internal class Demo12ROfFxSingle : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 12: predict R(fx) based on a standard diffusion approximation solution to the time-dependent RTE

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var op = new OpticalProperties(mua: 0.1, musp: 1.2, g: 0.8, n: 1.4);
        var fxs = new DoubleRange(start: 0, stop: 0.2, number: 201).ToArray(); // range of spatial frequencies in 1/mm

        // predict the spatial frequency response at each specified optical properties
        var rOfFx = solver.ROfFx(op, fxs);

        // Plot reflectance as a function of spatial frequency 
        var rOfFxAmplitude = rOfFx.Select(Math.Abs).ToArray();
        var chart = LineChart(fxs, rOfFxAmplitude, xLabel: "fx [mm-1]", yLabel: $"R(fx) [unitless]", title: "Reflectance vs spatial frequency");

        if (showPlots)
        {
            chart.Show();
        }
    }
}