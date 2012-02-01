using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Extensions;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers.Extensions;

#if DESKTOP
using System.Runtime.InteropServices;
#endif

namespace Vts.Factories
{
#if DESKTOP
    [ComVisible(true)]
#endif
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
                !(solutionDomainType == SolutionDomainType.ROfRho) &&
                !(solutionDomainType == SolutionDomainType.ROfFx);
        }

        public static bool IsSolverWithConstantValues(FluenceSolutionDomainType solutionDomainType)
        {
            return
                !(solutionDomainType == FluenceSolutionDomainType.FluenceOfRho) &&
                !(solutionDomainType == FluenceSolutionDomainType.FluenceOfFx);
        }

        public static bool IsComplexSolver(SolutionDomainType solutionDomainType)
        {
            return
                (solutionDomainType == SolutionDomainType.ROfRhoAndFt) ||
                (solutionDomainType == SolutionDomainType.ROfFxAndFt);
        }

        private static double[] FlattenRealAndImaginary(this Complex[] values)
        {
            var flattened = new double[values.Length * 2];
            var nValues = values.Length;
            for (int i = 0; i < nValues; i++)
            {
                flattened[i] = values[i].Real;
                flattened[i + nValues] = values[i].Imaginary;
            }
            return flattened;
        }

#if DESKTOP
    [ComVisible(true)] 
#endif
        /// <summary>
        /// String-overloaded version of factory method for forward solver computation
        /// </summary>
        /// <param name="forwardSolverType"></param>
        /// <param name="solutionDomainType"></param>
        /// <param name="forwardAnalysisType"></param>
        /// <param name="independentVariableAxisType"></param>
        /// <param name="independentValues"></param>
        /// <param name="opticalProperties"></param>
        /// <param name="constantValues"></param>
        /// <returns></returns>
        public static double[] GetVectorizedIndependentVariableQueryNew(
             string forwardSolverType,
             string solutionDomainType,
             string forwardAnalysisType,
             string independentVariableAxisType,
             double[] independentValues,
             double[] opticalProperties,
             double[] constantValues)
        {
            return GetVectorizedIndependentVariableQueryNew(
                (ForwardSolverType)Enum.Parse(typeof(ForwardSolverType), forwardSolverType, true),
                (SolutionDomainType)Enum.Parse(typeof(SolutionDomainType), solutionDomainType, true),
                (ForwardAnalysisType)Enum.Parse(typeof(ForwardAnalysisType), forwardAnalysisType, true),
                (IndependentVariableAxis)Enum.Parse(typeof(IndependentVariableAxis), independentVariableAxisType, true),
                independentValues,
                new OpticalProperties(opticalProperties[0], opticalProperties[1], opticalProperties[2], opticalProperties[3]),
                constantValues).ToArray();
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
            double[][] independentValues,
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
            double[][] independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            var parameters = new double[4] { opticalProperties.Mua, opticalProperties.Musp, opticalProperties.G, opticalProperties.N };
            
            // todo: current assumption below is that the second axis is z. need to generalize
            var func = GetForwardFluenceFunc(forwardSolver, solutionDomainType, independentAxesTypes[0]);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            List<object> inputValues = new List<object>(independentValues);
            constantValues.ForEach(cv => inputValues.Add(cv));

            return func(parameters, inputValues.ToArray());
        }

        /// <summary>
        /// Overload of GetPHD that uses internal DI framework-supplied solver singletons
        /// </summary>
        /// <param name="forwardSolverType"></param>
        /// <param name="fluence"></param>
        /// <param name="sdSeparation"></param>
        /// <param name="ops"></param>
        /// <param name="rhos"></param>
        /// <param name="zs"></param>
        /// <returns></returns>
        public static IEnumerable<double> GetPHD(ForwardSolverType forwardSolverType, IEnumerable<double> fluence, double sdSeparation, IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            return GetPHD(SolverFactory.GetForwardSolver(forwardSolverType), fluence, sdSeparation, ops, rhos, zs);
        }

        public static IEnumerable<double> GetPHD(IForwardSolver forwardSolver, IEnumerable<double> fluence, double sdSeparation, IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            var rhoPrimes =
                from r in rhos
                select r - sdSeparation;

            var greensFunction = forwardSolver.SteadyStateFluence2SurfacePointPHD(ops, rhoPrimes, zs);

            return System.Linq.Enumerable.Zip(fluence, greensFunction, (flu, green) => flu * green);
        }
        /// <summary>
        /// Method to generate absorbed energy given fluence and mua.  Note only works for homogeneous tissue.
        /// </summary>
        /// <param name="fluence">fluence serialized to a 1D IEnumerable of double</param>
        /// <param name="mua">absorption coefficient for entire tissue</param>
        /// <returns>absorbed energy in a 1D IEnumerable of double</returns>
        public static IEnumerable<double> GetAbsorbedEnergy(IEnumerable<double> fluence, double mua)
        {
            return fluence.Select(flu => flu * mua);
        }

        public static double[] ConstructAndExecuteVectorizedOptimizer(
            ForwardSolverType forwardSolverType,
            OptimizerType optimizerType,
            SolutionDomainType solutionDomainType,
            IndependentVariableAxis independentAxisType,
            double[] independentValues,
            double[] dependentValues,
            double[] standardDeviationValues,
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
            double[] independentValues,
            double[] dependentValues,
            double[] standardDeviationValues,
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
                case SolutionDomainType.ROfRho:
                    return (fitData, otherData) => fs.ROfRho(getOP(fitData), (double[])otherData[0]);
                case SolutionDomainType.ROfFx:
                    return (fitData, otherData) => fs.ROfFx(getOP(fitData), (double[])otherData[0]);
                case SolutionDomainType.ROfRhoAndT:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.ROfRhoAndT(getOP(fitData), (double[])otherData[0], (double)otherData[1]);
                        case IndependentVariableAxis.T:
                            return (fitData, otherData) => fs.ROfRhoAndT(getOP(fitData), (double)otherData[1], (double[])otherData[0]);
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
                        //                   return EnumerableExtensions.Zip(muas,musps,(mua,musp)=>fs.ROfRhoAndT())...
                        //               }; 
                        //    return op => fs.ROfRhoAndT(op, ((double)constantValues[0]).AsEnumerable(), ((double)constantValues[1]).AsEnumerable());
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case SolutionDomainType.ROfFxAndT:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.ROfFxAndT(getOP(fitData), (double[])otherData[0], (double)otherData[1]);
                        case IndependentVariableAxis.T:
                            return (fitData, otherData) => fs.ROfFxAndT(getOP(fitData), (double)otherData[1], (double[])otherData[0]);
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case SolutionDomainType.ROfRhoAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.ROfRhoAndFt(getOP(fitData), (double[])otherData[0], (double)otherData[1]).FlattenRealAndImaginary();
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.ROfRhoAndFt(getOP(fitData), (double)otherData[1], (double[])otherData[0]).FlattenRealAndImaginary();
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case SolutionDomainType.ROfFxAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.ROfFxAndFt(getOP(fitData), (double[])otherData[0], (double)otherData[1]).FlattenRealAndImaginary();
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.ROfFxAndFt(getOP(fitData), (double)otherData[1], (double[])otherData[0]).FlattenRealAndImaginary();
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        // todo: array overloads for fluence forward solvers too
        private static Func<double[], object[], double[]> GetForwardFluenceFunc(
           IForwardSolver fs, FluenceSolutionDomainType type, IndependentVariableAxis axis)
        {
            Func<double[], OpticalProperties> getOP = op => new OpticalProperties(op[0], op[1], op[2], op[3]);

            // note: the following uses the convention that the independent variable(s) is (are) first in the forward data object array
            // note: secondly, if there are multiple independent axes, they will be assigned in order of appearance in the method signature
            switch (type)
            {
                case FluenceSolutionDomainType.FluenceOfRho:
                    return (fitData, otherData) => fs.FluenceOfRho(new[]{ getOP(fitData) }, (double[])otherData[0], (double[])otherData[1]);
                case FluenceSolutionDomainType.FluenceOfFx:
                    return (fitData, otherData) => fs.FluenceOfFx(new[]{ getOP(fitData) }, (double[])otherData[0], (double[])otherData[1]);
                case FluenceSolutionDomainType.FluenceOfRhoAndT:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.FluenceOfRhoAndT(new[]{getOP(fitData)}, (double[])otherData[0], (double[])otherData[1], new[]{(double)otherData[2]});
                        case IndependentVariableAxis.T:
                            return (fitData, otherData) => fs.FluenceOfRhoAndT(new[]{getOP(fitData)}, new[]{(double)otherData[2]}, (double[])otherData[1], (double[])otherData[0]);
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
                        //                   return EnumerableExtensions.Zip(muas,musps,(mua,musp)=>fs.ROfRhoAndT())...
                        //               }; 
                        //    return op => fs.ROfRhoAndT(op, ((double)constantValues[0]).AsEnumerable(), ((double)constantValues[1]).AsEnumerable());
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case FluenceSolutionDomainType.FluenceOfFxAndT:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.FluenceOfFxAndT(new[]{getOP(fitData)}, (double[])otherData[0], (double[])otherData[1], new[]{(double)otherData[2]});
                        case IndependentVariableAxis.T:
                            return (fitData, otherData) => fs.FluenceOfFxAndT(new[]{getOP(fitData)}, new[]{(double)otherData[2]}, (double[])otherData[1], (double[])otherData[0]);
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case FluenceSolutionDomainType.FluenceOfRhoAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.FluenceOfRhoAndFt(new[]{getOP(fitData)}, (double[])otherData[0], (double[])otherData[1], new[]{(double)otherData[2]});
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.FluenceOfRhoAndFt(new[]{getOP(fitData)}, new[]{(double)otherData[2]}, (double[])otherData[1], (double[])otherData[0]);
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case FluenceSolutionDomainType.FluenceOfFxAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.FluenceOfFxAndFt(new[]{getOP(fitData)}, (double[])otherData[0], (double[])otherData[1], new[]{(double)otherData[2]});
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.FluenceOfFxAndFt(new[]{getOP(fitData)}, new[]{(double)otherData[2]}, (double[])otherData[1], (double[])otherData[0]);
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