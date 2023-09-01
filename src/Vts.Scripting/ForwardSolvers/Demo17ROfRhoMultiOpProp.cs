namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of source-detector separation while varying absorption linearly
/// </summary>
internal class Demo17ROfRhoMultiOpProp : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 17: predict R(rho) based on a standard diffusion approximation solution to the time-dependent RTE
        // while varying absorption linearly

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var rhos = new DoubleRange(start: 0.2, stop: 1, number: 5).ToArray(); // range of source-detector separations in mm

        // create an array of chromophore absorbers, each with a given concentrations
        var chromophores = new IChromophoreAbsorber[]
        {
            new ChromophoreAbsorber(ChromophoreType.HbO2, 70), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.Hb, 30), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.H2O, 0.8), // fractional concentration
        };

        // construct a scatterer
        var scatterer = new PowerLawScatterer(a: 1.2, b: 1.42);

        // compose a tissue using the chromophores and scatterer
        var tissue = new Tissue(chromophores, scatterer, "", n: 1.4);

        // predict the tissue's optical properties spanning the visible and NIR spectral regimes
        var wavelengths = new DoubleRange(start: 450, stop: 1000, number: 1101).ToArray(); // range of wavelengths in nm
        var ops = tissue.GetOpticalProperties(wavelengths);

        // predict the radial reflectance response across the spectrum of optical properties
        var rOfFx = solver.ROfRho(ops, rhos);

        // Plot mua, log(mua), musp, and reflectance as a function of wavelength at each set of optical properties
        var rOfFxAmplitude = rOfFx.Select(Math.Abs).ToArray();
        var chart = Chart.Grid(new[]
        {
            LineChart(wavelengths, ops.Select(op => op.Mua).ToArray(), xLabel: "wavelength [nm]", yLabel: $"mua", title: "mua [mm-1]"),
            LineChart(wavelengths, ops.Select(op => Math.Log(op.Mua)).ToArray(), xLabel: "wavelength [nm]", yLabel: $"log(mua)", title: "log(mua [mm-1])"),
            LineChart(wavelengths, ops.Select(op => op.Musp).ToArray(), xLabel: "wavelength [nm]", yLabel: $"musp", title: "musp [mm-1]"),
            Chart.Combine(rhos.Select((rho, rhoIdx) => // plot R(wavelength)@rho for each rho in mm
                LineChart(wavelengths, rOfFxAmplitude.TakeEveryNth(rhos.Length, skip: rhoIdx).ToArray(),
                    xLabel: "wavelength [nm]", yLabel: $"R(wv)", title: $"R@rho={rho:F3} mm")))
        }, nRows: 4, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);

        if (showPlots)
        {
            chart.Show();
        }
    }
}