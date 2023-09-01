namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting photon hitting density (PHD) for a two-layer medium
/// as a function of radial extent and depth at a given set of optical properties and source-detector separation
/// </summary>
internal class Demo07PhdOfRhoAndZTwoLayer : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 07: Compute photon hitting density (PHD) for a two-layer medium as a function
        // of radial extent and depth at a given set of optical properties and source-detector separation

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new TwoLayerSDAForwardSolver { SourceConfiguration = SourceConfiguration.Distributed };
        var topLayerThickness = 5; // mm
        var opRegions = new IOpticalPropertyRegion[]
        {
            new LayerOpticalPropertyRegion
            ( 
                zRange: new DoubleRange(0, topLayerThickness, 2),
                regionOP: new OpticalProperties(mua: 0.1, musp: 1, g: 0.8, n: 1.4)
            ),
            new LayerOpticalPropertyRegion
            ( 
                zRange: new DoubleRange(topLayerThickness, double.PositiveInfinity, 2),
                regionOP: new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4)
            )
        };
        var rhos = new DoubleRange(start: 0.1, stop: 19.9, number: 100).ToArray(); // range of s-d separations in mm
        var zs = new DoubleRange(start: 0.1, stop: 19.9, number: 100).ToArray(); // range of depths in mm

        // predict the tissue's fluence(rho, z) for the given optical properties 
        var fluenceOfRhoAndZ = solver.FluenceOfRhoAndZ(new[] { opRegions }, rhos, zs );

        var sourceDetectorSeparation = 10; // mm
        var phd = ComputationFactory.GetPHD(forwardSolverType: ForwardSolverType.PointSourceSDA,
            fluenceOfRhoAndZ, sourceDetectorSeparation, new[]{ opRegions[0].RegionOP }, rhos, zs); // todo: pick op based on layer thickness?

        // plot the PHD @ 10 mm s-d separation
        var allRhos = rhos;//.Select(rho => -rho).Reverse().Concat(rhos).ToArray(); // duplicate for -rho to make symmetric
        var phdRowsToPlot = phd
            .Select(fluence => Math.Log(fluence)) // take log for visualization purposes
            .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)    
            .ToArray();
        var allPhdRowsToPlot = phdRowsToPlot;//.Reverse().Concat(phdRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var phdChart = Heatmap(
            values: allPhdRowsToPlot, x: allRhos, y: zs,
            xLabel: "ρ [mm]", yLabel: "z [mm]", title: $"PHD(ρ, z) [unitless] @ s-d: {sourceDetectorSeparation} mm");
        
        if (showPlots)
        {
            phdChart.Show();
        }
    }
}