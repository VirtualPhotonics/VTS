using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Extensions;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.Common;
using Vts.MonteCarlo.Detectors;

//using MathNet.Numerics.Interpolation;

namespace Vts.Modeling.ForwardSolvers
{
    public class MonteCarloLoader
    {
        public int nrReference; 
        public double drReference; 
        public int ntReference; 
        public double dtReference;
        public double muspReference;
        public int nfxReference;
        public double dfxReference;

        public double[] RhoReference { get; set; }
        public double[] TimeReference { get; set; }
        public double[] FxReference { get; set; }
        public double[,] RReferenceOfRhoAndTime { get; set; }
        public double[,] RReferenceOfFxAndTime { get; set; }

        /// CKH TODO: automate pointer to reference data 
        /// can't point to N1e7 until writing to isolated storage working for R_fxt code below
        private string folder = "ReferenceData/N1e8mua0musp1g0p8dr0p2dt0p005/";

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
            var output = Output.FromFolderInResources("Modeling/Resources/" + folder, "Vts");

            // todo: temp code to make this work with the new structure. revisit.
            var input = (ROfRhoAndTDetectorInput)output.Input.DetectorInputs.Where(di => di.TallyType == TallyType.ROfRhoAndTime).First();

            nrReference = input.Rho.Count;
            drReference = input.Rho.Delta;
            ntReference = input.Time.Count;
            dtReference = input.Time.Delta;  
            muspReference = output.Input.TissueInput.Regions[1].RegionOP.Mus *
                    (1 - output.Input.TissueInput.Regions[1].RegionOP.G);

            RhoReference = new DoubleRange(drReference / 2, drReference * nrReference - drReference / 2, nrReference).AsEnumerable().ToArray();
            TimeReference = new DoubleRange(dtReference / 2, dtReference * ntReference - dtReference / 2, ntReference).AsEnumerable().ToArray();
            FxReference = new DoubleRange(dfxReference / 2, dfxReference * nfxReference - dfxReference / 2, nfxReference).AsEnumerable().ToArray();

            RReferenceOfRhoAndTime = new double[nrReference, ntReference];
            for (int ir = 0; ir < nrReference; ir++)
            {
                double sum = 0.0;
                for (int it = 0; it < ntReference; it++)
                {
                        RReferenceOfRhoAndTime[ir, it] = output.R_rt[ir, it];
                        sum += output.R_rt[ir, it];  // debug line
                }
            }
            RReferenceOfFxAndTime = new double[nfxReference, ntReference];
            /// CKH TODO: automate this process somehow
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
                        RReferenceOfRhoAndTj[k] = RReferenceOfRhoAndTime[k, j]; // get RofRho at a particular Time Tj 
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
