using System;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.Optimizers;
using Vts.SpectralMapping;
using Microsoft.Extensions.DependencyInjection;

namespace Vts.Factories
{
    /// <summary>
    /// Factory methods for solvers
    /// </summary>
    public class SolverFactory
    {
        private static readonly ServiceCollection Services;
        private static readonly ServiceProvider ServiceProvider;

        /// <summary>
        /// Constructor for solver factory
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
        /// Uses convention to map classes implementing TInterface to Enum types
        /// e.g. ForwardSolverType.Nurbs will register to NurbsForwardSolver 
        /// This is done for each Enum type that correctly matches the interface name
        /// </summary>
        /// <typeparam name="TEnum">The Enum type</typeparam>
        /// <typeparam name="TInterface">The interface type</typeparam>
        /// <param name="namespaceString">The namespace</param>
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
        /// Method to get forward solver from enum type
        /// </summary>
        /// <param name="forwardSolverType">The ForwardSolverType enum</param>
        /// <returns>An IForwardSolver for the specified enum or null if it does not exist</returns>
        public static IForwardSolver GetForwardSolver(ForwardSolverType forwardSolverType)
        {
            return GetForwardSolver(forwardSolverType.ToString());
        }

        /// <summary>
        /// method to get forward solver from string name
        /// </summary>
        /// <param name="forwardSolverType">The forward solver name</param>
        /// <returns>An IForwardSolver for the specified name or null if it does not exist</returns>
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
        /// <param name="scatteringType">The ScatteringType enum</param>
        /// <returns>An IScatter for the specified enum or null if it does not exist</returns>
        public static IScatterer GetScattererType(ScatteringType scatteringType)
        {
            return GetScattererType(scatteringType.ToString());
        }

        /// <summary>
        /// method to get Scatter from scattering name string
        /// </summary>
        /// <param name="scatteringType">The scattering type string name</param>
        /// <returns>An IScatter for the specified name or null if it does not exist</returns>
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
        /// <param name="type">The OptimizerType enum</param>
        /// <returns>An IOptimizer for the specified enum or null if it does not exist</returns>
        public static IOptimizer GetOptimizer(OptimizerType type)
        {
            return GetOptimizer(type.ToString());
        }

        /// <summary>
        /// method to get optimizer from name string
        /// </summary>
        /// <param name="optimizerType">The optimizer name string</param>
        /// <returns>An IOptimizer for the specified name or null if it does not exist</returns>
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
