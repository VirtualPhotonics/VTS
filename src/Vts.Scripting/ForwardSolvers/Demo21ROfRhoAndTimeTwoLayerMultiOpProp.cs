namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of time at a fixed source-detector location
/// for multiple sets of optical properties using a two-layer forward solver
/// </summary>
internal class Demo21ROfRhoAndFtTwoLayerMultiOpProp : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 21: predict R(t) at a fixed source-detector location based on a standard diffusion approximation solution
        // to the time-dependent RTE for multiple sets of optical properties using a two-layer forward solver

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new TwoLayerSDAForwardSolver();
        var ts = new DoubleRange(start: 0, stop: 0.5, number: 51).ToArray(); // range of times in 1/mm
        
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

        // predict the bulk tissue's NIR optical properties
        var wavelengths = new DoubleRange(start: 650, stop: 850, number: 3).ToArray(); // range of wavelengths in nm
        var opsBottomLayer = tissue.GetOpticalProperties(wavelengths);

        // perturb the top layer's mua by a multiplicative factor
        var muaPerturbationFactor = 1.1; 
        var opsTopLayer = opsBottomLayer
            .Select(op => new OpticalProperties(mua: op.Mua * muaPerturbationFactor, musp: op.Musp, g: op.G, n: op.N))
            .ToArray();

        // create the multilayer optical properties with the perturbed top layer values
        var op1 = new IOpticalPropertyRegion[]
        {
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: opsTopLayer[0]),
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: opsBottomLayer[0]),
        };
        var op2 = new IOpticalPropertyRegion[]
        {
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: opsTopLayer[1]),
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: opsBottomLayer[1]),
        };
        var op3 = new IOpticalPropertyRegion[]
        {
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: opsTopLayer[2]),
            new LayerOpticalPropertyRegion(zRange: new DoubleRange(0, 2, 2),  regionOP: opsBottomLayer[2]),
        };

        // predict reflectance versus time at each specified optical properties for the given s-d separation
        var rho = 10; // s-d separation in mm
        var rOfTime1 = solver.ROfRhoAndTime(op1, rho, ts);
        var rOfTime2 = solver.ROfRhoAndTime(op2, rho, ts);
        var rOfTime3 = solver.ROfRhoAndTime(op3, rho, ts);

        // Plot reflectance as a function of times at each set of optical properties
        var chart = Chart.Combine(
            new[] {
                LineChart(ts, rOfTime1, xLabel: "t [ns]", yLabel: $"R(t) [mm-2*ns-1] @ rho={rho}mm",
                    title: $"R(t) [mm-2*s-1] @ rho={rho}mm (mua1={op1[0].RegionOP.Mua:F3}, mua2={op1[1].RegionOP.Mua:F3})"),
                LineChart(ts, rOfTime2, xLabel: "t [ns]", yLabel: $"R(t) [mm-2*ns-1] @ rho={rho}mm",
                    title: $"R(t) [mm-2*s-1] @ rho={rho}mm (mua1={op2[0].RegionOP.Mua:F3}, mua2={op2[1].RegionOP.Mua:F3})"),
                LineChart(ts, rOfTime3, xLabel: "t [ns]", yLabel: $"R(t) [mm-2*ns-1] @ rho= {rho}mm",
                    title: $"R(t) [mm-2*s-1] @ rho={rho}mm (mua1={op3[0].RegionOP.Mua:F3}, mua2={op3[1].RegionOP.Mua:F3})")
            }
        );

        if (showPlots)
        {
            chart.Show();
        }
    }
}