using System;
using System.Linq;

namespace Vts.Modeling
{
    public static class NumericalDerivativeExtensions
    {
        private static double _delta = 0.01;

        public static void SetDelta(double delta)
        {
            _delta = delta;
        }
        
        public static Func<object[], double[]> GetDerivativeFunc(
           this Func<object[], double[]> myFunc, ForwardAnalysisType analysisType)
        {
            switch (analysisType)
            {
                case ForwardAnalysisType.dRdMua:
                    return parameters =>
                    {
                        var less = parameters;
                        var more = parameters;
                        var op = (OpticalProperties[])parameters;
                        less[0] = op.Select(opi => new OpticalProperties(opi.Mua * (1 - _delta), opi.Musp, opi.G, opi.N)).ToArray();
                        more[0] = op.Select(opi => new OpticalProperties(opi.Mua * (1 + _delta), opi.Musp, opi.G, opi.N)).ToArray();
                        return Vts.Extensions.EnumerableExtensions.Zip(
                            myFunc(more),
                            myFunc(less),
                            op,
                            (lessi, morei, opi) => (lessi - morei) / (2 * opi.Mua * _delta)).ToArray();
                    };
                case ForwardAnalysisType.dRdMusp:
                    return parameters =>
                    {
                        var less = parameters;
                        var more = parameters;
                        var op = (OpticalProperties[])parameters;
                        less[0] = op.Select(opi => new OpticalProperties(opi.Mua, opi.Musp * (1 - _delta), opi.G, opi.N)).ToArray();
                        more[0] = op.Select(opi => new OpticalProperties(opi.Mua, opi.Musp * (1 + _delta), opi.G, opi.N)).ToArray();
                        return Vts.Extensions.EnumerableExtensions.Zip(
                            myFunc(more),
                            myFunc(less),
                            op,
                            (lessi, morei, opi) => (lessi - morei) / (2 * opi.Musp * _delta)).ToArray();
                    };
                case ForwardAnalysisType.dRdG:
                    return parameters =>
                    {
                        var less = parameters;
                        var more = parameters;
                        var op = (OpticalProperties[])parameters;
                        less[0] = op.Select(opi => new OpticalProperties(opi.Mua, opi.Musp, opi.G * (1 - _delta), opi.N)).ToArray();
                        more[0] = op.Select(opi => new OpticalProperties(opi.Mua, opi.Musp, opi.G * (1 + _delta), opi.N)).ToArray();
                        return Vts.Extensions.EnumerableExtensions.Zip(
                            myFunc(more),
                            myFunc(less),
                            op,
                            (lessi, morei, opi) => (lessi - morei) / (2 * opi.G * _delta)).ToArray();
                    };
                case ForwardAnalysisType.dRdN:
                    return parameters =>
                    {
                        var less = parameters;
                        var more = parameters;
                        var op = (OpticalProperties[])parameters;
                        less[0] = op.Select(opi => new OpticalProperties(opi.Mua, opi.Musp, opi.G, opi.N * (1 - _delta))).ToArray();
                        more[0] = op.Select(opi => new OpticalProperties(opi.Mua, opi.Musp, opi.G, opi.N * (1 + _delta))).ToArray();
                        return Vts.Extensions.EnumerableExtensions.Zip(
                            myFunc(more),
                            myFunc(less),
                            op,
                            (lessi, morei, opi) => (lessi - morei) / (2 * opi.N * _delta)).ToArray();
                    };
                default:
                    throw new ArgumentOutOfRangeException("analysisType");
            }
        }
    }
}
