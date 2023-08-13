using Vts.Common;
using Vts.Modeling.ForwardSolvers;
using Vts.SpectralMapping;
using Plotly.NET;

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

        // Plot the CW fluence: log(fluence(rho, z, ft=0GHz)) 
        var fluenceRowsToPlot = fluenceOfRhoAndZAndFt          
            .Take(imageSize) // take the first rho-z map (CW data)
            .Select(fluence => Math.Log(fluence.Magnitude)) // take log for visualization purposes
            .Chunk(zs.Length); // break the heatmap into rows (inner dimension is zs)        
        var fluenceDataToPlot = fluenceRowsToPlot.Reverse().Concat(fluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var cwFluenceChart = Heatmap(values: fluenceDataToPlot, x: zs, y: rhos.Reverse().Concat(rhos).ToArray(), title: "log(Φ(rho, z, ft=0Ghz))");


        // Plot the frequency-domain fluence amplitude at 1GHz: log(fluence(rho, z, ft=0GHz)) 
        var fluenceRowsToPlot2 = fluenceOfRhoAndZAndFt          
            .Skip(imageSize) // skip to the second rho-z map (f-d data)
            .Select(fluence => Math.Log(fluence.Magnitude)) // take log for visualization purposes
            .Chunk(zs.Length); // break the heatmap into rows (inner dimension is zs)        
        var fluenceDataToPlot2 = fluenceRowsToPlot.Reverse().Concat(fluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
        var fdFluenceChart = Heatmap(values: fluenceDataToPlot2, x: zs, y: rhos.Reverse().Concat(rhos).ToArray(), title: "log(Φ(rho, z, ft=1Ghz))");

        Chart.Grid(new[]{ cwFluenceChart, fdFluenceChart }, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled).Show();
    }
}