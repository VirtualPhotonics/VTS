using Vts.Common;
using Vts.Modeling.ForwardSolvers;
using Plotly.NET.CSharp;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of spatial frequency and time frequency
/// </summary>
public class FS02_ROfFxAndFt : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo()
    {
        // Example 02: predict R(fx, ft) based on a standard diffusion approximation solution to the time-dependent RTE
        // Note: this example only computes teh value at fx=0, but updating the fx range will plot for multiple fx values

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var op = new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4);
        var fx = new DoubleRange(start: 0, stop: 0, number: 1).AsEnumerable().ToArray(); // range of spatial frequencies in 1/mm
        var ft = new DoubleRange(start: 0, stop: 0.5, number: 51).AsEnumerable().ToArray(); // range of temporal frequencies in GHz

        // predict the temporal frequency response at the specified optical properties and s-d separation
        var rOfFxAndFt = new Complex[fx.Length][];
        var rOfFxAndFtAmplitude = new double[fx.Length][];
        var rOfFxAndFtPhase = new double[fx.Length][];
        for (int fxi = 0; fxi < fx.Length; fxi++)
        {
            rOfFxAndFt[fxi] = solver.ROfFxAndFt(op, fx[fxi], ft);
            rOfFxAndFtAmplitude[fxi] = rOfFxAndFt[fxi].Select(r => r.Magnitude).ToArray();
            rOfFxAndFtPhase[fxi] = rOfFxAndFt[fxi].Select(r => -r.Phase).ToArray();
        }

        // Plot reflectance as a function of temporal-frequency at the specified spatial frequencies
        var xLabel = "time frequency [GHz]";
        var charts = new[]
        {
            Chart.Combine(Enumerable.Range(0, fx.Length).Select(fxi =>
                LineChart(ft, rOfFxAndFtAmplitude[fxi], xLabel, yLabel: $"|R(ft)| [unitless]"))),
            Chart.Combine(Enumerable.Range(0, fx.Length).Select(fxi =>
                LineChart(ft, rOfFxAndFtPhase[fxi], xLabel, yLabel: $"Φ(R(ft)) [rad]")))
        };
        Chart.Grid(charts, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled).Show();
    }
}