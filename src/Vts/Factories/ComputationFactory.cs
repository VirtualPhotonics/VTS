using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vts.Common;
using Vts.Extensions;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.ForwardSolvers.Extensions;
#if DESKTOP
#endif

namespace Vts.Factories
{
    /// <summary>
    /// The <see cref="Factories"/> namespace contains the factory classes for the Virtual Tissue Simulator
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Class that composes forward and optimization calculations based on high-level inputs
    /// </summary>
    public static class ComputationFactory
    {
        // the following two methods are a result of a leaky abstraction?
        // if we did our job of abstracting the computation, external users wouldn't have to worry about this

        // the following methods are necessary to the GUI
        /// <summary>
        /// Method to determine if forward solver is solver with constant values
        /// </summary>
        /// <param name="solutionDomainType">The SolutionDomainType Enum</param>
        /// <returns>A Boolean indicating if solver with constant values</returns>
        public static bool IsSolverWithConstantValues(SolutionDomainType solutionDomainType)
        {
            return
                (solutionDomainType != SolutionDomainType.ROfRho) &&
                (solutionDomainType != SolutionDomainType.ROfFx);
        }

        /// <summary>
        /// Method to determine if forward solver is solver with constant values
        /// </summary>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType Enum</param>
        /// <returns>A Boolean indicating if solver with constant values</returns>
        public static bool IsSolverWithConstantValues(FluenceSolutionDomainType solutionDomainType)
        {
            return
                (solutionDomainType != FluenceSolutionDomainType.FluenceOfRhoAndZ) &&
                (solutionDomainType != FluenceSolutionDomainType.FluenceOfFxAndZ);
        }

        /// <summary>
        /// Method to indicate whether forward solver is a complex solver
        /// </summary>
        /// <param name="solutionDomainType">The SolutionDomainType Enum</param>
        /// <returns>A Boolean indicating whether forward solver is complex or not</returns>
        public static bool IsComplexSolver(SolutionDomainType solutionDomainType)
        {
            return
                (solutionDomainType == SolutionDomainType.ROfRhoAndFt) ||
                (solutionDomainType == SolutionDomainType.ROfFxAndFt);
        }

        /// <summary>
        /// Method to indicate whether forward solver is a complex solver
        /// </summary>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType Enum</param>
        /// <returns>A Boolean indicating whether forward solver is complex or not</returns>
        public static bool IsComplexSolver(FluenceSolutionDomainType solutionDomainType)
        {
            return
                (solutionDomainType == FluenceSolutionDomainType.FluenceOfRhoAndZAndFt) ||
                (solutionDomainType == FluenceSolutionDomainType.FluenceOfFxAndZAndFt);
        }

        /// <summary>
        /// Method to flatten the real and imaginary parts of an array of complex values
        /// </summary>
        /// <param name="values">The complex values to flatten</param>
        /// <returns>A 1D array of real (1st half) and imaginary (2nd half) </returns>
        private static double[] FlattenRealAndImaginary(this IReadOnlyList<Complex> values)
        {
            var flattened = new double[values.Count * 2];
            var nValues = values.Count;
            for (var i = 0; i < nValues; i++)
            {
                flattened[i] = values[i].Real;
                flattened[i + nValues] = values[i].Imaginary;
            }
            return flattened;
        }

        /// <summary>
        /// ComputeReflectance determines reflectance. It uses the first parameter
        /// forwardSolverType to determine appropriate IForwardSolver and then calls the method overload with that value.
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum (e.g. PointSourceSDA, DistributedPointSourceSDA, etc.)</param>
        /// <param name="solutionDomainType">The SolutionDomainType enum (e.g. RofRho, RofRx, etc.)</param>
        /// <param name="forwardAnalysisType">The ForwardAnalysisType enum (e.g. R, dRdMua, dRdMusp, etc.)</param>
        /// <param name="independentValues">The array of objects: first element = OpticalProperties,
        /// second element = double[] of x-axis values, for example:
        /// new object[]{ new[]{ new OpticalProperties(0.01, 1, 0.8, 1.4) }, new double[] { 1, 2, 3 } })</param>
        /// <returns>A double[] of resulting reflectance values</returns>
        public static double[] ComputeReflectance(
            ForwardSolverType forwardSolverType,
            SolutionDomainType solutionDomainType,
            ForwardAnalysisType forwardAnalysisType,
            object[] independentValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            // call the method below once ForwardSolver obtained using ForwardSolverType
            return ComputeReflectance(
                SolverFactory.GetForwardSolver(forwardSolverType),
                solutionDomainType,
                forwardAnalysisType,
                independentValues);
        }

        /// <summary>
        /// ComputeReflectance overload determines reflectance. It uses the first parameter
        /// IForwardSolver instead of ForwardSolverType.
        /// </summary>
        /// <param name="forwardSolver">The forward solver class</param>
        /// <param name="solutionDomainType">The SolutionDomainType enum (e.g. RofRho, RofRx, etc.)</param>
        /// <param name="forwardAnalysisType">The ForwardAnalysisType enum (e.g. R, dRdMua, dRdMusp, etc.)</param>
        /// <param name="independentValues">The array of objects: first element = OpticalProperties,
        /// second element = double[] of x-axis values, for example:
        /// new object[]{ new[]{ new OpticalProperties(0.01, 1, 0.8, 1.4) }, new double[] { 1, 2, 3 } })</param>
        /// <returns>A double[] of resulting reflectance values</returns>
        public static double[] ComputeReflectance(
            IForwardSolver forwardSolver,
            SolutionDomainType solutionDomainType,
            ForwardAnalysisType forwardAnalysisType,
            object[] independentValues)
        {
            var func = GetForwardReflectanceFunc(forwardSolver, solutionDomainType);
            //Func<SolutionDomainType, ForwardAnalysisType, IndependentVariableAxis[], double[]> getOptimizationParameters = (_,_,_) => 
            //    new[] { op.Mua, op.Musp, op.G, op.N }
            //double[] optimizationParameters = GetOptimizationParameters(forwardSolver, solutionDomainType, independentAxisTypes); // will need this for inverse solver

            if (IsComplexSolver(solutionDomainType) && forwardAnalysisType != ForwardAnalysisType.R) // complex derivative
            {
                // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
                // create Complex derivative function with re/im and dre/dim in it
                return func(independentValues)
                    .Concat(func.GetDerivativeFunc(forwardAnalysisType)(independentValues)).ToArray();
            }

            // prior code
            return forwardAnalysisType == ForwardAnalysisType.R
                ? func(independentValues)
                : func.GetDerivativeFunc(forwardAnalysisType)(independentValues);
        }

        /// <summary>
        /// ComputeFluence determines fluence. It uses the first parameter forwardSolverType to determine appropriate
        /// IForwardSolver and then calls the method overload with that value.
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum (e.g. PointSourceSDA, DistributedPointSourceSDA, etc.)</param>
        /// <param name="solutionDomainType">The SolutionDomainType enum (e.g. RofRho, RofRx, etc.)</param>
        /// <param name="independentAxesTypes">The IndependentVariableAxis enum (Rho, Time, Fx, Ft, Z)</param>
        /// <param name="independentValues">The double array of independent type axis values</param>
        /// <param name="opticalProperties">The array of optical properties</param>
        /// <param name="constantValues">The double array of variable length with the constant values</param>
        /// <returns>A double[] of resulting reflectance values</returns>
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

        /// <summary>
        /// ComputeFluence overload determines fluence which uses the first parameter ForwardSolverType to determine
        /// appropriate IForwardSolver and then calls overload.
        /// This overload also has a single set of OpticalProperties parameter rather than an array.
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum (e.g. PointSourceSDA, DistributedPointSourceSDA, etc.)</param>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType enum (e.g. FluenceOfRhoAndZ, FluenceOfFxAndZ etc.)</param>
        /// <param name="independentAxesTypes">The IndependentVariableAxis enum (Rho, Time, Fx, Ft, Z)</param>
        /// <param name="independentValues">The double array of independent axis type values</param>
        /// <param name="opticalProperties">The single set of optical properties</param>
        /// <param name="constantValues">The double array of variable length with the constant values</param>
        /// <returns>A double[] of resulting reflectance values</returns>
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

        /// <summary>
        /// ComputeFluence overload computes fluence
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum (e.g. PointSourceSDA, DistributedPointSourceSDA, etc.)</param>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType enum (e.g. FluenceOfRhoAndZ, FluenceOfFxAndZ etc.)</param>
        /// <param name="independentAxesTypes">The array of IndependentVariableAxis enum (Rho, Time, Fx, Ft, Z)</param>
        /// <param name="independentValues">The jagged array of independent values</param>
        /// <param name="tissueRegions">The array of optical property regions</param>
        /// <param name="constantValues">The double array of variable length with the constant values</param>
        /// <returns>A double[] of resulting linearized fluence values</returns>
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

        // overload for ITissueRegion forward solvers: merge with above?
        /// <summary>
        /// ComputeFluence overload determines fluence given the input parameters.  
        /// </summary>
        /// <param name="forwardSolver">The forward solver class</param>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType enum (e.g. FluenceOfRhoAndZ, FluenceOfFxAndZ etc.)</param>
        /// <param name="independentAxesTypes">The array of IndependentVariableAxis enum (Rho, Time, Fx, Ft, Z)</param>
        /// <param name="independentValues">The double array of independent type axis values</param>
        /// <param name="tissueRegions">The array of IOpticalPropertyRegion which allows for multi-region tissue</param>
        /// <param name="constantValues">The double array of variable length with the constant values</param>
        /// <returns>A double[] of resulting linearized fluence value</returns>
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
                var layerRegion = region as ILayerOpticalPropertyRegion;
                if (layerRegion != null)
                {
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
                    throw new ArgumentException("The tissue region did not contain a valid layer region", nameof(tissueRegions));
                }
                return regionParameters;
            }).ToArray();

            // current assumption below is that the second axis is z. need to generalize
            var func = GetForwardFluenceFunc(forwardSolver, solutionDomainType, independentAxesTypes[0]);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            var inputValues = new List<object>(independentValues);
            constantValues.ForEach(cv => inputValues.Add(cv));

            return func(parameters, inputValues.ToArray());
        }

        /// <summary>
        /// ComputeFluence overload computes fluence for specified forward solver.
        /// </summary>
        /// <param name="forwardSolver">The forward solver class</param>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType enum (e.g. FluenceOfRhoAndZ, FluenceOfFxAndZ etc.)</param>
        /// <param name="independentAxesTypes">The array of IndependentVariableAxis enum (Rho, Time, Fx, Ft, Z)</param>
        /// <param name="independentValues">The double array of independent type axis values</param>
        /// <param name="opticalProperties">The array of optical properties</param>
        /// <param name="constantValues">The double array of variable length with the constant values</param>
        /// <returns>A double[] of resulting linearized fluence value</returns>
        public static double[] ComputeFluence(
            IForwardSolver forwardSolver,
            FluenceSolutionDomainType solutionDomainType, // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            OpticalProperties[] opticalProperties,
            params double[] constantValues)
        {
            // current assumption below is that the second axes is z. need to generalize?
            var func = GetForwardFluenceFunc(forwardSolver, solutionDomainType, independentAxesTypes[0]);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            var inputValues = new List<object>(independentValues);
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
            for (var opi = 0; opi < numOp; opi++) // parallelize?
            {
                var op = opticalProperties[opi];
                var parameters = new[] { op.Mua, op.Musp, op.G, op.N };
                var tempValues = func(parameters, inputValues.ToArray());

                for (var ivi = 0; ivi < numIv; ivi++)
                {
                    fluence[opi * numIv + ivi] = tempValues[ivi];
                }
            }
            return fluence;
        }

        /// <summary>
        /// ComputeFluence determines fluence. This overload has a single set of OpticalProperties parameters
        /// rather than an array.
        /// </summary>
        /// <param name="forwardSolver">The forward solver class</param>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType enum (e.g. FluenceOfRhoAndZ, FluenceOfFxAndZ etc.)</param>
        /// <param name="independentAxesTypes">The array of IndependentVariableAxis enum (Rho, Time, Fx, Ft, Z)</param>
        /// <param name="independentValues">The double array of independent type axis values</param>
        /// <param name="opticalProperties">The single set of optical properties</param>
        /// <param name="constantValues">The double array of variable length with the constant values</param>
        /// <returns>A double[] of resulting linearized fluence value</returns>
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

        /// <summary>
        /// ComputeFluenceComplex computes fluence for complex forward solvers.
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum (e.g. PointSourceSDA, DistributedPointSourceSDA, etc.)</param>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType enum (e.g. FluenceOfRhoAndZAndFt, FluenceOfFxAndZAndFt etc.)</param>
        /// <param name="independentAxesTypes">The array of IndependentVariableAxis enum (Rho, Time, Fx, Ft, Z)</param>
        /// <param name="independentValues">The double array of independent type axis values</param>
        /// <param name="opticalProperties">The single set of optical properties</param>
        /// <param name="constantValues">The double array of variable length with the constant values</param>
        /// <returns>A Complex[] of resulting linearized fluence value</returns>
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

        /// <summary>
        /// ComputeFluenceComplex overload computes fluence for complex forward solvers.
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum (e.g. PointSourceSDA, DistributedPointSourceSDA, etc.)</param>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType enum (e.g. FluenceOfRhoAndZAndFt, FluenceOfFxAndZAndFt etc.)</param>
        /// <param name="independentAxesTypes">The array of IndependentVariableAxis enum (Rho, Time, Fx, Ft, Z)</param>
        /// <param name="independentValues">The double array of independent type axis values</param>
        /// <param name="tissueRegions">The array of IOpticalPropertyRegions</param>
        /// <param name="constantValues">The double array of variable length with the constant values</param>
        /// <returns>A Complex[] of resulting linearized fluence value</returns>
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

        /// <summary>
        /// ComputeFluenceComplex overload computes fluence for complex forward solvers.
        /// </summary>
        /// <param name="forwardSolver">The forward solver class</param>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType enum (e.g. FluenceOfRhoAndZAndFt, FluenceOfFxAndZAndFt etc.)</param>
        /// <param name="independentAxesTypes">The array of IndependentVariableAxis enum (Rho, Time, Fx, Ft, Z)</param>
        /// <param name="independentValues">The double array of independent type axis values</param>
        /// <param name="opticalProperties">The single set of optical properties</param>
        /// <param name="constantValues">The double array of variable length with the constant values</param>
        /// <returns>A Complex[] of resulting linearized fluence value</returns>
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

            // current assumption below is that the second axes is z. need to generalize?
            var func = GetForwardFluenceFuncComplex(forwardSolver, solutionDomainType, independentAxesTypes[0]);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            var inputValues = new List<object>(independentValues);
            constantValues.ForEach(cv => inputValues.Add(cv));

            return func(parameters, inputValues.ToArray());
        }

        /// <summary>
        /// ComputeFluenceComplex overload computes fluence for complex forward solvers. Overload
        /// parameter specifies tissue regions using IOpticalPropertyRegion[] instead of a single set of
        /// optical properties.
        /// </summary>
        /// <param name="forwardSolver">The forward solver class</param>
        /// <param name="solutionDomainType">The FluenceSolutionDomainType enum (e.g. FluenceOfRhoAndZAndFt, FluenceOfFxAndZAndFt etc.)</param>
        /// <param name="independentAxesTypes">The array of IndependentVariableAxis enum (Rho, Time, Fx, Ft, Z)</param>
        /// <param name="independentValues">The double array of independent type axis values</param>
        /// <param name="tissueRegions">The array of IOpticalPropertyRegions</param>
        /// <param name="constantValues">The double array of variable length with the constant values</param>
        /// <returns>A Complex[] of resulting linearized fluence value</returns>
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
                var layerRegion = region as ILayerOpticalPropertyRegion;
                if (layerRegion != null)
                {
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
                    throw new ArgumentException("The tissue region did not contain a valid layer region", nameof(tissueRegions));
                }
                return regionParameters;
            }).ToArray();

            // current assumption below is that the second axes is z. need to generalize?
            var func = GetForwardFluenceFuncComplex(forwardSolver, solutionDomainType, independentAxesTypes[0]);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            var inputValues = new List<object>(independentValues);
            constantValues.ForEach(cv => inputValues.Add(cv));

            return func(parameters, inputValues.ToArray());
        }

        /// <summary>
        /// Overload of GetPHD that uses internal DI framework-supplied solver singletons
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum (e.g. PointSourceSDA, DistributedPointSourceSDA, etc.)</param>
        /// <param name="fluence">The linearized fluence to be used to generate PHD, column major</param>
        /// <param name="sdSeparation">The source detector separation (in mm)</param>
        /// <param name="ops">The optical properties</param>
        /// <param name="rhos">The detector locations (in mm)</param>
        /// <param name="zs">The z values (in mm)</param>
        /// <returns>A double[] of resulting linearized PHD values</returns>
        public static double[] GetPHD(ForwardSolverType forwardSolverType, double[] fluence, double sdSeparation,
                                      OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            return GetPHD(SolverFactory.GetForwardSolver(forwardSolverType), fluence, sdSeparation, ops, rhos, zs);
        }

        /// <summary>
        /// Method to generate Photon Hitting Density (PHD) Map 
        /// </summary>
        /// <param name="forwardSolver">The forward solver class</param>
        /// <param name="fluence">The linearized fluence used to generate PHD, column major</param>
        /// <param name="sdSeparation">The source detector separation (in mm)</param>
        /// <param name="ops">The optical properties</param>
        /// <param name="rhos">The detector locations (in mm)</param>
        /// <param name="zs">The z values (in mm)</param>
        /// <returns>A double[] of resulting linearized PHD values</returns>
        public static double[] GetPHD(IForwardSolver forwardSolver, double[] fluence, double sdSeparation,
                                      OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            var rhoPrimes =
                from r in rhos
                select r - sdSeparation;

            var greensFunction = forwardSolver.SteadyStateFluence2SurfacePointPHD(ops, rhoPrimes, zs);

            var phd = new double[fluence.Length];

            phd.PopulateFromEnumerable(fluence.Zip(greensFunction, (flu, green) => flu * green));

            return phd;

        }

        /// <summary>
        /// Overload of GetPHD that uses internal DI framework-supplied solver singletons
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum (e.g. PointSourceSDA, DistributedPointSourceSDA, etc.)</param>
        /// <param name="fluence">The linearized fluence used to generate PHD, column major</param>
        /// <param name="sdSeparation">The source detector separation (in mm)</param>
        /// <param name="timeModulationFrequency">The modulation frequency of Time-Domain fluence</param>
        /// <param name="ops">The optical properties</param>
        /// <param name="rhos">The detector locations (in mm)</param>
        /// <param name="zs">The z values (in mm)</param>
        /// <returns>A double array o photon hitting density map</returns>
        public static double[] GetPHD(ForwardSolverType forwardSolverType, Complex[] fluence, double sdSeparation,
                                      double timeModulationFrequency, OpticalProperties[] ops, double[] rhos,
                                      double[] zs)
        {
            return GetPHD(SolverFactory.GetForwardSolver(forwardSolverType), fluence, sdSeparation,
                          timeModulationFrequency, ops, rhos, zs);
        }

        /// <summary>
        /// Method to generate Photon Hitting Density (PHD) map
        /// </summary>
        /// <param name="forwardSolver">The forward solver class</param>
        /// <param name="fluence">The linearized fluence used to generate PHD, column major</param>
        /// <param name="sdSeparation">The source detector separation (in mm)</param>
        /// <param name="modulationFrequency">The modulation frequency of Time-Domain fluence</param>
        /// <param name="ops">The optical properties</param>
        /// <param name="rhos">The detector locations (in mm)</param>
        /// <param name="zs">The z values (in mm)</param>
        /// <returns>A double[] of resulting linearized PHD values</returns>
        public static double[] GetPHD(IForwardSolver forwardSolver, Complex[] fluence, double sdSeparation,
                                      double modulationFrequency, OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            var rhoPrimes =
                from r in rhos
                select r - sdSeparation;

            var greensFunction = forwardSolver.TimeFrequencyDomainFluence2SurfacePointPHD(modulationFrequency, ops,
                                                                                          rhoPrimes, zs);

            var phd = new double[fluence.Length];

            phd.PopulateFromEnumerable(fluence.Zip(greensFunction, (flu, green) => (flu * green).Magnitude));

            return phd;

        }

        /// <summary>
        /// Method to generate absorbed energy given fluence and mua.  Note only works for homogeneous tissue.
        /// </summary>
        /// <param name="fluence">The fluence serialized to a 1D IEnumerable of double</param>
        /// <param name="mua">The absorption coefficient for entire tissue</param>
        /// <returns>Absorbed energy in a 1D IEnumerable of double</returns>
        public static IEnumerable<double> GetAbsorbedEnergy(IEnumerable<double> fluence, double mua)
        {
            return fluence.Select(flu => flu * mua);
        }

        /// <summary>
        /// Method to generate absorbed energy given fluence and mua.  Note only works for homogeneous tissue.
        /// </summary>
        /// <param name="fluence">The fluence serialized to a 1D IEnumerable of double</param>
        /// <param name="mua">The absorption coefficient for entire tissue</param>
        /// <returns>Absorbed energy in a 1D IEnumerable of Complex</returns>
        public static IEnumerable<Complex> GetAbsorbedEnergy(IEnumerable<Complex> fluence, double mua)
        {
            return fluence.Select(flu => flu * mua); 
        }

        /// <summary>
        /// Method to generate absorbed energy given fluence and mua for heterogeneous tissue.
        /// </summary>
        /// <param name="fluence">The fluence serialized to a 1D IEnumerable</param>
        /// <param name="muas">The absorption coefficient serialized to a 1D IEnumerable with Count equal to that of fluence</param>
        /// <returns>Absorbed energy in a 1D IEnumerable of double</returns>
        public static IEnumerable<double> GetAbsorbedEnergy(IEnumerable<double> fluence, IEnumerable<double> muas)
        {
            var fluenceArray = fluence as double[] ?? fluence.ToArray();
            var muaArray = muas as double[] ?? muas.ToArray();
            if (fluenceArray.Length != muaArray.Length) throw new ArgumentException("The arguments fluence and muas must be same length");
            var result = fluenceArray.Zip(muaArray, (flu, mua) => flu * mua);
            return result;
        }

        /// <summary>
        /// Method to provide the inverse solution.
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum (e.g. PointSourceSDA, DistributedPointSourceSDA, etc.)</param>
        /// <param name="optimizerType">The OptimizerType enum (e.g. MPFitLevenbergMarquardt)</param>
        /// <param name="solutionDomainType">The SolutionDomainType enum (e.g. ROfRho, ROfFx)</param>
        /// <param name="dependentValues">The measured data length = x axis variable count</param>
        /// <param name="standardDeviationValues">The standard deviation of measured data</param>
        /// <param name="inverseFitType">The InverseFitType enum (e.g. Mua, Musp, MuaMusp, MuaMuspG)</param>
        /// <param name="independentValues">The array of objects: first element = OpticalProperties,
        /// second element = double[] of x axis values, for example:
        /// new object[]{ new[]{ new OpticalProperties(0.01, 1, 0.8, 1.4) }, new double[] { 1, 2, 3 } })</param>
        /// <returns>A double array of optical property inverse solution</returns>
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

        /// <summary>
        /// Method to provide the inverse solution.
        /// </summary>
        /// <param name="forwardSolver">The forward solver class</param>
        /// <param name="optimizer">The optimizer class</param>
        /// <param name="solutionDomainType">The SolutionDomainType enum (e.g. ROfRho, ROfFx)</param>
        /// <param name="dependentValues">The measured data length = x axis variable count</param>
        /// <param name="standardDeviationValues">The standard deviation of measured data</param>
        /// <param name="inverseFitType">The InverseFitType enum (e.g. Mua, Musp, MuaMusp, MuaMuspG)</param>
        /// <param name="independentValues">The array of objects: first element = OpticalProperties,
        /// second element = double[] of x axis values, for example:
        /// new object[]{ new[]{ new OpticalProperties(0.01, 1, 0.8, 1.4) }, new double[] { 1, 2, 3 } })</param>
        /// <returns>A double array of optical property inverse solution</returns>
        public static double[] SolveInverse(
            IForwardSolver forwardSolver,
            IOptimizer optimizer,
            SolutionDomainType solutionDomainType,
            double[] dependentValues,
            double[] standardDeviationValues,
            InverseFitType inverseFitType,
            object[] independentValues)
        {
            var parametersToFit = GetParametersToFit(inverseFitType);
            
            var opticalPropertyGuess = (OpticalProperties[])(independentValues[0]);
            var fitParametersArray = opticalPropertyGuess.SelectMany(opgi => new[] { opgi.Mua, opgi.Musp, opgi.G, opgi.N }).ToArray();
            var parametersToFitArray = Enumerable.Range(0, opticalPropertyGuess.Count()).SelectMany(_ => parametersToFit).ToArray();

            var func = GetForwardReflectanceFuncForOptimization(forwardSolver, solutionDomainType);

            var fit = optimizer.Solve(fitParametersArray, parametersToFitArray, dependentValues.ToArray(),
                                      standardDeviationValues.ToArray(), func, independentValues.ToArray());

            return fit;
        }

        /// <summary>
        /// Method to provide the inverse solution.
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum (e.g. PointSourceSDA, DistributedPointSourceSDA, etc.)</param>
        /// <param name="optimizerType">The OptimizerType enum (e.g. MPFitLevenbergMarguardt</param>
        /// <param name="solutionDomainType">The SolutionDomainType enum (e.g. ROfRho, ROfFx)</param>
        /// <param name="dependentValues">The measured data length = x axis variable count</param>
        /// <param name="standardDeviationValues">The standard deviation of measured data</param>
        /// <param name="inverseFitType">The InverseFitType enum (e.g. Mua, Musp, MuaMusp, MuaMuspG)</param>
        /// <param name="independentValues">The array of objects: first element = OpticalProperties,
        /// second element = double[] of x axis values, for example:
        /// new object[]{ new[]{ new OpticalProperties(0.01, 1, 0.8, 1.4) }, new double[] { 1, 2, 3 } })</param>
        /// <param name="lowerBounds">constrained fit with lower bounds for OPs, size = 4 for all OPs</param>
        /// <param name="upperBounds">constrained fit with upper bounds for OPs, size = 4 for all OPs</param>
        /// <returns>A double array of optical property inverse solution</returns>
        public static double[] SolveInverse(
        ForwardSolverType forwardSolverType,
        OptimizerType optimizerType,
        SolutionDomainType solutionDomainType,
        double[] dependentValues,
        double[] standardDeviationValues,
        InverseFitType inverseFitType,
        object[] independentValues,
        double[] lowerBounds,
        double[] upperBounds)
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
                independentValues,
                lowerBounds,
                upperBounds);
        }

        /// <summary>
        /// Method to provide the inverse solution.
        /// </summary>
        /// <param name="forwardSolver">The forward solver class</param>
        /// <param name="optimizer">The optimizer class</param>
        /// <param name="solutionDomainType">The SolutionDomainType enum (e.g. ROfRho, ROfFx)</param>
        /// <param name="dependentValues">The measured data length = x axis variable count</param>
        /// <param name="standardDeviationValues">The standard deviation of measured data</param>
        /// <param name="inverseFitType">The InverseFitType enum (e.g. Mua, Musp, MuaMusp, MuaMuspG)</param>
        /// <param name="independentValues">The array of objects: first element = OpticalProperties,
        /// second element = double[] of xaxis values, for example:
        /// new object[]{ new[]{ new OpticalProperties(0.01, 1, 0.8, 1.4) }, new double[] { 1, 2, 3 } })</param>
        /// <param name="lowerBounds">The constrained fit with lower bounds for OPs, size = 4 for all OPs</param>
        /// <param name="upperBounds">The constrained fit with upper bounds for OPs, size = 4 for all OPs</param>
        /// <returns>A double array of optical property inverse solution</returns>
        public static double[] SolveInverse(
            IForwardSolver forwardSolver,
            IOptimizer optimizer,
            SolutionDomainType solutionDomainType,
            double[] dependentValues,
            double[] standardDeviationValues,
            InverseFitType inverseFitType,
            object[] independentValues,
            double[] lowerBounds,
            double[] upperBounds)
        {
            var parametersToFit = GetParametersToFit(inverseFitType);

            var opticalPropertyGuess = (OpticalProperties[])(independentValues[0]);
            var fitParametersArray = opticalPropertyGuess.SelectMany(opgi => new[] { opgi.Mua, opgi.Musp, opgi.G, opgi.N }).ToArray();
            var parametersToFitArray = Enumerable.Range(0, opticalPropertyGuess.Length).SelectMany(_ => parametersToFit).ToArray();
            var lowerBoundsArray = Enumerable.Range(0, opticalPropertyGuess.Length).SelectMany(_ => lowerBounds).ToArray();
            var upperBoundsArray = Enumerable.Range(0, opticalPropertyGuess.Length).SelectMany(_ => upperBounds).ToArray();

            var func = GetForwardReflectanceFuncForOptimization(forwardSolver, solutionDomainType);

            var fit = optimizer.SolveWithConstraints(fitParametersArray, parametersToFitArray, lowerBoundsArray, upperBoundsArray, dependentValues.ToArray(),
                                      standardDeviationValues.ToArray(), func, independentValues.ToArray());

            return fit;
        }


        private static Func<object[], double[]> GetForwardReflectanceFunc(IForwardSolver fs, SolutionDomainType type)
        {
            switch (type)
            {
                case SolutionDomainType.ROfRho:
                    if (fs is TwoLayerSDAForwardSolver) // future generalization to IMultiRegionForwardSolver?
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
                    return forwardData => fs.ROfRhoAndTime(ops: (OpticalProperties[])forwardData[0], rhos: (double[])forwardData[1], times: (double[])forwardData[2]);
                case SolutionDomainType.ROfFxAndTime:
                    if (fs is TwoLayerSDAForwardSolver)
                    {
                        return forwardData => fs.ROfFxAndTime((IOpticalPropertyRegion[][])forwardData[0], (double[])forwardData[1], (double[])forwardData[2]);
                    }
                    return forwardData => fs.ROfFxAndTime(ops: (OpticalProperties[])forwardData[0], fxs: (double[])forwardData[1], times: (double[])forwardData[2]);
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
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }


        private static Func<double[], ILayerOpticalPropertyRegion[]> getLayerTissueRegionArray = layerProps =>
        {
            var numRegions = layerProps.Length / 5; // mua, musp, g, n, thickness (delta)
            var regionArray = new ILayerOpticalPropertyRegion[numRegions];
            regionArray[0] = new LayerOpticalPropertyRegion(
                zRange: new DoubleRange(0, layerProps[4]),
                regionOP: new OpticalProperties(layerProps[0], layerProps[1], layerProps[2], layerProps[3]));
            for (var i = 1; i < numRegions; i++)
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
            var forwardReflectanceFunc = GetForwardReflectanceFunc(fs, type);

            return (fitData, allParameters) =>
            {
                // place optical property array in the first position, and the rest following
                var forwardData = ((object)UnFlattenOpticalProperties(fitData)).AsEnumerable().Concat(allParameters.Skip(1)).ToArray();
                return forwardReflectanceFunc(forwardData);
            };
        }

        /// <summary>
        /// method to rehydrate OpticalProperties class from array of doubles
        /// </summary>
        /// <param name="ops">The array of doubles to unflatten</param>
        /// <returns>An array of OpticalProperties</returns>
        public static OpticalProperties[] UnFlattenOpticalProperties(double[] ops)
        {
            var nOp = ops.Length / 4;
            var opArray = new OpticalProperties[nOp];
            for (var opi = 0; opi < nOp; opi++)
            {
                opArray[opi] = new OpticalProperties(ops[opi * 4], ops[opi * 4 + 1], ops[opi * 4 + 2], ops[opi * 4 + 3]);
            }
            return opArray;
        }

        // should we array overloads for fluence forward solvers too?
        private static Func<double[], object[], double[]> GetForwardFluenceFunc(
           IForwardSolver fs, FluenceSolutionDomainType type, IndependentVariableAxis axis)
        {
            Func<double[], OpticalProperties> getOP = op => new OpticalProperties(op[0], op[1], op[2], op[3]);

            // note: the following uses the convention that the independent variable(s) is (are) first in the forward data object array
            // note: secondly, if there are multiple independent axes, they will be assigned in order of appearance in the method signature
            switch (type)
            {
                case FluenceSolutionDomainType.FluenceOfRhoAndZ:
                    if (fs is TwoLayerSDAForwardSolver) // future generalization to IMultiRegionForwardSolver?
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
                            throw new ArgumentOutOfRangeException(nameof(axis));
                    }
                case FluenceSolutionDomainType.FluenceOfRhoAndZAndFt:
                case FluenceSolutionDomainType.FluenceOfFxAndZAndFt:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
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
                            if (fs is TwoLayerSDAForwardSolver) // future generalization to IMultiRegionForwardSolver?
                            {
                                return (fitData, otherData) => fs.FluenceOfRhoAndZAndFt(new[] { getLayerTissueRegionArray(fitData) }, (double[])otherData[0], (double[])otherData[1], new[] { (double)otherData[2] });
                            }
                            return (fitData, otherData) => fs.FluenceOfRhoAndZAndFt(new[] { getOP(fitData) }, (double[])otherData[0], (double[])otherData[1], new[] { (double)otherData[2] });
                        case IndependentVariableAxis.Ft:
                            if (fs is TwoLayerSDAForwardSolver) 
                            {
                                return (fitData, otherData) => fs.FluenceOfRhoAndZAndFt(new[] { getLayerTissueRegionArray(fitData) },  new [] { (double)otherData[2] }, (double[])otherData[1], (double[])otherData[0]);
                            }
                            return (fitData, otherData) => fs.FluenceOfRhoAndZAndFt(new[] { getOP(fitData) }, new[] { (double)otherData[2] }, (double[])otherData[1], (double[])otherData[0]);
                        case IndependentVariableAxis.Time:
                        case IndependentVariableAxis.Fx:
                        case IndependentVariableAxis.Z:
                        case IndependentVariableAxis.Wavelength:
                        default:
                            throw new ArgumentOutOfRangeException(nameof(axis));
                    }
                case FluenceSolutionDomainType.FluenceOfFxAndZAndFt:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.FluenceOfFxAndZAndFt(new[] { getOP(fitData) }, (double[])otherData[0], (double[])otherData[1], new[] { (double)otherData[2] });
                        case IndependentVariableAxis.Ft:
                            return (fitData, otherData) => fs.FluenceOfFxAndZAndFt(new[] { getOP(fitData) }, new[] { (double)otherData[2] }, (double[])otherData[1], (double[])otherData[0]);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(axis));
                    }
                case FluenceSolutionDomainType.FluenceOfRhoAndZ:
                case FluenceSolutionDomainType.FluenceOfFxAndZ:
                case FluenceSolutionDomainType.FluenceOfRhoAndZAndTime:
                case FluenceSolutionDomainType.FluenceOfFxAndZAndTime:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        private static bool[] GetParametersToFit(InverseFitType fitType)
        {
            switch (fitType)
            {
                case InverseFitType.MuaMusp:
                default:
                    return new[] { true, true, false, false };
                case InverseFitType.Mua:
                    return new[] { true, false, false, false };
                case InverseFitType.Musp:
                    return new[] { false, true, false, false };
                case InverseFitType.MuaMuspG:
                    return new[] { true, true, true, false };
            }
        }
    }
}