using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.IO;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// class to handle loading of scaled Monte Carlo database
    /// </summary>
    public class MonteCarloLoader
    {
        /// <summary>
        /// number of rho bins in reference database
        /// </summary>
        public int nrReference { get; set; }
        /// <summary>
        /// size of rho bins in reference database
        /// </summary>
        public double drReference { get; set; }
        /// <summary>
        /// number of time bins in reference database
        /// </summary>
        public int ntReference { get; set; }
        /// <summary>
        /// size of time bins in reference database
        /// </summary>
        public double dtReference { get; set; }
        /// <summary>
        /// mus' value used in reference database
        /// </summary>
        public double muspReference { get; set; }
        /// <summary>
        /// number of spatial frequency bins in reference 
        /// </summary>
        public int nfxReference { get; set; }
        /// <summary>
        /// size of spatial frequency bins in reference
        /// </summary>
        public double dfxReference { get; set; }

        /// <summary>
        /// array of rho bin centers in reference database
        /// </summary>
        public double[] RhoReference { get; set; }
        /// <summary>
        /// array of time bin centers in reference database
        /// </summary>
        public double[] TimeReference { get; set; }
        /// <summary>
        /// array of spatial frequency centers in reference
        /// </summary>
        public double[] FxReference { get; set; }
        /// <summary>
        /// reference database R(rho,time)
        /// </summary>
        public double[,] RReferenceOfRhoAndTime { get; set; }
        /// <summary>
        /// reference database R(fx,time)
        /// </summary>
        public double[,] RReferenceOfFxAndTime { get; set; }

        /// can't point to N1e7 until writing to isolated storage working for R_fxt code below
        private string folder = "ReferenceData/N1e8mua0musp1g0p8dr0p2dt0p005/";

        /// <summary>
        /// constructor that loads scaled Monte Carlo reference data and database 
        /// </summary>
        public MonteCarloLoader()
        {
            InitializeVectorsAndInterpolators();
        }

        /// <summary>
        /// InitializeVectorsAndInterpolators reads in reference database and initializes data 
        /// Note that the reference data is now in mm/ns so no conversion needed
        /// </summary>
        private void InitializeVectorsAndInterpolators()
        {
            // load R(rho,time) reference data
            var rOfRhoAndTime = (dynamic)DetectorIO.ReadDetectorFromFileInResources("ROfRhoAndTime", "Modeling/Resources/" + folder, "Vts");

            nrReference = rOfRhoAndTime.Rho.Count - 1;
            drReference = rOfRhoAndTime.Rho.Delta;
            ntReference = rOfRhoAndTime.Time.Count - 1;
            dtReference = rOfRhoAndTime.Time.Delta;  
            // assume mus' used by Kienle
            muspReference = 1.0;  

            RhoReference = new DoubleRange(drReference / 2, drReference * nrReference - drReference / 2, nrReference).ToArray();
            TimeReference = new DoubleRange(dtReference / 2, dtReference * ntReference - dtReference / 2, ntReference).ToArray();
           
            RReferenceOfRhoAndTime = new double[nrReference, ntReference];
            for (int ir = 0; ir < nrReference; ir++)
            {
                double sum = 0.0;
                for (int it = 0; it < ntReference; it++)
                {
                        RReferenceOfRhoAndTime[ir, it] = rOfRhoAndTime.Mean[ir, it];
                        sum += rOfRhoAndTime.Mean[ir, it];  // debug line
                }
            }

            // load R(fx,time) reference data
            var rOfFxAndTime = (dynamic)DetectorIO.ReadDetectorFromFileInResources("ROfFxAndTime", "Modeling/Resources/" + folder, "Vts");

            nfxReference = rOfFxAndTime.Fx.Count;
            dfxReference = 1.0/nfxReference;

            FxReference = new DoubleRange(dfxReference / 2, dfxReference * nfxReference - dfxReference / 2, nfxReference).ToArray();

            RReferenceOfFxAndTime = new double[nfxReference, ntReference];
            for (int ifx = 0; ifx < nfxReference; ifx++)
            {
                for (int it = 0; it < ntReference; it++) // this only goes to 800 not 801 because ntReference determined from ROfRhoAndTime.txt
                {
                    RReferenceOfFxAndTime[ifx, it] = rOfFxAndTime.Mean[ifx, it].Real;
                }
            }

        }
        /// <summary>
        /// method to get all scaled rho values
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <returns>scaled rho values</returns>
        public IEnumerable<double> GetAllScaledRhos(OpticalProperties op)
        {
            return RhoReference.Select(rho => rho * muspReference / op.Musp).ToArray();
        }
        /// <summary>
        /// method to get all scaled time values
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <returns>scaled time values</returns>
        public IEnumerable<double> GetAllScaledTimes(OpticalProperties op)
        {
            return TimeReference.Select(time => time * muspReference / op.Musp).ToArray();
        }
        /// <summary>
        /// method to get all scaled spatial-frequencies
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <returns>scaled fx values</returns>
        public IEnumerable<double> GetAllScaledFxs(OpticalProperties op)
        {
            return FxReference.Select(fx => fx * op.Musp / muspReference).ToArray();
        }
        /// <summary>
        /// method to get Fresnel value 
        /// </summary>
        /// <param name="nIn">refractive index of incoming ray</param>
        /// <param name="nOut">refractive index of outgoing ray</param>
        /// <param name="theta">angle of inception</param>
        /// <returns>Fresnel value</returns>
        public double GetFresnel(double nIn, double nOut, double theta)
        {
            double thetaPrime = (nIn / nOut) * Math.Sin(theta);
            double cosTheta = Math.Cos(theta);
            double cosThetaPrime = Math.Cos(thetaPrime);
            double dum1 = nIn * cosThetaPrime - nOut * cosTheta;
            double dum2 = nIn * cosThetaPrime + nOut * cosTheta;
            double dum3 = nIn * cosTheta - nOut * cosThetaPrime;
            double dum4 = nIn * cosTheta + nOut * cosThetaPrime;
            return 0.5 * (dum1 / dum2) * (dum1 / dum2) +
                   0.5 * (dum3 / dum4) * (dum3 / dum4);
        }
    }
}
