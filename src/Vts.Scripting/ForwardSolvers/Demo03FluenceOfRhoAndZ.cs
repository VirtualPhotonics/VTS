namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of 
/// radial extent and depth using optical properties from a list of chromophore absorbers 
/// with their concentrations and a power law scatterer for a range of wavelengths
/// </summary>
internal class Demo03FluenceOfRhoAndZ : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 03: Evaluate fluence as a function of radial extent and depth using optical properties from a list of
        // chromophore absorbers with their concentrations and a power law scatterer for a range of wavelengths.

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var rhos = new DoubleRange(start: 0.1, stop: 10, number: 100).ToArray(); // range of s-d separations in mm
        var zs = new DoubleRange(start: 0.1, stop: 10, number: 100).ToArray(); // range of depths in mm

        // retrieve desired optical properties, based on spectral data information 

        // create an array of chromophore absorbers, each with a given concentrations
        var chromophores = new IChromophoreAbsorber[]
        {
            new ChromophoreAbsorber(ChromophoreType.HbO2, 70), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.Hb, 30), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.H2O, 0.8), // fractional concentration
        };

        // construct a scatterer
        var scatterer = new PowerLawScatterer(a: 1.2, b: 1.42);
        // or: var scatterer = new IntralipidScatterer(volumeFraction: 0.5);
        // or: var scatterer = new MieScatterer(particleRadius: 0.5, particleRefractiveIndex: 1.4, mediumRefractiveIndex: 1.0, volumeFraction: 0.5);
        
        // compose a tissue using the chromophores and scatterer
        var tissue = new Tissue(chromophores, scatterer, "", n: 1.4);

        // predict the tissue's fluence(rho, z) for tissue optical properties spanning the visible and NIR spectral regimes
        var wavelengths = new DoubleRange(start: 450, stop: 1000, number: 1101).ToArray(); // range of wavelengths in nm
        var op = tissue.GetOpticalProperties(wavelengths);
        var fluenceOfRhoAndZ = solver.FluenceOfRhoAndZ(op, rhos, zs);

        // Plot the log(fluence(rho, z)) for the last wavelength
        var wvi = wavelengths.Length - 1; // index of last wavelength (1000nm)
        var imageSize = rhos.Length * zs.Length;
        var allRhos = rhos.Select(rho => -rho).Reverse().Concat(rhos).ToArray(); // duplicate for -rho to make symmetric
        var fluenceRowsToPlot = fluenceOfRhoAndZ            
            .Skip(wvi * imageSize) // skip to the last wavelength
            .Select(fluence => Math.Log(fluence)) // take log for visualization purposes
            .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)
            .ToArray();
        var fluenceDataToPlot = fluenceRowsToPlot.Reverse().Concat(fluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var map = Heatmap(values: fluenceDataToPlot, x: allRhos, y: zs,
            xLabel: "ρ [mm]", yLabel: "z [mm]", title: $"log(Φ(ρ, z) @λ={wavelengths[wvi]}nm");

        if (showPlots)
        {
            map.Show();
        }
    }
}