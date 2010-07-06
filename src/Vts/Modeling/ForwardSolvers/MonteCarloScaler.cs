using System;
using System.Linq;
using System.Collections.Generic;
using Vts.Extensions;
using Vts.MonteCarlo;
//using MathNet.Numerics.Interpolation;

namespace Vts.Modeling.ForwardSolvers
{
    public class MonteCarloScaler
    {
        # region fields
        public static int nrReference; 
        public static double drReference; 
        public static int ntReference; 
        public static double dtReference;
        public static int nlReference; // won't work for two regions
        public static double muspReference;
        public static string folder = "ReferenceData/N1e7mua0musp1g0.8dr0.2dt2.5/"; 

        private static double[] RhoReference { get; set; }
        private static double[] TimeReference { get; set; }
        private static double[,] R0_homo { get; set; }
        private static double[,] D_homo { get; set; }
        #endregion

        static MonteCarloScaler()
        {
            InitializeVectorsAndInterpolators();
        }
        /// <summary>
        /// InitializeVectorsAndIntReferenceerpolators reads in reference database and initializes data 
        /// Note that the reference data is in cm/ps, but will get converted to mm/ns upon
        /// exiting this code
        /// </summary>
        private static void InitializeVectorsAndInterpolators()
        {
            InitializeOutput();

            RhoReference = (drReference / 2).To(drReference * nrReference - drReference / 2, drReference).ToArray();
            TimeReference = (dtReference / 2).To(dtReference * ntReference - dtReference / 2, dtReference).ToArray();
            
        }
        private static void InitializeOutput()
        {
            var output = Output.FromFolderInResources("Resources/" + folder, "Vts.Modeling");
            nrReference = output.input.detector.nr;
            drReference = output.input.detector.dr * 10; // change from cm to mm
            ntReference = output.input.detector.nt;
            dtReference = output.input.detector.dt / 1000;  // change from ps to ns
            nlReference = output.input.tissptr.num_layers + 1; //CKH fix
            muspReference = output.input.tissptr.layerprops[1].mus * 
                (1 - output.input.tissptr.layerprops[1].g) / 10; // convert to mm

            D_homo = new double[nrReference, ntReference];
            R0_homo = new double[nrReference, ntReference];
            for (int i = 0; i < nrReference; i++)
            {
                for (int j = 0; j < ntReference; j++)
                {
                        D_homo[i, j] = output.D_rt_layer[i, j, nlReference - 1] * 10;   //convert from cm to mm                     
                        R0_homo[i, j] = output.R0_rt[i, j] / 100; //convert from cm-2 to mm-2
                }
            }
        }
        //<summary>
        //GetScaledMonteCarlo  
        //</summary>
        //<param name="op">optical properties</param>
        //<param name="rho">rho (mm)</param>
        //<returns>scaled value at given rho</returns>
        public static IEnumerable<double> GetScaledMonteCarlo(IEnumerable<OpticalProperties> ops, 
            IEnumerable<double> rhos)
        {
            double c = 300; // speed of light [mm/ns]
            double[] RatRhoMC = new double[nrReference];
            double[] rhoScaled = new double[nrReference];
            foreach (var op in ops)
            {
                double v = c / op.N;
                for (int i = 0; i < nrReference; i++)
                {
                    rhoScaled[i] = RhoReference[i] * muspReference / op.Musp;
                    for (int j = 0; j < ntReference; j++)
                    {
                        RatRhoMC[i] += R0_homo[i, j] *
                            Math.Exp(-op.Mua * v * TimeReference[j] * muspReference / op.Musp) *
                            //(dt * muspReference / op.Musp) * 
                            (op.Musp / muspReference) * (op.Musp / muspReference) * (op.Musp / muspReference);
                    }
                }
                foreach (var rho in rhos)
                {
                    yield return Vts.Common.Math.Interpolation.interp1(rhoScaled, RatRhoMC, rho);
                }
            }
        }

        public static IEnumerable<double> GetAllScaledRhos(OpticalProperties op)
        {
            return RhoReference.Select(rho => rho * op.Musp / muspReference).ToArray();
        }
        public static IEnumerable<double> GetAllScaledTimes(OpticalProperties op)
        {
            return TimeReference.Select(time => time * op.Musp / muspReference).ToArray();
        }
        /// <summary>
        /// GetScaledMonteCarlo  
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">rho (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>scaled value at given rhos</returns>
        public static IEnumerable<double> GetScaledMonteCarlo(IEnumerable<OpticalProperties> ops,
            IEnumerable<double> rhos, IEnumerable<double> times)
        {
            double c = 300; // speed of light [mm/ns]
            double v;
            double[] rhoScaled = new double[nrReference];
            double[] timeScaled = new double[ntReference];
            double[,] RScaled = new double[nrReference, ntReference];
            foreach (var op in ops)
            {
                v = c / op.N;
                // rescale first then interpolate `
                for (int i = 0; i < nrReference; i++)
                {
                    rhoScaled[i] = RhoReference[i] * muspReference / op.Musp;
                    for (int j = 0; j < ntReference; j++)
                    {
                        timeScaled[j] = TimeReference[j] * muspReference / op.Musp;
                        // normalize R0 by per unit time
                        RScaled[i, j] = R0_homo[i, j] / (dtReference * muspReference / op.Musp) *
                            Math.Exp(-op.Mua * v * TimeReference[j] * muspReference / op.Musp) *
                            (op.Musp / muspReference) * (op.Musp / muspReference) * (op.Musp / muspReference);
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
        /// GetScaledMontReferenceeCarlo  
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rho">rho (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>scaled value at given rho and time</returns>
        public static double GetScaledMonteCarlo(OpticalProperties op, double rho, double time)
        {
            double RscaledAtPointReference;
            double c = 300; // speed of light [mm/ns]
            double v = c / op.N;
            double[] rhoScaled = new double[nrReference];
            double[] timeScaled = new double[ntReference];
            double[,] RScaled = new double[nrReference, ntReference];
            for (int i = 0; i < nrReference; i++)
            {
                rhoScaled[i] = RhoReference[i] * muspReference / op.Musp;
                for (int j = 0; j < ntReference; j++)
                {
                    timeScaled[j] = TimeReference[j] * muspReference / op.Musp;
                    // normalize R0 by per unit time
                    RScaled[i, j] = R0_homo[i, j] / (dtReference * muspReference / op.Musp) *
                        Math.Exp(-op.Mua * v * TimeReference[j] * muspReference / op.Musp) *
                        (op.Musp / muspReference) * (op.Musp / muspReference) * (op.Musp / muspReference);
                }
            }
            RscaledAtPointReference = Vts.Common.Math.Interpolation.interp2(rhoScaled, timeScaled, RScaled, rho, time);
            // normalize R0 by per unit time
            //double R0 = Vts.Common.Math.IntReferenceerpolation.intReferenceerp2(RhoReference, TimeReference,
            //    R0_homo, rho * op.Musp / muspReference, time * op.Musp / muspReference) / (dt * muspReference / op.Musp);
            ////double D = Vts.Common.Math.IntReferenceerpolation.intReferenceerp2(RhoReference, TimeReference,
            ////    D_homo, rho * op.Musp / muspReference, time * op.Musp / muspReference);
            ////double D = (time * op.Musp / muspReference) * v;
            //double D = (time) * v;
            //if (!double.IsInfinity(Math.Abs(R0)) && !double.IsNaN(R0) &&
            //    !double.IsInfinity(Math.Abs(D)) && !double.IsNaN(D))
            //    RscaledAtPointReference = R0 * Math.Exp(-op.Mua * D) /
            //        (op.Musp / muspReference) * (op.Musp / muspReference) * (op.Musp / muspReference);
            //else
            //    RscaledAtPointReference = 0;
            return RscaledAtPointReference;
        }
    }
}
