namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting fluence as a function of 
/// radial extent, depth, and time frequency at a given set of optical properties
/// </summary>
internal class Demo04FluenceOfRhoAndZAndFt : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 04: Evaluate fluence as a function of radial extent, depth, 
        // and time frequency at a given set of optical properties

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var op = new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4);
        var rhos = new DoubleRange(start: 0.1, stop: 19.9, number: 100).ToArray(); // range of s-d separations in mm
        var zs = new DoubleRange(start: 0.1, stop: 19.9, number: 100).ToArray(); // range of depths in mm
        var fts = new DoubleRange(start: 0, stop: 1, number: 2).ToArray(); // range of time frequencies in GHz

        // predict the tissue's fluence(rho, z, ft) for the given optical properties 
        var fluenceOfRhoAndZAndFt = solver.FluenceOfRhoAndZAndFt(new[]{ op }, rhos, zs, fts);

        // collect the steady-state (CW) and frequency domain fluence maps for plotting
        // (innermost axis is ft, so we need to take every 2nd starting at index 0)
        var (ft1, ft2) = (0, 1); // indices of the two time frequencies, ft, to plot
        var cwFluenceOfRhoAndZ = fluenceOfRhoAndZAndFt.TakeEveryNth(n: fts.Length, skip: ft1).ToArray();
        var fdFluenceOfRhoAndZ = fluenceOfRhoAndZAndFt.TakeEveryNth(n: fts.Length, skip: ft2).ToArray();

        var allRhos = rhos.Select(rho => -rho).Reverse().Concat(rhos).ToArray(); // duplicate for -rho to make symmetric
        // Plot the CW fluence: log(fluence(rho, z, ft=0GHz)) 
        var cwFluenceRowsToPlot = cwFluenceOfRhoAndZ
            .Select(fluence => Math.Log(fluence.Magnitude)) // take log for visualization purposes
            .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)
            .ToArray();
        var allCwFluenceRowsToPlot = cwFluenceRowsToPlot.Reverse().Concat(cwFluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var cwFluenceChart = Heatmap(values: allCwFluenceRowsToPlot, x: allRhos, y: zs, xLabel: "ρ", yLabel: "z", title: "log(Φ(ρ, z, ft=0Ghz))");

        // Plot the frequency-domain fluence amplitude at 1GHz: log(fluence(rho, z, ft=0GHz)) 
        var fdFluenceRowsToPlot = fdFluenceOfRhoAndZ
            .Select(fluence => Math.Log(fluence.Magnitude)) // take log for visualization purposes
            .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)
            .ToArray();
        var allFdFluenceRowsToPlot = fdFluenceRowsToPlot.Reverse().Concat(fdFluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var fdFluenceChart = Heatmap(values: allFdFluenceRowsToPlot, x: allRhos, y: zs, xLabel: "ρ", yLabel: "z", title: "log(Φ(ρ, z, ft=1Ghz))");

        // calculate the modulation by taking an element-wise ratio of the fluence maps (i.e. FD/Cw)
        var modFluenceOfRhoAndZ = fdFluenceOfRhoAndZ.Zip(cwFluenceOfRhoAndZ, (fd, cw) => fd.Magnitude / cw.Magnitude).ToArray();

        // Plot the modulation at 1GHz: log(fluence(rho, z, ft=1GHz)/fluence(rho, z, ft=0GHz)) 
        var modFluenceRowsToPlot = modFluenceOfRhoAndZ
            .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)
            .ToArray();
        var allModFluenceRowsToPlot = modFluenceRowsToPlot.Reverse().Concat(modFluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var modFluenceChart = Heatmap(values: allModFluenceRowsToPlot, x: allRhos, y: zs,
            xLabel: "ρ [mm]", yLabel: "z [mm]", title: "modulation(ρ, z) @ ft=1Ghz))");

        if (showPlots)
        {
            cwFluenceChart.Show();
            fdFluenceChart.Show();
            modFluenceChart.Show();
        }
    }
}