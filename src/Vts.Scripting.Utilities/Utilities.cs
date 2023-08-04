using Vts.Common;
using Vts.MonteCarlo;

namespace Vts.Scripting.Utilities;

public static class PlottingExtensions
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
}
