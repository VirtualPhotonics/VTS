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
        public override double RofRho(OpticalProperties op, double rho)
        {
            return RofRho(op.AsEnumerable(), rho.AsEnumerable()).First();
        }

        public override IEnumerable<double> RofRho(IEnumerable<OpticalProperties> ops,
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

        public override double RofRhoAndT(OpticalProperties op, double rho, double t)
        {
            return RofRhoAndT(op.AsEnumerable(), rho.AsEnumerable(), t.AsEnumerable()).First();
        }

        public override IEnumerable<double> RofRhoAndT(IEnumerable<OpticalProperties> ops,
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
        public override Complex RofRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            return RofRhoAndFt(op.AsEnumerable(), rho.AsEnumerable(), ft.AsEnumerable()).First();
        }
        public override IEnumerable<Complex> RofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            double[] time = new double[_monteCarloLoader.ntReference];
            foreach (var op in ops)
            {
                time = _monteCarloLoader.GetAllScaledTimes(op).ToArray();
                var dTime = time[1] - time[0];
                double[] rOfRhoAndT = RofRhoAndT(op.AsEnumerable(), rhos, time).ToArray();
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
        public override double RofFx(OpticalProperties op, double fx)
        {
            return RofFx(op.AsEnumerable(), fx.AsEnumerable()).First();
        }
        public override IEnumerable<double> RofFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs)
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

        public override double RofFxAndT(OpticalProperties op, double fx, double t)
        {
            return RofFxAndT(op.AsEnumerable(), fx.AsEnumerable(), fx.AsEnumerable()).First();
        }
        public override IEnumerable<double> RofFxAndT(IEnumerable<OpticalProperties> ops,
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

        public override Complex RofFxAndFt(OpticalProperties op, double fx, double ft)
        {
            return RofFxAndFt(op.AsEnumerable(), fx.AsEnumerable(), ft.AsEnumerable()).First();
        }
        public override IEnumerable<Complex> RofFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            double[] time = new double[_monteCarloLoader.ntReference];
            foreach (var op in ops)
            {
                time = _monteCarloLoader.GetAllScaledTimes(op).ToArray();
                var dTime = time[1] - time[0];
                double[] rOfFxAndT = RofFxAndT(op.AsEnumerable(), fxs, time).ToArray();
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


        public override IEnumerable<double> FluenceofRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceofRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceofFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceofFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> ts)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<double> FluenceofFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }
    }
}
