using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;

//using MathNet.Numerics.Interpolation;

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
        public int nrReference; 
        /// <summary>
        /// size of rho bins in reference database
        /// </summary>
        public double drReference;
        /// <summary>
        /// number of time bins in reference database
        /// </summary>
        public int ntReference; 
        /// <summary>
        /// size of time bins in reference database
        /// </summary>
        public double dtReference;
        /// <summary>
        /// mus' value used in reference database
        /// </summary>
        public double muspReference;
        /// <summary>
        /// number of spatial frequency bins in reference 
        /// </summary>
        public int nfxReference;
        /// <summary>
        /// size of spatial frequency bins in reference
        /// </summary>
        public double dfxReference;

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

        /// CKH TODO: automate pointer to reference data 
        /// can't point to N1e7 until writing to isolated storage working for R_fxt code below
        private string folder = "ReferenceData/N1e8mua0musp1g0p8dr0p2dt0p005/";

        /// <summary>
        /// constructor that loads scaled Monte Carlo reference data and database 
        /// </summary>
        public MonteCarloLoader()
        {
            nfxReference = 100;
            dfxReference = 1.0 / nfxReference; 
            InitializeVectorsAndInterpolators();
        }

        /// <summary>
        /// InitializeVectorsAndInterpolators reads in reference database and initializes data 
        /// Note that the reference data is now in mm/ns so no conversion needed
        /// </summary>
        private void InitializeVectorsAndInterpolators()
        {
            var rOfRhoAndTime = (ROfRhoAndTimeDetector)DetectorIO.ReadDetectorFromFileInResources(TallyType.ROfRhoAndTime, "Modeling/Resources/" + folder, "Vts");

            nrReference = rOfRhoAndTime.Rho.Count - 1;
            drReference = rOfRhoAndTime.Rho.Delta;
            ntReference = rOfRhoAndTime.Time.Count - 1;
            dtReference = rOfRhoAndTime.Time.Delta;  
            // assume mus' used by Kienle
            muspReference = 1.0;  

            RhoReference = new DoubleRange(drReference / 2, drReference * nrReference - drReference / 2, nrReference).AsEnumerable().ToArray();
            TimeReference = new DoubleRange(dtReference / 2, dtReference * ntReference - dtReference / 2, ntReference).AsEnumerable().ToArray();
            FxReference = new DoubleRange(dfxReference / 2, dfxReference * nfxReference - dfxReference / 2, nfxReference).AsEnumerable().ToArray();

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
            RReferenceOfFxAndTime = new double[nfxReference, ntReference];
            // CKH TODO: automate this process somehow
            //if (File.Exists("Resources/" + folder + @"R_fxt"))
            if (true)
            {
                RReferenceOfFxAndTime = (double[,])FileIO.ReadArrayFromBinaryInResources<double>("Modeling/Resources/" +
                    folder + @"R_fxt", "Vts");
            }
            else
            {
                double[] RReferenceOfRhoAndTj = new double[nrReference];
                double[] RReferenceOfFxAndTj = new double[nfxReference];
                for (int j = 0; j < ntReference; j++)
                {
                    for (int k = 0; k < nrReference; k++)
                    {
                        RReferenceOfRhoAndTj[k] = RReferenceOfRhoAndTime[k, j]; // get ROfRho at a particular Time Tj 
                    }
                    for (int i = 0; i < nfxReference; i++)
                    {
                        RReferenceOfFxAndTime[i, j] = LinearDiscreteHankelTransform.GetHankelTransform(RhoReference,
                            RReferenceOfRhoAndTj, drReference, dfxReference * i);
                    }
                }
                FileIO.WriteArrayToBinary<double>(RReferenceOfFxAndTime, @"/R_fxt");
            }
        }

        public IEnumerable<double> GetAllScaledRhos(OpticalProperties op)
        {
            return RhoReference.Select(rho => rho * muspReference / op.Musp).ToArray();
        }
        public IEnumerable<double> GetAllScaledTimes(OpticalProperties op)
        {
            return TimeReference.Select(time => time * muspReference / op.Musp).ToArray();
        }
        public IEnumerable<double> GetAllScaledFxs(OpticalProperties op)
        {
            return FxReference.Select(fx => fx * op.Musp / muspReference).ToArray();
        }
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
