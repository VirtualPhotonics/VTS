namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of spatial frequency 
/// where optical properties vary as a function of wavelength using a Mie-theory scatterer
/// </summary>
internal class Demo15ROfFxMultiOpPropMie : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 15: predict R(fx) based on a standard diffusion approximation solution to the time-dependent RTE
        // where optical properties vary as a function of wavelength using a Mie-theory scatterer

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var fxs = new DoubleRange(start: 0, stop: 0.2, number: 5).ToArray(); // range of spatial frequencies in 1/mm

        // retrieve desired optical properties, based on spectral data information 

        // create an array of chromophore absorbers, each with a given concentrations
        var chromophores = new IChromophoreAbsorber[]
        {
            new ChromophoreAbsorber(ChromophoreType.HbO2, 70), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.Hb, 30), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.H2O, 0.8), // fractional concentration
        };

        // construct a scatterer
        var scatterer = new MieScatterer(particleRadius: 0.5, particleRefractiveIndex: 1.4, mediumRefractiveIndex: 1.0, volumeFraction: 0.001);
        // or: var scatterer = new IntralipidScatterer(volumeFraction: 0.01);
        // or: var scatterer = new PowerLawScatterer(a: 1.2, b: 1.42);

        // compose a tissue using the chromophores and scatterer
        var tissue = new Tissue(chromophores, scatterer, "", n: 1.4);

        // predict the tissue's optical properties spanning the visible and NIR spectral regimes
        var wavelengths = new DoubleRange(start: 450, stop: 1000, number: 1101).ToArray(); // range of wavelengths in nm
        var ops = tissue.GetOpticalProperties(wavelengths);

        // predict the spatial frequency response at each specified optical properties
        var rOfFx = solver.ROfFx(ops, fxs);

        // Plot mua, log(mua), musp, and reflectance as a function of wavelength at each set of optical properties
        var rOfFxAmplitude = rOfFx.Select(Math.Abs).ToArray();
        var chart = Chart.Grid(new[]
        {
            LineChart(wavelengths, ops.Select(op => op.Mua).ToArray(), xLabel: "wavelength [nm]", yLabel: $"mua", title: "mua [mm-1]"),
            LineChart(wavelengths, ops.Select(op => Math.Log(op.Mua)).ToArray(), xLabel: "wavelength [nm]", yLabel: $"log(mua)", title: "log(mua [mm-1])"),
            LineChart(wavelengths, ops.Select(op => op.Musp).ToArray(), xLabel: "wavelength [nm]", yLabel: $"musp", title: "musp [mm-1]"),
            Chart.Combine(fxs.Select((fx, fxi) => // plot R(wavelength)@fx for each spatial frequency, fx
                LineChart(wavelengths, rOfFxAmplitude.TakeEveryNth(fxs.Length, skip: fxi).ToArray(),
                    xLabel: "wavelength [nm]", yLabel: $"R(wv)", title: $"R@fx={fx:F3} mm-1")))
        }, nRows: 4, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);

        if (showPlots)
        {
            chart.Show();
        }
    }
}