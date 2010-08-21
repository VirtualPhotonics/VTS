using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Extensions;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers.Extensions;
using MathNet.Numerics;

namespace Vts.Factories
{
    /// <summary>
    /// Class that composes forward and optimization calculations based on high-level inputs
    /// </summary>
    public static class ComputationFactory
    {
        // todo: the following two methods are a result of a leaky abstraction 
        // if we did our job of abstracting the computaiton, external users wouldn't have to worry about this
        public static bool IsSolverWithConstantValues(SolutionDomainType solutionDomainType)
        {
            return
                !(solutionDomainType == SolutionDomainType.RofRho) &&
                !(solutionDomainType == SolutionDomainType.RofFx);
        }

        public static bool IsSolverWithConstantValues(FluenceSolutionDomainType solutionDomainType)
        {
            return
                !(solutionDomainType == FluenceSolutionDomainType.FluenceofRho) &&
                !(solutionDomainType == FluenceSolutionDomainType.FluenceofFx);
        }

        public static bool IsComplexSolver(SolutionDomainType solutionDomainType)
        {
            return
                (solutionDomainType == SolutionDomainType.RofRhoAndFt) ||
                (solutionDomainType == SolutionDomainType.RofFxAndFt);
        }

        private static double[] FlattenRealAndImaginary(this IEnumerable<Complex> values)
        {            
            var real = values.Select(v => v.Real);
            var imag = values.Select(v => -v.Imaginary);
            return real.Concat(imag).ToArray();
            //// would have written this with Linq operators, but wasn't sure what was most efficient
            //var tempSize = temp.Length;
            //var flattened = new double[temp.Length*2];
            //for (int i = 0; i < temp.Length; i += 2)
            //{
            //    flattened[i] = temp[i].Real;
            //    flattened[i + tempSize] = temp[i + 1].Imaginary;
            //}
            //return flattened;
        }

        public static IEnumerable<double> GetVectorizedIndependentVariableQueryNew(
            ForwardSolverType forwardSolverType,
            SolutionDomainType solutionDomainType,
            ForwardAnalysisType forwardAnalysisType,
            IndependentVariableAxis independentAxisType,
            IEnumerable<double> independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            return GetVectorizedIndependentVariableQueryNew(
                SolverFactory.GetForwardSolver(forwardSolverType), 
                solutionDomainType,
                forwardAnalysisType,
                independentAxisType,
                independentValues,
                opticalProperties,
                constantValues);
        }

        public static IEnumerable<double> GetVectorizedIndependentVariableQueryNew(
            IForwardSolver forwardSolver,
            SolutionDomainType solutionDomainType,
            ForwardAnalysisType forwardAnalysisType,
            IndependentVariableAxis independentAxisType,
            IEnumerable<double> independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            var parameters = new double[4] { opticalProperties.Mua, opticalProperties.Musp, opticalProperties.G, opticalProperties.N };

            Func<double[], object[], double[]> func = GetForwardReflectanceFunc(forwardSolver, solutionDomainType, independentAxisType);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            List<object> inputValues = new List<object>();
            inputValues.Add(independentValues.ToArray());
            constantValues.ForEach(cv => inputValues.Add(cv));

            if (forwardAnalysisType == ForwardAnalysisType.R)
            {
                return func(parameters, inputValues.ToArray());
            }
            else
            {
                return func.GetDerivativeFunc(forwardAnalysisType)(parameters, inputValues.ToArray());
            }
        }

        public static IEnumerable<double> GetVectorizedMultidimensionalIndependentVariableQueryNew(
            ForwardSolverType forwardSolverType,
            FluenceSolutionDomainType solutionDomainType, // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            IEnumerable<double>[] independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            return GetVectorizedMultidimensionalIndependentVariableQueryNew(
                SolverFactory.GetForwardSolver(forwardSolverType),
                solutionDomainType,
                independentAxesTypes,
                independentValues,
                opticalProperties,
                constantValues);
        }

        public static IEnumerable<double> GetVectorizedMultidimensionalIndependentVariableQueryNew(
            IForwardSolver forwardSolver,
            FluenceSolutionDomainType solutionDomainType, // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            IEnumerable<double>[] independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            var parameters = new double[4] { opticalProperties.Mua, opticalProperties.Musp, opticalProperties.G, opticalProperties.N };
            
            // todo: current assumption below is that the second axis is z. need to generalize
            var func = GetForwardFluenceFunc(forwardSolver, solutionDomainType, independentAxesTypes[0]);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            List<object> inputValues = new List<object>();
            independentValues.ForEach(iv => inputValues.Add(iv.ToArray()));
            constantValues.ForEach(cv => inputValues.Add(cv));

            return func(parameters, inputValues.ToArray());
        }

        public static IEnumerable<double> GetPHD(IForwardSolver forwardSolver, IEnumerable<double> fluence, double sdSeparation, IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            var rhoPrimes =
                from r in rhos
                select r - sdSeparation;

            var greensFunction = forwardSolver.SteadyStateFluence2SurfacePointPHD(ops, rhoPrimes, zs);

            return fluence.Zip(greensFunction, (flu, green) => flu * green);
        }

        public static IEnumerable<double> GetAbsorbedEnergy(IEnumerable<double> fluence, double mua)
        {
            return fluence.Select(flu => flu * mua);
        }

        public static double[] ConstructAndExecuteVectorizedOptimizer(
            ForwardSolverType forwardSolverType,
            OptimizerType optimizerType,
            SolutionDomainType solutionDomainType,
            IndependentVariableAxis independentAxisType,
            IEnumerable<double> independentValues,
            IEnumerable<double> dependentValues,
            IEnumerable<double> standardDeviationValues,
            OpticalProperties opticalPropertyGuess,
            InverseFitType inverseFitType,
            params double[] constantValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            return ConstructAndExecuteVectorizedOptimizer(
                SolverFactory.GetForwardSolver(forwardSolverType), 
                SolverFactory.GetOptimizer(optimizerType),
                solutionDomainType,
                independentAxisType,
                independentValues,
                dependentValues,
                standardDeviationValues,
                opticalPropertyGuess,
                inverseFitType,
                constantValues);
        }

        public static double[] ConstructAndExecuteVectorizedOptimizer(
            IForwardSolver forwardSolver,
            IOptimizer optimizer,
            SolutionDomainType solutionDomainType,
            IndependentVariableAxis independentAxisType,
            IEnumerable<double> independentValues,
            IEnumerable<double> dependentValues,
            IEnumerable<double> standardDeviationValues,
            OpticalProperties opticalPropertyGuess,
            InverseFitType inverseFitType,
            params double[] constantValues)
        {
            // todo: make some kind of parameter selector that filters on the object array to create an initial guess. e.g. Func<object[], double[]>
            var parameters = new double[4] { opticalPropertyGuess.Mua, opticalPropertyGuess.Musp, opticalPropertyGuess.G, opticalPropertyGuess.N };

            var parametersToFit = GetParametersToFit(inverseFitType);

            Func<double[], object[], double[]> func = GetForwardReflectanceFunc(forwardSolver, solutionDomainType, independentAxisType);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            //var inputValues = IsComplexSolver(solutionDomainType) ?
            //    (independentValues.Concat(independentValues)).Select(iv => (object) iv).ToList() :
            //    independentValues.Select(iv => (object) iv).ToList();

            List<object> inputValues = new List<object>();
            inputValues.Add(independentValues);
            constantValues.ForEach(cv => inputValues.Add(cv));

            var fit = optimizer.Solve(parameters, parametersToFit, dependentValues.ToArray(), standardDeviationValues.ToArray(), func, inputValues.ToArray());

            return fit;
        }

        private static Func<double[], object[], double[]> GetForwardReflectanceFunc(
           IForwardSolver fs, SolutionDomainType type, IndependentVariableAxis axis)
        {
            Func<double[], OpticalProperties> getOP = op => new OpticalProperties(op[0], op[1], op[2], op[3]);

            // note: the following uses the convention that the independent variable(s) is (are) first in the forward data object array
            // note: secondly, if there are multiple independent axes, they will be assigned in order of appearance in the method signature
            switch (type)
            {
                case SolutionDomainType.RofRho:
                    return (fitData, otherData) => fs.RofRho(getOP(fitData).AsEnumerable(), (double[])otherData[0]).ToArray();
                case SolutionDomainType.RofFx:
                    return (fitData, otherData) => fs.RofFx(getOP(fitData).AsEnumerable(), (double[])otherData[0]).ToArray();
                case SolutionDomainType.RofRhoAndT:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.RofRhoAndT(getOP(fitData).AsEnumerable(), (double[])otherData[0], ((double)otherData[1]).AsEnumerable()).ToArray();
                        case IndependentVariableAxis.T:
                            return (fitData, otherData) => fs.RofRhoAndT(getOP(fitData).AsEnumerable(), ((double)otherData[1]).AsEnumerable(), (double[])otherData[0]).ToArray();
                        //case IndependentVariableAxis.Wavelength:
                        //    return (chromPlusMusp, constantData) =>
                        //               {
                        //                   var wv = (double[]) constantData[0];
                        //                   var tissue = (Tissue) constantData[1];
                        //                   int i = 0;
                        //                   tissue.Absorbers.ForEach(abs => abs.Concentration = chromPlusMusp[i++]);
                        //                   tissue.Scatterer = new PowerLawScatterer(chromPlusMusp[i], chromPlusMusp[i + 1]);
                        //                   var muas = wv.Select(w => tissue.GetMua(w)); 
                        //                   var musps = wv.Select(w => tissue.GetMusp(w));
                        //                   return EnumerableExtensions.Zip(muas,musps,(mua,musp)=>fs.RofRhoAndT())...
                        //               }; 
                        //    return op => fs.RofRhoAndT(op, ((double)constantValues[0]).AsEnumerable(), ((double)constantValues[1]).AsEnumerable());
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case SolutionDomainType.RofFxAndT:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.RofFxAndT(getOP(fitData).AsEnumerable(), (double[])otherData[0], ((double)otherData[1]).AsEnumerable()).ToArray();
                        case IndependentVariableAxis.T:
                            return (fitData, otherData) => fs.RofFxAndT(getOP(fitData).AsEnumerable(), ((double)otherData[1]).AsEnumerable(), (double[])otherData[0]).ToArray();
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case SolutionDomainType.RofRhoAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.RofRhoAndFt(getOP(fitData).AsEnumerable(), (double[])otherData[0], ((double)otherData[1]).AsEnumerable()).FlattenRealAndImaginary();
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.RofRhoAndFt(getOP(fitData).AsEnumerable(), ((double)otherData[1]).AsEnumerable(), (double[])otherData[0]).FlattenRealAndImaginary();
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case SolutionDomainType.RofFxAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.RofFxAndFt(getOP(fitData).AsEnumerable(), (double[])otherData[0], ((double)otherData[1]).AsEnumerable()).FlattenRealAndImaginary();
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.RofFxAndFt(getOP(fitData).AsEnumerable(), ((double)otherData[1]).AsEnumerable(), (double[])otherData[0]).FlattenRealAndImaginary();
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private static Func<double[], object[], double[]> GetForwardFluenceFunc(
           IForwardSolver fs, FluenceSolutionDomainType type, IndependentVariableAxis axis)
        {
            Func<double[], OpticalProperties> getOP = op => new OpticalProperties(op[0], op[1], op[2], op[3]);

            // note: the following uses the convention that the independent variable(s) is (are) first in the forward data object array
            // note: secondly, if there are multiple independent axes, they will be assigned in order of appearance in the method signature
            switch (type)
            {
                case FluenceSolutionDomainType.FluenceofRho:
                    return (fitData, otherData) => fs.FluenceofRho(getOP(fitData).AsEnumerable(), (double[])otherData[0], (double[])otherData[1]).ToArray();
                case FluenceSolutionDomainType.FluenceofFx:
                    return (fitData, otherData) => fs.FluenceofFx(getOP(fitData).AsEnumerable(), (double[])otherData[0], (double[])otherData[1]).ToArray();
                case FluenceSolutionDomainType.FluenceofRhoAndT:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.FluenceofRhoAndT(getOP(fitData).AsEnumerable(), (double[])otherData[0],  (double[])otherData[1], ((double)otherData[2]).AsEnumerable()).ToArray();
                        case IndependentVariableAxis.T:
                            return (fitData, otherData) => fs.FluenceofRhoAndT(getOP(fitData).AsEnumerable(), ((double)otherData[2]).AsEnumerable(), (double[])otherData[1], (double[])otherData[0]).ToArray();
                        //case IndependentVariableAxis.Wavelength:
                        //    return (chromPlusMusp, constantData) =>
                        //               {
                        //                   var wv = (double[]) constantData[0];
                        //                   var tissue = (Tissue) constantData[1];
                        //                   int i = 0;
                        //                   tissue.Absorbers.ForEach(abs => abs.Concentration = chromPlusMusp[i++]);
                        //                   tissue.Scatterer = new PowerLawScatterer(chromPlusMusp[i], chromPlusMusp[i + 1]);
                        //                   var muas = wv.Select(w => tissue.GetMua(w)); 
                        //                   var musps = wv.Select(w => tissue.GetMusp(w));
                        //                   return EnumerableExtensions.Zip(muas,musps,(mua,musp)=>fs.RofRhoAndT())...
                        //               }; 
                        //    return op => fs.RofRhoAndT(op, ((double)constantValues[0]).AsEnumerable(), ((double)constantValues[1]).AsEnumerable());
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case FluenceSolutionDomainType.FluenceofFxAndT:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.FluenceofFxAndT(getOP(fitData).AsEnumerable(), (double[])otherData[0], (double[])otherData[1], ((double)otherData[2]).AsEnumerable()).ToArray();
                        case IndependentVariableAxis.T:
                            return (fitData, otherData) => fs.FluenceofFxAndT(getOP(fitData).AsEnumerable(), ((double)otherData[2]).AsEnumerable(), (double[])otherData[1], (double[])otherData[0]).ToArray();
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case FluenceSolutionDomainType.FluenceofRhoAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.FluenceofRhoAndFt(getOP(fitData).AsEnumerable(), (double[])otherData[0], (double[])otherData[1], ((double)otherData[2]).AsEnumerable()).ToArray();
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.FluenceofRhoAndFt(getOP(fitData).AsEnumerable(), ((double)otherData[2]).AsEnumerable(), (double[])otherData[1], (double[])otherData[0]).ToArray();
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case FluenceSolutionDomainType.FluenceofFxAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.FluenceofFxAndFt(getOP(fitData).AsEnumerable(), (double[])otherData[0], (double[])otherData[1], ((double)otherData[2]).AsEnumerable()).ToArray();
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.FluenceofFxAndFt(getOP(fitData).AsEnumerable(), ((double)otherData[2]).AsEnumerable(), (double[])otherData[1], (double[])otherData[0]).ToArray();
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private static bool[] GetParametersToFit(InverseFitType fitType)
        {
            switch (fitType)
            {
                case InverseFitType.MuaMusp:
                default:
                    return new bool[] { true, true, false, false };
                case InverseFitType.Mua:
                    return new bool[] { true, false, false, false };
                case InverseFitType.Musp:
                    return new bool[] { false, true, false, false };
                case InverseFitType.MuaMuspG:
                    return new bool[] { true, true, true, false };
            }
        }
    }
}