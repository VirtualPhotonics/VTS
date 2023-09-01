namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate using the Perturbation Monte Carlo post-processor to calculate optical properties (i.e. "inversion")
/// </summary>
internal class Demo18ROfRhoMultiOpPropInversion : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 18: determine chromophore concentration by performing a non-linear least squares fit to a simulated measured
        // reflectance spectrum at a given source-detector separation, using a diffusion theory forward model

        // create an array of chromophore absorbers, each with a given concentrations
        var chromophoresForMeasuredData = new IChromophoreAbsorber[]
        {
            new ChromophoreAbsorber(ChromophoreType.HbO2, 70), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.Hb, 30), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.H2O, 0.8), // fractional concentration
        };

        // construct a scatterer
        var scatterer = new PowerLawScatterer(a: 1.2, b: 1.42);

        // compose a tissue using the chromophores and scatterer
        var tissue = new Tissue(chromophoresForMeasuredData, scatterer, "", n: 1.4); 

        // predict the tissue's optical properties spanning the visible and NIR spectral regimes
        //var wavelengths = new DoubleRange(start: 400, stop: 1000, number: 13).AsEnumerable().ToArray(); // range of wavelengths in nm
        var wavelengths = new DoubleRange(start: 400, stop: 1000, number: 601).ToArray(); // range of wavelengths in nm
        var measuredOPs = tissue.GetOpticalProperties(wavelengths);

        // Create some measurements, based on a Nurbs-based White Monte Carlo forward solver
        var rho = 1; //source - detector separation in mm
        var measurementForwardSolver = new NurbsForwardSolver();
        var measuredData = measurementForwardSolver.ROfRho(measuredOPs, rho);

        // Create a  forward solver based on pMC prediction (see implementation below; note: implemented for ROfRho only)
        var forwardSolverForInversion = new PointSourceSDAForwardSolver();

        // declare a local forward reflectance function that computes reflectance from chromophores
        // note that some variables are captured from the outer scope for simplicity (scatterer, wavelengths
        double[] CalculateReflectanceVsWavelengthFromChromophoreConcentration(double[] chromophoreConcentration, params object[] otherValuesNeededForForwardSolution)
        {
            // create an array of chromophore absorbers, each with a given concentrations
            var chromophoresLocal = new IChromophoreAbsorber[]
            {
                new ChromophoreAbsorber(ChromophoreType.HbO2, chromophoreConcentration[0]), // molar concentration
                new ChromophoreAbsorber(ChromophoreType.Hb, chromophoreConcentration[1]), // molar concentration
                new ChromophoreAbsorber(ChromophoreType.H2O, chromophoreConcentration[2]), // fractional concentration
            };

            // compose a tissue using the chromophores and scatterer, and return its optical properties
            var opsLocal = new Tissue(chromophoresLocal, scatterer, "", n: 1.4).GetOpticalProperties(wavelengths);

            // compute the reflectance based on that OP spectrum
            var measuredDataLocal = forwardSolverForInversion.ROfRho(opsLocal, rho);

            return measuredDataLocal;
        }

        // Run the inversion, based on Levenberg-Marquardt optimization.
        // Note: chi-squared weighting in the inversion is based on standard deviation of measured data
        var optimizer = new MPFitLevenbergMarquardtOptimizer();
        var initialGuess = new[] { 70, 30, 0.8 };
        var parametersToFit = new[] { true, true, true }; // fit all three chromophores simultaneously
        var fit = optimizer.Solve(
            initialGuess.Select(ig => ig).ToArray(), // make a copy, because the solver mutates this array
            parametersToFit,
            measuredData,
            measuredData.Select(_ => 1.0).ToArray(), // weight spectrum equally
            CalculateReflectanceVsWavelengthFromChromophoreConcentration);

        // calculate the final reflectance prediction at the fit values
        var fitChromophores = new IChromophoreAbsorber[]
        {
            new ChromophoreAbsorber(ChromophoreType.HbO2, fit[0]), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.Hb, fit[1]), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.H2O, fit[2]), // fractional concentration
        };
        var fitTissue = new Tissue(fitChromophores, scatterer, "", n: 1.4);
        var fitOpticalProperties = fitTissue.GetOpticalProperties(wavelengths);
        var fitReflectanceSpectrum = forwardSolverForInversion.ROfRho(fitOpticalProperties, rho);

        // calculate the initial guess reflectance prediction
        var initialGuessChromophores = new IChromophoreAbsorber[]
        {
            new ChromophoreAbsorber(ChromophoreType.HbO2, initialGuess[0]), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.Hb, initialGuess[1]), // molar concentration
            new ChromophoreAbsorber(ChromophoreType.H2O, initialGuess[2]), // fractional concentration
        };
        var initialGuessTissueOpProp = new Tissue(initialGuessChromophores, scatterer, "", n: 1.4).GetOpticalProperties(wavelengths);
        var initialGuessReflectanceSpectrum = forwardSolverForInversion.ROfRho(initialGuessTissueOpProp, rho);

        // plot and compare the results using Plotly.NET
        var logMeasuredReflectance = measuredData.Select(r => Math.Log(r)).ToArray();
        var logGuessReflectance = initialGuessReflectanceSpectrum.Select(r => Math.Log(r)).ToArray();
        var logFitReflectance = fitReflectanceSpectrum.Select(r => Math.Log(r)).ToArray();
        var (xLabel, yLabel) = ("wavelength [nm]", "log(R(λ)) [mm-2]");
        var chart = Chart.Combine(new[]
        {
            ScatterChart(wavelengths, logMeasuredReflectance, xLabel, yLabel, title: "Measured Data"),
            LineChart(wavelengths, logGuessReflectance, xLabel, yLabel, title: $"Initial guess (HbO2:{initialGuess[0]:F3}uM, Hb:{initialGuess[1]:F3}uM, H2O:{initialGuess[2]*100:F3}%)"),
            LineChart(wavelengths, logFitReflectance, xLabel, yLabel, title: $"Converged result (HbO2:{fit[0]:F3}uM, Hb:{fit[1]:F3}uM, H2O:{fit[2]*100:F3}%)")
        }); // show all three charts together

        if (showPlots)
        {
            chart.Show();
        }
    }
}