namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of source-detector separation
/// for multiple sets of optical properties using a two-layer forward solver
/// </summary>
internal class Demo19ROfRhoTwoLayerMultiOpProp : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 19: predict R(rho) based on a standard diffusion approximation solution to the time-dependent RTE
        // for multiple sets of optical properties using a two-layer forward solver

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new TwoLayerSDAForwardSolver();
        var rhos = new DoubleRange(start: 0.5, stop: 9.5, number: 19).ToArray(); // range of radial distances in 1/mm
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

        // predict the reflectance at each specified optical properties for the given s-d separation
        var rOfRho1 = solver.ROfRho(op1, rhos);
        var rOfRho2 = solver.ROfRho(op2, rhos);
        var rOfRho3 = solver.ROfRho(op3, rhos);

        // Plot log(reflectance) as a function of source-detector separation at each set of optical properties
        var chart = Chart.Combine(
            new[] {
                LineChart(rhos, rOfRho1.Select(r => Math.Log(r)).ToArray(), xLabel: "rho [mm]", yLabel: $"R(rho) [mm-2]",
                    title: $"log(R(rho) [mm-2])@ mua1={op1[0].RegionOP.Mua:F3}"),
                LineChart(rhos, rOfRho2.Select(r => Math.Log(r)).ToArray(), xLabel: "rho [mm]", yLabel: $"R(rho) [mm-2]",
                    title: $"log(R(rho) [mm-2])@ mua1={op2[0].RegionOP.Mua:F3}"),
                LineChart(rhos, rOfRho3.Select(r => Math.Log(r)).ToArray(), xLabel: "rho [mm]", yLabel: $"R(rho) [mm-2]",
                    title: $"log(R(rho) [mm-2])@ mua1={op3[0].RegionOP.Mua:F3}")
            }
        );

        if (showPlots)
        {
            chart.Show();
        }
    }
}