using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using Vts.Common;
using Vts.Extensions;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.ForwardSolvers.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Helpers;
#if DESKTOP
#endif
using Vts.SpectralMapping;

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
                !(solutionDomainType == SolutionDomainType.ROfRho) &&
                !(solutionDomainType == SolutionDomainType.ROfFx);
        }

        public static bool IsSolverWithConstantValues(FluenceSolutionDomainType solutionDomainType)
        {
            return
                !(solutionDomainType == FluenceSolutionDomainType.FluenceOfRhoAndZ) &&
                !(solutionDomainType == FluenceSolutionDomainType.FluenceOfFxAndZ);
        }

        // CH proposed new extension method prior version is not refined enough, need to 
        // know independent axis variable to know whether solver is complex, e.g. ROfRhoAndFt
        // with independent axis varaible = rho is not complex
        //public static bool IsComplexSolver(IndependentVariableAxis independentVariableAxis)
        //{
        //    return (independentVariableAxis == IndependentVariableAxis.Ft);
        //}

        public static bool IsComplexSolver(SolutionDomainType solutionDomainType)
        {
            return
                (solutionDomainType == SolutionDomainType.ROfRhoAndFt) ||
                (solutionDomainType == SolutionDomainType.ROfFxAndFt);
        }

        public static bool IsComplexSolver(FluenceSolutionDomainType solutionDomainType)
        {
            return
                (solutionDomainType == FluenceSolutionDomainType.FluenceOfRhoAndZAndFt) ||
                (solutionDomainType == FluenceSolutionDomainType.FluenceOfFxAndZAndFt);
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

        public static double[] ComputeReflectance(
            ForwardSolverType forwardSolverType,
            SolutionDomainType solutionDomainType,
            ForwardAnalysisType forwardAnalysisType,
            object[] independentValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            return ComputeReflectance(
                SolverFactory.GetForwardSolver(forwardSolverType),
                solutionDomainType,
                forwardAnalysisType,
                independentValues);
        }
        
        public static double[] ComputeReflectance(
            IForwardSolver forwardSolver,
            SolutionDomainType solutionDomainType,
            ForwardAnalysisType forwardAnalysisType,
            object[] independentValues)
        {
            Func<object[], double[]> func = GetForwardReflectanceFunc(forwardSolver, solutionDomainType);
            //Func<SolutionDomainType, ForwardAnalysisType, IndependentVariableAxis[], double[]> getOptimizationParameters = (_,_,_) => 
            //    new[] { op.Mua, op.Musp, op.G, op.N }
            //double[] optimizationParameters = GetOptimizationParameters(forwardSolver, solutionDomainType, independentAxisTypes); // will need this for inverse solver

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above

            return forwardAnalysisType == ForwardAnalysisType.R
                ? func(independentValues)
                : func.GetDerivativeFunc(forwardAnalysisType)(independentValues);
        }
        
        public static double[] ComputeFluence(
            ForwardSolverType forwardSolverType,
            FluenceSolutionDomainType solutionDomainType, // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            OpticalProperties[] opticalProperties,
            params double[] constantValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            return ComputeFluence(
                SolverFactory.GetForwardSolver(forwardSolverType),
                solutionDomainType,
                independentAxesTypes,
                independentValues,
                opticalProperties,
                constantValues);
        }

        // overload that calls the above method with just one set of optical properties
        public static double[] ComputeFluence(
            ForwardSolverType forwardSolverType,
            FluenceSolutionDomainType solutionDomainType,
            // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            return ComputeFluence(
                forwardSolverType,
                solutionDomainType,
                independentAxesTypes,
                independentValues,
                new[] { opticalProperties },
                constantValues);
        }

        public static double[] ComputeFluence(
            ForwardSolverType forwardSolverType,
            FluenceSolutionDomainType solutionDomainType,
            // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            IOpticalPropertyRegion[] tissueRegions,
            params double[] constantValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            return ComputeFluence(
                SolverFactory.GetForwardSolver(forwardSolverType),
                solutionDomainType,
                independentAxesTypes,
                independentValues,
                tissueRegions,
                constantValues);
        }
        // overload for ITissueRegion forward solvers todo: merge with above?
        public static double[] ComputeFluence(
            IForwardSolver forwardSolver,
            FluenceSolutionDomainType solutionDomainType,
            // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            IOpticalPropertyRegion[] tissueRegions,
            params double[] constantValues)
        {
            var parameters = tissueRegions.SelectMany(region =>
            {
                double[] regionParameters = null;
                if (region is ILayerOpticalPropertyRegion)
                {
                    var layerRegion = (ILayerOpticalPropertyRegion)region;
                    regionParameters = new[]
                        {
                            layerRegion.RegionOP.Mua,
                            layerRegion.RegionOP.Musp,
                            layerRegion.RegionOP.G,
                            layerRegion.RegionOP.N,
                            layerRegion.ZRange.Delta
                        };
                }
                //else if(region is EllipsoidTissueRegion)
                //{
                //  
                //}
                else
                {
                    throw new Exception("Forward model " +
                                        forwardSolver.ToString() +
                                        " is not supported.");
                }
                return regionParameters;
            }).ToArray();

            // todo: current assumption below is that the second axis is z. need to generalize
            var func = GetForwardFluenceFunc(forwardSolver, solutionDomainType, independentAxesTypes[0]);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            List<object> inputValues = new List<object>(independentValues);
            constantValues.ForEach(cv => inputValues.Add(cv));

            return func(parameters, inputValues.ToArray());
        }

        public static double[] ComputeFluence(
            IForwardSolver forwardSolver,
            FluenceSolutionDomainType solutionDomainType, // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            OpticalProperties[] opticalProperties,
            params double[] constantValues)
        {
            // todo: current assumption below is that the second axes is z. need to generalize
            var func = GetForwardFluenceFunc(forwardSolver, solutionDomainType, independentAxesTypes[0]);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            List<object> inputValues = new List<object>(independentValues);
            constantValues.ForEach(cv => inputValues.Add(cv));

            if (opticalProperties.Length == 1) // optimization that skips duplicate arrays if we're not multiplexing over optical properties (e.g. wavelength)
            {
                var op = opticalProperties[0];
                var parameters = new[] { op.Mua, op.Musp, op.G, op.N };
                return func(parameters, inputValues.ToArray());
            }

            var numOp = opticalProperties.Length;
            var numIv = independentValues.Length;
            var fluence = new double[numOp * numIv];
            for (int opi = 0; opi < numOp; opi++) // todo: parallelize
            {
                var op = opticalProperties[opi];
                var parameters = new[] { op.Mua, op.Musp, op.G, op.N };
                var tempValues = func(parameters, inputValues.ToArray());

                for (int ivi = 0; ivi < numIv; ivi++)
                {
                    fluence[opi * numIv + ivi] = tempValues[ivi];
                }
            }
            return fluence;
        }

        // overload that calls the above method with just one set of optical properties
        public static double[] ComputeFluence(
            IForwardSolver forwardSolver,
            FluenceSolutionDomainType solutionDomainType,
            // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            return ComputeFluence(
                forwardSolver,
                solutionDomainType,
                independentAxesTypes,
                independentValues,
                new[] { opticalProperties },
                constantValues);
        }

        public static Complex[] ComputeFluenceComplex(
            ForwardSolverType forwardSolverType,
            FluenceSolutionDomainType solutionDomainType,
            // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            return ComputeFluenceComplex(
                SolverFactory.GetForwardSolver(forwardSolverType),
                solutionDomainType,
                independentAxesTypes,
                independentValues,
                opticalProperties,
                constantValues);
        }

        public static Complex[] ComputeFluenceComplex(
            ForwardSolverType forwardSolverType,
            FluenceSolutionDomainType solutionDomainType,
            // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            IOpticalPropertyRegion[] tissueRegions,
            params double[] constantValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            return ComputeFluenceComplex(
                SolverFactory.GetForwardSolver(forwardSolverType),
                solutionDomainType,
                independentAxesTypes,
                independentValues,
                tissueRegions,
                constantValues);
        }

        public static Complex[] ComputeFluenceComplex(
            IForwardSolver forwardSolver,
            FluenceSolutionDomainType solutionDomainType,
            // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            var parameters = new double[4]
                                 {
                                     opticalProperties.Mua, opticalProperties.Musp, opticalProperties.G,
                                     opticalProperties.N
                                 };

            // todo: current assumption below is that the second axes is z. need to generalize
            var func = GetForwardFluenceFuncComplex(forwardSolver, solutionDomainType, independentAxesTypes[0]);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            List<object> inputValues = new List<object>(independentValues);
            constantValues.ForEach(cv => inputValues.Add(cv));

            return func(parameters, inputValues.ToArray());
        }

        public static Complex[] ComputeFluenceComplex(
            IForwardSolver forwardSolver,
            FluenceSolutionDomainType solutionDomainType,
            // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            IOpticalPropertyRegion[] tissueRegions,
            params double[] constantValues)
        {
            var parameters = tissueRegions.SelectMany(region =>
            {
                double[] regionParameters = null;
                if (region is ILayerOpticalPropertyRegion)
                {
                    var layerRegion = (ILayerOpticalPropertyRegion)region;
                    regionParameters = new[]
                        {
                            layerRegion.RegionOP.Mua,
                            layerRegion.RegionOP.Musp,
                            layerRegion.RegionOP.G,
                            layerRegion.RegionOP.N,
                            layerRegion.ZRange.Delta
                        };
                }
                else
                {
                    throw new Exception("Forward model " +
                                        forwardSolver.ToString() +
                                        " is not supported.");
                }
                return regionParameters;
            }).ToArray();

            // todo: current assumption below is that the second axes is z. need to generalize
            var func = GetForwardFluenceFuncComplex(forwardSolver, solutionDomainType, independentAxesTypes[0]);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            List<object> inputValues = new List<object>(independentValues);
            constantValues.ForEach(cv => inputValues.Add(cv));

            return func(parameters, inputValues.ToArray());
        }

        /// <summary>
        /// Overload of GetPHD that uses internal DI framework-supplied solver singletons
        /// </summary>
        /// <param name="forwardSolverType">enum of forward solver type</param>
        /// <param name="fluence">fluence</param>
        /// <param name="sdSeparation">source detector separation (in mm)</param>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">detector locations (in mm)</param>
        /// <param name="zs">z values (in mm)</param>
        /// <returns></returns>
        public static double[] GetPHD(ForwardSolverType forwardSolverType, double[] fluence, double sdSeparation,
                                      OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            return GetPHD(SolverFactory.GetForwardSolver(forwardSolverType), fluence, sdSeparation, ops, rhos, zs);
        }

        /// <summary>
        /// Method to generate PHD 
        /// </summary>
        /// <param name="forwardSolver">forward solver</param>
        /// <param name="fluence">fluence</param>
        /// <param name="sdSeparation">source detector separation (in mm)</param>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">detector locations (in mm)</param>
        /// <param name="zs">z values (in mm)</param>
        /// <returns></returns>
        public static double[] GetPHD(IForwardSolver forwardSolver, double[] fluence, double sdSeparation,
                                      OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            var rhoPrimes =
                from r in rhos
                select r - sdSeparation;

            var greensFunction = forwardSolver.SteadyStateFluence2SurfacePointPHD(ops, rhoPrimes, zs);

            var phd = new double[fluence.Length];

            phd.PopulateFromEnumerable(Enumerable.Zip(fluence, greensFunction, (flu, green) => flu * green));

            return phd;

            // replaced with pre-initialized array + "PopulateFromEnumerable" call instead of growing array dynamically 
            //System.Linq.Enumerable.Zip(fluence, greensFunction, (flu, green) => flu * green).ToArray();
        }

        /// <summary>
        /// Overload of GetPHD that uses internal DI framework-supplied solver singletons
        /// </summary>
        /// <param name="forwardSolverType">enum of forward solver type</param>
        /// <param name="fluence">fluence</param>
        /// <param name="sdSeparation">source detector separation (in mm)</param>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">detector locations (in mm)</param>
        /// <param name="zs">z values (in mm)</param>
        /// <returns></returns>
        public static double[] GetPHD(ForwardSolverType forwardSolverType, Complex[] fluence, double sdSeparation,
                                      double timeModulationFrequency, OpticalProperties[] ops, double[] rhos,
                                      double[] zs)
        {
            return GetPHD(SolverFactory.GetForwardSolver(forwardSolverType), fluence, sdSeparation,
                          timeModulationFrequency, ops, rhos, zs);
        }

        /// <summary>
        /// Method to generate PHD 
        /// </summary>
        /// <param name="forwardSolver">forward solver</param>
        /// <param name="fluence">fluence</param>
        /// <param name="sdSeparation">source detector separation (in mm)</param>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">detector locations (in mm)</param>
        /// <param name="zs">z values (in mm)</param>
        /// <returns></returns>
        public static double[] GetPHD(IForwardSolver forwardSolver, Complex[] fluence, double sdSeparation,
                                      double modulationFrequency, OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            var rhoPrimes =
                from r in rhos
                select r - sdSeparation;

            var greensFunction = forwardSolver.TimeFrequencyDomainFluence2SurfacePointPHD(modulationFrequency, ops,
                                                                                          rhoPrimes, zs);

            var phd = new double[fluence.Length];

            phd.PopulateFromEnumerable(Enumerable.Zip(fluence, greensFunction, (flu, green) => (flu * green).Magnitude));

            return phd;

            // replaced with pre-initialized array + "PopulateFromEnumerable" call instead of growing array dynamically 
            //System.Linq.Enumerable.Zip(fluence, greensFunction, (flu, green) => flu * green).ToArray();
        }

        ///// <summary>
        ///// Method to generate PHD 
        ///// </summary>
        ///// <param name="forwardSolver">forward solver</param>
        ///// <param name="fluence">fluence</param>
        ///// <param name="sdSeparation">source detector separation (in mm)</param>
        ///// <param name="ops">optical properties</param>
        ///// <param name="rhos">detector locations (in mm)</param>
        ///// <param name="zs">z values (in mm)</param>
        ///// <returns></returns>
        //public static Complex[] GetPHD(IForwardSolver forwardSolver, Complex[] fluence, double sdSeparation, double timeModulationFrequency, OpticalProperties[] ops, double[] rhos, double[] zs)
        //{
        //    var rhoPrimes =
        //        from r in rhos
        //        select r - sdSeparation;

        //    var greensFunction = forwardSolver.TimeFrequencyDomainFluence2SurfacePointPHD(timeModulationFrequency, ops, rhoPrimes, zs);

        //    var phd = new Complex[fluence.Length];

        //    phd.PopulateFromEnumerable(Enumerable.Zip(fluence, greensFunction, (flu, green) => flu * green));

        //    return phd;

        //    // replaced with pre-initialized array + "PopulateFromEnumerable" call instead of growing array dynamically 
        //    //System.Linq.Enumerable.Zip(fluence, greensFunction, (flu, green) => flu * green).ToArray();
        //}

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

        /// <summary>
        /// Method to generate absorbed energy given fluence and mua.  Note only works for homogeneous tissue.
        /// </summary>
        /// <param name="fluence">fluence serialized to a 1D IEnumerable of double</param>
        /// <param name="mua">absorption coefficient for entire tissue</param>
        /// <returns>absorbed energy in a 1D IEnumerable of double</returns>
        public static IEnumerable<Complex> GetAbsorbedEnergy(IEnumerable<Complex> fluence, double mua)
        {
            return fluence.Select(flu => flu * mua); // todo: is this correct?? DC 12/08/12
        }

        /// <summary>
        /// Method to generate absorbed energy given fluence and mua for heterogeneous tissue.
        /// </summary>
        /// <param name="fluence">fluence serialized to a 1D IEnumerable of double</param>
        /// <param name="mua">absorption coefficient serialized to a 1D IEnumerable</param>
        /// <returns>absorbed energy in a 1D IEnumerable of double</returns>
        public static IEnumerable<double> GetAbsorbedEnergy(IEnumerable<double> fluence, IEnumerable<double> muas)
        {
            if (fluence.Count() != muas.Count())
                throw new ArgumentException("fluence and muas must be same length");
            IEnumerable<double> result = Enumerable.Zip(fluence, muas, (flu, mua) => flu * mua);
            return result;
        }

        public static double[] SolveInverse(
            ForwardSolverType forwardSolverType,
            OptimizerType optimizerType,
            SolutionDomainType solutionDomainType,
            double[] dependentValues,
            double[] standardDeviationValues,
            InverseFitType inverseFitType,
            object[] independentValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            return SolveInverse(
                SolverFactory.GetForwardSolver(forwardSolverType),
                SolverFactory.GetOptimizer(optimizerType),
                solutionDomainType,
                dependentValues,
                standardDeviationValues,
                inverseFitType,
                independentValues);
        }

        public static double[] SolveInverse(
            IForwardSolver forwardSolver,
            IOptimizer optimizer,
            SolutionDomainType solutionDomainType,
            double[] dependentValues,
            double[] standardDeviationValues,
            InverseFitType inverseFitType,
            object[] independentValues)
        {
            //var opticalPropertyGuess = ((OpticalProperties[]) (independentValues[0])).First();
            //var fitParameters = new double[4] { opticalPropertyGuess.Mua, opticalPropertyGuess.Musp, opticalPropertyGuess.G, opticalPropertyGuess.N };
            var parametersToFit = GetParametersToFit(inverseFitType);
            
            var opticalPropertyGuess = (OpticalProperties[])(independentValues[0]);
            var fitParametersArray = opticalPropertyGuess.SelectMany(opgi => new[] { opgi.Mua, opgi.Musp, opgi.G, opgi.N }).ToArray();
            var parametersToFitArray = Enumerable.Range(0, opticalPropertyGuess.Count()).SelectMany(_ => parametersToFit).ToArray();

            Func<double[], object[], double[]> func = GetForwardReflectanceFuncForOptimization(forwardSolver, solutionDomainType);

            //var fit = optimizer.Solve(fitParameters, parametersToFit, dependentValues.ToArray(),
            //                          standardDeviationValues.ToArray(), func, independentValues.ToArray());
            var fit = optimizer.Solve(fitParametersArray, parametersToFitArray, dependentValues.ToArray(),
                                      standardDeviationValues.ToArray(), func, independentValues.ToArray());

            return fit;
        }


        private static Func<object[], double[]> GetForwardReflectanceFunc(IForwardSolver fs, SolutionDomainType type)
        {
            switch (type)
            {
                case SolutionDomainType.ROfRho:
                    if (fs is TwoLayerSDAForwardSolver) // todo: future generalization to IMultiRegionForwardSolver?
                    {
                        return forwardData => fs.ROfRho((IOpticalPropertyRegion[][])forwardData[0], (double[])forwardData[1]);
                    }
                    return forwardData => fs.ROfRho(ops: (OpticalProperties[])forwardData[0], rhos: (double[])forwardData[1]);

                case SolutionDomainType.ROfFx:
                    if (fs is TwoLayerSDAForwardSolver)
                    {
                        return forwardData => fs.ROfFx((IOpticalPropertyRegion[][])forwardData[0], (double[])forwardData[1]);
                    }
                    return forwardData => fs.ROfFx(ops: (OpticalProperties[])forwardData[0], fxs: (double[])forwardData[1]);

                case SolutionDomainType.ROfRhoAndTime:
                    if (fs is TwoLayerSDAForwardSolver)
                    {
                        return forwardData => fs.ROfRhoAndTime((IOpticalPropertyRegion[][])forwardData[0], (double[])forwardData[1], (double[])forwardData[2]);
                    }
                    return forwardData => fs.ROfRhoAndTime(ops: (OpticalProperties[])forwardData[0], rhos: (double[])forwardData[1], ts: (double[])forwardData[2]);
                case SolutionDomainType.ROfFxAndTime:
                    if (fs is TwoLayerSDAForwardSolver)
                    {
                        return forwardData => fs.ROfFxAndTime((IOpticalPropertyRegion[][])forwardData[0], (double[])forwardData[1], (double[])forwardData[2]);
                    }
                    return forwardData => fs.ROfFxAndTime(ops: (OpticalProperties[])forwardData[0], fxs: (double[])forwardData[1], ts: (double[])forwardData[2]);
                case SolutionDomainType.ROfRhoAndFt:
                    if (fs is TwoLayerSDAForwardSolver)
                    {
                        return forwardData => fs.ROfRhoAndFt((IOpticalPropertyRegion[][])forwardData[0], (double[])forwardData[1], (double[])forwardData[2]).FlattenRealAndImaginary();
                    }
                    return forwardData => fs.ROfRhoAndFt(ops: (OpticalProperties[])forwardData[0], rhos: (double[])forwardData[1], fts: (double[])forwardData[2]).FlattenRealAndImaginary();
                case SolutionDomainType.ROfFxAndFt:
                    if (fs is TwoLayerSDAForwardSolver)
                    {
                        return forwardData => fs.ROfFxAndFt((IOpticalPropertyRegion[][])forwardData[0], (double[])forwardData[1], (double[])forwardData[2]).FlattenRealAndImaginary();
                    }
                    return forwardData => fs.ROfFxAndFt(ops: (OpticalProperties[])forwardData[0], fxs: (double[])forwardData[1], fts: (double[])forwardData[2]).FlattenRealAndImaginary();

                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }


        private static Func<double[], ILayerOpticalPropertyRegion[]> getLayerTissueRegionArray = layerProps =>
        {
            int numRegions = layerProps.Length / 5; // mua, musp, g, n, thickness (delta)
            var regionArray = new ILayerOpticalPropertyRegion[numRegions];
            regionArray[0] = new LayerOpticalPropertyRegion(
                zRange: new DoubleRange(0, layerProps[4]),
                regionOP: new OpticalProperties(layerProps[0], layerProps[1], layerProps[2], layerProps[3]));
            for (int i = 1; i < numRegions; i++)
            {
                var currentLayerProps = layerProps.Skip(i * 5).Take(5).ToArray();
                var previousStop = regionArray[i - 1].ZRange.Stop;
                regionArray[i] = new LayerOpticalPropertyRegion(
                    zRange: new DoubleRange(previousStop, previousStop + currentLayerProps[4]),
                    regionOP: new OpticalProperties(currentLayerProps[0], currentLayerProps[1], currentLayerProps[2], currentLayerProps[3]));
            }
            return regionArray;
        };

        private static Func<double[], object[], double[]> GetForwardReflectanceFuncForOptimization(
           IForwardSolver fs, SolutionDomainType type)
        {
            Func<object[], double[]> forwardReflectanceFunc = GetForwardReflectanceFunc(fs, type);

            return (fitData, allParameters) =>
            {
                // place optical property array in the first position, and the rest following
                var forwardData = ((object)UnFlattenOpticalProperties(fitData)).AsEnumerable().Concat(allParameters.Skip(1)).ToArray();
                return forwardReflectanceFunc(forwardData);
            };
        }

        public static OpticalProperties[] UnFlattenOpticalProperties(double[] ops)
        {
            var nOp = ops.Length / 4;
            var opArray = new OpticalProperties[nOp];
            for (int opi = 0; opi < nOp; opi++)
            {
                opArray[opi] = new OpticalProperties(ops[opi * 4], ops[opi * 4 + 1], ops[opi * 4 + 2], ops[opi * 4 + 3]);
            }
            return opArray;
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
                case FluenceSolutionDomainType.FluenceOfRhoAndZ:
                    if (fs is TwoLayerSDAForwardSolver) // todo: future generalization to IMultiRegionForwardSolver?
                    {
                        return (fitData, otherData) => fs.FluenceOfRhoAndZ(new[] { getLayerTissueRegionArray(fitData) }, (double[])otherData[0], (double[])otherData[1]);
                    }
                    return (fitData, otherData) => fs.FluenceOfRhoAndZ(new[] { getOP(fitData) }, (double[])otherData[0], (double[])otherData[1]);
                case FluenceSolutionDomainType.FluenceOfFxAndZ:
                    return (fitData, otherData) => fs.FluenceOfFxAndZ(new[] { getOP(fitData) }, (double[])otherData[0], (double[])otherData[1]);
                case FluenceSolutionDomainType.FluenceOfRhoAndZAndTime:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.FluenceOfRhoAndZAndTime(new[] { getOP(fitData) }, (double[])otherData[0], (double[])otherData[1], new[] { (double)otherData[2] });
                        case IndependentVariableAxis.Time:
                            return (fitData, otherData) => fs.FluenceOfRhoAndZAndTime(new[] { getOP(fitData) }, new[] { (double)otherData[2] }, (double[])otherData[1], (double[])otherData[0]);
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
                        //                   return EnumerableExtensions.Zip(muas,musps,(mua,musp)=>fs.ROfRhoAndTime())...
                        //               }; 
                        //    return op => fs.ROfRhoAndTime(op, ((double)constantValues[0]).AsEnumerable(), ((double)constantValues[1]).AsEnumerable());
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case FluenceSolutionDomainType.FluenceOfFxAndZAndTime:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.FluenceOfFxAndZAndTime(new[] { getOP(fitData) }, (double[])otherData[0], (double[])otherData[1], new[] { (double)otherData[2] });
                        case IndependentVariableAxis.Time:
                            return (fitData, otherData) => fs.FluenceOfFxAndZAndTime(new[] { getOP(fitData) }, new[] { (double)otherData[2] }, (double[])otherData[1], (double[])otherData[0]);
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private static Func<double[], object[], Complex[]> GetForwardFluenceFuncComplex(
           IForwardSolver fs, FluenceSolutionDomainType type, IndependentVariableAxis axis)
        {
            Func<double[], OpticalProperties> getOP = op => new OpticalProperties(op[0], op[1], op[2], op[3]);

            // note: the following uses the convention that the independent variable(s) is (are) first in the forward data object array
            // note: secondly, if there are multiple independent axes, they will be assigned in order of appearance in the method signature
            switch (type)
            {
                case FluenceSolutionDomainType.FluenceOfRhoAndZAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.FluenceOfRhoAndZAndFt(new[] { getOP(fitData) }, (double[])otherData[0], (double[])otherData[1], new[] { (double)otherData[2] });
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.FluenceOfRhoAndZAndFt(new[] { getOP(fitData) }, new[] { (double)otherData[2] }, (double[])otherData[1], (double[])otherData[0]);
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case FluenceSolutionDomainType.FluenceOfFxAndZAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.FluenceOfFxAndZAndFt(new[] { getOP(fitData) }, (double[])otherData[0], (double[])otherData[1], new[] { (double)otherData[2] });
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.FluenceOfFxAndZAndFt(new[] { getOP(fitData) }, new[] { (double)otherData[2] }, (double[])otherData[1], (double[])otherData[0]);
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