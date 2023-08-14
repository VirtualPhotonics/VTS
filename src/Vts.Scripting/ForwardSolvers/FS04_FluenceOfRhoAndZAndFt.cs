using Vts.Common;
using Vts.Modeling.ForwardSolvers;
using Plotly.NET.CSharp;
using System;

namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of 
/// s-d separation, depth, and time frequency at a given set of optical properties
/// </summary>
public class FS04_FluenceOfRhoAndZAndFt : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo()
    {
        // Example 04: Evaluate fluence as a function of rs-d separation, depth, 
        // and time frequency at a given set of optical properties

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var op = new OpticalProperties(mua: 0.01, musp: 1, g: 0.8, n: 1.4);
        var rhos = new DoubleRange(start: 0.1, stop: 19.9, number: 100).AsEnumerable().ToArray(); // range of s-d separations in mm
        var zs = new DoubleRange(start: 0.1, stop: 19.9, number: 100).AsEnumerable().ToArray(); // range of depths in mm
        var fts = new DoubleRange(start: 0, stop: 1, number: 2).AsEnumerable().ToArray(); // range of time frequencies in GHz

        // predict the tissue's fluence(rho, z, ft) for the given optical properties 
        var fluenceOfRhoAndZAndFt = solver.FluenceOfRhoAndZAndFt(new[]{ op }, rhos, zs, fts);

        // collecte the steady-state (CW) and frequency domain fluence maps for plotting
        // (innermost axis is ft, so we need to take every 2nd starting at index 0)
        var (ft1, ft2) = (0, 1); // indices of the two time frequencies, ft, to plot
        var cwFluenceOfRhoAndZ = fluenceOfRhoAndZAndFt.Where((_, i) => (i - ft1) % fts.Length == 0);
        var fdFluenceOfRhoAndZ = fluenceOfRhoAndZAndFt.Where((_, i) => (i - ft2) % fts.Length == 0);

        var imageSize = rhos.Length * zs.Length;
        var allRhos = rhos.Select(rho => -rho).Reverse().Concat(rhos).ToArray(); // duplicate for -rho to make symmetric
        // Plot the CW fluence: log(fluence(rho, z, ft=0GHz)) 
        var cwFluenceRowsToPlot = cwFluenceOfRhoAndZ
            .Select(fluence => Math.Log(fluence.Magnitude)) // take log for visualization purposes
            .Chunk(zs.Length); // break the heatmap into rows (inner dimension is zs)        
        var allCwFluenceRowsToPlot = cwFluenceRowsToPlot.Reverse().Concat(cwFluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var cwFluenceChart = Heatmap(values: allCwFluenceRowsToPlot, x: allRhos, y: zs, xLabel: "rho", yLabel: "z", title: "log(Φ(rho, z, ft=0Ghz))");

        // Plot the frequency-domain fluence amplitude at 1GHz: log(fluence(rho, z, ft=0GHz)) 
        var fdFluenceRowsToPlot = fdFluenceOfRhoAndZ
            .Select(fluence => Math.Log(fluence.Magnitude)) // take log for visualization purposes
            .Chunk(zs.Length); // break the heatmap into rows (inner dimension is zs)        
        var allFdFluenceRowsToPlot = fdFluenceRowsToPlot.Reverse().Concat(fdFluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var fdFluenceChart = Heatmap(values: allFdFluenceRowsToPlot, x: allRhos, y: zs, xLabel: "rho", yLabel: "z", title: "log(Φ(rho, z, ft=1Ghz))");

        // show on separate plots, until we can figure out how to get separate colorbars for each
        cwFluenceChart.Show(); 
        fdFluenceChart.Show();
        //Chart.Grid(new[]{ cwFluenceChart, fdFluenceChart }, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Independent).Show();

        // calculate the modulation by taking an element-wise ratio of the fluence maps (i.e. FD/Cw)
        var modFluenceOfRhoAndZ = fdFluenceOfRhoAndZ.Zip(cwFluenceOfRhoAndZ, (fd, cw) => fd.Magnitude / cw.Magnitude).ToArray();

        // Plot the frequency-domain fluence amplitude at 1GHz: log(fluence(rho, z, ft=0GHz)) 
        var modFluenceRowsToPlot = modFluenceOfRhoAndZ
            .Chunk(zs.Length); // break the heatmap into rows (inner dimension is zs)        
        var allModFluenceRowsToPlot = modFluenceRowsToPlot.Reverse().Concat(modFluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var modFluenceChart = Heatmap(values: allModFluenceRowsToPlot, x: allRhos, y: zs, xLabel: "rho", yLabel: "z", title: "modulation(rho, z) @ ft=1Ghz))");
        modFluenceChart.Show();

        // todo: implement/use slicing and Span instead of slow LINQ queries
    }
}