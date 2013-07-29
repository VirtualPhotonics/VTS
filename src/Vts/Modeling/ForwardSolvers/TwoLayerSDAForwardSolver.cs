using System;
using System.Numerics;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// This implements Kienle's two-layer SDA solutions described in:
    /// 1) Kienle et al., "Noninvasive determination of the optical properties of two-layered
    /// turbid media", Applied Optics 37(4), 1998.
    /// 2) Kienle et al., "In vivo determination of the optical properties of muscle with time-
    /// resolved reflectance using a layered model, Phys. Med. Biol. 44, 1999 (in particular, the
    /// appendix)
    /// Notes:
    /// 1) this solution assumes that the embedded source is within top layer.
    /// 2) zp = location of embedded isotropic source is determined using layer 1 opt. props.
    /// 3) zb = extrapolated boundary is determined using layer 1 opt. props.
    /// </summary>
    public class TwoLayerSDAForwardSolver : ForwardSolverBase
    {     
        /// <summary>
        /// Returns an instance of TwoLayerSDAForwardSolver
        /// </summary>
        public TwoLayerSDAForwardSolver() :
            base(SourceConfiguration.Point, 0.0)
        {
        }

        // this assumes: first region in ITissueRegion[] is top layer of tissue because need to know what OPs 
        // to use for FresnelReflection and so I can define layer thicknesses
        public override double ROfRho(ITissueRegion[] regions, double rho)
        {
            // get ops of top tissue region
            var op0 = regions[0].RegionOP;
            var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op0.N);
            var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op0.N);

            var diffusionParameters = GetDiffusionParameters(regions);
            var layerThicknesses = GetLayerThicknesses(regions);

            // check that embedded source is within top layer, otherwise solution invalid
            if (diffusionParameters[0].zp > layerThicknesses[0])
            {
                throw new ArgumentException("Top layer thickness must be greater than l* = 1/(mua+musp)");
            }

            return StationaryReflectance(diffusionParameters, layerThicknesses, rho, fr1, fr2);
        }

        public override Complex ROfRhoAndFt(ITissueRegion[] regions, double rho, double ft)
        {
            // get ops of top tissue region
            var op0 = regions[0].RegionOP;
            var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op0.N);
            var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op0.N);

            var diffusionParameters = GetDiffusionParameters(regions);
            var layerThicknesses = GetLayerThicknesses(regions);

            // check that embedded source is within top layer, otherwise solution invalid
            if (diffusionParameters[0].zp > layerThicknesses[0])
            {
                throw new ArgumentException("Top layer thickness must be greater than l* = 1/(mua+musp)");
            }

            return TemporalFrequencyReflectance(diffusionParameters, layerThicknesses, rho, ft, fr1, fr2);
        }
        private static DiffusionParameters[] GetDiffusionParameters(ITissueRegion[] regions)
        {
            var diffusionParameters = new DiffusionParameters[regions.Length];
            for (int i = 0; i < regions.Length; i++)
            {
                diffusionParameters[i] = DiffusionParameters.Create(
                    new OpticalProperties(regions[i].RegionOP.Mua, regions[i].RegionOP.Musp, regions[i].RegionOP.G, regions[i].RegionOP.N),
                    ForwardModel.SDA);
            }
            return diffusionParameters;
        }

        private static double[] GetLayerThicknesses(ITissueRegion[] regions)
        {
            var layerThicknesses = new double[regions.Length];
            for (int i = 0; i < regions.Length; i++)
            {
                layerThicknesses[i] = ((LayerRegion)regions[i]).ZRange.Stop;
            }
            return layerThicknesses;
        }

        /// <summary>
        /// Evaluate the stationary radially resolved reflectance with the point source-image configuration
        /// </summary>
        /// <param name="dp">DiffusionParameters object for each tissue region</param>
        /// <param name="rho">radial location</param>
        /// <param name="fr1">Fresnel moment 1, R1</param>
        /// <param name="fr2">Fresnel moment 2, R2</param>
        /// <returns>reflectance</returns>
        private static double StationaryReflectance(DiffusionParameters[] dp, double[] layerThicknesses,
                                            double rho, double fr1, double fr2)
        {
            // this could use GetBackwardHemisphereIntegralDiffuseReflectance possibly? no protected
            return (1 - fr1) / 4 * StationaryFluence(rho, 0.0, dp, layerThicknesses) -
                (fr2 - 1) / 2 * dp[0].D * StationaryFlux(rho, 0.0, dp, layerThicknesses);
        }

        /// <summary>
        /// Evaluate the stationary radially resolved fluence with the point source-image
        /// configuration
        /// </summary>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="dp">DiffusionParameters for layer 1 and 2</param>
        /// <param name="layerThicknesses">in this class, layer thickness</param>
        /// <returns>fluence</returns>
        private static double StationaryFluence(double rho, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            double fluence;
            if (z < layerThickness) // top layer phi1 solution
            {
                fluence = HankelTransform.DigitalFilterOfOrderZero(
                    rho, s => Phi1(s, z, dp, layerThicknesses));
            }
            else // bottome layer phi2 solution
            {
                fluence = HankelTransform.DigitalFilterOfOrderZero(
                    rho, s => Phi2(s, z, dp, layerThicknesses));
            }           
            return fluence/(2*Math.PI); 
        }

        /// <summary>
        /// Evaluate the stationary radially resolved z-flux with the point source-image
        /// configuration
        /// </summary>
        /// <param name="rho">radial location</param>
        /// <param name="z">depth location</param>
        /// <param name="dp">DiffusionParameters for layer 1 and 2</param>
        /// <param name="layerThicknesses">thickness of top layer, array but only need first element</param>
        /// <returns></returns>
        private static double StationaryFlux(double rho, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            double flux;
            if (z < layerThickness) // top layer dphi1/dz solution
            {
                flux = HankelTransform.DigitalFilterOfOrderZero(
                            rho, s => dPhi1(s, z, dp, layerThicknesses));
            }
            else // bottom layer phi2/dz solution
            {
                flux = HankelTransform.DigitalFilterOfOrderZero(
                            rho, s => dPhi2(s, z, dp, layerThicknesses));
            }
            return flux/(2*Math.PI); 
        }
        public static Complex TemporalFrequencyReflectance(
            DiffusionParameters[] dp, double[] layerThicknesses, double rho, double temporalFrequency, double fr1, double fr2)
        {
            return (1 - fr1) / 4 * TemporalFrequencyFluence(rho, 0.0, temporalFrequency, dp, layerThicknesses) -
                (fr2 - 1) / 2 * dp[0].D * TemporalFrequencyZFlux(rho, 0.0, temporalFrequency, dp, layerThicknesses);
        }

        public static Complex TemporalFrequencyFluence(double rho,
            double z, double temporalFrequency, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            Complex fluence;
            if (z < layerThickness) // top layer phi1 solution
            {
                fluence = HankelTransform.DigitalFilterOfOrderZero(
                    rho, s => TemporalFrequencyPhi1(s, z, temporalFrequency, dp, layerThicknesses).Real) + 
                          HankelTransform.DigitalFilterOfOrderZero(
                    rho, s => TemporalFrequencyPhi1(s, z, temporalFrequency, dp, layerThicknesses).Imaginary) *
                    Complex.ImaginaryOne;
            }
            else // bottome layer phi2 solution
            {
                fluence = HankelTransform.DigitalFilterOfOrderZero(
                    rho, s => TemporalFrequencyPhi2(s, z, temporalFrequency, dp, layerThicknesses).Real) + 
                          HankelTransform.DigitalFilterOfOrderZero(
                    rho,s => TemporalFrequencyPhi2(s, z, temporalFrequency, dp, layerThicknesses).Imaginary) *
                    Complex.ImaginaryOne;
            }
            return fluence / (2 * Math.PI);
        }

        public static Complex TemporalFrequencyZFlux(double rho, double z, double temporalFrequency,
            DiffusionParameters[] dp, double[] layerThicknesses )
        {
            var layerThickness = layerThicknesses[0];
            Complex flux;
            if (z < layerThickness) // top layer dphi1/dz solution
            {
                flux = HankelTransform.DigitalFilterOfOrderZero(
                            rho, s => TemporalFrequencydPhi1(s, z, temporalFrequency, dp, layerThicknesses).Real) + 
                       HankelTransform.DigitalFilterOfOrderZero(
                            rho, s => TemporalFrequencydPhi1(s, z, temporalFrequency, dp, layerThicknesses).Imaginary) *
                            Complex.ImaginaryOne;
            }
            else // bottom layer phi2/dz solution
            {
                flux = HankelTransform.DigitalFilterOfOrderZero(
                            rho, s => TemporalFrequencydPhi2(s, z, temporalFrequency, dp, layerThicknesses).Real) + 
                       HankelTransform.DigitalFilterOfOrderZero(
                            rho, s => TemporalFrequencydPhi2(s, z, temporalFrequency, dp, layerThicknesses).Imaginary) *
                            Complex.ImaginaryOne;
            }
            return flux / (2 * Math.PI);
        }
        // Note that the "guts" of Phi1 and TemporalFrequencyPhi1, and all other pairs, are equivalent,
        // the only difference is changing alpha1 and alpha2 to be comples
        private static double Phi1(double s, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            var alpha1 = Math.Sqrt((dp[0].D * s * s + dp[0].mua) / dp[0].D);
            var alpha2 = Math.Sqrt((dp[1].D * s * s + dp[1].mua) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var dum1 = Math.Exp(-alpha1 * (dp[0].zp - z)) -
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + z));
            var dum2 = Math.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness - z)) -
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + 2 * layerThickness - z)) -
                       Math.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness + 2 * dp[0].zb + z)) +
                       Math.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 2 * layerThickness + z));
            return (dum1 + Da * dum2) / (2 * dp[0].D * alpha1);
        }
        private static Complex TemporalFrequencyPhi1(double s, double z, double temporalFrequency, 
            DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            var alpha1 = Complex.Sqrt((dp[0].D * s * s + dp[0].mua + 2 * Math.PI * temporalFrequency * Complex.ImaginaryOne / dp[0].cn) / dp[0].D);
            var alpha2 = Complex.Sqrt((dp[1].D * s * s + dp[1].mua + 2 * Math.PI * temporalFrequency * Complex.ImaginaryOne / dp[0].cn) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var dum1 = Complex.Exp(-alpha1 * (dp[0].zp - z)) -
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + z));
            var dum2 = Complex.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness - z)) -
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + 2 * layerThickness - z)) -
                       Complex.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness + 2 * dp[0].zb + z)) +
                       Complex.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 2 * layerThickness + z));
            return (dum1 + Da * dum2) / (2 * dp[0].D * alpha1);
        }
        private static double Phi2(double s, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var alpha1 = Math.Sqrt((dp[0].D * s * s + dp[0].mua) / dp[0].D);
            var alpha2 = Math.Sqrt((dp[1].D * s * s + dp[1].mua) / dp[1].D);
            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum3 = Math.Exp(alpha2 * (layerThickness - z)) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum4 = Math.Exp(alpha1 * (dp[0].zp - layerThickness)) -
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + layerThickness));
            var dum5 = Math.Exp(alpha1 * (dp[0].zp - 3 * layerThickness - 2 * dp[0].zb)) -
                       Math.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 3 * layerThickness));
            return dum3 * (dum4 - Da * dum5);
        }
        private static Complex TemporalFrequencyPhi2(double s, double z, double temporalFrequency, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var alpha1 = Complex.Sqrt((dp[0].D * s * s + dp[0].mua + 2 * Math.PI * temporalFrequency * Complex.ImaginaryOne / dp[0].cn) / dp[0].D);
            var alpha2 = Complex.Sqrt((dp[1].D * s * s + dp[1].mua + 2 * Math.PI * temporalFrequency * Complex.ImaginaryOne / dp[0].cn) / dp[1].D);
            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum3 = Complex.Exp(alpha2 * (layerThickness - z)) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum4 = Complex.Exp(alpha1 * (dp[0].zp - layerThickness)) -
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + layerThickness));
            var dum5 = Complex.Exp(alpha1 * (dp[0].zp - 3 * layerThickness - 2 * dp[0].zb)) -
                       Complex.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 3 * layerThickness));
            return dum3 * (dum4 - Da * dum5);
        }
        private static double dPhi1(double s, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var alpha1 = Math.Sqrt((dp[0].D * s * s + dp[0].mua) / dp[0].D);
            var alpha2 = Math.Sqrt((dp[1].D * s * s + dp[1].mua) / dp[1].D);

            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum1 = Math.Exp(-alpha1 * (dp[0].zp - z)) +
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + z));
            var dum2 = Math.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness - z)) -
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + 2 * layerThickness - z)) +
                       Math.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness + 2 * dp[0].zb + z)) -
                       Math.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 2 * layerThickness + z));
            return ((dum1 + Da * dum2) / (2 * dp[0].D));
        }
        private static Complex TemporalFrequencydPhi1(double s, double z, double temporalFrequency, 
            DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var alpha1 = Complex.Sqrt((dp[0].D * s * s + dp[0].mua + 2 * Math.PI * temporalFrequency * Complex.ImaginaryOne / dp[0].cn) / dp[0].D);
            var alpha2 = Complex.Sqrt((dp[1].D * s * s + dp[1].mua + 2 * Math.PI * temporalFrequency * Complex.ImaginaryOne / dp[0].cn) / dp[1].D);

            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum1 = Complex.Exp(-alpha1 * (dp[0].zp - z)) +
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + z));
            var dum2 = Complex.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness - z)) -
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + 2 * layerThickness - z)) +
                       Complex.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness + 2 * dp[0].zb + z)) -
                       Complex.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 2 * layerThickness + z));
            return ((dum1 + Da * dum2) / (2 * dp[0].D));
        }
        private static double dPhi2(double s, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var alpha1 = Math.Sqrt((dp[0].D * s * s + dp[0].mua) / dp[0].D);
            var alpha2 = Math.Sqrt((dp[1].D * s * s + dp[1].mua) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum3 = Math.Exp(alpha2 * (layerThickness - z)) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum4 = Math.Exp(alpha1 * (dp[0].zp - layerThickness)) -
                       Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + layerThickness));
            var dum5 = Math.Exp(alpha1 * (dp[0].zp - 3 * layerThickness - 2 * dp[0].zb)) -
                       Math.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 3 * layerThickness));
            return -alpha2 * (dum3 * (dum4 - Da * dum5));
        }
        private static Complex TemporalFrequencydPhi2(double s, double z, double temporalFrequency, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
            var alpha1 = Complex.Sqrt((dp[0].D * s * s + dp[0].mua + 2 * Math.PI * temporalFrequency * Complex.ImaginaryOne / dp[0].cn) / dp[0].D);
            var alpha2 = Complex.Sqrt((dp[1].D * s * s + dp[1].mua + 2 * Math.PI * temporalFrequency * Complex.ImaginaryOne / dp[0].cn) / dp[1].D);
            var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum3 = Complex.Exp(alpha2 * (layerThickness - z)) / (dp[0].D * alpha1 + dp[1].D * alpha2);
            var dum4 = Complex.Exp(alpha1 * (dp[0].zp - layerThickness)) -
                       Complex.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + layerThickness));
            var dum5 = Complex.Exp(alpha1 * (dp[0].zp - 3 * layerThickness - 2 * dp[0].zb)) -
                       Complex.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 3 * layerThickness));
            return -alpha2 * (dum3 * (dum4 - Da * dum5));
        }
    }
}
