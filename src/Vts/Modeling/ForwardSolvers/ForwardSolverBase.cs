using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// The <see cref="ForwardSolvers"/> namespace contains the forward solver classes for the Virtual Tissue Simulator
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

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

        #region Dummy reflectance virtual methods - must be implemented in child classes

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
        /// Scalar ROfRho function.  Determines reflectance at source-detector separation rho - must be implemented in child class
        /// </summary>
        /// <param name="regions">tissue regions of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <returns>reflectance at given tissue regions and single rho</returns>     
        public virtual double ROfRho(IOpticalPropertyRegion[] regions, double rho)
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
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given single set of optical properties, single rho and single time</returns>
        public virtual double ROfRhoAndTime(OpticalProperties op, double rho, double time)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Scalar ROfRhoAndTime function.  Determines reflectance for tissue with regions, at
        /// source-detector separation rho and time t - must be implemented in child class
        /// </summary>
        /// <param name="regions">medium regions</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given single set of optical properties, single rho and single time</returns>
        public virtual double ROfRhoAndTime(IOpticalPropertyRegion[] regions, double rho, double time)
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
        /// Scalar ROfRhoAndFt function.  Determines reflectance at source-detector separation rho and modulation frequency ft - must be implemented in child class
        /// </summary>
        /// <param name="regions">tissue regions of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at given single set of optical properties, single rho and single modulation frequency ft</returns>
        public virtual Complex ROfRhoAndFt(IOpticalPropertyRegion[] regions, double rho, double ft)
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
        /// Scalar ROfFx function.  Determines reflectance at spatial frequency fx - must be implemented in child class
        /// </summary>
        /// <param name="regions">regions of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>reflectance at given tissue regions and single spatial frequency fx</returns>
        public virtual double ROfFx(IOpticalPropertyRegion[] regions, double fx)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Scalar ROfFxAndTime function.  Determines reflectance at spatial frequency fx and time t - must be implemented in child class
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given single set of optical properties, single spatial frequency fx and single time t</returns>
        public virtual double ROfFxAndTime(OpticalProperties op, double fx, double time)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Scalar ROfFxAndTime function.  Determines reflectance at spatial frequency fx and time t - must be implemented in child class
        /// </summary>
        /// <param name="regions">regions of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given tissue regions, single spatial frequency fx and single time t</returns>
        public virtual double ROfFxAndTime(IOpticalPropertyRegion[] regions, double fx, double time)
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
        /// <summary>
        /// Determines reflectance at spatial frequency fx and modulation frequency ft - must be implemented in child class
        /// </summary>
        /// <param name="regions">regions of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at given tissue regions, single spatial frequency fx and single modulation frequency ft</returns>
        public virtual Complex ROfFxAndFt(IOpticalPropertyRegion[] regions, double fx, double ft)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Dummy default reflectance versions of the vectorized methods. Override these in child classes to take advantage of optimization strategies.

        /// <summary>
        /// Vector ROfRho function. Determines reflectance at optical properties 'ops' and source-detector separations 'rhos'
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

        /// <summary>
        /// Vector ROfRho function. Determines reflectance at regions and source-detector separations 'rhos'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="regions">sets of optical and geometrical properties of the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>reflectance at given optical properties and rhos</returns>
        public virtual IEnumerable<double> ROfRho(
            IEnumerable<IOpticalPropertyRegion[]> regions,
            IEnumerable<double> rhos)
        {
            return ((Func<IOpticalPropertyRegion[], double, double>)ROfRho).LoopOverVariables(regions, rhos);
        }
        /// <summary>
        /// reflectance as function of theta
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="thetas">thetas</param>
        /// <returns>R(thetas)</returns>
        public virtual IEnumerable<double> ROfTheta(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> thetas)
        {
            return ((Func<OpticalProperties, double, double>)ROfTheta).LoopOverVariables(ops, thetas);
        }

        /// <summary>
        /// Vector ROfRhoAndTime function. Determines reflectance at optical properties 'ops', source-detector separations 'rhos' and times 'times'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public virtual IEnumerable<double> ROfRhoAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> times)
        {
            return ((Func<OpticalProperties, double, double, double>)ROfRhoAndTime).LoopOverVariables(ops, rhos, times);
        }
        /// <summary>
        /// Vector ROfRhoAndTime function. Determines reflectance of tissue with 'regions', source-detector separations 'rhos' and times 'times'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance of tissue regions, rhos and times</returns>
        public virtual IEnumerable<double> ROfRhoAndTime(
            IEnumerable<IOpticalPropertyRegion[]> regions,
            IEnumerable<double> rhos,
            IEnumerable<double> times)
        {
            return ((Func<IOpticalPropertyRegion[], double, double, double>)ROfRhoAndTime).LoopOverVariables(regions, rhos, times);
        }
        /// <summary>
        /// Vector ROfRhoAndFt function. Determines reflectance at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
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
        /// Vector ROfRhoAndFt function. Determines reflectance at tissue regions 'regions', source-detector separations 'rhos' and time frequencies 'fts'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="regions">tissue regions of the medium</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given optical properties, rhos and modulation frequencies</returns>
        public virtual IEnumerable<Complex> ROfRhoAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            return ((Func<IOpticalPropertyRegion[], double, double, Complex>)ROfRhoAndFt).LoopOverVariables(regions, rhos, fts);
        }
        /// <summary>
        /// Vector ROfFx function. Determines reflectance at optical properties 'ops' and spatial frequencies 'fxs'
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
        /// Vector ROfFx function. Determines reflectance at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="regions">sets tissue regions of medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns>reflectance at given optical properties and spatial frequencies</returns>
        public virtual IEnumerable<double> ROfFx(
            IEnumerable<IOpticalPropertyRegion[]> regions,
            IEnumerable<double> fxs)
        {
            return ((Func<IOpticalPropertyRegion[], double, double>)ROfFx).LoopOverVariables(regions, fxs);
        }
        /// <summary>
        /// Vector ROfFxAndTime function. Determines reflectance at optical properties 'ops', spatial frequencies 'fxs' and times 'times'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="ops">sets of optical properties of the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and times</returns>
        public virtual IEnumerable<double> ROfFxAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> times)
        {
            return ((Func<OpticalProperties, double, double, double>)ROfFxAndTime).LoopOverVariables(ops, fxs, times);
        }
        /// <summary>
        /// Vector ROfFxAndTime function. Determines reflectance of tissue regions, spatial frequencies 'fxs' and times 'times'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="regions">sets of tissue regions of the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance of tissue regions, spatial frequencies and times</returns>
        public virtual IEnumerable<double> ROfFxAndTime(
            IEnumerable<IOpticalPropertyRegion[]> regions,
            IEnumerable<double> fxs,
            IEnumerable<double> times)
        {
            return ((Func<IOpticalPropertyRegion[], double, double, double>)ROfFxAndTime).LoopOverVariables(regions, fxs, times);
        }
        /// <summary>
        /// Vector ROfFxAndFt function. Determines reflectance at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
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
        /// <summary>
        /// Vector ROfFxAndFt function. Determines reflectance of tissue 'regions', spatial frequencies 'fxs' and time frequencies 'fts'
        /// Override these in child classes to take advantage of optimization strategies.
        /// </summary>
        /// <param name="regions">sets of tissue regions of the medium</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance of given tissue regions, spatial frequencies and modulation frequencies</returns>
        public virtual IEnumerable<Complex> ROfFxAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            return ((Func<IOpticalPropertyRegion[], double, double, Complex>)ROfFxAndFt).LoopOverVariables(regions, fxs, fts);
        }
        #endregion

        #region Convenience reflectance array overloads (todo: these could alternatively be IForwardSolverExtensions instead of part of the contract)

        /// <summary>
        /// Convenience array overload of ROfRho. Determines reflectance at optical properties 'ops' and source-detector separations 'rhos'
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
        /// Convenience array overload of ROfRho. Determines reflectance at optical properties 'ops' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties of each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>Reflectance at given optical properties and rhos</returns>
        public double[] ROfRho(IOpticalPropertyRegion[][] regions, double[] rhos)
        {
            var output = new double[regions.Length * rhos.Length];
            var query = ROfRho((IEnumerable<IOpticalPropertyRegion[]>)regions, (IEnumerable<double>)rhos);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Convenience array overload of ROfTheta.  Determines reflectance at optical properties 'ops' and polar angle 'thetas'
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
        /// Convenience array overload of ROfRhoAndTime. Determines reflectance at optical properties 'ops', source-detector separations 'rhos' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(OpticalProperties[] ops, double[] rhos, double[] times)
        {
            var output = new double[ops.Length * rhos.Length * times.Length];
            var query = ROfRhoAndTime((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)times);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Convenience array overload of ROfRhoAndTime. Determines reflectance at regions, source-detector separations 'rhos' and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(IOpticalPropertyRegion[][] regions, double[] rhos, double[] times)
        {
            var output = new double[regions.Length * rhos.Length * times.Length];
            var query = ROfRhoAndTime((IEnumerable<IOpticalPropertyRegion[]>)regions, (IEnumerable<double>)rhos, (IEnumerable<double>)times);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Convenience array overload of ROfRhoAndFt. Determines reflectance at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
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
        /// Convenience array overload of ROfRho. Determines reflectance of tissue 'regions', source-detector 
        /// separations 'rhos', and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties of each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">temporal-frequencies</param>
        /// <returns>Reflectance at given optical properties and rhos</returns>
        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[][] regions, double[] rhos, double[] fts)
        {
            var output = new Complex[regions.Length * rhos.Length * fts.Length];
            var query = ROfRhoAndFt((IEnumerable<IOpticalPropertyRegion[]>)regions, (IEnumerable<double>)rhos, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Convenience array overload of ROfFx. Determines reflectance at optical properties 'ops' and spatial frequencies 'fxs'
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
        /// Convenience array overload of ROfFx. Determines reflectance of tissue 'regions' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="regions">sets of medium tissue regions</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <returns>reflectance of given tissue regions and spatial frequencies</returns>
        public double[] ROfFx(IOpticalPropertyRegion[][] regions, double[] fxs)
        {
            var output = new double[regions.Length * fxs.Length];
            var query = ROfFx((IEnumerable<IOpticalPropertyRegion[]>)regions, (IEnumerable<double>)fxs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Convenience array overload of ROfFxAndTime. Determines reflectance at optical properties 'ops', spatial frequencies 'fxs' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and times</returns>
        public double[] ROfFxAndTime(OpticalProperties[] ops, double[] fxs, double[] times)
        {
            var output = new double[ops.Length * fxs.Length * times.Length];
            var query = ROfFxAndTime((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)times);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Convenience array overload of ROfFxAndTime. Determines reflectance of tissue 'regions', spatial frequencies 'fxs' and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance of given tissue region, spatial frequencies and times</returns>
        public double[] ROfFxAndTime(IOpticalPropertyRegion[][] regions, double[] fxs, double[] times)
        {
            var output = new double[regions.Length * fxs.Length * times.Length];
            var query = ROfFxAndTime((IEnumerable<IOpticalPropertyRegion[]>)regions, (IEnumerable<double>)fxs, (IEnumerable<double>)times);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Convenience array overload of ROfFxAndFt. Determines reflectance at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
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
        /// <summary>
        /// Convenience array overload of ROfFxAndFt. Determines reflectance of tissue 'regions', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium tissue regions</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance of given tissue regions, spatial frequencies and modulation frequencies</returns>
        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[][] regions, double[] fxs, double[] fts)
        {
            var output = new Complex[regions.Length * fxs.Length * fts.Length];
            var query = ROfFxAndFt((IEnumerable<IOpticalPropertyRegion[]>)regions, (IEnumerable<double>)fxs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        #region array overloads that simplify single parameter specification

        /// <summary>
        /// Overload of ROfRho. Determines reflectance at optical properties 'op' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>reflectance at given optical properties and rhos</returns>
        public double[] ROfRho(OpticalProperties op, double[] rhos)
        {
            return ROfRho(new[] { op }, rhos);
        }

        /// <summary>
        /// Overload of ROfRho. Determines reflectance at optical properties 'op' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="regions">medium optical and geometrical properties of each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>reflectance at given optical properties and rhos</returns>
        public double[] ROfRho(IOpticalPropertyRegion[] regions, double[] rhos)
        {
            return ROfRho(new[] { regions }, rhos);
        }

        /// <summary>
        /// Overload of ROfRho. Determines reflectance at optical properties 'ops' and source-detector separation 'rho'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separations (mm)</param>
        /// <returns>reflectance at given optical properties and rhos</returns>
        public double[] ROfRho(OpticalProperties[] ops, double rho)
        {
            return ROfRho(ops, new[] { rho });
        }

        /// <summary>
        /// Overload of ROfRho. Determines reflectance at optical properties 'ops' and source-detector separation 'rho'
        /// </summary>
        /// <param name="regions">sets of medium optical properties</param>
        /// <param name="rho">source-detector separations (mm)</param>
        /// <returns>reflectance at given optical properties and rhos</returns>
        public double[] ROfRho(IOpticalPropertyRegion[][] regions, double rho)
        {
            return ROfRho(regions, new[] { rho });
        }

        /// <summary>
        /// Overload of ROFTheta.  Determines reflectance at optical properties 'ops' and polar angles 'thetas'
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="thetas">polar angles</param>
        /// <returns>reflectance at given optical properties and polar angles</returns>
        public double[] ROfTheta(OpticalProperties op, double[] thetas)
        {
            return ROfTheta(new[] { op }, thetas);
        }
        /// <summary>
        /// Overload of ROFTheta.  Determines reflectance at optical properties 'ops' and polar angles 'thetas'
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
        /// Overload of ROfRhoAndTime. Determines reflectance at optical properties 'op', source-detector separations 'rhos' and times 'times'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(OpticalProperties op, double[] rhos, double[] times)
        {
            return ROfRhoAndTime(new[] { op }, rhos, times);
        }
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance of tissue regions 'regions', source-detector 
        /// separations 'rhos' and times 'times'
        /// </summary>
        /// <param name="regions">medium regions</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(IOpticalPropertyRegion[] regions, double[] rhos, double[] times)
        {
            return ROfRhoAndTime(new[] { regions }, rhos, times);
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance at optical properties 'ops', source-detector separation 'rho' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(OpticalProperties[] ops, double rho, double[] times)
        {
            return ROfRhoAndTime(ops, new[] { rho }, times);
        }
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance of tissue with regions 'regions', source-detector 
        /// separation 'rho' and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(IOpticalPropertyRegion[][] regions, double rho, double[] times)
        {
            return ROfRhoAndTime(regions, new[] { rho }, times);
        }

        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance at optical properties 'ops', source-detector separations 'rhos' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times </returns>
        public double[] ROfRhoAndTime(OpticalProperties[] ops, double[] rhos, double time)
        {
            return ROfRhoAndTime(ops, rhos, new[] { time });
        }
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance of tissue with 'regions', source-detector 
        /// separations 'rhos' and time 't'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times </returns>
        public double[] ROfRhoAndTime(IOpticalPropertyRegion[][] regions, double[] rhos, double time)
        {
            return ROfRhoAndTime(regions, rhos, new[] { time });
        }
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance at optical properties 'op', source-detector separation 'rho' and times 'times'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(OpticalProperties op, double rho, double[] times)
        {
            return ROfRhoAndTime(new[] { op }, new[] { rho }, times);
        }
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance of tissue with 'regions', source-detector 
        /// separation 'rho' and times 'times'
        /// </summary>
        /// <param name="regions">medium regions</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and times</returns>
        public double[] ROfRhoAndTime(IOpticalPropertyRegion[] regions, double rho, double[] times)
        {
            return ROfRhoAndTime(new[] { regions }, new[] { rho }, times);
        }
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance at optical properties 'op', source-detector separations 'rhos' and time 't'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and time</returns>
        public double[] ROfRhoAndTime(OpticalProperties op, double[] rhos, double time)
        {
            return ROfRhoAndTime(new[] { op }, rhos, new[] { time });
        }
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance at tissue regions 'regions', source-detector separations 'rhos' and time 't'
        /// </summary>
        /// <param name="regions">medium regions</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given optical properties, rhos and time</returns>
        public double[] ROfRhoAndTime(IOpticalPropertyRegion[] regions, double[] rhos, double time)
        {
            return ROfRhoAndTime(new[] { regions }, rhos, new[] { time });
        }
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance at optical properties 'ops', source-detector separation 'rho' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given optical properties, rho and time</returns>
        public double[] ROfRhoAndTime(OpticalProperties[] ops, double rho, double time)
        {
            return ROfRhoAndTime(ops, new[] { rho }, new[] { time });
        }
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance of tissue with 'regions', source-detector 
        /// separation 'rho' and time 't'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given optical properties, rho and time</returns>
        public double[] ROfRhoAndTime(IOpticalPropertyRegion[][] regions, double rho, double time)
        {
            return ROfRhoAndTime(regions, new[] { rho }, new[] { time });
        }

        //ROfRhoAndFt
        /// <summary>
        /// Overload of ROfRhoAndFt. Determines reflectance at optical properties 'op', source-detector separations 'rhos' and time frequencies 'fts'
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
        /// Overload of ROfRhoAndFt. Determines reflectance at tissue regions 'regions', source-detector 
        /// separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="regions">medium regions</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given single set of optical properties, rhos and modulation frequencies </returns>
        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double[] rhos, double[] fts)
        {
            return ROfRhoAndFt(new[] { regions }, rhos, fts);
        }
        /// <summary>
        /// Overload of ROfRhoAndFt. Determines reflectance at optical properties 'ops', source-detector separation 'rho' and time frequencies 'fts'
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
        /// Overload of ROfRhoAndFt. Determines reflectance at tissue regions 'regions', source-detector separation 'rho' and time frequencies 'fts'
        /// </summary>
        /// <param name="regions">tissue regions</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given optical properties, single rho, and modulation frequencies</returns>
        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[][] regions, double rho, double[] fts)
        {
            return ROfRhoAndFt(regions, new[] { rho }, fts);
        }
        /// <summary>
        /// Overload of ROfRhoAndFt. Determines reflectance at optical properties 'ops', source-detector separations 'rhos' and time frequency 'ft'
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
        /// Overload of ROfRhoAndFt. Determines reflectance of tissue 'regions', source-detector separations 'rhos' and time frequency 'ft'
        /// </summary>
        /// <param name="regions">sets of tissue regions</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance of given tissue regions, rhos and single modulation frequencies</returns>
        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[][] regions, double[] rhos, double ft)
        {
            return ROfRhoAndFt(regions, rhos, new[] { ft });
        }
        /// <summary>
        /// Overload of ROfRhoAndFt. Determines reflectance of tissue 'regions', source-detector separations 'rhos' and time frequency 'ft'
        /// </summary>
        /// <param name="regions">tissue regions</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance of given tissue regions, rhos and single modulation frequencies</returns>
        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double[] rhos, double ft)
        {
            return ROfRhoAndFt(regions, rhos, new[] { ft });
        }
        /// <summary>
        /// Overload of ROfRhoAndFt. Determines reflectance at optical properties 'op', source-detector separation 'rho' and time frequencies 'fts'
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
        /// Overload of ROfRhoAndFt. Determines reflectance of tissue 'regions', source-detector separation 'rho' and time frequencies 'fts'
        /// </summary>
        /// <param name="regions">medium regions</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given single set of optical properties, single rho and modulation frequencies</returns>
        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double rho, double[] fts)
        {
            return ROfRhoAndFt(new[] { regions }, new[] { rho }, fts);
        }
        /// <summary>
        /// Overload of ROfRhoAndTime. Determines reflectance at optical properties 'op', source-detector separations 'rhos' and time frequency 'ft'
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
        /// Overload of ROfRhoAndTime. Determines reflectance at optical properties 'ops', source-detector separation 'rho' and time frequency 'ft'
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
        /// Overload of ROfFx. Determines reflectance at optical properties 'op' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns>reflectance at given single set of optical properties and spatial frequencies</returns>
        public double[] ROfFx(OpticalProperties op, double[] fxs)
        {
            return ROfFx(new[] { op }, fxs);
        }
        /// <summary>
        /// Overload of ROfFx. Determines reflectance at tissue 'regions' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="regions">medium optical and geometric properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns>reflectance at given single set of optical properties and spatial frequencies</returns>
        public double[] ROfFx(IOpticalPropertyRegion[] regions, double[] fxs)
        {
            return ROfFx(new[] { regions }, fxs);
        }
        /// <summary>
        /// Overload of ROfFx. Determines reflectance at optical properties 'ops' and spatial frequency 'fx'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>reflectance at given optical properties and single spatial frequency</returns>
        public double[] ROfFx(OpticalProperties[] ops, double fx)
        {
            return ROfFx(ops, new[] { fx });
        }
        /// <summary>
        /// Overload of ROfFx. Determines reflectance of tissue 'regions' and spatial frequency 'fx'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>reflectance of given tissue regions and single spatial frequency</returns>
        public double[] ROfFx(IOpticalPropertyRegion[][] regions, double fx)
        {
            return ROfFx(regions, new[] { fx });
        }
        // ROfFxAndTime
        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectance at optical properties 'ops', spatial frequencies 'fxs' and times 'times'
        /// </summary>
        /// <param name="op">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given single set of optical properties, spatial frequencies and times</returns>
        public double[] ROfFxAndTime(OpticalProperties op, double[] fxs, double[] times)
        {
            return ROfFxAndTime(new[] { op }, fxs, times);
        }
        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectance of tissue 'regions', spatial frequencies 'fxs' and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance of given tissue regions, spatial frequencies and times</returns>
        public double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double[] fxs, double[] times)
        {
            return ROfFxAndTime(new[] { regions }, fxs, times);
        }
        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectance at optical properties 'ops', spatial frequency 'fx' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, single spatial frequency and times</returns>
        public double[] ROfFxAndTime(OpticalProperties[] ops, double fx, double[] times)
        {
            return ROfFxAndTime(ops, new[] { fx }, times);
        }
        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectance of tissue 'regions', spatial frequency 'fx' and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance of given tissue regions, single spatial frequency and times</returns>
        public double[] ROfFxAndTime(IOpticalPropertyRegion[][] regions, double fx, double[] times)
        {
            return ROfFxAndTime(regions, new[] { fx }, times);
        }
        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectance at optical properties 'ops', spatial frequencies 'fxs' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and single time</returns>
        public double[] ROfFxAndTime(OpticalProperties[] ops, double[] fxs, double time)
        {
            return ROfFxAndTime(ops, fxs, new[] { time });
        }

        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectance at optical properties 'op', spatial frequency 'fx' and times 'times'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given single set of optical properties, single spatial frequency and times</returns>
        public double[] ROfFxAndTime(OpticalProperties op, double fx, double[] times)
        {
            return ROfFxAndTime(new[] { op }, new[] { fx }, times);
        }
        /// <summary>
        /// reflectance as a function of spatial-frequency and time
        /// </summary>
        /// <param name="regions">tissue region's optical properties</param>
        /// <param name="fx">spatial-frequency</param>
        /// <param name="times">times</param>
        /// <returns>R(fx,times)</returns>
        public double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double fx, double[] times)
        {
            return ROfFxAndTime(regions, new[] { fx }, times);
        }
        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectance at optical properties 'op', spatial frequencies 'fxs' and time 't'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given single set of optical properties, spatial frequencies, and single time </returns>
        public double[] ROfFxAndTime(OpticalProperties op, double[] fxs, double time)
        {
            return ROfFxAndTime(new[] { op }, fxs, new[] { time });
        }

        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectance at optical properties 'ops', spatial frequency 'fx' and time 't'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance at given optical properties, single spatial frequency and single time</returns>
        public double[] ROfFxAndTime(OpticalProperties[] ops, double fx, double time)
        {
            return ROfFxAndTime(ops, new[] { fx }, new[] { time });
        }

        // ROfFxAndFt
        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectance at optical properties 'op', spatial frequencies 'fxs' and time frequencies 'fts'
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
        /// Overload of ROfFxAndFt. Determines reflectance of tissue 'regions', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="regions">medium regions</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance at given single set of optical properties, spatial frequencies and modulation frequencies</returns>
        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double[] fxs, double[] fts)
        {
            return ROfFxAndFt(new[] { regions }, fxs, fts);
        }
        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectance at optical properties 'ops', spatial frequency 'fx' and time frequencies 'fts'
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
        /// Overload of ROfFxAndFt. Determines reflectance of tissue 'regions', spatial frequency 'fx' and time frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>reflectance of given tissue regions, single spatial frequency and modulation frequencies</returns>
        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double fx, double[] fts)
        {
            return ROfFxAndFt(regions, new[] { fx }, fts);
        }
        /// <summary>
        /// Overload of ROfFxAndTime. Determines reflectance of tissue 'regions', spatial frequencies 'fxs' and time 't'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>reflectance of given tissue regions, single spatial frequencies and time</returns>
        public double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double[] fxs, double time)
        {
            return ROfFxAndTime(regions, fxs, new[] {time} );
        }
        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectance at optical properties 'ops', spatial frequencies 'fxs' and time frequency 'ft'
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
        /// Overload of ROfFxAndFt. Determines reflectance of tissue 'regions', spatial frequency 'fx' and time frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance of given tissue regions, single spatial frequency and modulation frequencies</returns>
        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[][] regions, double[] fxs, double ft)
        {
            return ROfFxAndFt(regions, fxs, new[] { ft });
        }
        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectance at optical properties 'op', spatial frequency 'fx' and time frequencies 'fts'
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
        /// Overload of ROfFxAndFt. Determines reflectance of tissue 'regions', spatial frequency 'fx' and time frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium regions</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance of given tissue regions, single spatial frequency and modulation frequencies</returns>
        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double[] fxs, double ft)
        {
            return ROfFxAndFt(regions, fxs, new[] { ft });
        }
        /// <summary>
        /// Overload of ROfFxAndFt. Determines reflectance at optical properties 'op', spatial frequencies 'fxs' and time frequency 'ft'
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
        /// Overload of ROfFxAndFt. Determines reflectance at optical properties 'ops', spatial frequency 'fx' and time frequency 'ft'
        /// </summary>
        /// <param name="ops">medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at given optical properties, single spatial frequency and single modulation frequency</returns>
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
        /// Overload of scalar FluenceOfRhoAndZ function. Determines fluences at optical properties 'ops' and source-detector separations 'rhos' and 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>fluence at given optical properties, rhos and depths (zs)</returns>
        public virtual IEnumerable<double> FluenceOfRhoAndZ(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Overload of scalar FluenceOfRhoAndZ function. Determines fluences of tissue 'regions' and source-detector separations 'rhos' and 'zs'
        /// </summary>
        /// <param name="regions">sets of medium regions </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>fluence of given tissue regions, rhos and depths (zs)</returns>
        public virtual IEnumerable<double> FluenceOfRhoAndZ(
            IEnumerable<IOpticalPropertyRegion[]> regions,
            IEnumerable<double> rhos,
            IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfRhoAndZAndTime function. Determines fluences at optical properties 'ops', source-detector separations 'rhos', 'zs' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>fluence at given optical properties, rhos, depths (zs) and times</returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public virtual IEnumerable<double> FluenceOfRhoAndZAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfRhoAndZAndTime function. Determines fluences at optical properties 'ops', source-detector separations 'rhos', 'zs' and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium regions </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>fluence at given optical properties, rhos, depths (zs) and times</returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public virtual IEnumerable<double> FluenceOfRhoAndZAndTime(
            IEnumerable<IOpticalPropertyRegion[]> regions,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfRhoAndZAndFt function. Determines reflectance at optical properties 'ops', source-detector separations 'rhos', 'zs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>fluence at given optical properties, rhos, depths (zs) and modulation frequencies</returns>
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
        /// Overload of scalar FluenceOfRhoAndZAndFt function. Determines reflectance at tissue 'regions', source-detector separations 'rhos', 'zs' and time frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium regions </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>fluence at given tissue regions, rhos, depths (zs) and modulation frequencies</returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        public virtual IEnumerable<Complex> FluenceOfRhoAndZAndFt(
            IEnumerable<IOpticalPropertyRegion[]> regions,
            IEnumerable<double> rhos,
            IEnumerable<double> zs,
            IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfFxAndZ function. Determines fluences at optical properties 'ops' and spatial frequencies 'fxs' and 'zs'
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
        /// Overload of scalar FluenceOfFxAndZAndTime function. Determines fluences at optical properties 'ops', spatial frequencies 'fxs', 'zs' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies, depths and times</returns>
        public virtual IEnumerable<double> FluenceOfFxAndZAndTime(
            IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs,
            IEnumerable<double> zs,
            IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overload of scalar FluenceOfFxAndZAndFt function. Determines fluences at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>fluence at given optical properties, spatial frequencies, z values and modulation frequencies</returns>
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
        /// Overload of FluenceOfRhoAndZ function. Determines fluences at optical properties 'ops', source-detector separations 'rhos' and 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>fluence at given optical properties, rhos and z values</returns>
        public double[] FluenceOfRhoAndZ(OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            var output = new double[ops.Length * rhos.Length * zs.Length];
            var query = FluenceOfRhoAndZ((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Overload of FluenceOfRhoAndZ function. Determines fluences of tissue 'regions', source-detector separations 'rhos' and 'zs'
        /// </summary>
        /// <param name="regions">sets of medium regions </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>fluence of given tissue regions, rhos and z values</returns>
        public double[] FluenceOfRhoAndZ(IOpticalPropertyRegion[][] regions, double[] rhos, double[] zs)
        {
            var output = new double[regions.Length * rhos.Length * zs.Length];
            var query = FluenceOfRhoAndZ((IEnumerable<IOpticalPropertyRegion[]>)regions, (IEnumerable<double>)rhos, (IEnumerable<double>)zs);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Overload of FluenceOfRhoAndZAndTime function. Determines fluences at optical properties 'ops', source-detector separations 'rhos', 'zs' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>fluence at given optical properties, rhos, z values (depths) and times</returns>
        public double[] FluenceOfRhoAndZAndTime(OpticalProperties[] ops, double[] rhos, double[] zs, double[] times)
        {
            var output = new double[ops.Length * rhos.Length * zs.Length * times.Length];
            var query = FluenceOfRhoAndZAndTime((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs, (IEnumerable<double>)times);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Overload of FluenceOfRhoAndZAndTime function. Determines fluences at optical properties 'ops', source-detector separations 'rhos', 'zs' and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium regions </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>fluence at given optical properties, rhos, z values (depths) and times</returns>
        public double[] FluenceOfRhoAndZAndTime(IOpticalPropertyRegion[][] regions, double[] rhos, double[] zs, double[] times)
        {
            var output = new double[regions.Length * rhos.Length * zs.Length * times.Length];
            var query = FluenceOfRhoAndZAndTime((IEnumerable<IOpticalPropertyRegion[]>)regions, (IEnumerable<double>)rhos, (IEnumerable<double>)zs, (IEnumerable<double>)times);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfRhoAndZAndFt function. Determines reflectance at optical properties 'ops', source-detector separations 'rhos', 'zs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>fluence at given optical properties, rhos, z values (depths) and modulation frequencies</returns>
        public Complex[] FluenceOfRhoAndZAndFt(OpticalProperties[] ops, double[] rhos, double[] zs, double[] fts)
        {
            var output = new Complex[ops.Length * rhos.Length * zs.Length * fts.Length];
            var query = FluenceOfRhoAndZAndFt((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)rhos, (IEnumerable<double>)zs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfRhoAndZAndFt function. Determines reflectance at tissue 'regions', source-detector separations 'rhos', 'zs' and time frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium regions </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>fluence at given tissue regions, rhos, z values (depths) and modulation frequencies</returns>
        public Complex[] FluenceOfRhoAndZAndFt(IOpticalPropertyRegion[][] regions, double[] rhos, double[] zs, double[] fts)
        {
            var output = new Complex[regions.Length * rhos.Length * zs.Length * fts.Length];
            var query = FluenceOfRhoAndZAndFt((IEnumerable<IOpticalPropertyRegion[]>)regions, (IEnumerable<double>)rhos, (IEnumerable<double>)zs, (IEnumerable<double>)fts);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }
        /// <summary>
        /// Overload of FluenceOfFxAndZ function. Determines reflectance at optical properties 'ops', spatial frequencies 'fxs' and 'zs'
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
        /// Overload of FluenceOfFxAndZAndTime function. Determines reflectance at optical properties 'ops', spatial frequencies 'fxs', 'zs' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>reflectance at given optical properties, spatial frequencies and z values (depths) and times</returns>        
        public double[] FluenceOfFxAndZAndTime(OpticalProperties[] ops, double[] fxs, double[] zs, double[] times)
        {
            var output = new double[ops.Length * fxs.Length * zs.Length * times.Length];
            var query = FluenceOfFxAndZAndTime((IEnumerable<OpticalProperties>)ops, (IEnumerable<double>)fxs, (IEnumerable<double>)zs, (IEnumerable<double>)times);
            Vts.Extensions.IEnumerableArrayExtensions.PopulateFromEnumerable(output, query);
            return output;
        }

        /// <summary>
        /// Overload of FluenceOfFxAndZAndFt function. Determines reflectance at optical properties 'ops', spatial frequency 'fx', 'zs' and time frequencies 'fts'
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
