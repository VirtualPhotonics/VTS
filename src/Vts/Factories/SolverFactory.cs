using System;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.Optimizers;
using Vts.SpectralMapping;
using Microsoft.Extensions.DependencyInjection;

namespace Vts.Factories
{
    /// <summary>
    /// factory methods for solvers
    /// </summary>
    public class SolverFactory
    {
        private static readonly ServiceCollection Services;
        private static readonly ServiceProvider ServiceProvider;

        /// <summary>
        /// constructor for solver factory
        /// </summary>
        static SolverFactory()
        {
            Services = new ServiceCollection();

            // use convention to map fs names (w/o "ForwardSolver") to enum types
            // e.g. ForwardSolverType.Nurbs will register to NurbsForwardSolver 
            RegisterClassesToEnumTypesByConvention<ForwardSolverType, IForwardSolver>(
                typeof(ForwardSolverBase).Namespace);

            RegisterClassesToEnumTypesByConvention<OptimizerType, IOptimizer>(
                typeof(MPFitLevenbergMarquardtOptimizer).Namespace);

            RegisterClassesToEnumTypesByConvention<ScatteringType, IScatterer>(
                typeof(IntralipidScatterer).Namespace);
            ServiceProvider = Services.BuildServiceProvider();
        }

        /// <summary>
        /// Uses convention to map classes implementing TInterface to enum types
        /// e.g. ForwardSolverType.Nurbs will register to NurbsForwardSolver 
        /// This is done for each enum type that correctly matches the interface name
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="namespaceString"></param>
        private static void RegisterClassesToEnumTypesByConvention<TEnum, TInterface>(string namespaceString)
        {
            var enumValues = EnumHelper.GetValues<TEnum>();
            foreach (var enumValue in enumValues)
            {
                var interfaceType = typeof(TInterface);
                var interfaceBasename = interfaceType.Name.Substring(1);
                var classType = Type.GetType(namespaceString + @"." + enumValue + interfaceBasename, false, true);
                if (!object.Equals(classType, null))
                {
                    Services.AddSingleton(classType);
                }
            }
        }

        /// <summary>
        /// method to get forward solver from enum type
        /// </summary>
        /// <param name="forwardSolverType">ForwardSolverType enum</param>
        /// <returns>IForwardSolver</returns>
        public static IForwardSolver GetForwardSolver(ForwardSolverType forwardSolverType)
        {
            return GetForwardSolver(forwardSolverType.ToString());
        }

        /// <summary>
        /// method to get forward solver from string name
        /// </summary>
        /// <param name="forwardSolverType">string name</param>
        /// <returns>IForwardSolver</returns>
        public static IForwardSolver GetForwardSolver(string forwardSolverType)
        {
            try
            {
                var classNameString = $"{typeof(ForwardSolverBase).Namespace}.{forwardSolverType}ForwardSolver";
                var classType = Type.GetType(classNameString);
                return (IForwardSolver)ServiceProvider.GetService(classType);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// method to get Scatter from enum
        /// </summary>
        /// <param name="scatteringType">ScatteringType enum</param>
        /// <returns>IScatter</returns>
        public static IScatterer GetScattererType(ScatteringType scatteringType)
        {
            return GetScattererType(scatteringType.ToString());
        }

        /// <summary>
        /// method to get Scatter from scattering name string
        /// </summary>
        /// <param name="scatteringType">scattering string name</param>
        /// <returns>IScatter</returns>
        public static IScatterer GetScattererType(string scatteringType)
        {
            try
            {
                // add overload of GetScattererType that takes in a tissue type 
                // for choosing good defaults. Need to understand how to configure Unity
                // to allow for both types of resolution (right now, calls default constructor)
                var classNameString = $"{typeof(IntralipidScatterer).Namespace}.{scatteringType}Scatterer";
                var classType = Type.GetType(classNameString);
                return (IScatterer)ServiceProvider.GetService(classType);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// method to get optimizer from enum
        /// </summary>
        /// <param name="type">OptimizerType enum</param>
        /// <returns>IOptimizer</returns>
        public static IOptimizer GetOptimizer(OptimizerType type)
        {
            return GetOptimizer(type.ToString());
        }

        /// <summary>
        /// method to get optimizer from name string
        /// </summary>
        /// <param name="optimizerType">optimizer name string</param>
        /// <returns>IOptimizer</returns>
        public static IOptimizer GetOptimizer(string optimizerType)
        {
            try
            {
                var classNameString = $"{typeof(MPFitLevenbergMarquardtOptimizer).Namespace}.{optimizerType}Optimizer";
                var classType = Type.GetType(classNameString);
                return (IOptimizer)ServiceProvider.GetService(classType);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
