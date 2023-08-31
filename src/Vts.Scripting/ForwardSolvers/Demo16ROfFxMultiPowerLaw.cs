namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of spatial frequency 
/// where optical properties vary as a function of wavelength using a range of power-law scatterer prefactors, A
/// </summary>
internal class Demo16ROfFxMultiPowerLaw : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 16: predict R(fx) based on a standard diffusion approximation solution to the time-dependent RTE
        // where optical properties vary as a function of wavelength using a range of power-law scatterer prefactors, A

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();

        // retrieve desired optical properties, based on spectral data information 

        // create an array of chromophore absorbers, each with a given concentrations
        var chromophores = new IChromophoreAbsorber[]
        {
            new ChromophoreAbsorber(ChromophoreType.HbO2, 70), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.Hb, 30), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.H2O, 0.8), // fractional concentration
        };

        var fx = 0; // spatial frequency in 1/mm

        // predict the tissue's optical properties spanning the visible and NIR spectral regimes
        var wavelengths = new DoubleRange(start: 450, stop: 1000, number: 1101).ToArray(); // range of wavelengths in nm

        var prefactorAs = new DoubleRange(start: 0.5, stop: 2.5, number: 9).ToArray(); // range of spatial frequencies in 1/mm
        var opsForMultipleA = prefactorAs.Select(prefactorA => 
            new Tissue(chromophores, new PowerLawScatterer(a: prefactorA, b: 1.42), "", n: 1.4).GetOpticalProperties(wavelengths)).ToArray();

        // predict the wavelength response for each spectrum of optical properties given by a specific A prefactor
        var rVsWavelengthForEachA = solver.ROfFx(opsForMultipleA.SelectMany(op => op).ToArray(), fx );

        // Plot mua, log(mua), musp, and reflectance as a function of wavelength at each set of optical properties
        var chart = Chart.Grid(new[]
        {
            LineChart(wavelengths, opsForMultipleA[0].Select(op => op.Mua).ToArray(), xLabel: "wavelength [nm]", yLabel: $"mua", title: "mua [mm-1]"),
            LineChart(wavelengths, opsForMultipleA[0].Select(op => Math.Log(op.Mua)).ToArray(), xLabel: "wavelength [nm]", yLabel: $"log(mua)", title: "log(mua [mm-1])"),
            Chart.Combine(prefactorAs.Select((prefactorA, pai) => 
                LineChart(wavelengths, opsForMultipleA[pai].Select(op => op.Musp).ToArray(), 
                    xLabel: "wavelength [nm]", yLabel: $"musp", title: $"musp@A={prefactorA:F3} [mm-1]"))
            ),
            Chart.Combine(prefactorAs.Select((prefactorA, pai) => // plot R(wavelength) @ fx=0 for each prefactor A
                LineChart(wavelengths, rVsWavelengthForEachA.Skip(wavelengths.Length * pai).Take(wavelengths.Length).ToArray(),
                    xLabel: "wavelength [nm]", yLabel: $"R(wv)", title: $"R@A={prefactorA:F3}"))
            )
        }, nRows: 4, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);

        if (showPlots)
        {
            chart.Show();
        }
    }
}