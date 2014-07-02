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
        /// Scalar ROfRho function.  Determines reflectance at source-detector separation rho - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <returns>reflectance at given single set of optical properties and single rho</returns>     
        public virtual double ROfRho(OpticalProperties op, double rho)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Scalar ROfTheta function.  Determines reflectance at polar angle theta
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="theta">polar angle of reflected photon</param>
        /// <returns>reflectance at given single set of optical properties and single polar angle theta</returns>
        public virtual double ROfTheta(OpticalProperties op, double theta)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Scalar ROfRhoAndTime function.  Determines reflectance at source-detector separation rho and time t - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at given single set of optical properties, single rho and single time</returns>
        public virtual double ROfRhoAndTime(OpticalProperties op, double rho, double t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Scalar ROfRhoAndFt function.  Determines reflectance at source-detector separation rho and modulation frequency ft - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at given single set of optical properties, single rho and single modulation frequency ft</returns>
        public virtual Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Scalar ROfFx function.  Determines reflectance at spatial frequency fx - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>reflectance at given single set of optical properties and single spatial frequency fx</returns>
        public virtual double ROfFx(OpticalProperties op, double fx)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Scalar ROfFxAndTime function.  Determines reflectance at spatial frequency fx and time t - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at given single set of optical properties, single spatial frequency fx and single time t</returns>
        public virtual double ROfFxAndTime(OpticalProperties op, double fx, double t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines reflectance at spatial frequency fx and modulation frequency ft - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at given single set of optical properties, single spatial frequency fx and single modulation frequency ft</returns>
        public virtual Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Dummy default versions of the vectorized methods. Override these in child classes to take advantage of optimization strategies.

        /// <summary>
        /// Vector ROfRho function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>reflectance at given optical properties and rhos</returns>
        public virtual IEnumerable<double> ROfRho(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos)
        {
            return ((Func<OpticalProperties, double, double>)ROfRho).LoopOverVariables(ops, rhos);
        }

        public virtual IEnumerable<double> ROfTheta(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> thetas)
        {
            return ((Func<OpticalProperties, double, double>)ROfTheta).LoopOverVariables(ops, thetas);
        }

        /// <summary>
        /// Vector ROfRhoAndTime function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public virtual IEnumerable<double> ROfRhoAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> ts)
        {
            return ((Func<OpticalProperties, double, double, double>)ROfRhoAndTime).LoopOverVariables(ops, rhos, ts);
        }

        /// <summary>
        /// Vector ROfRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given optical properties, rhos and modulation frequencies</returns>
        public virtual IEnumerable<Complex> ROfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            return ((Func<OpticalProperties, double, double, Complex>)ROfRhoAndFt).LoopOverVariables(ops, rhos, fts);
        }

        /// <summary>
        /// Vector ROfFx function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns>reflectance at given optical properties and spatial frequencies</returns>
        public virtual IEnumerable<double> ROfFx(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs)
        {
            return ((Func<OpticalProperties, double, double>)ROfFx).LoopOverVariables(ops, fxs);
        }

        /// <summary>
        /// Vector ROfFxAndTime function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and times</returns>
        public virtual IEnumerable<double> ROfFxAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> ts)
        {
            return ((Func<OpticalProperties, double, double, double>)ROfFxAndTime).LoopOverVariables(ops, fxs, ts);
        }

        /// <summary>
        /// Vector ROfFxAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and modulation frequencies</returns>
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
        /// <returns>Reflectance at given optical properties and rhos</returns>
        public double[] ROfRho(OpticalProperties[] ops, double[] rhos)
        {
            var output = new double[ops.Length * rhos.Length];
            var query = ROfRho((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Convenience array overload of ROfTheta.  Determines reflectances at optical properties 'ops' and polar angle 'thetas'
        /// </summary>
        /// <param name="ops">sets of optical properties</param>
        /// <param name="thetas">polar angles</param>
        /// <returns>reflectance at given optical properties and polar angles</returns>
        public double[] ROfTheta(OpticalProperties[] ops, double[] thetas)
        {
            var output = new double[ops.Length * thetas.Length];
            var query = ROfTheta((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)thetas);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Convenience array overload of ROfRhoAndTime. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(OpticalProperties[] ops, double[] rhos, double[] ts)
        {
            var output = new double[ops.Length * rhos.Length * ts.Length];
            var query = ROfRhoAndTime((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Convenience array overload of ROfRhoAndFt. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given optical properties, rhos and modulation frequencies</returns>
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
        /// <returns>reflectance at given optical properties and spatial frequencies</returns>
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
        /// <returns>reflectance at given optical properties, spatial frequencies and times</returns>
        public double[] ROfFxAndTime(OpticalProperties[] ops, double[] fxs, double[] ts)
        {
            var output = new double[ops.Length * fxs.Length * ts.Length];
            var query = ROfFxAndTime((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Convenience array overload of ROfFxAndFt. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and modulation frequencies</returns>
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
        /// <returns>reflectance at given optical properties and rhos</returns>
        public double[] ROfRho(OpticalProperties op, double[] rhos)
        {
            return ROfRho(new[] { op }, rhos);
        }

        /// <summary>
        /// Overload of ROfRho. Determines reflectances at optical properties 'ops' and source-detector separation 'rho'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separations (mm)</param>
        /// <returns>reflectance at given optical properties and rhos</returns>
        public double[] ROfRho(OpticalProperties[] ops, double rho)
        {
            return ROfRho(ops, new[] { rho });
        }
        /// <summary>
        /// Overload of ROFTheta.  Determines reflectances at optical properties 'ops' and polar angles 'thetas'
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="thetas">polar angles</param>
        /// <returns>reflectance at given optical properties and polar angles</returns>
        public double[] ROfTheta(OpticalProperties op, double[] thetas)
        {
            return ROfTheta(new[] { op }, thetas);
        }
        /// <summary>
        /// Overload of ROFTheta.  Determines reflectances at optical properties 'ops' and polar angles 'thetas'
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="theta">polar angles</param>
        /// <returns>reflectance at given optical properties and polar angles</returns>
        public double[] ROfTheta(OpticalProperties[] ops, double theta)
        {
            return ROfTheta(ops, new[] { theta });
        }

        // ROfRhoAndTime
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'op', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(OpticalProperties op, double[] rhos, double[] ts)
        {
            return ROfRhoAndTime(new[] { op }, rhos, ts);
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'ops', source-detector separation 'rho' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(OpticalProperties[] ops, double rho, double[] ts)
        {
            return ROfRhoAndTime(ops, new[] { rho }, ts);
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times </returns>
        public double[] ROfRhoAndTime(OpticalProperties[] ops, double[] rhos, double t)
        {
            return ROfRhoAndTime(ops, rhos, new[] { t });
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'op', source-detector separation 'rho' and times 'ts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(OpticalProperties op, double rho, double[] ts)
        {
            return ROfRhoAndTime(new[] { op }, new[] { rho }, ts);
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'op', source-detector separations 'rhos' and time 't'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and time</returns>
        public double[] ROfRhoAndTime(OpticalProperties op, double[] rhos, double t)
        {
            return ROfRhoAndTime(new[] { op }, rhos, new[] { t });
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'ops', source-detector separation 'rho' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at given optical properties, rho and time</returns>
        public double[] ROfRhoAndTime(OpticalProperties[] ops, double rho, double t)
        {
            return ROfRhoAndTime(ops, new[] { rho }, new[] { t });
        }

        //ROfRhoAndFt
        /// <summary>
        /// Overload of ROfRhoAndFt. Determines reflectances at optical properties 'op', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given single set of optical properties, rhos and modulation frequencies </returns>
        public Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double[] fts)
        {
            return ROfRhoAndFt(new[] { op }, rhos, fts);
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'ops', source-detector separation 'rho' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given optical properties, single rho, and modulation frequencies</returns>
        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double[] fts)
        {
            return ROfRhoAndFt(ops, new[] { rho }, fts);
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequency 'ft'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at given optical properties, rhos and single modulation frequencies</returns>
        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double ft)
        {
            return ROfRhoAndFt(ops, rhos, new[] { ft });
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'op', source-detector separation 'rho' and time frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given single set of optical properties, single rho and modulation frequencies</returns>
        public Complex[] ROfRhoAndFt(OpticalProperties op, double rho, double[] fts)
        {
            return ROfRhoAndFt(new[] { op }, new[] { rho }, fts);
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'op', source-detector separations 'rhos' and time frequency 'ft'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at given single set of optical properties, rhos and single modulation frequency</returns>
        public Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double ft)
        {
            return ROfRhoAndFt(new[] { op }, rhos, new[] { ft });
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectances at optical properties 'ops', source-detector separation 'rho' and time frequency 'ft'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at given optical properties, single rho and single modulation frequency</returns>
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
        /// <returns>reflectance at given single set of optical properties and spatial frequencies</returns>
        public double[] ROfFx(OpticalProperties op, double[] fxs)
        {
            return ROfFx(new[] { op }, fxs);
        }

        /// <summary>
        /// Overload of ROfFx. Determines reflectances at optical properties 'ops' and spatial frequency 'fx'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>reflectance at given optical properties and single spatial frequency</returns>
        public double[] ROfFx(OpticalProperties[] ops, double fx)
        {
            return ROfFx(ops, new[] { fx });
        }

        // ROfFxAndTime
        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="op">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given single set of optical properties, spatial frequencies and times</returns>
        public double[] ROfFxAndTime(OpticalProperties op, double[] fxs, double[] ts)
        {
            return ROfFxAndTime(new[] { op }, fxs, ts);
        }

        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectances at optical properties 'ops', spatial frequency 'fx' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given optical properties, single spatial frequency and times</returns>
        public double[] ROfFxAndTime(OpticalProperties[] ops, double fx, double[] ts)
        {
            return ROfFxAndTime(ops, new[] { fx }, ts);
        }

        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and single time</returns>
        public double[] ROfFxAndTime(OpticalProperties[] ops, double[] fxs, double t)
        {
            return ROfFxAndTime(ops, fxs, new[] { t });
        }

        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectances at optical properties 'op', spatial frequency 'fx' and times 'ts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given single set of optical properties, single spatial frequency and times</returns>
        public double[] ROfFxAndTime(OpticalProperties op, double fx, double[] ts)
        {
            return ROfFxAndTime(new[] { op }, new[] { fx }, ts);
        }

        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectances at optical properties 'op', spatial frequencies 'fxs' and time 't'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at given single set of optical properties, spatial frequencies, and single time </returns>
        public double[] ROfFxAndTime(OpticalProperties op, double[] fxs, double t)
        {
            return ROfFxAndTime(new[] { op }, fxs, new[] { t });
        }

        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectances at optical properties 'ops', spatial frequency 'fx' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at given optical properties, single spatial frequency and single time</returns>
        public double[] ROfFxAndTime(OpticalProperties[] ops, double fx, double t)
        {
            return ROfFxAndTime(ops, new[] { fx }, new[] { t });
        }

        // ROfFxAndFt
        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectances at optical properties 'op', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given single set of optical properties, spatial frequencies and modulation frequencies</returns>
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
        /// <returns>reflectance at given optical properties, single spatial frequency and modulation frequencies</returns>
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
        /// <returns>reflectance at given optical properties, spatial frequencies and single modulation frequency</returns>
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
        /// <returns>reflectance at given single set of optical properties, single spatial frequency and modulation frequencies</returns>
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
        /// <returns>reflectance at given single set of optical properties, spatial frequencies and single modulation frequency</returns>
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
        /// <returns>reflectance at given optical properteis, single spatial frequency and single modulation frequency</returns>
        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double ft)
        {
            return ROfFxAndFt(ops, new[] { fx }, new[] { ft });
        }

        #endregion

        #endregion

        // The following methods are designed to return values based on iteration 
        // with the leftmost IEnumerable<Time> input being the top-level, and so-on
        // such that the right-most input is at the inner-most loop
        #region Dummy default fluence versions of the vectorized methods. Override these in child classes to take advantage of optimization strategies.

        /// <summary>
        /// Overload of scalar FluenceOfRhoAndZ function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos' and 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>reflectance at given optical properties, rhos and depths (zs)</returns>
        public virtual IEnumerable<double> FluenceOfRhoAndZ(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfRhoAndZAndTime function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos', 'zs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at vien optical properties, rhos, depths (zs) and times</returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public virtual IEnumerable<double> FluenceOfRhoAndZAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfRhoAndZAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos', 'zs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given optical properties, rhos, depths (zs) and modulation frequencies</returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public virtual IEnumerable<Complex> FluenceOfRhoAndZAndFt(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfFxAndZ function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs' and 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and depths (zs)</returns>
        public virtual IEnumerable<double> FluenceOfFxAndZ(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfFxAndZAndTime function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs', 'zs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies, depths and times</returns>
        public virtual IEnumerable<double> FluenceOfFxAndZAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs,
            IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfFxAndZAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at vien optical properties, spatial frequencies, z values and modulation frequencies</returns>
        public virtual IEnumerable<Complex> FluenceOfFxAndZAndFt(
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
        /// <returns>reflectance at given optical properties, rhos and z values</returns>
        public double[] FluenceOfRhoAndZ(OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            var output = new double[ops.Length * rhos.Length * zs.Length];
            var query = FluenceOfRhoAndZ((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfRhoAndZAndTime function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos', 'zs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos, z values (depths) and times</returns>
        public double[] FluenceOfRhoAndZAndTime(OpticalProperties[] ops, double[] rhos, double[] zs, double[] ts)
        {
            var output = new double[ops.Length * rhos.Length * zs.Length * ts.Length];
            var query = FluenceOfRhoAndZAndTime((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfRhoAndZAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos', 'zs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given optical properties, rhos, z values (depths) and modulation frequencies</returns>
        public Complex[] FluenceOfRhoAndZAndFt(OpticalProperties[] ops, double[] rhos, double[] zs, double[] fts)
        {
            var output = new Complex[ops.Length * rhos.Length * zs.Length * fts.Length];
            var query = FluenceOfRhoAndZAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfFxAndZ function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and z values (depths)</returns>
        public double[] FluenceOfFxAndZ(OpticalProperties[] ops, double[] fxs, double[] zs)
        {
            var output = new double[ops.Length * fxs.Length * zs.Length];
            var query = FluenceOfFxAndZ((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)zs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfFxAndZAndTime function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs', 'zs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and z values (depths) and times</returns>        
        public double[] FluenceOfFxAndZAndTime(OpticalProperties[] ops, double[] fxs, double[] zs, double[] ts)
        {
            var output = new double[ops.Length * fxs.Length * zs.Length * ts.Length];
            var query = FluenceOfFxAndZAndTime((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)zs, (IEnumerable<double>)ts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfFxAndZAndFt function. Determines reflectances at optical properties 'ops', spatial frequency 'fx', 'zs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies z values (depths) and modulation frequencies</returns>        
        public Complex[] FluenceOfFxAndZAndFt(OpticalProperties[] ops, double[] fx, double[] zs, double[] fts)
        {
            var output = new Complex[ops.Length * fx.Length * zs.Length * fts.Length];
            var query = FluenceOfFxAndZAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fx, (IEnumerable<double>)zs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        #region array overloads that simplify single parameter specification

        // none yet implemented

        #endregion

        #endregion
    }
}
