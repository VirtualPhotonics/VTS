using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using Vts.Extensions;

namespace Vts.Modeling.ForwardSolvers
{
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
        public override double ROfRho(OpticalProperties op, double rho)
        {
            return ROfRho(op.AsEnumerable(), rho.AsEnumerable()).First();
        }

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

        public override double ROfRhoAndT(OpticalProperties op, double rho, double t)
        {
            return ROfRhoAndT(op.AsEnumerable(), rho.AsEnumerable(), t.AsEnumerable()).First();
        }

        public override IEnumerable<double> ROfRhoAndT(IEnumerable<OpticalProperties> ops,
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
        /// <returns></returns>
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
                double[] rOfRhoAndT = ROfRhoAndT(op.AsEnumerable(), rhos, time).ToArray();
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

        public override double ROfFxAndT(OpticalProperties op, double fx, double t)
        {
            return ROfFxAndT(op.AsEnumerable(), fx.AsEnumerable(), fx.AsEnumerable()).First();
        }
        public override IEnumerable<double> ROfFxAndT(IEnumerable<OpticalProperties> ops,
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

        public override Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            return ROfFxAndFt(op.AsEnumerable(), fx.AsEnumerable(), ft.AsEnumerable()).First();
        }
        public override IEnumerable<Complex> ROfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            double[] time = new double[_monteCarloLoader.ntReference];
            foreach (var op in ops)
            {
                time = _monteCarloLoader.GetAllScaledTimes(op).ToArray();
                var dTime = time[1] - time[0];
                double[] rOfFxAndT = ROfFxAndT(op.AsEnumerable(), fxs, time).ToArray();
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


        public override IEnumerable<double> FluenceOfRhoAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceOfRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceOfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceOfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceOfFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceOfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }
    }
}
