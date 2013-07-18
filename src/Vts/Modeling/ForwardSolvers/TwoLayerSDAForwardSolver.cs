using System;
using System.Linq;
using Vts.MonteCarlo;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// This implements Kienle's two-layer SDA solutions described in:
    /// Kienle et al., "Noninvasive determination of the optical properties of two-layered
    /// turbid media", Applied Optics 37(4), 1998.
    /// </summary>
    public class TwoLayerSDAForwardSolver : DiffusionForwardSolverBase
    {
        public TwoLayerSDAForwardSolver()
            : base(SourceConfiguration.Point, 0.0) { }

        /// <summary>
        /// Evaluate the stationary radially resolved reflectance with the point source-image configuration
        /// </summary>
        /// <param name="dp">DiffusionParameters object for each tissue region</param>
        /// <param name="rho">radial location</param>
        /// <param name="fr1">Fresnel moment 1, R1</param>
        /// <param name="fr2">Fresnel moment 2, R2</param>
        /// <returns>reflectance</returns>
        public override double StationaryReflectance(DiffusionParameters[] dp, double[] tissueParameters,
            double rho, double fr1, double fr2)
        {
            return GetBackwardHemisphereIntegralDiffuseReflectance(StationaryFluence(rho, 0.0, dp, tissueParameters),
                StationaryFlux(rho, 0.0, dp, tissueParameters),
                fr1, fr2);
        }

        public override double StationaryReflectance(DiffusionParameters dp, double rho, double fr1, double fr2)
        {
            throw new NotImplementedException();
        }

        public override double TemporalReflectance(DiffusionParameters dp, double rho, double t, double fr1, double fr2)
        {
            throw new NotImplementedException();
        }

        public override System.Numerics.Complex TemporalFrequencyReflectance(DiffusionParameters dp, double rho, System.Numerics.Complex k, double fr1, double fr2)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Evaluate the stationary radially resolved fluence with the point source-image
        /// configuration
        /// </summary>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="tissueParameters">in this class, layer thickness</param>
        /// <returns>fluence</returns>
        public override double StationaryFluence(double rho, double z, DiffusionParameters[] dp, double[] tissueParameters)
        {
            var layerThickness = tissueParameters[0];
            var alpha1 = Math.Sqrt(dp[0].D * rho * rho * dp[0].mua);
            var alpha2 = Math.Sqrt(dp[1].D * rho * rho * dp[1].mua);
            var dum = ( Math.Sinh(alpha1*(dp[0].zb + dp[0].zp)/(dp[0].D*alpha1)) ) *
                       ( dp[0].D*alpha1*Math.Cosh(alpha1*(layerThickness - z)) +
                         dp[1].D*alpha2*Math.Sinh(alpha1*(layerThickness - z)) ) /
                       ( dp[0].D*alpha1*Math.Cosh(alpha1*(layerThickness + dp[0].zb)) +
                         dp[2].D*alpha2*Math.Sinh(alpha1*(layerThickness + dp[0].zb)) );
            if (z < dp[0].zp)
            {
                return dum - Math.Sinh(alpha1*(dp[0].zb - z))/(dp[0].D*alpha1);
            }
            return dum;
        }
        /// <summary>
        /// Evaluate the stationary radially resolved z-flux with the point source-image
        /// configuration
        /// </summary>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="dp">DiffusionParamters object</param>
        /// <returns></returns>
        public double StationaryFlux(double rho, double z, DiffusionParameters[] dp, double[] tissueParameters)
        {
            var layerThickness = tissueParameters[0];
            var alpha1 = Math.Sqrt(dp[0].D * rho * rho * dp[0].mua);
            var alpha2 = Math.Sqrt(dp[1].D * rho * rho * dp[1].mua);
            var dum = (Math.Sinh(alpha1 * (dp[0].zb + dp[0].zp) / (dp[0].D * alpha1))) *
                       (-dp[0].D * alpha1 * alpha1 * Math.Sinh(alpha1 * (layerThickness - z)) -
                         dp[1].D * alpha2 * alpha1 * Math.Cosh(alpha1 * (layerThickness - z))) /
                       ( dp[0].D * alpha1 * Math.Cosh(alpha1 * (layerThickness + dp[0].zb)) +
                         dp[2].D * alpha2 * Math.Sinh(alpha1 * (layerThickness + dp[0].zb)));
            if (z < dp[0].zp)
            {
                return dum + Math.Cosh(alpha1 * (dp[0].zb - z)) / (dp[0].D * alpha1);
            }
            return dum;
        }

        public override double TemporalFluence(DiffusionParameters dp, double rho, double z, double t)
        {
            throw new NotImplementedException();
        }

        public override System.Numerics.Complex TemporalFrequencyFluence(DiffusionParameters dp, double rho, double z, System.Numerics.Complex k)
        {
            throw new NotImplementedException();
        }
    }
}
