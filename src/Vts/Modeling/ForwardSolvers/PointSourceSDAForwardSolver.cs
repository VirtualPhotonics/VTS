using MathNet.Numerics;

namespace Vts.Modeling.ForwardSolvers
{
    public class PointSourceSDAForwardSolver : DiffusionForwardSolverBase
    {
        public PointSourceSDAForwardSolver()
            : base(SourceConfiguration.Point, 0.0) { }

        /// <summary>
        /// Evaluate the stationary radially resolved reflectance with the point source-image configuration
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="rho">radial location</param>
        /// <param name="fr1">Fresnel moment 1, R1</param>
        /// <param name="fr2">Fresnel moment 2, R2</param>
        /// <returns>reflectance</returns>
        public override double StationaryReflectance(DiffusionParameters dp, double rho, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(StationaryFluence(rho, 0.0, dp),
                StationaryFlux(rho, 0.0, dp),
                fr1, fr2);
        }

        /// <summary>
        /// Evaluate the stationary radially resolved fluence with the point source-image
        /// configuration
        /// </summary>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="dp">DiffusionParameters object</param>
        /// <returns>fluence</returns>
        public override double StationaryFluence(double rho, double z, DiffusionParameters dp)
        {
            return DiffusionGreensFunctions.StationaryPointSourceImageGreensFunction(dp,
                CalculatorToolbox.GetRadius(rho, z - dp.zp),
                CalculatorToolbox.GetRadius(rho, z + dp.zp + 2 * dp.zb));
        }

        /// <summary>
        /// Evaluate the stationary radially resolved z-flux with the point source-image
        /// configuration
        /// </summary>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="dp">DiffusionParamters object</param>
        /// <returns></returns>
        public double StationaryFlux(double rho, double z, DiffusionParameters dp)
        {
            var zSource = z - dp.zp;
            var zImage = z + dp.zp + 2 * dp.zb;
            return
                DiffusionGreensFunctions.StationaryPointSourceImageGreensFunctionZFlux(dp,
                    CalculatorToolbox.GetRadius(rho, zSource), zSource,
                    CalculatorToolbox.GetRadius(rho, zImage), zImage);
        }

        /// <summary>
        /// Evaluate the temporally-radially resolved reflectance with the point source-image
        /// configuration
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="rho">radial location</param>
        /// <param name="t">time</param>
        /// <param name="fr1">Fresnel Moment 1</param>
        /// <param name="fr2">Fresnel Moment 2</param>
        /// <returns>reflectance</returns>
        public override double TemporalReflectance(
            DiffusionParameters dp, double rho, double t, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(
                    TemporalFluence(dp, rho, 0.0, t),
                    TemporalFlux(dp, rho, 0.0, t),
                    fr1, fr2);
        }

        /// <summary>
        /// Evaluate the temporally-radially resolved fluence using the point source-image configuration
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="t">time</param>
        /// <returns>fluence</returns>
        public override double TemporalFluence(
            DiffusionParameters dp, double rho, double z, double t)
        {
            return DiffusionGreensFunctions.TemporalPointSourceImageGreensFunction(dp,
                    CalculatorToolbox.GetRadius(rho, z - dp.zp),
                    CalculatorToolbox.GetRadius(rho, z + dp.zp + 2 * dp.zb),
                    t);
        }

        /// <summary>
        /// Evaluate the temporally-radially resolved z-flux using the point source-image configuration
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="t">time</param>
        /// <returns></returns>
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
