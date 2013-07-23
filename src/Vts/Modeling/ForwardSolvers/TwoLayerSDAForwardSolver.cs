using System;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// This implements Kienle's two-layer SDA solutions described in:
    /// Kienle et al., "Noninvasive determination of the optical properties of two-layered
    /// turbid media", Applied Optics 37(4), 1998.
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
            return (1 - fr1) / 4 * StationaryFluence(rho, 0.0, dp, layerThicknesses) + 
                (fr2 - 1) /2 * StationaryFlux(rho, 0.0, dp, layerThicknesses);
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
                double scaledHankelPoint = hankelPoints[i] / rho;
                double scaledHankelPointSq = scaledHankelPoint * scaledHankelPoint;
                var alpha1 = Math.Sqrt(dp[0].D * scaledHankelPointSq * dp[0].mua);
                var alpha2 = Math.Sqrt(dp[1].D * scaledHankelPointSq * dp[1].mua);
                var dum = (Math.Sinh(alpha1 * (dp[0].zb + dp[0].zp) / (dp[0].D * alpha1))) *
                          (dp[0].D * alpha1 * Math.Cosh(alpha1 * (layerThickness - z)) +
                           dp[1].D * alpha2 * Math.Sinh(alpha1 * (layerThickness - z))) /
                          (dp[0].D * alpha1 * Math.Cosh(alpha1 * (layerThickness + dp[0].zb)) +
                           dp[1].D * alpha2 * Math.Sinh(alpha1 * (layerThickness + dp[0].zb)));
                if (z < dp[0].zp)
                {
                    fluence += (dum - Math.Sinh(alpha1 * (dp[0].zb - z)) / (dp[0].D * alpha1)) * hankelWeights[i];
                }
                else
                {
                    fluence += dum*hankelWeights[i];
                }
            }
            return fluence / rho; // scale back...
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
                var alpha1 = Math.Sqrt(dp[0].D*scaledHankelPointSq*dp[0].mua);
                var alpha2 = Math.Sqrt(dp[1].D*scaledHankelPointSq*dp[1].mua);
                var dum = (Math.Sinh(alpha1*(dp[0].zb + dp[0].zp)/(dp[0].D*alpha1)))*
                          (-dp[0].D*alpha1*alpha1*Math.Sinh(alpha1*(layerThickness - z)) -
                           dp[1].D*alpha2*alpha1*Math.Cosh(alpha1*(layerThickness - z)))/
                          (dp[0].D*alpha1*Math.Cosh(alpha1*(layerThickness + dp[0].zb)) +
                           dp[1].D*alpha2*Math.Sinh(alpha1*(layerThickness + dp[0].zb)));
                if (z < dp[0].zp)
                {
                    flux += (dum + Math.Cosh(alpha1*(dp[0].zb - z))/(dp[0].D*alpha1))*hankelWeights[i];
                }
                else
                {
                    flux += dum*hankelWeights[i];
                }
            }
            return flux/rho;
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
