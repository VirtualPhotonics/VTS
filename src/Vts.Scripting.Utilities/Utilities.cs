using Vts.Common;

namespace Vts.Scripting.Utilities;

public static class PlottingExtensions
{
    public static double[] GetMidpoints(this DoubleRange endpointRange)
    {
        var endpoints = endpointRange.AsEnumerable().ToArray();
        if (endpoints.Length < 2)
        {
            throw new ArgumentException("Endpoints must have at least two elements");
        }

        var midpoints = new double[endpoints.Length - 1];
        for (int i = 0; i < midpoints.Length; i++)
        {
            midpoints[i] = endpoints[i + 1] - endpoints[i];
        }
        return endpoints;
    }
}
