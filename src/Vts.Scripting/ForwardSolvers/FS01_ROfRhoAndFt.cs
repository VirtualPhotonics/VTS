﻿using Vts.Common;
using Vts.Modeling.ForwardSolvers;
using Plotly.NET.CSharp;

namespace Vts.Scripting.ForwardSolvers;

/// <summary>
/// Class using the Vts.dll library to demonstrate predicting reflectance as a function of rho and time frequency
/// </summary>
public class FS01_ROfRhoAndFt : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo()
    {
        // Example 01: predict R(rho, ft) based on a standard diffusion approximation solution to the time-dependent RTE

        // Solver type options:
        // PointSourceSDA,DistributedGaussianSourceSDA, DistributedPointSourceSDA,
        // MonteCarlo(basic scaled), Nurbs(scaled with smoothing and adaptive binning)
        var solver = new PointSourceSDAForwardSolver();
        var op = new OpticalProperties(mua: 0.01, musp: 1.2, g: 0.8, n: 1.4);
        var rho = 10; // s-d separation, in mm
        var ft = new DoubleRange(start: 0, stop: 0.5, number: 51).AsEnumerable().ToArray(); // range of temporal frequencies in GHz

        // predict the temporal frequency response at the specified optical properties and s-d separation
        var rOfFt = solver.ROfRhoAndFt(op, rho, ft);
        var magnitudeScale = 1E4;
        var rOfFtAmplitude = rOfFt.Select(r => r.Magnitude * magnitudeScale).ToArray();
        var rOfFtPhase = rOfFt.Select(r => -r.Phase).ToArray();

        //%% Example ROfRhoAndFt
        //% Evaluate reflectance as a function of rho and temporal-frequency with one 
        var xLabel = "time frequency [GHz]";
        var charts = new[]
        {
            LineChart(ft, rOfFtAmplitude, xLabel, yLabel: $"|R(ft)@ρ={rho}mm| [mm-2] * {magnitudeScale:E0}"),
            LineChart(ft, rOfFtPhase, xLabel, yLabel: $"Φ(R(ft)@ρ={rho}mm) [rad]")
        };
        Chart.Grid(charts, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled).Show();
    }
}