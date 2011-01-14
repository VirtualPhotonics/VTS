using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Extensions;

namespace Vts.Modeling
{
    public static class NumericalDerivativeExtensions
    {
        private static double _delta = 0.01;

        public static void SetDelta(double delta)
        {
            _delta = delta;
        }

        //public static double EstimateDerivative(
        //    this FuncWithParams<OpticalProperties, double, double> myFunc,
        //    ForwardAnalysisType analysisType,
        //    int ia,
        //    OpticalProperties op,
        //    params double[] a)
        //{
        //    switch (analysisType)
        //    {
        //        case ForwardAnalysisType.dRdMua:
        //            double delta_mua = _delta*op.Mua;
        //            return (
        //                       myFunc(new OpticalProperties(op) {Mua = op.Mua + delta_mua}, a) -
        //                       myFunc(new OpticalProperties(op) {Mua = op.Mua - delta_mua}, a))/(2*delta_mua);
        //        case ForwardAnalysisType.dRdMusp:
        //            double delta_musp = _delta*op.Musp;
        //            return (
        //                       myFunc(new OpticalProperties(op) {Musp = op.Musp + delta_musp}, a) -
        //                       myFunc(new OpticalProperties(op) {Musp = op.Musp - delta_musp}, a))/(2*delta_musp);
        //        case ForwardAnalysisType.dRdG:
        //            double delta_g = _delta*op.G;
        //            return (
        //                       myFunc(new OpticalProperties(op) {G = op.G + delta_g}, a) -
        //                       myFunc(new OpticalProperties(op) {G = op.G - delta_g}, a))/(2*delta_g);
        //        case ForwardAnalysisType.dRdN:
        //            double delta_n = _delta*op.N;
        //            return (
        //                       myFunc(new OpticalProperties(op) {N = op.N + delta_n}, a) -
        //                       myFunc(new OpticalProperties(op) {N = op.N - delta_n}, a))/(2*delta_n);
        //        case ForwardAnalysisType.dRdIV:
        //        default:
        //            double delta_a = _delta*a[ia];

        //            double[] aPlusDelta = new double[a.Length].InitializeTo(a);
        //            double[] aMinusDelta = new double[a.Length].InitializeTo(a);

        //            aPlusDelta[ia] += delta_a;
        //            aMinusDelta[ia] -= delta_a;

        //            return (myFunc(op, aPlusDelta) - myFunc(op, aMinusDelta))/(2*delta_a);
        //    }
        //}

        //public static IEnumerable<double> EstimateDerivative(
        //    this FuncWithParams<IEnumerable<OpticalProperties>, IEnumerable<double>, IEnumerable<double>> myFunc,
        //    ForwardAnalysisType analysisType,
        //    int ia,
        //    OpticalProperties op,
        //    params IEnumerable<double>[] a)
        //{
        //    switch (analysisType)
        //    {
        //        case ForwardAnalysisType.dRdMua:
        //            double delta_mua = _delta * op.Mua;
        //            return EnumerableExtensions.Zip(
        //                myFunc(new OpticalProperties(op) { Mua = op.Mua + delta_mua }.AsEnumerable(), a).ToArray(), // call the vectorized version of the forward model
        //                myFunc(new OpticalProperties(op) { Mua = op.Mua - delta_mua }.AsEnumerable(), a).ToArray(), // for now, doesn't utilized OP vectorization
        //                (left, right) => (left - right) / (2 * delta_mua));
        //        case ForwardAnalysisType.dRdMusp:
        //            double delta_musp = _delta * op.Musp;
        //            return EnumerableExtensions.Zip(
        //                myFunc(new OpticalProperties(op) { Musp = op.Musp + delta_musp }.AsEnumerable(), a).ToArray(),
        //                myFunc(new OpticalProperties(op) { Musp = op.Musp - delta_musp }.AsEnumerable(), a).ToArray(),
        //                (left, right) => (left - right) / (2 * delta_musp));
        //        case ForwardAnalysisType.dRdG:
        //            double delta_g = _delta * op.G;
        //            return EnumerableExtensions.Zip(
        //                myFunc(new OpticalProperties(op) { G = op.G + delta_g }.AsEnumerable(), a).ToArray(),
        //                myFunc(new OpticalProperties(op) { G = op.G - delta_g }.AsEnumerable(), a).ToArray(),
        //                (left, right) => (left - right) / (2 * delta_g));
        //        case ForwardAnalysisType.dRdN:
        //            double delta_n = _delta * op.N;
        //            return EnumerableExtensions.Zip(
        //                myFunc(new OpticalProperties(op) { N = op.N + delta_n }.AsEnumerable(), a).ToArray(),
        //                myFunc(new OpticalProperties(op) { N = op.N - delta_n }.AsEnumerable(), a).ToArray(),
        //                (left, right) => (left - right) / (2 * delta_n));
        //        case ForwardAnalysisType.dRdIV:
        //        default:
        //            var delta_a = a[ia].Select(x => x * _delta).ToArray();

        //            var aPlusDelta = a.Select(ai => ai.ToArray()).ToArray(); // clone 'a' first...
        //            aPlusDelta[ia] = aPlusDelta[ia].Zip(delta_a, (a_i, delta_a_i) => a_i + delta_a_i).ToArray(); // then re-assign the section with a delta change

        //            var aMinusDelta = a.Select(ai => ai.ToArray()).ToArray(); // clone 'a' first...
        //            aMinusDelta[ia] = aMinusDelta[ia].Zip(delta_a, (a_i, delta_a_i) => a_i - delta_a_i).ToArray(); // then re-assign the section with a delta change

        //            return EnumerableExtensions.Zip(
        //                myFunc(op.AsEnumerable(), aPlusDelta).ToArray(),
        //                myFunc(op.AsEnumerable(), aMinusDelta).ToArray(),
        //                delta_a,
        //                (first, second, third) => (first - second) / (2 * third));
        //    }
        //}

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
