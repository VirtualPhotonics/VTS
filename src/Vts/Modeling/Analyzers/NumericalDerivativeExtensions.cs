using System;
using System.Linq;
using Vts.Extensions;

namespace Vts.Modeling
{
    /// <summary>
    /// extension methods to determine derivatives of forward solvers
    /// </summary>
    public static class NumericalDerivativeExtensions
    {
        private static double _delta = 0.01;

        /// <summary>
        /// method to set the delta value used in the finite difference determination of the derivative
        /// </summary>
        /// <param name="delta">delta value to be set</param>
        public static void SetDelta(double delta)
        {
            _delta = delta;
        }
        /// <summary>
        /// method to get derivative function
        /// </summary>
        /// <param name="myFunc">function for derivative</param>
        /// <param name="analysisType">ForwardAnalysisType enum</param>
        /// <returns>derivative function</returns>
        public static Func<object[], double[]> GetDerivativeFunc(
           this Func<object[], double[]> myFunc, ForwardAnalysisType analysisType)
        {
            switch (analysisType)
            {
                case ForwardAnalysisType.dRdMua:
                    return parameters =>
                    {
                        var less = new object[parameters.Length];
                        var more = new object[parameters.Length];
                        for (var i = 1; i < parameters.Length; i++)
                        {
                            //loop through objects 2 to the end and set the data equal to parameters
                            less[i] = (double[])parameters[i];
                            more[i] = (double[])parameters[i];
                        }
                        var ops = (OpticalProperties[])parameters[0];
                        less[0] = ops.Select(opi => new OpticalProperties(opi.Mua * (1 - _delta), opi.Musp, opi.G, opi.N)).ToArray();
                        more[0] = ops.Select(opi => new OpticalProperties(opi.Mua * (1 + _delta), opi.Musp, opi.G, opi.N)).ToArray();
                        var lessValues = myFunc(less);
                        var moreValues = myFunc(more);
                        var opList = new OpticalProperties[lessValues.Length];
                        var counter = 0;
                        foreach (var o in ops)
                        {
                            for (var j = 0; j < lessValues.Length / ops.Length; j++)
                            {
                                //Add the values to the array
                                opList[counter] = o;
                                counter ++;
                            }
                        }
                        return EnumerableExtensions.Zip(
                        lessValues.ToEnumerable<double>(),
                        moreValues.ToEnumerable<double>(),
                        opList,
                        (lessi, morei, opi) => (morei - lessi) / (2 * opi.Mua * _delta)).ToArray();
                    };
                case ForwardAnalysisType.dRdMusp:
                    return parameters =>
                    {
                        var less = new object[parameters.Length];
                        var more = new object[parameters.Length];
                        for (var i = 1; i < parameters.Length; i++)
                        {
                            //loop through objects 2 to the end and set the data equal to parameters
                            less[i] = (double[])parameters[i];
                            more[i] = (double[])parameters[i];
                        }
                        var ops = (OpticalProperties[])parameters[0];
                        less[0] = ops.Select(opi => new OpticalProperties(opi.Mua, opi.Musp * (1 - _delta), opi.G, opi.N)).ToArray();
                        more[0] = ops.Select(opi => new OpticalProperties(opi.Mua, opi.Musp * (1 + _delta), opi.G, opi.N)).ToArray();
                        var lessValues = myFunc(less);
                        var moreValues = myFunc(more);
                        var opList = new OpticalProperties[lessValues.Length];
                        var counter = 0;
                        foreach (var o in ops)
                        {
                            for (var j = 0; j < lessValues.Length / ops.Length; j++)
                            {
                                //Add the values to the array
                                opList[counter] = o;
                                counter++;
                            }
                        }
                        return EnumerableExtensions.Zip(
                            lessValues.ToEnumerable<double>(),
                            moreValues.ToEnumerable<double>(),
                            opList,
                            (lessi, morei, opi) => (morei - lessi) / (2 * opi.Musp * _delta)).ToArray();
                    };
                case ForwardAnalysisType.dRdG:
                    return parameters =>
                    {
                        var less = new object[parameters.Length];
                        var more = new object[parameters.Length];
                        for (var i = 1; i < parameters.Length; i++)
                        {
                            //loop through objects 2 to the end and set the data equal to parameters
                            less[i] = (double[])parameters[i];
                            more[i] = (double[])parameters[i];
                        }
                        var ops = (OpticalProperties[])parameters[0];
                        less[0] = ops.Select(opi => new OpticalProperties(opi.Mua, opi.Musp, opi.G * (1 - _delta), opi.N)).ToArray();
                        more[0] = ops.Select(opi => new OpticalProperties(opi.Mua, opi.Musp, opi.G * (1 + _delta), opi.N)).ToArray();
                        var lessValues = myFunc(less);
                        var moreValues = myFunc(more);
                        var opList = new OpticalProperties[lessValues.Length];
                        var counter = 0;
                        foreach (var o in ops)
                        {
                            for (var j = 0; j < lessValues.Length / ops.Length; j++)
                            {
                                //Add the values to the array
                                opList[counter] = o;
                                counter++;
                            }
                        }
                        return EnumerableExtensions.Zip(
                            lessValues.ToEnumerable<double>(),
                            moreValues.ToEnumerable<double>(),
                            opList,
                            (lessi, morei, opi) => (morei - lessi) / (2 * opi.G * _delta)).ToArray();
                    };
                case ForwardAnalysisType.dRdN:
                    return parameters =>
                    {
                        var less = new object[parameters.Length];
                        var more = new object[parameters.Length];
                        for (var i = 1; i < parameters.Length; i++)
                        {
                            //loop through objects 2 to the end and set the data equal to parameters
                            less[i] = (double[])parameters[i];
                            more[i] = (double[])parameters[i];
                        }
                        var ops = (OpticalProperties[])parameters[0];
                        less[0] = ops.Select(opi => new OpticalProperties(opi.Mua, opi.Musp, opi.G, opi.N * (1 - _delta))).ToArray();
                        more[0] = ops.Select(opi => new OpticalProperties(opi.Mua, opi.Musp, opi.G, opi.N * (1 + _delta))).ToArray();
                        var lessValues = myFunc(less);
                        var moreValues = myFunc(more);
                        var opList = new OpticalProperties[lessValues.Length];
                        var counter = 0;
                        foreach (var o in ops)
                        {
                            for (var j = 0; j < lessValues.Length / ops.Length; j++)
                            {
                                //Add the values to the array
                                opList[counter] = o;
                                counter++;
                            }
                        }
                        return EnumerableExtensions.Zip(
                            lessValues.ToEnumerable<double>(),
                            moreValues.ToEnumerable<double>(),
                            opList,
                            (lessi, morei, opi) => (morei - lessi) / (2 * opi.N * _delta)).ToArray();
                    };
                default:
                    throw new ArgumentOutOfRangeException("analysisType");
            }
        }
    }
}
