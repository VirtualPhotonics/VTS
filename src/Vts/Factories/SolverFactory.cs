using System;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.Optimizers;
using Vts.SpectralMapping;

namespace Vts.Factories
{
    /// <summary>
    /// factory methods for solvers
    /// </summary>
    public class SolverFactory
    {
        private static readonly UnityContainer _container;

        /// <summary>
        /// constructor for solver factory
        /// </summary>
        static SolverFactory()
        {
            _container = new UnityContainer();

            // use convention to map fs names (w/o "ForwardSolver") to enum types
            // e.g. ForwardSolverType.Nurbs will register to NurbsForwardSolver 
            RegisterClassesToEnumTypesByConvention<ForwardSolverType, IForwardSolver>(
                typeof(ForwardSolverBase).Namespace, true, true);

            RegisterClassesToEnumTypesByConvention<OptimizerType, IOptimizer>(
                typeof(MPFitLevenbergMarquardtOptimizer).Namespace, true, true);

            RegisterClassesToEnumTypesByConvention<ScatteringType, IScatterer>(
                typeof(IntralipidScatterer).Namespace, false, true);
        }

        /// <summary>
        /// Uses convention to map classes implementing TInterface to enum types
        /// e.g. ForwardSolverType.Nurbs will register to NurbsForwardSolver 
        /// This is done for each enum type that correctly matches the interface name
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="namespaceString"></param>
        /// <param name="useSingleton"></param>
        /// <param name="useDefaultConstructor"></param>
        private static void RegisterClassesToEnumTypesByConvention<TEnum, TInterface>(
            string namespaceString,
            bool useSingleton,
            bool useDefaultConstructor)
        {
            // is this what AutoMapper is for?
            var enumValues = EnumHelper.GetValues<TEnum>();
            foreach (var enumValue in enumValues)
            {
                var interfaceType = typeof(TInterface);
                var interfaceBasename = interfaceType.Name.Substring(1);
                var classType = Type.GetType(namespaceString + @"." + enumValue + interfaceBasename, false, true);
                if (!object.Equals(classType, null))
                {
                    if (useSingleton)
                    {
                        _container.RegisterSingleton(
                            interfaceType,
                            classType,
                            enumValue.ToString(),
                            useDefaultConstructor ? new InjectionMember[] { new InjectionConstructor() } : null);
                    }
                    else
                    {
                        _container.RegisterType(
                            interfaceType,
                            classType,
                            enumValue.ToString(),
                            useDefaultConstructor ? new InjectionMember[] { new InjectionConstructor() } : null);
                    }

                    //_container.RegisterType(
                    // interfaceType,
                    // classType,
                    // enumValue.ToString(), // use the enum string to register each class
                    // useSingleton ? new ContainerControlledLifetimeManager() : null,
                    // useDefaultConstructor ? new InjectionMember[] { new InjectionConstructor() } : null)
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
                return _container.Resolve<IForwardSolver>(forwardSolverType);
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
                return _container.Resolve<IScatterer>(scatteringType);
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
        /// <param name="type">optimizer name string</param>
        /// <returns>IOptimizer</returns>
        public static IOptimizer GetOptimizer(string type)
        {
            try
            {
                return _container.Resolve<IOptimizer>(type);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
