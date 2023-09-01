namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting absorbed energy as a function of 
/// radial extent and depth at a given set of optical properties 
/// </summary>
internal class Demo08AbsorbedEnergyOfRhoAndZ : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 08: Compute absorbed energy as a function of radial extent and depth at a given set of optical properties 

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver { SourceConfiguration = SourceConfiguration.Distributed };
        var op = new OpticalProperties(mua: 0.1, musp: 1, g: 0.8, n: 1.4);
        var rhos = new DoubleRange(start: 0.1, stop: 19.9, number: 100).ToArray(); // range of s-d separations in mm
        var zs = new DoubleRange(start: 0.1, stop: 19.9, number: 100).ToArray(); // range of depths in mm

        // predict the tissue's AbsorbedEnergy(rho, z) for the given optical properties         
        var fluenceOfRhoAndZ = solver.FluenceOfRhoAndZ(new[] { op }, rhos, zs );
        var absorbedEnergyOfRhoAndZ = ComputationFactory.GetAbsorbedEnergy(fluenceOfRhoAndZ, op.Mua).ToArray();

        var allRhos = rhos.Select(rho => -rho).Reverse().Concat(rhos).ToArray(); // duplicate for -rho to make symmetric
        var absorbedEnergyRowsToPlot = absorbedEnergyOfRhoAndZ
            .Select(ae => Math.Log(ae)) // take log for visualization purposes
            .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)        
            .ToArray();
        var allAbsorbedEnergyRowsToPlot = absorbedEnergyRowsToPlot.Reverse()
            .Concat(absorbedEnergyRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var absorbedEnergyChart = Heatmap(
            values: allAbsorbedEnergyRowsToPlot, x: allRhos, y: zs,
            xLabel: "ρ [mm]", yLabel: "z [mm]", title: $"log(AbsorbedEnergy(ρ, z) [mm-3])");

        if (showPlots)
        {
            absorbedEnergyChart.Show();
        }
    }
}