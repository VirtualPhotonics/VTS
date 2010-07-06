using System;
using MathNet.Numerics;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Contains distributed point source solutions for the diffusion point source-image solution in the
    /// semi-infinite domain.
    /// </summary>
    public class DistributedPointSourceDiffusionForwardSolver : DiffusionForwardSolverBase // : SDAForwardSolver
    {
        private PointSourceDiffusionForwardSolver _pointSourceForwardSolver;

        public DistributedPointSourceDiffusionForwardSolver()
            : base(SourceConfiguration.Distributed, 0.0)
        {
            _pointSourceForwardSolver = new PointSourceDiffusionForwardSolver();
        }

        #region SteadyState

        public override double StationaryReflectance(
            DiffusionParameters dp, double rho, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(
                StationaryFluence(rho, 0.0, dp),
                StationaryFlux(rho, 0.0, dp),
                fr1, fr2);
        }

        public override double StationaryFluence(double rho, double z, DiffusionParameters dp)
        {
            var dpLocal = DiffusionParameters.Copy(dp);

            return CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                (zp) =>
                {
                    dpLocal.zp = zp;
                    return _pointSourceForwardSolver.StationaryFluence(rho, z, dpLocal);
                },
                dp.mutTilde);
        }

        private double StationaryFlux(double rho, double z, DiffusionParameters dp)
        {
            var dpLocal = DiffusionParameters.Copy(dp);

            return CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                zp =>
                {
                    dpLocal.zp = zp;
                    return _pointSourceForwardSolver.StationaryFlux(rho, z, dpLocal);
                },
                dp.mutTilde);
        }

        #endregion SteadyState

        #region Temporal Solutions

        public override double TemporalReflectance(DiffusionParameters dp, double rho, double t, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(
                TemporalFluence(dp, rho, 0.0, t),
                TemporalFlux(dp, rho, 0.0, t),
                fr1, fr2);
        }

        public override double TemporalFluence(
            DiffusionParameters dp, double rho, double z, double t)
        {
            var dpLocal = DiffusionParameters.Copy(dp);

            return CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                zp =>
                {
                    dpLocal.zp = zp;
                    return _pointSourceForwardSolver.TemporalFluence(dpLocal, rho, z, t);
                },
                dp.mutTilde) *
                dp.musTilde / dp.mutTilde;
        }

        private double TemporalFlux(
            DiffusionParameters dp, double rho, double z, double t)
        {
            var dpLocal = DiffusionParameters.Copy(dp);

            return CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                zp =>
                {
                    dpLocal.zp = zp;
                    return
                        _pointSourceForwardSolver.TemporalFlux(dpLocal, rho, z, t);
                },
                dp.mutTilde) *
                dp.musTilde / dp.mutTilde;
        }

        #endregion Temporal Solutions

        #region Temporal Frequency Solutions


        public override Complex TemporalFrequencyReflectance(DiffusionParameters dp,
            double rho, Complex k, double fr1, double fr2)
        {
            var dpLocalReal = DiffusionParameters.Copy(dp);
            var dpLocalImag = DiffusionParameters.Copy(dp);

            Func<DiffusionParameters, double, Complex, double, Complex>
                kernelFunc = (dpLocal, r, kLocal, zp) =>
                {
                    dpLocal.zp = zp;
                    return
                        _pointSourceForwardSolver.TemporalFrequencyReflectance(dpLocal, r, kLocal, fr1, fr2);
                };

            return
                CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                        zp => kernelFunc(dpLocalReal, rho, k, zp).Real,
                        dp.mutTilde) * dp.musTilde
                + Complex.ImaginaryOne *
                CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                        zp => kernelFunc(dpLocalImag, rho, k, zp).Imaginary,
                        dp.mutTilde) * dp.musTilde;

        }

        public override Complex TemporalFrequencyFluence(DiffusionParameters dp, double rho, double z, Complex k)
        {
            var dpLocalReal = DiffusionParameters.Copy(dp);
            var dpLocalImag = DiffusionParameters.Copy(dp);

            Func<DiffusionParameters, double, Complex>
                kernelFunc = (dpLocal, zp) =>
                {
                    dpLocal.zp = zp;
                    return
                        _pointSourceForwardSolver.TemporalFrequencyFluence(dpLocal, rho, z, k);
                };

            return
                CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                        zp => kernelFunc(dpLocalReal, zp).Real,
                        dp.mutTilde) * dp.musTilde
                + Complex.ImaginaryOne *
                CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                        zp => kernelFunc(dpLocalImag, zp).Imaginary,
                        dp.mutTilde) * dp.musTilde;
        }

        #endregion Temporal Frequency Solutions

    }
}