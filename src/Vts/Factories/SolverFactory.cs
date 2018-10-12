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
            // todo: is this what AutoMapper is for?
            var enumValues = EnumHelper.GetValues<TEnum>();
            foreach (var enumValue in enumValues)
            {
                var interfaceType = typeof(TInterface);
                var interfaceBasename = interfaceType.Name.Substring(1);
                var classType = Type.GetType(namespaceString + @"." + enumValue + interfaceBasename, false, true);
                if (!object.Equals(classType, null))
                {
                    _container.RegisterType(
                     interfaceType,
                     classType,
                     enumValue.ToString(), // use the enum string to register each class
                     useSingleton ? new ContainerControlledLifetimeManager() : null,
                     useDefaultConstructor ? new InjectionMember[] { new InjectionConstructor() } : null);
                }
            }
        }

        public static IForwardSolver GetForwardSolver(ForwardSolverType forwardSolverType)
        {
            return GetForwardSolver(forwardSolverType.ToString());
        }

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

        public static IScatterer GetScattererType(ScatteringType scatteringType)
        {
            return GetScattererType(scatteringType.ToString());
        }

        public static IScatterer GetScattererType(string scatteringType)
        {
            try
            {
                // todo: add overload of GetScattererType that takes in a tissue type 
                // for choosing good defaults. Need to understand how to configure Unity
                // to allow for both types of resolution (right now, calls default constructor)
                return _container.Resolve<IScatterer>(scatteringType);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IOptimizer GetOptimizer(OptimizerType type)
        {
            return GetOptimizer(type.ToString());
        }

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
