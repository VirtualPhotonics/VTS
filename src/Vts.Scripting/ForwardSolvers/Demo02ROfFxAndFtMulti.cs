namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of spatial frequency and time frequency
/// </summary>
internal class Demo02ROfFxAndFtMulti : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 02: predict R(fx, ft) based on a standard diffusion approximation solution to the time-dependent RTE
        // Note:
        //     - this example computes and displays R(ft) at multiple spatial frequencies, to demonstrate that multiplexing functionality
        //     - updating the fx range to emit a single value will isolate to the frequency desired
        //     - e.g. var fxs = new DoubleRange(start: 0, stop: 0, number: 1) will plot R(ft) at fx=0 only (i.e. planar illumination)

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var op = new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4);
        var fxs = new DoubleRange(start: 0, stop: 0.1, number: 4).ToArray(); // range of spatial frequencies in 1/mm
        var fts = new DoubleRange(start: 0, stop: 0.5, number: 51).ToArray(); // range of temporal frequencies in GHz

        // predict the temporal frequency response at each specified optical property and spatial frequency
        var rOfFxAndFt = fxs.Select(fx => solver.ROfFxAndFt(op, fx, fts)).ToArray();

        // Plot reflectance as a function of temporal-frequency at the specified spatial frequencies
        var xLabel = "time frequency [GHz]";
        var amplitudeAndPhaseCharts = new[]
        {
            Chart.Combine(fxs.Select((fx, fxi) =>
                LineChart(fts, rOfFxAndFt[fxi].Select(r => r.Magnitude).ToArray(), xLabel, yLabel: $"|R(ft)| [unitless]", title: $"amp@fx={fx:F3}"))),
            Chart.Combine(fxs.Select((fx, fxi) =>
                LineChart(fts, rOfFxAndFt[fxi].Select(r => -r.Phase).ToArray(), xLabel, yLabel: $"Φ(R(ft)) [rad]", title: $"phase@fx={fx:F3}")))
        };
        var grid1 = Chart.Grid(amplitudeAndPhaseCharts, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);

        var realAndImaginaryCharts = new[]
        {
            Chart.Combine(fxs.Select((fx, fxi) =>
                LineChart(fts, rOfFxAndFt[fxi].Select(r => r.Real).ToArray(), xLabel, yLabel: $"R(ft) (real) [unitless]", title: $"real@fx={fx:F3}"))),
            Chart.Combine(fxs.Select((fx, fxi) =>
                LineChart(fts, rOfFxAndFt[fxi].Select(r => r.Imaginary).ToArray(), xLabel, yLabel: $"R(ft) (imag) [unitless]", title: $"imag@fx={fx:F3}")))
        };
        var grid2 = Chart.Grid(realAndImaginaryCharts, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);

        if (showPlots)
        {
            grid1.Show();
            grid2.Show();
        }
    }
}