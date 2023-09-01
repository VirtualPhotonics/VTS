namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of spatial frequency
/// for multiple sets of optical properties using a two-layer forward solver
/// </summary>
internal class Demo20ROfFxTwoLayerMultiOpProp : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 20: predict R(fx) based on a standard diffusion approximation solution to the time-dependent RTE
        // for multiple sets of optical properties using a two-layer forward solver

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new TwoLayerSDAForwardSolver();
        var fxs = new DoubleRange(start: 0, stop: 0.5, number: 51).ToArray(); // range of spatial frequencies in 1/mm
        var op1 = new IOpticalPropertyRegion[]
        {
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4)),
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4)),
        };
        var op2 = new IOpticalPropertyRegion[]
        {
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: new OpticalProperties(mua: 0.02, musp: 1, g: 0.8, n: 1.4)),
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4)),
        };
        var op3 = new IOpticalPropertyRegion[]
        {
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: new OpticalProperties(mua: 0.03, musp: 1, g: 0.8, n: 1.4)),
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4)),
        };

        // predict the reflectance versus spatial frequency at each specified optical properties
        var rOfFx1 = solver.ROfFx(op1, fxs);
        var rOfFx2 = solver.ROfFx(op2, fxs);
        var rOfFx3 = solver.ROfFx(op3, fxs);

        // Plot reflectance as a function of spatial frequencies at each set of optical properties
        var chart = Chart.Combine(
            new[] {
                LineChart(fxs, rOfFx1, xLabel: "fx [mm-1]", yLabel: $"R(fx) [unitless]",
                    title: $"R(fx) [unitless] @ mua1={op1[0].RegionOP.Mua:F3}"),
                LineChart(fxs, rOfFx2, xLabel: "fx [mm-1]", yLabel: $"R(fx) [unitless]",
                    title: $"R(fx) [unitless] @ mua1={op2[0].RegionOP.Mua:F3}"),
                LineChart(fxs, rOfFx3, xLabel: "fx [mm-1]", yLabel: $"R(fx) [unitless]",
                    title: $"R(fx) [unitless] @ mua1={op3[0].RegionOP.Mua:F3}")
            }
        );

        if (showPlots)
        {
            chart.Show();
        }
    }
}