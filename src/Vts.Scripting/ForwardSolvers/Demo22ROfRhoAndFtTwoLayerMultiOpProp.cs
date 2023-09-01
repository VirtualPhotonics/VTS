namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of time at a fixed source-detector location
/// for multiple sets of optical properties using a two-layer forward solver
/// </summary>
internal class Demo22ROfRhoAndFtTwoLayerMultiOpProp : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 22: predict R(ft) at a fixed source-detector location based on a standard diffusion approximation solution
        // to the time-dependent RTE for multiple sets of optical properties using a two-layer forward solver

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new TwoLayerSDAForwardSolver();
        var fts = new DoubleRange(start: 0, stop: 0.5, number: 51).ToArray(); // range of time frequencies in GHz
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
        var rho = 10; // s-d separation in mm
        var rOfFt1 = solver.ROfRhoAndFt(op1, rho, fts);
        var rOfFt2 = solver.ROfRhoAndFt(op2, rho, fts);
        var rOfFt3 = solver.ROfRhoAndFt(op3, rho, fts);

        // Plot reflectance amplitude and phase as a function of times at each set of optical properties
        var magnitudeScale = 1E4;
        var amplitudeChart = Chart.Combine(
            new[] {
                LineChart(fts, rOfFt1.Select(r => r.Magnitude * magnitudeScale).ToArray(), xLabel: "ft [GHz]", yLabel: $"R(ft) [mm-2] * {magnitudeScale:E0}",
                    title: $"|R(ft)| [mm-2] @ rho={rho}mm (mua1={op1[0].RegionOP.Mua:F3}, mua2={op1[1].RegionOP.Mua:F3})"),
                LineChart(fts, rOfFt2.Select(r => r.Magnitude * magnitudeScale).ToArray(), xLabel: "ft [GHz]", yLabel: $"R(ft) [mm-2] * {magnitudeScale:E0}",
                    title: $"|R(ft)| [mm-2] @ rho={rho}mm (mua1={op2[0].RegionOP.Mua:F3}, mua2={op2[1].RegionOP.Mua:F3})"),
                LineChart(fts, rOfFt3.Select(r => r.Magnitude * magnitudeScale).ToArray(), xLabel: "ft [GHz]", yLabel: $"R(ft) [mm-2] * {magnitudeScale:E0}",
                    title: $"|R(ft)| [mm-2] @ rho={rho}mm (mua1={op3[0].RegionOP.Mua:F3}, mua2={op3[1].RegionOP.Mua:F3})")
            }
        );
        var phaseChart = Chart.Combine(
            new[] {
                LineChart(fts, rOfFt1.Select(r => -r.Phase).ToArray(), xLabel: "ft [GHz]", yLabel: $"Φ(R(ft)) [rad] @ rho={rho}mm",
                    title: $"Φ(R(ft)) [rad] @ rho={rho}mm (mua1={op1[0].RegionOP.Mua:F3}, mua2={op1[1].RegionOP.Mua:F3})"),
                LineChart(fts, rOfFt2.Select(r => -r.Phase).ToArray(), xLabel: "ft [GHz]", yLabel: $"Φ(R(ft)) [rad] @ rho={rho}mm",
                    title: $"Φ(R(ft)) [rad] @ rho={rho}mm (mua1={op2[0].RegionOP.Mua:F3}, mua2={op2[1].RegionOP.Mua:F3})"),
                LineChart(fts, rOfFt3.Select(r => -r.Phase).ToArray(), xLabel: "ft [GHz]", yLabel: $"Φ(R(ft)) [rad] @ rho={rho}mm",
                    title: $"Φ(R(ft)) [rad] @ rho={rho}mm (mua1={op3[0].RegionOP.Mua:F3}, mua2={op3[1].RegionOP.Mua:F3})")
            }
        );        
        var realChart = Chart.Combine(
        new[] {
                LineChart(fts, rOfFt1.Select(r => r.Real * magnitudeScale).ToArray(), xLabel: "ft [GHz]", yLabel: $"Re(R(ft)) [mm-2]*{magnitudeScale:E0}",
                    title: $"Re(R(ft)) [mm-2] @ rho={rho}mm (mua1={op1[0].RegionOP.Mua:F3}, mua2={op1[1].RegionOP.Mua:F3})"),       
                LineChart(fts, rOfFt2.Select(r => r.Real * magnitudeScale).ToArray(), xLabel: "ft [GHz]", yLabel: $"Re(R(ft)) [mm-2]*{magnitudeScale:E0}",
                    title: $"Re(R(ft)) [mm-2] @ rho={rho}mm (mua1={op2[0].RegionOP.Mua:F3}, mua2={op2[1].RegionOP.Mua:F3})"),       
                LineChart(fts, rOfFt3.Select(r => r.Real * magnitudeScale).ToArray(), xLabel: "ft [GHz]", yLabel: $"Re(R(ft)) [mm-2]*{magnitudeScale:E0}",
                    title: $"Re(R(ft)) [mm-2] @ rho={rho}mm (mua1={op3[0].RegionOP.Mua:F3}, mua2={op3[1].RegionOP.Mua:F3})")
            }
        );
        var imagChart = Chart.Combine(
        new[] {
                LineChart(fts, rOfFt1.Select(r => r.Imaginary * magnitudeScale).ToArray(), xLabel: "ft [GHz]", yLabel: $"Imag(R(ft)) [mm-2]*{magnitudeScale:E0}",
                    title: $"Imag(R(ft)) [mm-2] @ rho={rho}mm (mua1={op1[0].RegionOP.Mua:F3}, mua2={op1[1].RegionOP.Mua:F3})"),            
                LineChart(fts, rOfFt2.Select(r => r.Imaginary * magnitudeScale).ToArray(), xLabel: "ft [GHz]", yLabel: $"Imag(R(ft)) [mm-2]*{magnitudeScale:E0}",
                    title: $"Imag(R(ft)) [mm-2] @ rho={rho}mm (mua1={op2[0].RegionOP.Mua:F3}, mua2={op2[1].RegionOP.Mua:F3})"),            
                LineChart(fts, rOfFt3.Select(r => r.Imaginary * magnitudeScale).ToArray(), xLabel: "ft [GHz]", yLabel: $"Imag(R(ft)) [mm-2]*{magnitudeScale:E0}",
                    title: $"Imag(R(ft)) [mm-2] @ rho={rho}mm (mua1={op3[0].RegionOP.Mua:F3}, mua2={op3[1].RegionOP.Mua:F3})")
            }
        );
        var realImagChart = Chart.Grid(new[] { realChart, imagChart }, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);
        var ampPhaseChart = Chart.Grid(new[] { amplitudeChart, phaseChart }, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);
        if (showPlots)
        {
            realImagChart.Show();
            ampPhaseChart.Show();
        }
    }
}