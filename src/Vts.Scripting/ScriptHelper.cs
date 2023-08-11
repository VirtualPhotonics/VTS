global using static Vts.Scripting.ScriptHelper;
using Vts.Common;
using Vts.MonteCarlo;
using Plotly.NET.CSharp;

namespace Vts.Scripting;

public static class ScriptHelper
{
    /// <summary>
    /// Helper extension method that returns an array of midpoints, located halfway between the endpoints of the specified range
    /// </summary>
    /// <param name="endpointRange">The range of endpoints</param>
    /// <returns>The corresponding midpoint outputs</returns>
    public static double[] GetMidpoints(this DoubleRange endpointRange)
    {
        var endpoints = endpointRange.AsEnumerable().ToArray();
        if (endpoints.Length < 2)
        {
            return Array.Empty<double>();
        }

        var midpoints = new double[endpoints.Length - 1];
        for (int i = 0; i < midpoints.Length; i++)
        {
            midpoints[i] = endpoints[i + 1] - endpoints[i];
        }
        return endpoints;
    }

    /// <summary>
    /// Helper extension method that returns an array of all detectors matching the concrete type TDetector
    /// </summary>
    /// <typeparam name="TDetector">Concrete type to match</typeparam>
    /// <param name="output">The simulation output source</param>
    /// <returns>Array of all detectors matching the concrete type TDetector</returns>
    public static TDetector[] GetAllDetectorsOfType<TDetector>(this SimulationOutput output)
    {
        var detectors = output?.ResultsDictionary?.Values
            .Where(d => d is TDetector)
            .Select(d => (TDetector)d)
            .ToArray();

        return detectors ?? Array.Empty<TDetector>();
    }

    public static Plotly.NET.GenericChart.GenericChart ScatterChart(double[] xValues, double[] yValues, string xLabel = "", string yLabel = "", string title = "")
    {
        return Chart.Point<double, double, string>(xValues, yValues).WithStandardStyling(xLabel, yLabel, title);
    }

    public static Plotly.NET.GenericChart.GenericChart LineChart(double[] xValues, double[] yValues, string xLabel = "", string yLabel = "", string title = "")
    {
        return Chart.Line<double, double, string>(xValues, yValues).WithStandardStyling(xLabel, yLabel, title);
    }

    /// <summary>
    /// Fluent helper method to apply standard styling to a chart
    /// </summary>
    /// <param name="chart"></param>
    /// <param name="title"></param>
    /// <param name="xLabel"></param>
    /// <param name="yLabel"></param>
    /// <returns></returns>
    private static Plotly.NET.GenericChart.GenericChart WithStandardStyling(
        this Plotly.NET.GenericChart.GenericChart chart, string xLabel = "", string yLabel = "", string title = "")
    {
        return chart.WithTraceInfo(title, ShowLegend: !string.IsNullOrWhiteSpace(title))
             .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init(xLabel))
             .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init(yLabel))
             .WithLegendStyle(X: 0, Y: 150);
    }
}
