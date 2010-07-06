using System;
using MathNet.Numerics;

namespace Vts.Modeling.ForwardSolvers
{
    public class PointSourceDiffusionForwardSolver : DiffusionForwardSolverBase
    {
        public PointSourceDiffusionForwardSolver()
            : base(SourceConfiguration.Point, 0.0) { }

        public override double StationaryReflectance(DiffusionParameters dp, double rho, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(StationaryFluence(rho, 0.0, dp),
                StationaryFlux(rho, 0.0, dp),
                fr1, fr2);
        }

        public override double StationaryFluence(double rho, double z, DiffusionParameters dp)
        {
            return DiffusionGreensFunctions.StationaryPointSourceImageGreensFunction(dp,
                CalculatorToolbox.GetRadius(rho, z - dp.zp),
                CalculatorToolbox.GetRadius(rho, z + dp.zp + 2 * dp.zb));
        }

        public double StationaryFlux(double rho, double z, DiffusionParameters dp)
        {
            var zSource = z - dp.zp;
            var zImage = z + dp.zp + 2 * dp.zb;
            return
                DiffusionGreensFunctions.StationaryPointSourceImageGreensFunctionZFlux(dp,
                    CalculatorToolbox.GetRadius(rho, z - dp.zp), zSource,
                    CalculatorToolbox.GetRadius(rho, z + dp.zp + 2 * dp.zb), zImage);
        }

        public override double TemporalReflectance(
            DiffusionParameters dp, double rho, double t, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(
                    TemporalFluence(dp, rho, 0.0, t),
                    TemporalFlux(dp, rho, 0.0, t),
                    fr1, fr2);
        }

        public override double TemporalFluence(
            DiffusionParameters dp, double rho, double z, double t)
        {
            return DiffusionGreensFunctions.TemporalPointSourceImageGreensFunction(dp,
                    CalculatorToolbox.GetRadius(rho, z - dp.zp),
                    CalculatorToolbox.GetRadius(rho, z + dp.zp + 2 * dp.zb),
                    t);
        }

        public double TemporalFlux(
            DiffusionParameters dp, double rho, double z, double t)
        {
            var zSource = z - dp.zp;
            var zImage = z + dp.zp + 2 * dp.zb;

            return DiffusionGreensFunctions.TemporalPointSourceImageGreensFunctionZFlux(dp,
                CalculatorToolbox.GetRadius(rho, zSource), zSource,
                CalculatorToolbox.GetRadius(rho, zImage), zImage, 
                t);
        }

        public override Complex TemporalFrequencyReflectance(
            DiffusionParameters dp, double rho, Complex k, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(
                TemporalFrequencyFluence(dp, rho, 0.0, k),
                TemporalFrequencyZFlux(dp, rho, 0.0, k),
                fr1, fr2);

        }

        public override Complex TemporalFrequencyFluence(DiffusionParameters dp, double rho,
            double z, Complex k)
        {
            return DiffusionGreensFunctions.TemporalFrequencyPointSourceImageGreensFunction(dp,
                CalculatorToolbox.GetRadius(rho, z - dp.zp),
                CalculatorToolbox.GetRadius(rho, z + dp.zp + 2 * dp.zb),
                k);
        }

        public Complex TemporalFrequencyZFlux(
            DiffusionParameters dp, double rho, double z, Complex k)
        {
            var zSource = z - dp.zp;
            var zImage = z + dp.zp + 2 * dp.zb;

            return DiffusionGreensFunctions.TemporalFrequencyPointSourceImageGreensFunctionZFlux(dp,
                CalculatorToolbox.GetRadius(rho, zSource), zSource,
                CalculatorToolbox.GetRadius(rho, zImage), zImage,
                k);
        }

    }
}
