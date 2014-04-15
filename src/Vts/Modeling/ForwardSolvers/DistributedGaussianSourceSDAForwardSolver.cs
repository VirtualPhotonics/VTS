using System;
using MathNet.Numerics;
using Vts.IO;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Evaluation of the distributed Gaussian diffusion forward solver. This model is a specific
    /// mathematical derivation in the stationary case for semi-infinite media.
    /// </summary>
    public class DistributedGaussianSourceSDAForwardSolver : DiffusionForwardSolverBase // : SDAForwardSolver
    {
        public DistributedGaussianSourceSDAForwardSolver()
            : base(SourceConfiguration.Gaussian, 1.0) { }

        public DistributedGaussianSourceSDAForwardSolver(double diameter)
            : base(SourceConfiguration.Gaussian, diameter) { }


        #region old implementation
        const int dataLength = 801;
        static double[] hankelPoints = new double[dataLength];
        static double[] hankelWeights = new double[dataLength];

        static DistributedGaussianSourceSDAForwardSolver()
        {
            //read input stuff
            string projectName = "Vts";
            string dataLocation = "Modeling/Resources/HankelData/";
            hankelPoints = (double[])FileIO.ReadArrayFromBinaryInResources<double>
               (dataLocation + @"basepoints.dat", projectName, dataLength);
            hankelWeights = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                (dataLocation + @"hankelweights.dat", projectName, dataLength);
        }

        /// <summary>
        /// Evaluation of the reflectance according to Carp et al. FILL IN (2004).
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="rho">radial location</param>
        /// <param name="fr1">First Fresnel Reflection Moment, not applied here</param>
        /// <param name="fr2">Second Fresnel Reflection Moment, not applied here</param>
        /// <returns></returns>
        public override double StationaryReflectance(DiffusionParameters dp, double rho,
            double fr1, double fr2)
        {
            var normFactor = 8 / (Math.PI * BeamDiameter * BeamDiameter) / (2 * dp.A);
            //var surfaceFluence = 
            //return normFactor * surfaceFluence;
            return normFactor * SteadyStateGaussianBeamSurfaceFluence(dp, BeamDiameter, rho);
        }

        public static double SteadyStateGaussianBeamSurfaceFluence(DiffusionParameters dp,
                   double diam, double rho)
        {
            var sqDiamOver8 = diam * diam / 8;
            var hprime = -1 / (2 * dp.A * dp.D);
            var lambda = 3 * dp.musTilde * (dp.mutr + dp.gTilde * dp.mutTilde);
            if (rho <= 0)
            {
                rho = 1e-9;
            }

            double fluence = 0.0;
            for (int i = 0; i < hankelPoints.Length; i++)
            {
                double scaledHankelPoint = hankelPoints[i] / rho;
                double scaledHankelPointSq = scaledHankelPoint * scaledHankelPoint;
                double sqrtArg = Math.Sqrt(scaledHankelPointSq + dp.mueff * dp.mueff);
                double exp1mod = Math.Exp(-sqDiamOver8 / 4 * scaledHankelPointSq) * scaledHankelPoint;
                double denom1 = 2 * (scaledHankelPointSq + dp.mueff * dp.mueff - dp.mutTilde * dp.mutTilde);
                double phi1 = 3 / 2 * dp.gTilde * dp.musTilde * sqDiamOver8 * exp1mod / (hprime - sqrtArg);
                double phi2 = lambda * sqDiamOver8 * exp1mod * (dp.mutTilde - hprime) / (denom1 * (hprime - sqrtArg));
                double phi3 = lambda * sqDiamOver8 * exp1mod / denom1;
                fluence += (phi1 + phi2 + phi3) * hankelWeights[i];

            }
            return fluence / rho; // scale back...
        }

        public override double StationaryFluence(double rho, double z, DiffusionParameters dp)
        {
            var sqDiamOver8 = BeamDiameter * BeamDiameter / 8;

            var hprime = -1 / (2 * dp.A * dp.D); // = -1/dp.zb
            var lambda = 3 * dp.musTilde * (dp.mutr + dp.gTilde * dp.mutTilde);
            double expVar3 = Math.Exp(-dp.mutTilde * z);

            var rhoInternal = Math.Abs(rho);
            if (rhoInternal <= 0)
                rhoInternal = 1e-9;

            double fluence = 0.0;
            for (int i = 0; i < hankelPoints.Length; i++)
            {
                double scaledHankelPoint = hankelPoints[i] / rhoInternal;
                double scaledHankelPointSq = scaledHankelPoint * scaledHankelPoint;
                double sqrtArg = Math.Sqrt(scaledHankelPointSq + dp.mueff * dp.mueff);
                double expVar1 = Math.Exp(-sqDiamOver8 / 4 * scaledHankelPointSq) * scaledHankelPoint;
                double expVar2 = Math.Exp(-sqrtArg * z);
                double denom1 = 2 * (scaledHankelPointSq + dp.mueff * dp.mueff - dp.mutTilde * dp.mutTilde);
                double phi1 = 3 / 2 * dp.gTilde * dp.musTilde * expVar2 / (hprime - sqrtArg);
                double phi2 = lambda * (dp.mutTilde - hprime) * expVar2 / (denom1 * (hprime - sqrtArg));
                double phi3 = lambda * expVar3 / denom1;
                fluence += (phi1 + phi2 + phi3) * sqDiamOver8 * expVar1 * hankelWeights[i];
            }
            //fluence /= rhoInternal; // scale back... diffuse component only...
            return fluence / rhoInternal +
                Math.Exp(-rhoInternal * rhoInternal / sqDiamOver8) * Math.Exp(-dp.mutTilde * z); //diffuse plus collimated
        }

        #endregion old implementation



        #region new implementation
        //public override double StationaryReflectance(DiffusionParameters dp, double rho, double fr1, double fr2)
        //{
        //    return DiffusionGaussianBeamFluence(dp, rho, 0.0) / 2 / dp.A;
        //}

        //public override double StationaryFluence(double rho, double z, DiffusionParameters dp)
        //{
        //    //if (this.ForwardModel == ForwardModel.DeltaPOne)
        //    //{
        //    //    return DiffusionGaussianBeamFluence(dp, rho, z) +
        //    //        Math.Exp(-dp.mutTilde * z);
        //    //}
        //    //else
        //    //{
        //        return DiffusionGaussianBeamFluence(dp, rho, z);
        //    //}
        //}

        //private double DiffusionGaussianBeamFluence(DiffusionParameters dp, double rho, double z)
        //{
        //    if (this.ForwardModel == ForwardModel.DeltaPOne)
        //    {
        //        return HankelTransform.DigitalFitlerOfOrderZero(rho,
        //            k => SFD_DiffusionGaussianBeamFluence(dp, k, z)) +
        //            Math.Exp(-dp.mutTilde * z);
        //    }
        //    else
        //    {
        //        return HankelTransform.DigitalFitlerOfOrderZero(rho,
        //            k => SFD_DiffusionGaussianBeamFluence(dp, k, z));
        //    }
        //}

        ////private double SFD_DiffusionGaussianBeamFluence(DiffusionParameters dp, double k, double z)
        ////{
        ////    if (dp.mueff == dp.mutTilde) // solution domain of ODE must be modified for this case!
        ////    {
        ////        throw new ArgumentException();
        ////    }

        ////    var gamma = 3 * dp.musTilde * (dp.mutr + dp.gTilde * dp.mutTilde) /
        ////        (k * k + dp.mueff * dp.mueff - dp.mutTilde * dp.mutTilde)
        ////        * BeamDiameter * BeamDiameter * Math.Exp(-BeamDiameter * BeamDiameter * k * k / 32) / 16;
        ////    var xi = -(3 * dp.gTilde * dp.musTilde * BeamDiameter * BeamDiameter *
        ////            Math.Exp(-BeamDiameter * BeamDiameter * k * k / 32) / 16 +
        ////            gamma * (1 / dp.A / 2 / dp.D + dp.mutTilde)) /
        ////            (1 / dp.A / 2 / dp.D + Math.Sqrt(k * k + dp.mueff * dp.mueff));
        ////    //can modify for surface fluence vs. fluence...
        ////    return (gamma * Math.Exp(-dp.mutTilde * z) + xi *
        ////        Math.Exp(-Math.Sqrt(k * k + dp.mueff * dp.mueff) * z)) * 8 /
        ////            (Math.PI * BeamDiameter * BeamDiameter);
        ////}


        #endregion new

        //// modified 8/3: for the purpose of checking the implementation of the "general" use of
        //// the SFD representation of the Gaussian beam solution
        //private double SFD_DiffusionGaussianBeamFluence(DiffusionParameters dp, double k, double z)
        //{
        //    if (dp.mueff == dp.mutTilde) // solution domain of ODE must be modified for this case!
        //    {
        //        throw new ArgumentException();
        //    }
        //    // still yet to add any modifications yet... working on the hand derivation - ....
        //    var gamma = 3 * dp.musTilde * (dp.mutr + dp.gTilde * dp.mutTilde) /
        //        (k * k + dp.mueff * dp.mueff - dp.mutTilde * dp.mutTilde)
        //        * BeamDiameter * BeamDiameter * Math.Exp(-BeamDiameter * BeamDiameter * k * k / 32) / 16;
        //    var xi = -(3 * dp.gTilde * dp.musTilde * BeamDiameter * BeamDiameter *
        //            Math.Exp(-BeamDiameter * BeamDiameter * k * k / 32) / 16 +
        //            gamma * (1 / dp.A / 2 / dp.D + dp.mutTilde)) /
        //            (1 / dp.A / 2 / dp.D + Math.Sqrt(k * k + dp.mueff * dp.mueff));
        //    //can modify for surface fluence vs. fluence...
        //    return (gamma * Math.Exp(-dp.mutTilde * z) + xi *
        //        Math.Exp(-Math.Sqrt(k * k + dp.mueff * dp.mueff) * z)) * 8 /
        //            (Math.PI * BeamDiameter * BeamDiameter);
        //}


        public override double TemporalReflectance(DiffusionParameters dp, double rho, double t, double fr1, double fr2)
        {
            throw new NotImplementedException();
        }

        public override Complex TemporalFrequencyReflectance(DiffusionParameters dp, double rho, Complex k, double fr1, double fr2)
        {
            throw new NotImplementedException();
        }

        public override double TemporalFluence(DiffusionParameters dp, double rho, double z, double t)
        {
            throw new NotImplementedException();
        }

        public override Complex TemporalFrequencyFluence(DiffusionParameters dp, double rho, double z, Complex k)
        {
            throw new NotImplementedException();
        }
    }
}
