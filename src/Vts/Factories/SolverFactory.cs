using System.Linq;

using Vts.Extensions;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.Optimizers;
using Vts.SpectralMapping;

using Microsoft.Practices.Unity;
using System;

namespace Vts.Factories
{
    public class SolverFactory
    {
        private static readonly UnityContainer _container;

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
                typeof (IntralipidScatterer).Namespace, true, true);
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
            // todo: is this what AutoMapper is for?
            var enumValues = EnumHelper.GetValues<TEnum>();
            foreach (var enumValue in enumValues)
            {
                var interfaceType = typeof (TInterface);
                var interfaceBasename = interfaceType.Name.Substring(1);
                var classType = Type.GetType(namespaceString + @"." + enumValue + interfaceBasename, false, true);
                _container.RegisterType(
                     interfaceType,
                     classType,
                     enumValue.ToString(), // use the enum string to register each class
                     useSingleton ? new ContainerControlledLifetimeManager() : null,
                     useDefaultConstructor ? new InjectionMember[] { new InjectionConstructor() } : null);
            }
        }

        public static IForwardSolver GetForwardSolver(ForwardSolverType type)
        {
            try
            {
                return _container.Resolve<IForwardSolver>(type.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IScatterer GetScattererType(ScatteringType scatteringType)
        {
            try
            {
                return _container.Resolve<IScatterer>(scatteringType.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IOptimizer GetOptimizer(OptimizerType type)
        {
            try
            {
                return _container.Resolve<IOptimizer>(type.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
