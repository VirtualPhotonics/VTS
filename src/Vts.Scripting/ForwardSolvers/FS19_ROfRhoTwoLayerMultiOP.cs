using Vts.Common;
using Vts.Modeling.ForwardSolvers;
using Plotly.NET.CSharp;

namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of source-detector separation
/// for multiple sets of optical properties using a two-layer forward solver
/// </summary>
internal class FS19_ROfRhoTwoLayerMultiOP : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo()
    {
        // Example 19: predict R(rho) based on a standard diffusion approximation solution to the time-dependent RTE
        // for multiple sets of optical properties using a two-layer forward solver

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new TwoLayerSDAForwardSolver();
        var rhos = new DoubleRange(start: 0.5, stop: 9.5, number: 19).AsEnumerable().ToArray(); // range of spatial frequencies in 1/mm
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
        var rOfRho1 = rhos.Select(rho => solver.ROfRho(op1, rho)).ToArray();
        var rOfRho2 = rhos.Select(rho => solver.ROfRho(op2, rho)).ToArray();
        var rOfRho3 = rhos.Select(rho => solver.ROfRho(op3, rho)).ToArray();

        // Plot log(reflectance) as a function of spatial frequencies at each set of optical properties
        Chart.Combine(
            new[] {
                LineChart(rhos, rOfRho1.Select(r => Math.Log(r)).ToArray(), xLabel: "rho [mm]", yLabel: $"R(rho) [mm-2]",
                    title: $"log(R(rho) [mm-2])@ mua1={op1[0].RegionOP.Mua:F3}"),
                LineChart(rhos, rOfRho2.Select(r => Math.Log(r)).ToArray(), xLabel: "rho [mm]", yLabel: $"R(rho) [mm-2]",
                    title: $"log(R(rho) [mm-2])@ mua1={op2[0].RegionOP.Mua:F3}"),
                LineChart(rhos, rOfRho3.Select(r => Math.Log(r)).ToArray(), xLabel: "rho [mm]", yLabel: $"R(rho) [mm-2]",
                    title: $"log(R(rho) [mm-2])@ mua1={op3[0].RegionOP.Mua:F3}")
            }
        ).Show();
    }
}