using System;
using Vts.Common;
using Vts.IO;
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
        private static DiffusionParameters[] _diffusionParameters;
        private static double[] _layerThicknesses;
        const int dataLength = 801;
        static double[] hankelPoints = new double[dataLength];
        static double[] hankelWeights = new double[dataLength];

        public TwoLayerSDAForwardSolver(ITissueRegion[] regions) : 
            base(SourceConfiguration.Point, 0.0)
        {
            ConvertToTwoLayerParameters(regions);
            // check that embedded source is within top layer, otherwise solution invalid
            if (_diffusionParameters[0].zp > _layerThicknesses[0])
            {
                throw new ArgumentException("Top layer thickness must be greater than l* = 1/(mua+musp)");
            }
            //read input stuff
            string projectName = "Vts";
            string dataLocation = "Modeling/Resources/HankelData/";
            hankelPoints = (double[])FileIO.ReadArrayFromBinaryInResources<double>
               (dataLocation + @"basepoints.dat", projectName, dataLength);
            hankelWeights = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                (dataLocation + @"hankelweights.dat", projectName, dataLength);
        }

        public TwoLayerSDAForwardSolver() :
            this(new LayerRegion[] {new LayerRegion(new DoubleRange(0, 100), new OpticalProperties())}) {}

        public static void ConvertToTwoLayerParameters(ITissueRegion[] regions)
        {
            _diffusionParameters = new DiffusionParameters[regions.Length];
            _layerThicknesses = new double[regions.Length];
            for (int i = 0; i < regions.Length; i++)
            {
                _diffusionParameters[i] = DiffusionParameters.Create(
                    new OpticalProperties(
                        regions[i].RegionOP.Mua, regions[i].RegionOP.Musp, regions[i].RegionOP.G, regions[i].RegionOP.N),
                        ForwardModel.SDA);
                _layerThicknesses[i] = ((LayerRegion)regions[i]).ZRange.Stop;
            } 
        }
        /// <summary>
        /// Evaluate the stationary radially resolved reflectance with the point source-image configuration
        /// </summary>
        /// <param name="dp">DiffusionParameters object for each tissue region</param>
        /// <param name="rho">radial location</param>
        /// <param name="fr1">Fresnel moment 1, R1</param>
        /// <param name="fr2">Fresnel moment 2, R2</param>
        /// <returns>reflectance</returns>
        public double StationaryReflectance(DiffusionParameters[] dp, double[] layerThicknesses,
                                            double rho, double fr1, double fr2)
        {
            // this could use GetBackwardHemisphereIntegralDiffuseReflectance possibly?
            return (1 - fr1) / 4 * StationaryFluence(rho, 0.0, dp, layerThicknesses) -
                (fr2 - 1) /2 * dp[0].D * StationaryFlux(rho, 0.0, dp, layerThicknesses);
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
        public double StationaryFluence(double rho, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            double fluence = 0.0;
            for (int i = 0; i < hankelPoints.Length; i++)
            {
                double scaledHankelPoint = hankelPoints[i]/rho; // divide by rho?
                double scaledHankelPointSq = scaledHankelPoint * scaledHankelPoint;
                var alpha1 = Math.Sqrt((dp[0].D * scaledHankelPointSq + dp[0].mua)/dp[0].D);
                var alpha2 = Math.Sqrt((dp[1].D * scaledHankelPointSq + dp[1].mua)/dp[1].D);
                // use approximation to avoid numerical errors ref: Kienle et al., Phys. Med. Biol. 44, 1999
                var Da = (dp[0].D*alpha1 - dp[1].D*alpha2)/(dp[0].D*alpha1 + dp[1].D*alpha2);
                var dum1 = Math.Exp(-alpha1*(dp[0].zp - z)) -
                           Math.Exp(-alpha1*(2*dp[0].zb + dp[0].zp + z));
                var dum2 = Math.Exp(-alpha1*(-dp[0].zp + 2*layerThickness - z)) -
                           Math.Exp(-alpha1*(2*dp[0].zb + dp[0].zp + 2*layerThickness - z)) -
                           Math.Exp(-alpha1*(-dp[0].zp + 2*layerThickness + 2*dp[0].zb + z)) +
                           Math.Exp(-alpha1*(4*dp[0].zb + dp[0].zp + 2*layerThickness + z));
                if (z < layerThickness) // phi1 solution
                {
                    // in this formulation phi1 for 0<z<zb and zb<z<l are the same
                    fluence += ((dum1 + Da * dum2) / (2*dp[0].D*alpha1)) * hankelWeights[i];
                }
                else // phi2 solution
                {
                    var dum3 = Math.Exp(alpha2*(layerThickness - z))/(dp[0].D*alpha1 + dp[1].D*alpha2);
                    var dum4 = Math.Exp(alpha1*(dp[0].zp - layerThickness)) -
                               Math.Exp(-alpha1*(2*dp[0].zb + dp[0].zp + layerThickness));
                    var dum5 = Math.Exp(alpha1*(dp[0].zp - 3*layerThickness - 2*dp[0].zb)) -
                               Math.Exp(-alpha1*(4*dp[0].zb + dp[0].zp + 3*layerThickness));
                    fluence += dum3*(dum4-Da*dum5)*hankelWeights[i];
                }
            }
            return fluence/(2*Math.PI*rho); // divide by rho?
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
        public double StationaryFlux(double rho, double z, DiffusionParameters[] dp, double[] layerThicknesses)
        {
            var layerThickness = layerThicknesses[0];
            double flux = 0.0;
            for (int i = 0; i < hankelPoints.Length; i++)
            {
                double scaledHankelPoint = hankelPoints[i]/rho;
                double scaledHankelPointSq = scaledHankelPoint*scaledHankelPoint;
                var alpha1 = Math.Sqrt((dp[0].D*scaledHankelPointSq+dp[0].mua)/dp[0].D);
                var alpha2 = Math.Sqrt((dp[1].D*scaledHankelPointSq+dp[1].mua)/dp[1].D);
                // use approximation to avoid numerical errors ref: Kienle et al., Phys. Med. Biol. 44, 1999
                var Da = (dp[0].D * alpha1 - dp[1].D * alpha2) / (dp[0].D * alpha1 + dp[1].D * alpha2);
                var dum1 = Math.Exp(-alpha1 * (dp[0].zp - z)) +
                           Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + z));
                var dum2 = Math.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness - z)) -
                           Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + 2 * layerThickness - z)) +
                           Math.Exp(-alpha1 * (-dp[0].zp + 2 * layerThickness + 2 * dp[0].zb + z)) -
                           Math.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 2 * layerThickness + z));
                if (z < layerThickness) // phi1 solution
                {
                    // in this formulation phi1 for 0<z<zb and zb<z<l are the same, so dphi1/dz are same
                    flux += ((dum1 + Da * dum2) / (2 * dp[0].D)) * hankelWeights[i];
                }
                else // phi2 solution
                {
                    var dum3 = Math.Exp(alpha2 * (layerThickness - z)) / (dp[0].D * alpha1 + dp[1].D * alpha2);
                    var dum4 = Math.Exp(alpha1 * (dp[0].zp - layerThickness)) -
                               Math.Exp(-alpha1 * (2 * dp[0].zb + dp[0].zp + layerThickness));
                    var dum5 = Math.Exp(alpha1 * (dp[0].zp - 3 * layerThickness - 2 * dp[0].zb)) -
                               Math.Exp(-alpha1 * (4 * dp[0].zb + dp[0].zp + 3 * layerThickness));
                    flux += -alpha2 * (dum3 * (dum4 - Da * dum5)) * hankelWeights[i];
                 }
            }
            return flux/(2*Math.PI*rho); // divide by rho?
        }
        // this assumes: 
        // first region in ITissueRegion[] is top layer of tissue because need to know what OPs 
        // to use for FresnelReflection and so I can define layer thicknesses
        public override double ROfRho(ITissueRegion[] regions, double rho)
        {
            // get ops of top tissue region
            var op0 = regions[0].RegionOP;
            var fr1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(op0.N);
            var fr2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(op0.N);
            ConvertToTwoLayerParameters(regions);
            return StationaryReflectance(_diffusionParameters, _layerThicknesses, rho, fr1, fr2);
        }
    }
}
