using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Forward solver based on the Scaled Monte Carlo approach, proposed by Kienle and Patterson,
    /// used to evaluate the reflectance of a semi-infinite homogenous medium with g = 0.8 and n = 1.4.
    /// </summary>
    public class MonteCarloForwardSolver : ForwardSolverBase
    {
        private static MonteCarloLoader _monteCarloLoader;

        /// <summary>
        /// constructor for scaled Monte Carlo Forward Solver
        /// </summary>
        public MonteCarloForwardSolver() 
            : base(SourceConfiguration.Point, 0.0) 
        {
            if (_monteCarloLoader == null)
            {
                _monteCarloLoader = new MonteCarloLoader();
            }
        }

        #region IForwardSolver Members

        #region Spatial Domain Solutions

        /// <summary>
        /// Evaluates the steady state reflectance at 
        /// a single source detector separation rho, for the specified single set of optical properties.
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">source detector separation</param>
        /// <returns>spatailly resolved reflectance</returns>
        public override double ROfRho(OpticalProperties op, double rho)
        {
            return ROfRho(op.AsEnumerable(), rho.AsEnumerable()).First();
        }
        /// <summary>
        /// Evaluates the steady state reflectance at multipl sets of optical properties and source-detector separations.
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">source detector separations</param>
        /// <returns>reflectance at specified optical properties and rhos</returns>
        public override IEnumerable<double> ROfRho(IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos)
        { 
            double[] RatRhoMC = new double[_monteCarloLoader.nrReference];
            double[] rhoScaled = new double[_monteCarloLoader.nrReference];
            foreach (var op in ops)
            {
                double v = GlobalConstants.C / op.N; // speed of light [mm/ns]
                double fresnel = _monteCarloLoader.GetFresnel(1.0, op.N, 0.0);
                rhoScaled = _monteCarloLoader.GetAllScaledRhos(op).ToArray();
                for (int i = 0; i < _monteCarloLoader.nrReference; i++)
                {
                    for (int j = 0; j < _monteCarloLoader.ntReference; j++)
                    {
                        RatRhoMC[i] += (_monteCarloLoader.RReferenceOfRhoAndTime[i, j] / (1 - fresnel)) *
                            Math.Exp(-op.Mua * v * _monteCarloLoader.TimeReference[j] * _monteCarloLoader.muspReference / op.Musp) *
                            // next factor due to division by dt 
                            (_monteCarloLoader.dtReference) *
                            // integration over t requires the following normalization
                            (_monteCarloLoader.muspReference / op.Musp) * // this factor is from dt
                            (op.Musp / _monteCarloLoader.muspReference) * (op.Musp / _monteCarloLoader.muspReference) * (op.Musp / _monteCarloLoader.muspReference);
                    }
                }
                foreach (var rho in rhos)
                {
                    yield return Vts.Common.Math.Interpolation.interp1(rhoScaled, RatRhoMC, rho);
                }
            }
        }
        /// <summary>
        /// Evaluates spatially- and temporally- resolved reflectance at specified optical properties, rho and time
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">source-detector separation</param>
        /// <param name="t">time</param>
        /// <returns>reflectance at specified optical properties, rho and time </returns>
        public override double ROfRhoAndTime(OpticalProperties op, double rho, double t)
        {
            return ROfRhoAndTime(op.AsEnumerable(), rho.AsEnumerable(), t.AsEnumerable()).First();
        }
        /// <summary>
        /// Evaluates spatially- and temporally- resolved reflectance at specified optical properties, rhos and times
        /// </summary>
        /// <param name="ops">multiple sets of optical properties</param>
        /// <param name="rhos">rhos</param>
        /// <param name="times">times</param>
        /// <returns>reflectance at specified optical properties, rhos and times</returns>
        public override IEnumerable<double> ROfRhoAndTime(IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos, IEnumerable<double> times)
        {
            double v;
            double[] rhoScaled = new double[_monteCarloLoader.nrReference];
            double[] timeScaled = new double[_monteCarloLoader.ntReference];
            double[,] RScaled = new double[_monteCarloLoader.nrReference, _monteCarloLoader.ntReference];
            foreach (var op in ops)
            {
                v = GlobalConstants.C / op.N;// speed of light [mm/ns]
                double fresnel = _monteCarloLoader.GetFresnel(1.0, op.N, 0.0);
                rhoScaled = _monteCarloLoader.GetAllScaledRhos(op).ToArray();
                timeScaled = _monteCarloLoader.GetAllScaledTimes(op).ToArray();
                // scale first then interpolate `
                for (int i = 0; i < _monteCarloLoader.nrReference; i++)
                {
                    for (int j = 0; j < _monteCarloLoader.ntReference; j++)
                    {
                        RScaled[i, j] = (_monteCarloLoader.RReferenceOfRhoAndTime[i, j] / (1 - fresnel)) *
                            Math.Exp(-op.Mua * v * _monteCarloLoader.TimeReference[j] * _monteCarloLoader.muspReference / op.Musp) *
                            (op.Musp / _monteCarloLoader.muspReference) * (op.Musp / _monteCarloLoader.muspReference) * (op.Musp / _monteCarloLoader.muspReference);
                    }
                }
                foreach (var rho in rhos)
                {
                    foreach (var time in times)
                    {
                        yield return Vts.Common.Math.Interpolation.interp2(rhoScaled,
                            timeScaled, RScaled, rho, time);
                    }
                }
            }
        }

        /// <summary>
        /// Call a method to compute a discrete Fourier Transform on R(rho,t) MC results.
        /// </summary>
        /// <param name="op">Optical Properties</param>
        /// <param name="rho">rho</param>
        /// <param name="ft">time frequency</param>
        /// <returns>reflectance at specified optical properties, rho and time</returns>
        public override Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            return ROfRhoAndFt(op.AsEnumerable(), rho.AsEnumerable(), ft.AsEnumerable()).First();
        }
        public override IEnumerable<Complex> ROfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            double[] time = new double[_monteCarloLoader.ntReference];
            foreach (var op in ops)
            {
                time = _monteCarloLoader.GetAllScaledTimes(op).ToArray();
                var dTime = time[1] - time[0];
                double[] rOfRhoAndT = ROfRhoAndTime(op.AsEnumerable(), rhos, time).ToArray();
                int rhoIndex = 0;
                foreach (var rho in rhos)
                {
                    double[] rOfTime = rOfRhoAndT.Skip(rhoIndex * _monteCarloLoader.ntReference).
                        Take(_monteCarloLoader.ntReference).ToArray();
                    foreach (var ft in fts)
                    {
                        yield return LinearDiscreteFourierTransform.GetFourierTransform(time, rOfTime, dTime, ft);
                    }
                    ++rhoIndex;
                }
            }
        }
        #endregion

        #region Spatial Frequency Domain Solutions
        /// <summary>
        /// Evaluates reflectance as a function of spatial frequency
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <returns>reflectance at specified optical properties and spatial frequency</returns>
        public override double ROfFx(OpticalProperties op, double fx)
        {
            return ROfFx(op.AsEnumerable(), fx.AsEnumerable()).First();
        }
        public override IEnumerable<double> ROfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs)
        {
            double[] RatFxMC = new double[_monteCarloLoader.nfxReference];
            double[] fxScaled = new double[_monteCarloLoader.nfxReference];
            foreach (var op in ops)
            {
                double v = GlobalConstants.C / op.N; // speed of light [mm/ns]
                fxScaled = _monteCarloLoader.GetAllScaledFxs(op).ToArray();
                double fresnel = _monteCarloLoader.GetFresnel(1.0, op.N, 0.0);
                for (int i = 0; i < _monteCarloLoader.nfxReference; i++)
                {
                    for (int j = 0; j < _monteCarloLoader.ntReference; j++)
                    {
                        RatFxMC[i] += (_monteCarloLoader.RReferenceOfFxAndTime[i, j] / (1 - fresnel)) *
                            Math.Exp(-op.Mua * v * _monteCarloLoader.TimeReference[j] * _monteCarloLoader.muspReference / op.Musp) *
                            (_monteCarloLoader.dtReference * _monteCarloLoader.muspReference / op.Musp) *
                            (op.Musp / _monteCarloLoader.muspReference); // * (op.Musp / _monteCarloLoader.muspReference) * 
                            //(op.Musp / _monteCarloLoader.muspReference);
                    }
                }
                foreach (var fx in fxs)
                {
                    yield return Vts.Common.Math.Interpolation.interp1(fxScaled, RatFxMC, fx);
                }
            }
        }
        /// <summary>
        /// Evaluates reflectance as aa function of spatial frequency and time
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="t">time</param>
        /// <returns>reflectance at specified optical properties, spatial frequency and time</returns>
        public override double ROfFxAndTime(OpticalProperties op, double fx, double t)
        {
            return ROfFxAndTime(op.AsEnumerable(), fx.AsEnumerable(), fx.AsEnumerable()).First();
        }
        public override IEnumerable<double> ROfFxAndTime(IEnumerable<OpticalProperties> ops,
            IEnumerable<double> fxs, IEnumerable<double> times)
        {
            double v;
            double[] fxScaled = new double[_monteCarloLoader.nfxReference];
            double[] timeScaled = new double[_monteCarloLoader.ntReference];
            double[,] RScaled = new double[_monteCarloLoader.nfxReference, _monteCarloLoader.ntReference];
            foreach (var op in ops)
            {
                v = GlobalConstants.C / op.N; // speed of light [mm/ns]
                double fresnel = _monteCarloLoader.GetFresnel(1.0, op.N, 0.0);
                fxScaled = _monteCarloLoader.GetAllScaledFxs(op).ToArray();
                timeScaled = _monteCarloLoader.GetAllScaledTimes(op).ToArray();
                // scale first then interpolate `
                for (int i = 0; i < _monteCarloLoader.nfxReference; i++)
                {
                    for (int j = 0; j < _monteCarloLoader.ntReference; j++)
                    {
                        RScaled[i, j] = (_monteCarloLoader.RReferenceOfFxAndTime[i, j] / (1 - fresnel)) *
                            Math.Exp(-op.Mua * v * _monteCarloLoader.TimeReference[j] * _monteCarloLoader.muspReference / op.Musp) *
                            (op.Musp / _monteCarloLoader.muspReference);
                    }
                }
                foreach (var fx in fxs)
                {
                    foreach (var time in times)
                    {
                        yield return Vts.Common.Math.Interpolation.interp2(fxScaled,
                            timeScaled, RScaled, fx, time);
                    }
                }
            }
        }
        /// <summary>
        /// Evaluates reflectance ay spatial frequency and modulation frequency
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="ft">modulation frequency</param>
        /// <returns>reflectance at specified optical properties, spatial frequency and modulation frequency</returns>
        public override Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            return ROfFxAndFt(op.AsEnumerable(), fx.AsEnumerable(), ft.AsEnumerable()).First();
        }
        /// <summary>
        /// Evaluates reflectance as a function of multiple sets of optical properties, spatial frequencies and modulation frequencies
        /// </summary>
        /// <param name="ops">multiple sets of optical properties</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="fts">modulation frequencies</param>
        /// <returns>reflectance at specified optical properties, spatial frequencies and modulation frequencies</returns>
        public override IEnumerable<Complex> ROfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            double[] time = new double[_monteCarloLoader.ntReference];
            foreach (var op in ops)
            {
                time = _monteCarloLoader.GetAllScaledTimes(op).ToArray();
                var dTime = time[1] - time[0];
                double[] rOfFxAndT = ROfFxAndTime(op.AsEnumerable(), fxs, time).ToArray();
                int fxIndex = 0;
                foreach (var fx in fxs)
                {
                    double[] rOfTime = rOfFxAndT.Skip(fxIndex * _monteCarloLoader.ntReference).
                        Take(_monteCarloLoader.ntReference).ToArray();
                    foreach (var ft in fts)
                    {
                        yield return LinearDiscreteFourierTransform.GetFourierTransform(time, rOfTime, dTime, ft);
                    }
                    ++fxIndex;
                }
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Evaluates fluence as a function of optical properties, source-detector separations (rhos) and depths (zs)
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">source-detector separations</param>
        /// <param name="zs">z values (depths)</param>
        /// <returns>reflectance at specified optical properties, rhos and depths</returns>
        public override IEnumerable<double> FluenceOfRhoAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Evaluates fluence as a function of optical properties, source-detector separations (rhos) and tines (ts)
        /// </summary>
        /// <param name="ops">optical properties</param>
        /// <param name="rhos">source-detector separations</param>
        /// <param name="zs">z values (depths)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns>reflectance at specified optical properties, rhos and depths</returns>
        public override IEnumerable<double> FluenceOfRhoAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceOfFxAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceOfFxAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Complex> FluenceOfFxAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }
    }
}
