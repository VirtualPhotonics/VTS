using System;
using System.Collections.Generic;
using MathNet.Numerics;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// This is a base class for all forward solvers. It contains default (virtual) vectorization methods such that only the scalar solver methods 
    /// must be implemented to create a new IForwardSolver-implementing class. Override these virtual methods to impose optimizations possible through vectorization.
    /// </summary>
    public abstract class ForwardSolverBase : BindableObject, IForwardSolver
    {
        private SourceConfiguration _SourceConfiguration;
        private double _BeamDiameter;

        public ForwardSolverBase( SourceConfiguration sourceConfiguration, double beamDiameter)
        {
            SourceConfiguration = sourceConfiguration;
            BeamDiameter = beamDiameter;
        }

        public ForwardSolverBase() : this(SourceConfiguration.Distributed, 0.0) { }

        public double BeamDiameter // temp - should go in ISourceConfiguration or something
        {
            get { return _BeamDiameter; }
            set
            {
                _BeamDiameter = value;
                this.OnPropertyChanged("BeamDiameter");
            }
        }

        public SourceConfiguration SourceConfiguration
        {
            get { return _SourceConfiguration; }
            set
            {
                _SourceConfiguration = value;
                this.OnPropertyChanged("SourceConfiguration");
            }
        }

        #region IForwardSolver Members

        #region Abstract methods - must be implemented in child classes

        public abstract double RofRho(OpticalProperties op, double rho);
        public abstract double RofRhoAndT(OpticalProperties op, double rho, double t);
        public abstract Complex RofRhoAndFt(OpticalProperties op, double rho, double ft);
        public abstract double RofFx(OpticalProperties op, double fx);
        public abstract double RofFxAndT(OpticalProperties op, double fx, double t);
        public abstract Complex RofFxAndFt(OpticalProperties op, double fx, double ft);

        #endregion 

        #region Dummy default versions of the vectorized methods. Override these in child classes to take advantage of optimization strategies.

        public virtual IEnumerable<double> RofRho(
            IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> rhos)
        {
            return ((Func<OpticalProperties, double, double>)RofRho).LoopOverVariables(ops, rhos);
        }

        public virtual IEnumerable<double> RofRhoAndT(
            IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> rhos,
            IEnumerable<double> ts)
        {
            return ((Func<OpticalProperties, double, double, double>)RofRhoAndT).LoopOverVariables(ops, rhos, ts);
        }

        public virtual IEnumerable<Complex> RofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            return ((Func<OpticalProperties, double, double, Complex>)RofRhoAndFt).LoopOverVariables(ops, rhos, fts);
        }

        public virtual IEnumerable<double> RofFx(
            IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> fxs)
        {
            return ((Func<OpticalProperties, double, double>)RofFx).LoopOverVariables(ops, fxs);
        }

        public virtual IEnumerable<double> RofFxAndT(
            IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> fxs, 
            IEnumerable<double> ts)
        {
            return ((Func<OpticalProperties, double, double, double>)RofFxAndT).LoopOverVariables(ops, fxs, ts);
        }

        public virtual IEnumerable<Complex> RofFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            return ((Func<OpticalProperties, double, double, Complex>)RofFxAndFt).LoopOverVariables(ops, fxs, fts);
        }

        #endregion

        // The following methods are designed to return values based on iteration 
        // with the leftmost IEnumerable<T> input being the top-level, and so-on
        // such that the right-most input is at the inner-most loop
        #region IEnumerable<T> Overloads for multi-valued inputs

        /// <summary>
        /// Overload of scalar RofRho function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        public abstract IEnumerable<double> FluenceofRho(
            IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> rhos, 
            IEnumerable<double> zs);

        /// <summary>
        /// Overload of scalar RofRhoAndT function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public abstract IEnumerable<double> FluenceofRhoAndT(
            IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> rhos, 
            IEnumerable<double> zs, 
            IEnumerable<double> ts);

        /// <summary>
        /// Overload of scalar RofRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public abstract IEnumerable<double> FluenceofRhoAndFt(
            IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> rhos, 
            IEnumerable<double> zs, 
            IEnumerable<double> fts);

        /// <summary>
        /// Overload of scalar RofFx function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        public abstract IEnumerable<double> FluenceofFx(
            IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> fxs, 
            IEnumerable<double> zs);

        /// <summary>
        /// Overload of scalar RofFxAndT function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public abstract IEnumerable<double> FluenceofFxAndT(
            IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> fxs, 
            IEnumerable<double> zs, 
            IEnumerable<double> ts);

        /// <summary>
        /// Overload of scalar RofFxAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public abstract IEnumerable<double> FluenceofFxAndFt(
            IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> fxs, 
            IEnumerable<double> zs, 
            IEnumerable<double> fts);

        #endregion

        #endregion
    }
}
