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

        /// <summary>
        /// Constructor for the forward solver base class
        /// </summary>
        /// <param name="sourceConfiguration">source configuration</param>
        /// <param name="beamDiameter">beam diameter</param>
        public ForwardSolverBase(SourceConfiguration sourceConfiguration, double beamDiameter)
        {
            SourceConfiguration = sourceConfiguration;
            BeamDiameter = beamDiameter;
        }
        
        /// <summary>
        /// default constructor
        /// </summary>
        public ForwardSolverBase() : this(SourceConfiguration.Distributed, 0.0) { }

        /// <summary>
        /// beam diameter
        /// </summary>
        public double BeamDiameter // temp - should go in ISourceConfiguration or something
        {
            get { return _BeamDiameter; }
            set
            {
                _BeamDiameter = value;
                this.OnPropertyChanged("BeamDiameter");
            }
        }

        /// <summary>
        /// source configuration - point, distributed or gaussian
        /// </summary>
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

        /// <summary>
        /// Determines reflectance at source-detector separation rho - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <returns></returns>
        public virtual double ROfRho(OpticalProperties op, double rho)
        {
            throw new NotImplementedException();
        }


        public virtual double ROfTheta(OpticalProperties op, double theta)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines reflectance at source-detector separation rho and time t - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns></returns>
        public virtual double ROfRhoAndT(OpticalProperties op, double rho, double t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines reflectance at source-detector separation rho and modulation frequency ft - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns></returns>
        public virtual Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines reflectance at spatial frequency fx - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns></returns>
        public virtual double ROfFx(OpticalProperties op, double fx)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines reflectance at spatial frequency fx and time t - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns></returns>
        public virtual double ROfFxAndT(OpticalProperties op, double fx, double t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines reflectance at spatial frequency fx and modulation frequency ft - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns></returns>
        public virtual Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Dummy default versions of the vectorized methods. Override these in child classes to take advantage of optimization strategies.

        /// <summary>
        /// Scalar ROfRho function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> ROfRho(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos)
        {
            return ((Func<OpticalProperties, double, double>)ROfRho).LoopOverVariables(ops, rhos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ops"></param>
        /// <param name="thetas"></param>
        /// <returns></returns>
        public virtual IEnumerable<double> ROfTheta(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> thetas)
        {
            return ((Func<OpticalProperties, double, double>)ROfTheta).LoopOverVariables(ops, thetas);
        }

        /// <summary>
        /// Scalar ROfRhoAndT function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> ROfRhoAndT(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> ts)
        {
            return ((Func<OpticalProperties, double, double, double>)ROfRhoAndT).LoopOverVariables(ops, rhos, ts);
        }

        /// <summary>
        /// Scalar ROfRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public virtual IEnumerable<Complex> ROfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            return ((Func<OpticalProperties, double, double, Complex>)ROfRhoAndFt).LoopOverVariables(ops, rhos, fts);
        }

        /// <summary>
        /// Scalar ROfFx function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> ROfFx(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs)
        {
            return ((Func<OpticalProperties, double, double>)ROfFx).LoopOverVariables(ops, fxs);
        }

        /// <summary>
        /// Scalar ROfFxAndT function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> ROfFxAndT(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> ts)
        {
            return ((Func<OpticalProperties, double, double, double>)ROfFxAndT).LoopOverVariables(ops, fxs, ts);
        }

        /// <summary>
        /// Scalar ROfFxAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public virtual IEnumerable<Complex> ROfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            return ((Func<OpticalProperties, double, double, Complex>)ROfFxAndFt).LoopOverVariables(ops, fxs, fts);
        }

        #endregion

        #region Convenience array overloads (todo: these could alternatively be IForwardSolverExtensions instead of part of the contract)

        /// <summary>
        /// Convenience array overload of ROfRho. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        public double[] ROfRho(OpticalProperties[] ops, double[] rhos)
        {
            var output = new double[ops.Length * rhos.Length];
            var query = ROfRho((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        public double[] ROfTheta(OpticalProperties[] ops, double[] thetas)
        {
            var output = new double[ops.Length * thetas.Length];
            var query = ROfTheta((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)thetas);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Convenience array overload of ROfRhoAndT. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public double[] ROfRhoAndT(OpticalProperties[] ops, double[] rhos, double[] ts)
        {
            var output = new double[ops.Length * rhos.Length * ts.Length];
            var query = ROfRhoAndT((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Convenience array overload of ROfRhoAndFt. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double[] fts)
        {
            var output = new Complex[ops.Length * rhos.Length * fts.Length];
            var query = ROfRhoAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Convenience array overload of ROfFx. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <returns></returns>
        public double[] ROfFx(OpticalProperties[] ops, double[] fxs)
        {
            var output = new double[ops.Length * fxs.Length];
            var query = ROfFx((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Convenience array overload of ROfFx. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public double[] ROfFxAndT(OpticalProperties[] ops, double[] fxs, double[] ts)
        {
            var output = new double[ops.Length * fxs.Length * ts.Length];
            var query = ROfFxAndT((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Convenience array overload of ROfFxAndFt. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double[] fxs, double[] fts)
        {
            var output = new Complex[ops.Length * fxs.Length * fts.Length];
            var query = ROfFxAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        #region array overloads that simplify single parameter specification

        /// <summary>
        /// Overload of ROfRho. Determines reflectances at optical properties 'op' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        public double[] ROfRho(OpticalProperties op, double[] rhos)
        {
            return ROfRho(new[] { op }, rhos);
        }

        /// <summary>
        /// Overload of ROfRho. Determines reflectances at optical properties 'ops' and source-detector separation 'rho'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separations (mm)</param>
        /// <returns></returns>
        public double[] ROfRho(OpticalProperties[] ops, double rho)
        {
            return ROfRho(ops, new[] { rho });
        }

        public double[] ROfTheta(OpticalProperties op, double[] thetas)
        {
            return ROfTheta(new[] { op }, thetas);
        }

        public double[] ROfTheta(OpticalProperties[] ops, double theta)
        {
            return ROfTheta(ops, new[] { theta });
        }

        // ROfRhoAndT
        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'op', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public double[] ROfRhoAndT(OpticalProperties op, double[] rhos, double[] ts)
        {
            return ROfRhoAndT(new[] { op }, rhos, ts);
        }

        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'ops', source-detector separation 'rho' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public double[] ROfRhoAndT(OpticalProperties[] ops, double rho, double[] ts)
        {
            return ROfRhoAndT(ops, new[] { rho }, ts);
        }

        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns></returns>
        public double[] ROfRhoAndT(OpticalProperties[] ops, double[] rhos, double t)
        {
            return ROfRhoAndT(ops, rhos, new[] { t });
        }

        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'op', source-detector separation 'rho' and times 'ts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public double[] ROfRhoAndT(OpticalProperties op, double rho, double[] ts)
        {
            return ROfRhoAndT(new[] { op }, new[] { rho }, ts);
        }

        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'op', source-detector separations 'rhos' and time 't'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns></returns>
        public double[] ROfRhoAndT(OpticalProperties op, double[] rhos, double t)
        {
            return ROfRhoAndT(new[] { op }, rhos, new[] { t });
        }

        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'ops', source-detector separation 'rho' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns></returns>
        public double[] ROfRhoAndT(OpticalProperties[] ops, double rho, double t)
        {
            return ROfRhoAndT(ops, new[] { rho }, new[] { t });
        }

        //ROfRhoAndFt
        /// <summary>
        /// Overload of ROfRhoAndFt. Determines reflectances at optical properties 'op', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double[] fts)
        {
            return ROfRhoAndFt(new[] { op }, rhos, fts);
        }

        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'ops', source-detector separation 'rho' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double[] fts)
        {
            return ROfRhoAndFt(ops, new[] { rho }, fts);
        }

        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequency 'ft'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double ft)
        {
            return ROfRhoAndFt(ops, rhos, new[] { ft });
        }

        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'op', source-detector separation 'rho' and time frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfRhoAndFt(OpticalProperties op, double rho, double[] fts)
        {
            return ROfRhoAndFt(new[] { op }, new[] { rho }, fts);
        }

        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'op', source-detector separations 'rhos' and time frequency 'ft'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double ft)
        {
            return ROfRhoAndFt(new[] { op }, rhos, new[] { ft });
        }

        /// <summary>
        /// Overload of ROfRhoAndT. Determines reflectances at optical properties 'ops', source-detector separation 'rho' and time frequency 'ft'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double ft)
        {
            return ROfRhoAndFt(ops, new[] { rho }, new[] { ft });
        }

        // ROfFx
        /// <summary>
        /// Overload of ROfFx. Determines reflectances at optical properties 'op' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        public double[] ROfFx(OpticalProperties op, double[] fxs)
        {
            return ROfFx(new[] { op }, fxs);
        }

        /// <summary>
        /// Overload of ROfFx. Determines reflectances at optical properties 'ops' and spatial frequency 'fx'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns></returns>
        public double[] ROfFx(OpticalProperties[] ops, double fx)
        {
            return ROfFx(ops, new[] { fx });
        }

        // ROfFxAndT
        /// <summary>
        /// Overload of ROfFxAndT. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="op">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts">times (ns)</param>
        public double[] ROfFxAndT(OpticalProperties op, double[] fxs, double[] ts)
        {
            return ROfFxAndT(new[] { op }, fxs, ts);
        }

        /// <summary>
        /// Overload of ROfFxAndT. Determines reflectances at optical properties 'ops', spatial frequency 'fx' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public double[] ROfFxAndT(OpticalProperties[] ops, double fx, double[] ts)
        {
            return ROfFxAndT(ops, new[] { fx }, ts);
        }

        /// <summary>
        /// Overload of ROfFxAndT. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns></returns>
        public double[] ROfFxAndT(OpticalProperties[] ops, double[] fxs, double t)
        {
            return ROfFxAndT(ops, fxs, new[] { t });
        }

        /// <summary>
        /// Overload of ROfFxAndT. Determines reflectances at optical properties 'op', spatial frequency 'fx' and times 'ts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public double[] ROfFxAndT(OpticalProperties op, double fx, double[] ts)
        {
            return ROfFxAndT(new[] { op }, new[] { fx }, ts);
        }

        /// <summary>
        /// Overload of ROfFxAndT. Determines reflectances at optical properties 'op', spatial frequencies 'fxs' and time 't'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns></returns>
        public double[] ROfFxAndT(OpticalProperties op, double[] fxs, double t)
        {
            return ROfFxAndT(new[] { op }, fxs, new[] { t });
        }

        /// <summary>
        /// Overload of ROfFxAndT. Determines reflectances at optical properties 'ops', spatial frequency 'fx' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns></returns>
        public double[] ROfFxAndT(OpticalProperties[] ops, double fx, double t)
        {
            return ROfFxAndT(ops, new[] { fx }, new[] { t });
        }

        // ROfFxAndFt
        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectances at optical properties 'op', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfFxAndFt(OpticalProperties op, double[] fxs, double[] fts)
        {
            return ROfFxAndFt(new[] { op }, fxs, fts);
        }

        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectances at optical properties 'ops', spatial frequency 'fx' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double[] fts)
        {
            return ROfFxAndFt(ops, new[] { fx }, fts);
        }

        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequency 'ft'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double[] fxs, double ft)
        {
            return ROfFxAndFt(ops, fxs, new[] { ft });
        }

        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectances at optical properties 'op', spatial frequency 'fx' and time frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfFxAndFt(OpticalProperties op, double fx, double[] fts)
        {
            return ROfFxAndFt(new[] { op }, new[] { fx }, fts);
        }

        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectances at optical properties 'op', spatial frequencies 'fxs' and time frequency 'ft'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfFxAndFt(OpticalProperties op, double[] fxs, double ft)
        {
            return ROfFxAndFt(new[] { op }, fxs, new[] { ft });
        }

        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectances at optical properties 'ops', spatial frequency 'fx' and time frequency 'ft'
        /// </summary>
        /// <param name="ops">medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns></returns>
        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double ft)
        {
            return ROfFxAndFt(ops, new[] { fx }, new[] { ft });
        }

        #endregion

        #endregion

        // The following methods are designed to return values based on iteration 
        // with the leftmost IEnumerable<T> input being the top-level, and so-on
        // such that the right-most input is at the inner-most loop
        #region Dummy default fluence versions of the vectorized methods. Override these in child classes to take advantage of optimization strategies.

        /// <summary>
        /// Overload of scalar FluenceOfRhoAndZ function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos' and 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> FluenceOfRhoAndZ(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfRhoAndT function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos', 'zs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public virtual IEnumerable<double> FluenceOfRhoAndT(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos', 'zs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public virtual IEnumerable<double> FluenceOfRhoAndFt(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfFx function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs' and 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> FluenceOfFx(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfFxAndT function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs', 'zs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> FluenceOfFxAndT(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs,
            IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfFxAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public virtual IEnumerable<double> FluenceOfFxAndFt(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs,
            IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Convenience array overloads for fluence methods
        
        /// <summary>
        /// Overload of FluenceOfRhoAndZ function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns></returns>
        public double[] FluenceOfRhoAndZ(OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            var output = new double[ops.Length * rhos.Length * zs.Length];
            var query = FluenceOfRhoAndZ((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfRhoAndT function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos', 'zs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public double[] FluenceOfRhoAndT(OpticalProperties[] ops, double[] rhos, double[] zs, double[] ts)
        {
            var output = new double[ops.Length * rhos.Length * zs.Length * ts.Length];
            var query = FluenceOfRhoAndT((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos', 'zs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public double[] FluenceOfRhoAndFt(OpticalProperties[] ops, double[] rhos, double[] zs, double[] fts)
        {
            var output = new double[ops.Length * rhos.Length * zs.Length * fts.Length];
            var query = FluenceOfRhoAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfFx function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns></returns>
        public double[] FluenceOfFx(OpticalProperties[] ops, double[] fxs, double[] zs)
        {
            var output = new double[ops.Length * fxs.Length * zs.Length];
            var query = FluenceOfFx((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)zs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfFxAndT function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs', 'zs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        public double[] FluenceOfFxAndT(OpticalProperties[] ops, double[] fxs, double[] zs, double[] ts)
        {
            var output = new double[ops.Length * fxs.Length * zs.Length * ts.Length];
            var query = FluenceOfFxAndT((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)zs, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfFxAndT function. Determines reflectances at optical properties 'ops', spatial frequency 'fx', 'zs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        public double[] FluenceOfFxAndFt(OpticalProperties[] ops, double[] fx, double[] zs, double[] fts)
        {
            var output = new double[ops.Length * fx.Length * zs.Length * fts.Length];
            var query = FluenceOfFxAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fx, (IEnumerable<double>)zs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        #region array overloads that simplify single parameter specification

        // none yet implemented

        #endregion

        #endregion
    }
}
