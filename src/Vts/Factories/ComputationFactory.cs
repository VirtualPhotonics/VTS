using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Extensions;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers.Extensions;
#if DESKTOP
using System.Runtime.InteropServices;
using Vts.SpectralMapping;

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
        public static bool IsComplexSolver(IndependentVariableAxis independentVariableAxis)
        {
            return (independentVariableAxis == IndependentVariableAxis.Ft);
        }

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
        public static double[] ComputeReflectance(
             string forwardSolverType,
             string solutionDomainType,
             string forwardAnalysisType,
             string independentVariableAxisType,
             double[] independentValues,
             double[] opticalProperties,
             double[] constantValues)
        {
            var opLength = opticalProperties.Length/4;
            var opClassArray = Enumerable.Range(0,opLength)
                .Select(i => new OpticalProperties(
                    opticalProperties[i*4], 
                    opticalProperties[i*4+1], 
                    opticalProperties[i*4+2], 
                    opticalProperties[i*4+3]))
                .ToArray();

            return ComputeReflectance(
                (ForwardSolverType)Enum.Parse(typeof(ForwardSolverType), forwardSolverType, true),
                (SolutionDomainType)Enum.Parse(typeof(SolutionDomainType), solutionDomainType, true),
                (ForwardAnalysisType)Enum.Parse(typeof(ForwardAnalysisType), forwardAnalysisType, true),
                (IndependentVariableAxis)Enum.Parse(typeof(IndependentVariableAxis), independentVariableAxisType, true),
                independentValues,
                opClassArray,
                constantValues).ToArray();
        }

        public static double[] ComputeReflectance(
            ForwardSolverType forwardSolverType,
            SolutionDomainType solutionDomainType,
            ForwardAnalysisType forwardAnalysisType,
            IndependentVariableAxis independentAxisType,
            double[] independentValues,
            OpticalProperties[] opticalProperties,
            params double[] constantValues)
        {
            // use factory method on each call, as opposed to injecting an instance from the outside
            // -- still time-efficient if singletons are used
            // -- potentially memory-inefficient if the user creates lots of large solver instances
            return ComputeReflectance(
                SolverFactory.GetForwardSolver(forwardSolverType), 
                solutionDomainType,
                forwardAnalysisType,
                independentAxisType,
                independentValues,
                opticalProperties,
                constantValues);
        }

        // overload that calls the above method with just one set of optical properties
        public static double[] ComputeReflectance(
            ForwardSolverType forwardSolverType,
            SolutionDomainType solutionDomainType,
            ForwardAnalysisType forwardAnalysisType,
            IndependentVariableAxis independentAxisType,
            double[] independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            return ComputeReflectance(
                forwardSolverType,
                solutionDomainType,
                forwardAnalysisType,
                independentAxisType,
                independentValues,
                new [] { opticalProperties },
                constantValues);
        }

        public static double[] ComputeReflectance(
            IForwardSolver forwardSolver,
            SolutionDomainType solutionDomainType,
            ForwardAnalysisType forwardAnalysisType,
            IndependentVariableAxis independentAxisType,
            double[] independentValues,
            OpticalProperties[] opticalProperties,
            params double[] constantValues)
        {
            Func<double[], object[], double[]> func = GetForwardReflectanceFunc(forwardSolver, solutionDomainType, independentAxisType);

            // create a list of inputs (besides optical properties) that corresponds to the behavior of the function above
            List<object> inputValues = new List<object>();
            inputValues.Add(independentValues.ToArray());
            constantValues.ForEach(cv => inputValues.Add(cv));

            if (opticalProperties.Length == 1) // optimization that skips duplicate arrays if we're not multiplexing over optical properties (e.g. wavelength)
            {
                var op = opticalProperties[0];
                var parameters = new[] { op.Mua, op.Musp, op.G, op.N };
                return forwardAnalysisType == ForwardAnalysisType.R
                    ? func(parameters, inputValues.ToArray())
                    : func.GetDerivativeFunc(forwardAnalysisType)(parameters, inputValues.ToArray());
            }

            var numOp = opticalProperties.Length;
            var numIv = independentValues.Length;
            var reflectance = new double[numOp * numIv];
            for (int opi = 0; opi < numOp; opi++) // todo: parallelize
            {
                var op = opticalProperties[opi];
                var parameters = new[] { op.Mua, op.Musp, op.G, op.N };
                var tempValues = forwardAnalysisType == ForwardAnalysisType.R
                    ? func(parameters, inputValues.ToArray())
                    : func.GetDerivativeFunc(forwardAnalysisType)(parameters, inputValues.ToArray());

                for (int ivi = 0; ivi < numIv; ivi++)
                {
                    reflectance[opi * numIv + ivi] = tempValues[ivi];
                }
            }
            return reflectance;
        }

        // overload that calls the above method with just one set of optical properties
        public static double[] ComputeReflectance(
            IForwardSolver forwardSolver,
            SolutionDomainType solutionDomainType,
            ForwardAnalysisType forwardAnalysisType,
            IndependentVariableAxis independentAxisType,
            double[] independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            return ComputeReflectance(
                forwardSolver,
                solutionDomainType,
                forwardAnalysisType,
                independentAxisType,
                independentValues,
                new[] { opticalProperties },
                constantValues);
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
                new [] { opticalProperties },
                constantValues);
        }

        public static double[] ComputeFluence(
            IForwardSolver forwardSolver,
            FluenceSolutionDomainType solutionDomainType, // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            OpticalProperties[] opticalProperties,
            params double[] constantValues)
        {
            // todo: current assumption below is that the second axis is z. need to generalize
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
                new []{ opticalProperties },
                constantValues);
        }

        public static Complex[] ComputeFluenceComplex(
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
            return ComputeFluenceComplex(
                SolverFactory.GetForwardSolver(forwardSolverType),
                solutionDomainType,
                independentAxesTypes,
                independentValues,
                opticalProperties,
                constantValues);
        }

        public static Complex[] ComputeFluenceComplex(
            IForwardSolver forwardSolver,
            FluenceSolutionDomainType solutionDomainType, // keeping us from uniting the above. needs to be a single SolutionDomainType enum
            IndependentVariableAxis[] independentAxesTypes,
            double[][] independentValues,
            OpticalProperties opticalProperties,
            params double[] constantValues)
        {
            var parameters = new double[4] { opticalProperties.Mua, opticalProperties.Musp, opticalProperties.G, opticalProperties.N };

            // todo: current assumption below is that the second axis is z. need to generalize
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
        public static double[] GetPHD(ForwardSolverType forwardSolverType, double[] fluence, double sdSeparation, OpticalProperties[] ops, double[] rhos, double[] zs)
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
        public static double[] GetPHD(IForwardSolver forwardSolver, double[] fluence, double sdSeparation, OpticalProperties[] ops, double[] rhos, double[] zs)
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
        public static double[] GetPHD(ForwardSolverType forwardSolverType, Complex[] fluence, double sdSeparation, double timeModulationFrequency, OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            return GetPHD(SolverFactory.GetForwardSolver(forwardSolverType), fluence, sdSeparation, timeModulationFrequency, ops, rhos, zs);
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
        public static double[] GetPHD(IForwardSolver forwardSolver, Complex[] fluence, double sdSeparation, double modulationFrequency, OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            var rhoPrimes =
                from r in rhos
                select r - sdSeparation;

            var greensFunction = forwardSolver.TimeFrequencyDomainFluence2SurfacePointPHD(modulationFrequency, ops, rhoPrimes, zs);

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

        public static double[] SolveInverse(
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
            return SolveInverse(
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

        public static double[] SolveInverse(
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
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                        case IndependentVariableAxis.Wavelength:
                            return (fitData, otherData) => fs.ROfRho(getOP(fitData), (double[])otherData[0]);
                        //case IndependentVariableAxis.Wavelength:
                        //    return (fitData, otherData) =>
                        //    {
                        //        var wv = (double[])otherData[0];
                        //        var rho = (double)otherData[1];
                        //        var tissue = (Tissue)otherData[2];
                        //        var numAbsorbers = tissue.Absorbers.Count;
                        //        tissue.Absorbers.ForEach((absorber, i) => absorber.Concentration = fitData[i]);
                        //        var A = fitData[numAbsorbers + 0];
                        //        var b = fitData[numAbsorbers + 1];
                        //        tissue.Scatterer = new PowerLawScatterer(A, b);
                        //        var ops = wv.Select(tissue.GetOpticalProperties).ToArray();
                        //        return ops.Select(opi => fs.ROfRho(opi, rho)).ToArray();
                        //    };
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case SolutionDomainType.ROfFx:
                    return (fitData, otherData) => fs.ROfFx(getOP(fitData), (double[])otherData[0]);
                case SolutionDomainType.ROfRhoAndTime:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.ROfRhoAndTime(getOP(fitData), (double[])otherData[0], (double)otherData[1]);
                        case IndependentVariableAxis.Time:
                            return (fitData, otherData) => fs.ROfRhoAndTime(getOP(fitData), (double)otherData[1], (double[])otherData[0]);
                        case IndependentVariableAxis.Wavelength:
                            //return (chromPlusMusp, constantData) =>
                            //           {
                            //               var wv = (double[]) constantData[0];
                            //               var tissue = (Tissue) constantData[1];
                            //               int i = 0;
                            //               tissue.Absorbers.ForEach(abs => abs.Concentration = chromPlusMusp[i++]);
                            //               tissue.Scatterer = new PowerLawScatterer(chromPlusMusp[i], chromPlusMusp[i + 1]);
                            //               var muas = wv.Select(w => tissue.GetMua(w)); 
                            //               var musps = wv.Select(w => tissue.GetMusp(w));
                            //               return EnumerableExtensions.Zip(muas,musps,(mua,musp)=>fs.ROfRhoAndTime())...
                            //           }; 
                            //return op => fs.ROfRhoAndTime(op, ((double)constantValues[0]).AsEnumerable(), ((double)constantValues[1]).AsEnumerable());
                        default:
                            throw new ArgumentOutOfRangeException("axis");
                    }
                case SolutionDomainType.ROfFxAndTime:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Fx:
                            return (fitData, otherData) => fs.ROfFxAndTime(getOP(fitData), (double[])otherData[0], (double)otherData[1]);
                        case IndependentVariableAxis.Time:
                            return (fitData, otherData) => fs.ROfFxAndTime(getOP(fitData), (double)otherData[1], (double[])otherData[0]);
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
                case FluenceSolutionDomainType.FluenceOfRhoAndZ:
                    return (fitData, otherData) => fs.FluenceOfRhoAndZ(new[]{ getOP(fitData) }, (double[])otherData[0], (double[])otherData[1]);
                case FluenceSolutionDomainType.FluenceOfFxAndZ:
                    return (fitData, otherData) => fs.FluenceOfFxAndZ(new[]{ getOP(fitData) }, (double[])otherData[0], (double[])otherData[1]);
                case FluenceSolutionDomainType.FluenceOfRhoAndZAndTime:
                    switch (axis)
                    {
                        case IndependentVariableAxis.Rho:
                            return (fitData, otherData) => fs.FluenceOfRhoAndZAndTime(new[]{getOP(fitData)}, (double[])otherData[0], (double[])otherData[1], new[]{(double)otherData[2]});
                        case IndependentVariableAxis.Time:
                            return (fitData, otherData) => fs.FluenceOfRhoAndZAndTime(new[]{getOP(fitData)}, new[]{(double)otherData[2]}, (double[])otherData[1], (double[])otherData[0]);
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
                            return (fitData, otherData) => fs.FluenceOfFxAndZAndTime(new[]{getOP(fitData)}, (double[])otherData[0], (double[])otherData[1], new[]{(double)otherData[2]});
                        case IndependentVariableAxis.Time:
                            return (fitData, otherData) => fs.FluenceOfFxAndZAndTime(new[]{getOP(fitData)}, new[]{(double)otherData[2]}, (double[])otherData[1], (double[])otherData[0]);
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