global using static Vts.Scripting.ScriptHelper;
using Vts.Common;
using Vts.MonteCarlo;
using Plotly.NET.CSharp;
using Plotly.NET.LayoutObjects;

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
    /// Helper extension method that returns every nth element of the enumerable, starting at the specified skip index
    /// </summary>
    /// <param name="values">the values being filtered</param>
    /// <param name="n">number of values to jump forward at a time</param>
    /// <param name="skip">number of values to initially skip</param>
    /// <returns></returns>
    public static IEnumerable<double> TakeEveryNth(this IEnumerable<double> values, int n, int skip = 0) =>
            values.Where((_, i) => (i - skip) % n == 0);

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
        return Plotly.NET.CSharp.Chart.Point<double, double, string>(xValues, yValues).WithStandardStyling(xLabel, yLabel, title);
    }

    public static Plotly.NET.GenericChart.GenericChart LineChart(double[] xValues, double[] yValues, string xLabel = "", string yLabel = "", string title = "")
    {
        return Plotly.NET.CSharp.Chart.Line<double, double, string>(xValues, yValues).WithStandardStyling(xLabel, yLabel, title);
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
        // uses Plotly.NET.CSharp.ChartExtensions (adding Plotly.NET to the using statements above will break this)
        return chart
            .WithTraceInfo(title, ShowLegend: !string.IsNullOrWhiteSpace(title))
            .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init(xLabel))
            .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init(yLabel))
            .WithLegendStyle(X: 0, Y: 150);
    }

    /// <summary>
    /// Helper method to format a heatmap chart
    /// </summary>
    /// <param name="values"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static Plotly.NET.GenericChart.GenericChart Heatmap(
        IEnumerable<double[]> values, 
        double[]? x = null, 
        double[]? y = null,
        string xLabel = "",
        string yLabel = "",
        string title = "")
    {
        // attn devs: for reference, the following are the type parameters used in the call to Chart2D.Chart.Heatmap:
        // Chart2D.Chart.Heatmap<a37: (row format), a38: (fluence value type), a39: X (rho value type), a40: Y (z value type), a41: Text type>(...)
        var chart = Plotly.NET.Chart2D.Chart.Heatmap<IEnumerable<double>, double, double, double, string>(
            zData: values,
            X: x, Y: y,
            ReverseScale: false, ReverseYAxis: true,
            Transpose: true,
            Text: title,
            ColorScale: Plotly.NET.StyleParam.Colorscale.Viridis
        ).WithTraceInfo(title, ShowLegend: !string.IsNullOrWhiteSpace(title))
         .WithLegendStyle(X: 0, Y: 150);
        
        //var chartLayout = Plotly.NET.GenericChart.getLayout(chart);
        //var yAxis = Plotly.NET.Layout.getLinearAxisById(Plotly.NET.StyleParam.SubPlotId.NewYAxis(1)).Invoke(chartLayout);
        //yAxis.SetValue("scaleanchor", Plotly.NET.StyleParam.LinearAxisId.NewX(1));
        //chart = Plotly.NET.GenericChartExtensions.WithYAxis(chart, yAxis);

        chart = Plotly.NET.GenericChartExtensions
            .WithYAxis(chart, LinearAxis.init<IConvertible, IConvertible, IConvertible, IConvertible, double, IConvertible>(
                ScaleAnchor: Plotly.NET.StyleParam.LinearAxisId.NewX(1), AxisType: Plotly.NET.StyleParam.AxisType.Linear))
            .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init(xLabel), MinMax: new Tuple<double, double>(x[0], x[^1]))
            .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init(yLabel), MinMax: new Tuple<double, double>(y[0], y[^1]));

        chart = Plotly.NET.GenericChartExtensions
            .WithColorbar(chart, title: Plotly.NET.Title.init(title));

        return chart;
    }
}
