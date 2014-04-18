using System;
using MathNet.Numerics;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Contains distributed point source solutions for the diffusion point source-image solution in the
    /// semi-infinite domain.
    /// </summary>
    public class DistributedPointSourceSDAForwardSolver : DiffusionForwardSolverBase // : SDAForwardSolver
    {
        private PointSourceSDAForwardSolver _pointSourceForwardSolver;

        public DistributedPointSourceSDAForwardSolver()
            : base(SourceConfiguration.Distributed, 0.0)
        {
            _pointSourceForwardSolver = new PointSourceSDAForwardSolver();
        }

        #region SteadyState
        /// <summary>
        /// Evaluation of the radially resolved stationary reflectance for the distribution of point sources in
        /// the source-image configuration.
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="rho">radial position</param>
        /// <param name="fr1">Fresnel Reflection Moment 1</param>
        /// <param name="fr2">Fresnel Reflection Moment 2</param>
        /// <returns>reflectance</returns>
        public override double StationaryReflectance(
            DiffusionParameters dp, double rho, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(
                StationaryFluence(rho, 0.0, dp),
                StationaryFlux(rho, 0.0, dp),
                fr1, fr2);
        }

        /// <summary>
        /// Evaluation of the radially resolved stationary fluence for the distribution of point
        /// sources under the source-image configuration.
        /// </summary>
        /// <param name="rho">radial position</param>
        /// <param name="z">depth position</param>
        /// <param name="dp">DiffusionParameters object</param>
        /// <returns>fluence</returns>
        public override double StationaryFluence(double rho, double z, DiffusionParameters dp)
        {
            var dpLocal = DiffusionParameters.Copy(dp);

            return CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                (zp) =>
                {
                    dpLocal.zp = zp;
                    return _pointSourceForwardSolver.StationaryFluence(rho, z, dpLocal);
                },
                dp.mutTilde) *
                dp.musTilde;
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
                dp.mutTilde) *
                dp.musTilde;
        }

        #endregion SteadyState

        #region Temporal Solutions
        /// <summary>
        /// Evaulation of the temporally resolved radial reflectance using the distribution of
        /// source-image point sources.
        /// </summary>
        /// <param name="dp">DiffusionParamters object</param>
        /// <param name="rho">radial location</param>
        /// <param name="t">time</param>
        /// <param name="fr1">Fresnel Moment 1</param>
        /// <param name="fr2">Fresnel Moment 2</param>
        /// <returns>reflectance</returns>
        public override double TemporalReflectance(DiffusionParameters dp, double rho, double t, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(
                TemporalFluence(dp, rho, 0.0, t),
                TemporalFlux(dp, rho, 0.0, t),
                fr1, fr2);
        }

        /// <summary>
        /// Evaluation of the temporally and radially resolved fluence rate using the distribution of
        /// source-image point sources.
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="t">time</param>
        /// <returns>fluence rate</returns>
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
                dp.musTilde;
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
                dp.musTilde;
        }

        #endregion Temporal Solutions

        #region Temporal Frequency Solutions

        /// <summary>
        /// Evaluates the temporal frequency radially resolved reflectance using the distribution of
        /// the source-image point source configuration.
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="rho">radial location</param>
        /// <param name="k">wavevector</param>
        /// <param name="fr1">Fresnel Moment 1</param>
        /// <param name="fr2">Fresnel Moment 2</param>
        /// <returns></returns>
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
                        _pointSourceForwardSolver.TemporalFrequencyReflectance(
                        dpLocal, r, kLocal, fr1, fr2);
                };

            return
                (CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                        zp => kernelFunc(dpLocalReal, rho, k, zp).Real,
                        dp.mutTilde)
                + Complex.ImaginaryOne *
                CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                        zp => kernelFunc(dpLocalImag, rho, k, zp).Imaginary,
                        dp.mutTilde)
                        ) *
                        dp.musTilde;

        }

        /// <summary>
        /// Evaluate the temporal frequency - radially resolved fluence with the distribution
        /// of point sources in the source-image configuration
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="k">wavevector</param>
        /// <returns>fluence rate</returns>
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
                (CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                        zp => kernelFunc(dpLocalReal, zp).Real,
                        dp.mutTilde)
                + Complex.ImaginaryOne *
                CalculatorToolbox.EvaluateDistributedExponentialLineSourceIntegral(
                        zp => kernelFunc(dpLocalImag, zp).Imaginary,
                        dp.mutTilde)
                        ) * dp.musTilde;
        }

        #endregion Temporal Frequency Solutions

    }
}