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

            // todo: consolidate the repetition below - base convention on interface type?

            // use convention to map fs names (w/o "ForwardSolver") to enum types
            // e.g. ForwardSolverType.Nurbs will register to NurbsForwardSolver 
            var forwardSolverTypes = EnumHelper.GetValues<ForwardSolverType>();
            var fsNamespace = typeof(ForwardSolverBase).Namespace;
            foreach (var forwardSolverType in forwardSolverTypes)
            {
                var type = Type.GetType(fsNamespace + @"." + forwardSolverType + "ForwardSolver", false, true);
                _container.RegisterType(
                    typeof(IForwardSolver),
                    type,
                    forwardSolverType.ToString(), // use the type string to register 
                    new ContainerControlledLifetimeManager(),
                    new InjectionMember[] { new InjectionConstructor() });
            }

            var optimizerTypes = EnumHelper.GetValues<OptimizerType>();
            var oNamespace = typeof (MPFitLevenbergMarquardtOptimizer).Namespace;
            foreach (var optimizerType in optimizerTypes)
            {
                var type = Type.GetType(oNamespace + @"." + optimizerType + "Optimizer", false, true);
                _container.RegisterType(
                     typeof(IOptimizer),
                     type,
                     optimizerType.ToString(), // use the type string to register 
                     new ContainerControlledLifetimeManager(),
                     new InjectionMember[] { new InjectionConstructor() });
            }

            var scatteringTypes = EnumHelper.GetValues<ScatteringType>();
            var sNamespace = typeof(IntralipidScatterer).Namespace;
            foreach (var scatteringType in scatteringTypes)
            {
                var type = Type.GetType(sNamespace + @"." + scatteringType + "Scatterer", false, true);
                _container.RegisterType(
                     typeof(IScatterer),
                     type,
                     scatteringType.ToString(), // use the type string to register 
                     new ContainerControlledLifetimeManager(),
                     new InjectionMember[] { new InjectionConstructor() });
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

            //switch (scatteringType)
            //{
            //    case ScatteringType.PowerLaw:
            //        return new PowerLawScatterer();
            //    case ScatteringType.Mie:
            //        return new MieScatterer();
            //    case ScatteringType.Intralipid:
            //        return new IntralipidScatterer();
            //    default:
            //        throw new ArgumentOutOfRangeException("scatteringType");
            //}
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

            //switch (type)
            //{
            //    default:
            //    case OptimizerType.MPFitLevenbergMarquardt:
            //        return new MPFitLevenbergMarquardtOptimizer();
            //}
        }
    }
}
