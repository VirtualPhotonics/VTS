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
        
        public static Func<double[], object[], double[]> GetDerivativeFunc(
           this Func<double[], object[], double[]> myFunc, ForwardAnalysisType analysisType)
        {
            switch (analysisType)
            {
                case ForwardAnalysisType.dRdMua:
                default:
                    return (parameters, constantValues) =>
                    {
                        var delta_mua = _delta * parameters[0];
                        var less = parameters.ToArray();
                        var more = parameters.ToArray();
                        less[0] = parameters[0] - delta_mua;
                        more[0] = parameters[0] + delta_mua;

                        return Enumerable.Zip(
                            myFunc(more, constantValues),
                            myFunc(less, constantValues),
                            (left, right) => (left - right) / (2 * delta_mua)).ToArray();
                    };
                case ForwardAnalysisType.dRdMusp:
                    return (parameters, constantValues) =>
                    {
                        var delta_musp = _delta * _delta * parameters[1];
                        var less = parameters.ToArray();
                        var more = parameters.ToArray();
                        less[1] -= delta_musp;
                        more[1] += delta_musp;
                        return Enumerable.Zip(
                            myFunc(more, constantValues),
                            myFunc(less, constantValues),
                            (left, right) => (left - right) / (2 * delta_musp)).ToArray();
                    };
                case ForwardAnalysisType.dRdG:
                    return (parameters, constantValues) =>
                    {
                        var delta_G = _delta * parameters[2];
                        var less = parameters.ToArray();
                        var more = parameters.ToArray();
                        less[2] -= delta_G;
                        more[2] += delta_G;
                        return Enumerable.Zip(
                            myFunc(more, constantValues),
                            myFunc(less, constantValues),
                            (left, right) => (left - right) / (2 * delta_G)).ToArray();
                    };
                case ForwardAnalysisType.dRdN:
                    return (parameters, constantValues) =>
                    {
                        var delta_N = _delta * _delta * parameters[3];
                        var less = parameters.ToArray();
                        var more = parameters.ToArray();
                        less[3] -= delta_N;
                        more[3] += delta_N;
                        return Enumerable.Zip(
                            myFunc(more, constantValues),
                            myFunc(less, constantValues),
                            (left, right) => (left - right) / (2 * delta_N)).ToArray();
                    };
                //case ForwardAnalysisType.dRdIV:
                //default:
                //    var delta_a = a[ia].Select(x => x * _delta).ToArray();

                //    var aPlusDelta = a.Select(ai => ai.ToArray()).ToArray(); // clone 'a' first...
                //    aPlusDelta[ia] = aPlusDelta[ia].Zip(delta_a, (a_i, delta_a_i) => a_i + delta_a_i).ToArray(); // then re-assign the section with a delta change

                //    var aMinusDelta = a.Select(ai => ai.ToArray()).ToArray(); // clone 'a' first...
                //    aMinusDelta[ia] = aMinusDelta[ia].Zip(delta_a, (a_i, delta_a_i) => a_i - delta_a_i).ToArray(); // then re-assign the section with a delta change

                //    return EnumerableExtensions.Zip(
                //        myFunc(op.AsEnumerable(), aPlusDelta).ToArray(),
                //        myFunc(op.AsEnumerable(), aMinusDelta).ToArray(),
                //        delta_a,
                //        (first, second, third) => (first - second) / (2 * third));
            }
        }
    }
}
