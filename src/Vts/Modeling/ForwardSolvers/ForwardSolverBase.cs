using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        public ForwardSolverBase(SourceConfiguration sourceConfiguration, double beamDiameter)
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


        #region Dummy virtual methods - must be implemented in child classes

        public virtual double RofRho(OpticalProperties op, double rho)
        {
            throw new NotImplementedException();
        }


        public virtual double RofTheta(OpticalProperties op, double theta)
        {
            throw new NotImplementedException();
        }


        public virtual double RofRhoAndT(OpticalProperties op, double rho, double t)
        {
            throw new NotImplementedException();
        }

        public virtual Complex RofRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            throw new NotImplementedException();
        }

        public virtual double RofFx(OpticalProperties op, double fx)
        {
            throw new NotImplementedException();
        }

        public virtual double RofFxAndT(OpticalProperties op, double fx, double t)
        {
            throw new NotImplementedException();
        }

        public virtual Complex RofFxAndFt(OpticalProperties op, double fx, double ft)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Dummy default versions of the vectorized methods. Override these in child classes to take advantage of optimization strategies.

        public virtual IEnumerable<double> RofRho(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos)
        {
            return ((Func<OpticalProperties, double, double>)RofRho).LoopOverVariables(ops, rhos);
        }

        public virtual IEnumerable<double> RofTheta(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> thetas)
        {
            return ((Func<OpticalProperties, double, double>)RofTheta).LoopOverVariables(ops, thetas);
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

        #region Convenience array overloads (todo: these could alternatively be IForwardSolverExtensions instead of part of the contract)

        public double[] RofRho(OpticalProperties[] ops, double[] rhos)
        {
            var output = new double[ops.Length * rhos.Length];
            var query = RofRho((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public double[] RofTheta(OpticalProperties[] ops, double[] thetas)
        {
            var output = new double[ops.Length * thetas.Length];
            var query = RofTheta((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)thetas);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public double[] RofRhoAndT(OpticalProperties[] ops, double[] rhos, double[] ts)
        {
            var output = new double[ops.Length * rhos.Length * ts.Length];
            var query = RofRhoAndT((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public Complex[] RofRhoAndFt(OpticalProperties[] ops, double[] rhos, double[] fts)
        {
            var output = new Complex[ops.Length * rhos.Length * fts.Length];
            var query = RofRhoAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public double[] RofFx(OpticalProperties[] ops, double[] fxs)
        {
            var output = new double[ops.Length * fxs.Length];
            var query = RofFx((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public double[] RofFxAndT(OpticalProperties[] ops, double[] fxs, double[] ts)
        {
            var output = new double[ops.Length * fxs.Length * ts.Length];
            var query = RofFxAndT((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public Complex[] RofFxAndFt(OpticalProperties[] ops, double[] fxs, double[] fts)
        {
            var output = new Complex[ops.Length * fxs.Length * fts.Length];
            var query = RofFxAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        #region array overloads that simplify single parameter specification

        public double[] RofRho(OpticalProperties op, double[] rhos)
        {
            return RofRho(new[] { op }, rhos);
        }

        public double[] RofRho(OpticalProperties[] ops, double rho)
        {
            return RofRho(ops, new[] { rho });
        }

        public double[] RofTheta(OpticalProperties op, double[] thetas)
        {
            return RofTheta(new[] { op }, thetas);
        }

        public double[] RofTheta(OpticalProperties[] ops, double theta)
        {
            return RofTheta(ops, new[] { theta });
        }

        // RofRhoAndT

        public double[] RofRhoAndT(OpticalProperties op, double[] rhos, double[] ts)
        {
            return RofRhoAndT(new[] { op }, rhos, ts);
        }

        public double[] RofRhoAndT(OpticalProperties[] ops, double rho, double[] ts)
        {
            return RofRhoAndT(ops, new[] { rho }, ts);
        }

        public double[] RofRhoAndT(OpticalProperties[] ops, double[] rhos, double t)
        {
            return RofRhoAndT(ops, rhos, new[] { t });
        }

        public double[] RofRhoAndT(OpticalProperties op, double rho, double[] ts)
        {
            return RofRhoAndT(new[] { op }, new[] { rho }, ts);
        }

        public double[] RofRhoAndT(OpticalProperties op, double[] rhos, double t)
        {
            return RofRhoAndT(new[] { op }, rhos, new[] { t });
        }

        public double[] RofRhoAndT(OpticalProperties[] ops, double rho, double t)
        {
            return RofRhoAndT(ops, new[] { rho }, new[] { t });
        }

        // RofRhoAndFt

        public Complex[] RofRhoAndFt(OpticalProperties op, double[] rhos, double[] fts)
        {
            return RofRhoAndFt(new[] { op }, rhos, fts);
        }

        public Complex[] RofRhoAndFt(OpticalProperties[] ops, double rho, double[] fts)
        {
            return RofRhoAndFt(ops, new[] { rho }, fts);
        }

        public Complex[] RofRhoAndFt(OpticalProperties[] ops, double[] rhos, double ft)
        {
            return RofRhoAndFt(ops, rhos, new[] { ft });
        }

        public Complex[] RofRhoAndFt(OpticalProperties op, double rho, double[] fts)
        {
            return RofRhoAndFt(new[] { op }, new[] { rho }, fts);
        }

        public Complex[] RofRhoAndFt(OpticalProperties op, double[] rhos, double ft)
        {
            return RofRhoAndFt(new[] { op }, rhos, new[] { ft });
        }

        public Complex[] RofRhoAndFt(OpticalProperties[] ops, double rho, double ft)
        {
            return RofRhoAndFt(ops, new[] { rho }, new[] { ft });
        }

        // RofFx

        public double[] RofFx(OpticalProperties op, double[] fxs)
        {
            return RofFx(new[] { op }, fxs);
        }

        public double[] RofFx(OpticalProperties[] ops, double fx)
        {
            return RofFx(ops, new[] { fx });
        }

        // RofFxAndT

        public double[] RofFxAndT(OpticalProperties op, double[] fxs, double[] ts)
        {
            return RofFxAndT(new[] { op }, fxs, ts);
        }

        public double[] RofFxAndT(OpticalProperties[] ops, double fx, double[] ts)
        {
            return RofFxAndT(ops, new[] { fx }, ts);
        }

        public double[] RofFxAndT(OpticalProperties[] ops, double[] fxs, double t)
        {
            return RofFxAndT(ops, fxs, new[] { t });
        }

        public double[] RofFxAndT(OpticalProperties op, double fx, double[] ts)
        {
            return RofFxAndT(new[] { op }, new[] { fx }, ts);
        }

        public double[] RofFxAndT(OpticalProperties op, double[] fxs, double t)
        {
            return RofFxAndT(new[] { op }, fxs, new[] { t });
        }

        public double[] RofFxAndT(OpticalProperties[] ops, double fx, double t)
        {
            return RofFxAndT(ops, new[] { fx }, new[] { t });
        }

        // RofFxAndFt

        public Complex[] RofFxAndFt(OpticalProperties op, double[] fxs, double[] fts)
        {
            return RofFxAndFt(new[] { op }, fxs, fts);
        }

        public Complex[] RofFxAndFt(OpticalProperties[] ops, double fx, double[] fts)
        {
            return RofFxAndFt(ops, new[] { fx }, fts);
        }

        public Complex[] RofFxAndFt(OpticalProperties[] ops, double[] fxs, double ft)
        {
            return RofFxAndFt(ops, fxs, new[] { ft });
        }

        public Complex[] RofFxAndFt(OpticalProperties op, double fx, double[] fts)
        {
            return RofFxAndFt(new[] { op }, new[] { fx }, fts);
        }

        public Complex[] RofFxAndFt(OpticalProperties op, double[] fxs, double ft)
        {
            return RofFxAndFt(new[] { op }, fxs, new[] { ft });
        }

        public Complex[] RofFxAndFt(OpticalProperties[] ops, double fx, double ft)
        {
            return RofFxAndFt(ops, new[] { fx }, new[] { ft });
        }

        #endregion

        #endregion

        // The following methods are designed to return values based on iteration 
        // with the leftmost IEnumerable<T> input being the top-level, and so-on
        // such that the right-most input is at the inner-most loop
        #region Dummy default fluence versions of the vectorized methods. Override these in child classes to take advantage of optimization strategies.

        /// <summary>
        /// Overload of scalar RofRho function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> FluenceofRho(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar RofRhoAndT function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public virtual IEnumerable<double> FluenceofRhoAndT(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar RofRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public virtual IEnumerable<double> FluenceofRhoAndFt(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar RofFx function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> FluenceofFx(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar RofFxAndT function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public virtual IEnumerable<double> FluenceofFxAndT(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs,
            IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar RofFxAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> FluenceofFxAndFt(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs,
            IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Convenience array overloads for fluence methods
        
        public double[] FluenceofRho(OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            var output = new double[ops.Length * rhos.Length * zs.Length];
            var query = FluenceofRho((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public double[] FluenceofRhoAndT(OpticalProperties[] ops, double[] rhos, double[] zs, double[] ts)
        {
            var output = new double[ops.Length * rhos.Length * zs.Length * ts.Length];
            var query = FluenceofRhoAndT((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public double[] FluenceofRhoAndFt(OpticalProperties[] ops, double[] rhos, double[] zs, double[] fts)
        {
            var output = new double[ops.Length * rhos.Length * zs.Length * fts.Length];
            var query = FluenceofRhoAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public double[] FluenceofFx(OpticalProperties[] ops, double[] fxs, double[] zs)
        {
            var output = new double[ops.Length * fxs.Length * zs.Length];
            var query = FluenceofFx((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)zs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public double[] FluenceofFxAndT(OpticalProperties[] ops, double[] fxs, double[] zs, double[] ts)
        {
            var output = new double[ops.Length * fxs.Length * zs.Length * ts.Length];
            var query = FluenceofFxAndT((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)zs, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public double[] FluenceofFxAndFt(OpticalProperties[] ops, double[] fx, double[] zs, double[] fts)
        {
            var output = new double[ops.Length * fx.Length * zs.Length * fts.Length];
            var query = FluenceofFxAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fx, (IEnumerable<double>)zs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        #region array overloads that simplify single parameter specification

        // none yet implemented

        #endregion

        #endregion
    }
}
