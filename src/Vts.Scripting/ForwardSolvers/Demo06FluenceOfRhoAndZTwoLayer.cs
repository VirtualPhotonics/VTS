namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting fluence in a multi-layer tissue
/// as a function of radial extent and depth at a given set of optical properties 
/// </summary>
internal class Demo06FluenceOfRhoAndZTwoLayer : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 06: Compute fluence for a two-layer medium as a function
        // of radial extent and depth at a given set of optical properties 

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

        var allRhos = rhos.Select(rho => -rho).Reverse().Concat(rhos).ToArray(); // duplicate for -rho to make symmetric
        var fluenceRowsToPlot = fluenceOfRhoAndZ
            .Select(fluence => Math.Log(fluence)) // take log for visualization purposes
            .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)        
            .ToArray();
        var allFluenceRowsToPlot = fluenceRowsToPlot.Reverse().Concat(fluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var fluenceChart = Heatmap(
            values: allFluenceRowsToPlot, x: allRhos, y: zs,
            xLabel: "ρ [mm]", yLabel: "z [mm]", title: $"log(Φ(ρ, z) [mm-3])");

        if (showPlots)
        {
            fluenceChart.Show();
        }
    }
}