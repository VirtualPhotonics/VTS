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

        var imageSize = rhos.Length * zs.Length;
        var allRhos = rhos.Select(rho => -rho).Reverse().Concat(rhos).ToArray(); // duplicate for -rho to make symmetric
        // Plot the CW fluence: log(fluence(rho, z, ft=0GHz)) 
        var fti = 0; // index of ft to plot
        var fluenceRowsToPlot = fluenceOfRhoAndZAndFt
            .Where((_, i) => (i - fti) % fts.Length == 0) // (innermost axis is ft, so we need to take every 2nd starting at index 0)
            .Select(fluence => Math.Log(fluence.Magnitude)) // take log for visualization purposes
            .Chunk(zs.Length); // break the heatmap into rows (inner dimension is zs)        
        var allFluenceRowsToPlot = fluenceRowsToPlot.Reverse().Concat(fluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var cwFluenceChart = Heatmap(values: allFluenceRowsToPlot, x: allRhos, y: zs, xLabel: "rho", yLabel: "z", title: "log(Φ(rho, z, ft=0Ghz))");

        // Plot the frequency-domain fluence amplitude at 1GHz: log(fluence(rho, z, ft=0GHz)) 
        var fti2 = 1; // index of ft to plot
        var fluenceRowsToPlot2 = fluenceOfRhoAndZAndFt
            .Where((_, i) => (i - fti2) % fts.Length == 0) // (innermost axis is ft, so we need to take every 2nd starting at index 1)
            .Select(fluence => Math.Log(fluence.Magnitude)) // take log for visualization purposes
            .Chunk(zs.Length); // break the heatmap into rows (inner dimension is zs)        
        var allFluenceRowsToPlot2 = fluenceRowsToPlot2.Reverse().Concat(fluenceRowsToPlot2).ToArray(); // duplicate for -rho to make symmetric
        var fdFluenceChart = Heatmap(values: allFluenceRowsToPlot2, x: allRhos, y: zs, xLabel: "rho", yLabel: "z", title: "log(Φ(rho, z, ft=1Ghz))");

        // show on separate plots, until we can figure out how to get separate colorbars for each
        cwFluenceChart.Show(); 
        fdFluenceChart.Show();
        //Chart.Grid(new[]{ cwFluenceChart, fdFluenceChart }, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Independent).Show();

        // todo: implement/use slicing and Span instead of slow LINQ queries
    }
}