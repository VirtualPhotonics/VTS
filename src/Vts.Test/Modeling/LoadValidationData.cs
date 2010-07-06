using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Extensions;
using Vts.Factories;
using Vts.MonteCarlo;

namespace Vts.Test.Modeling
{
    public enum ValidationType
    {
        Mua0p01Musp05,
        Mua0p01Musp07,
        Mua0p01Musp09,
        Mua0p01Musp10,
        Mua0p01Musp11,
        Mua0p01Musp13,
        Mua0p01Musp15,
        Mua0p1Musp05,
        Mua0p1Musp07,
        Mua0p1Musp09,
        Mua0p1Musp10,
        Mua0p1Musp11,
        Mua0p1Musp13,
        Mua0p1Musp15,
        Mua1Musp05,
        Mua1Musp07,
        Mua1Musp09,
        Mua1Musp10,
        Mua1Musp11,
        Mua1Musp13,
        Mua1Musp15,
    }
    public class LoadValidationData
    {
        # region fields
        public static double drValidation;
        public static double dtValidation;
        public static int nrValidation;
        public static int ntValidation;
        public static double[,] ValidationRrt;
        public static double[,] ValidationRrtSDUpper;
        public static double[,] ValidationRrtSDLower;
        public static double[] ValidationRrSDLower;
        public static double[] ValidationRrSDUpper;
        public static double muaValidation;
        public static double muspValidation;
        public static double gValidation;
        public static double nValidation;
        public static long N;
        private static string folder = "N1e6G0.8dt5/";
        private static string numberPhotons = "N1e7";
        private static string bins = "dr0.2dt5";
        private static string project = "Vts.Modeling.Test";
        private static string ValidationFolder;
        private static Output output;
        # endregion fields

        public static void InitializeValidationData(ValidationType selectedType)
        {
            LoadSelectedValidationData(selectedType);
            ConvertDataUnits();
        }
        private static void LoadSelectedValidationData(ValidationType selectedType)
        {
            UpdateValidationType(selectedType);
            output = Output.FromFolderInResources(folder + ValidationFolder, project);
        }
        private static void ConvertDataUnits()
        {
            // units conversion from MC: mua,musp[/cm] -> mua,musp(1cm/10mm) [/mm]
            //                           dr[cm] -> dr(10mm/1cm) [mm]
            //                           dt[ps] -> dt(1ns/1000ps) [ns]
            //                 ( R[/cm^2] )/dt[ns] -> ( R(1cm^2/100mm^2) [/mm^2])/dt [/mm^2 ns]
            double[,] ValidationRrt2;
            double variance;
            muaValidation = output.input.TissueInput.Regions[1].RegionOP.Mua / 10.0D; // convert to mm-1
            gValidation = output.input.TissueInput.Regions[1].RegionOP.G;
            N = (long)output.input.N;
            muspValidation = output.input.TissueInput.Regions[1].RegionOP.Mus * (1.0D - gValidation) / 10;
            nValidation = output.input.TissueInput.Regions[1].RegionOP.N;
            drValidation = output.input.DetectorInput.Rho.Delta * 10; // convert from cm to mm
            dtValidation = output.input.DetectorInput.Time.Delta / 1000;  // convert from ps to ns
            nrValidation = output.input.DetectorInput.Rho.Count;
            ntValidation = output.input.DetectorInput.Time.Count;
            ValidationRrt = output.R_rt;
            ValidationRrt2 = output.R_rt2;
            ValidationRrtSDUpper = new double[nrValidation, ntValidation];
            ValidationRrtSDLower = new double[nrValidation, ntValidation];
            ValidationRrSDUpper = new double[nrValidation];
            ValidationRrSDLower = new double[nrValidation];
            double relError, maxRelError = 0, avgRelError = 0;
            for (int i = 0; i < nrValidation; i++)
            {
                for (int j = 0; j < ntValidation; j++)
                {
                    // the following two have already been divided by N and C (area of ring)
                    // units are already per cm-2 so conversion to mm-2 for both Rrt and Rrt2 need only 100
                    // units have not been divided by dt, so Rrt2 needs squared factor
                    ValidationRrt[i, j] /= (100 * dtValidation); // convert to /mm-2 & normalize time (put in MC if correct)
                    ValidationRrt2[i, j] /= (100 * dtValidation); 
                    variance = (ValidationRrt2[i, j] - ValidationRrt[i, j] * ValidationRrt[i, j]) / N;
                    if (!double.IsNaN(ValidationRrt[i, j]) && (ValidationRrt[i, j] != 0))
                    {
                        relError = Math.Sqrt(variance) / ValidationRrt[i, j];
                        if (relError > maxRelError) maxRelError = relError;     
                        avgRelError += relError;
                    }
                    ValidationRrtSDUpper[i, j] = ValidationRrt[i, j] + 3 * Math.Sqrt(variance);
                    ValidationRrtSDLower[i, j] = ValidationRrt[i, j] - 3 * Math.Sqrt(variance);
                }
                variance = (output.R_r2[i] / 100 - output.R_r[i] / 100 * output.R_r[i] / 100) / N;
                relError = Math.Sqrt(variance) / (output.R_r[i] / 100);
                ValidationRrSDUpper[i] = output.R_r[i] / 100 + 3 * Math.Sqrt(variance);
                ValidationRrSDLower[i] = output.R_r[i] / 100 - 3 * Math.Sqrt(variance);
            }
            avgRelError /= nrValidation * ntValidation;
        }
        private static void UpdateValidationType(ValidationType selectedType)
        {
            switch (selectedType)
            {
                default:
                case ValidationType.Mua0p01Musp05:
                    ValidationFolder = numberPhotons + "mua0.001musp0.5g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p01Musp07:
                    ValidationFolder = numberPhotons + "mua0.001musp0.7g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p01Musp09:
                    ValidationFolder = numberPhotons + "mua0.001musp0.9g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p01Musp10:
                    ValidationFolder = numberPhotons + "mua0.001musp1g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p01Musp11:
                    ValidationFolder = numberPhotons + "mua0.001musp1.1g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p01Musp13:
                    ValidationFolder = numberPhotons + "mua0.001musp1.3g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p01Musp15:
                    ValidationFolder = numberPhotons + "mua0.001musp1.5g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p1Musp05:
                    ValidationFolder = numberPhotons + "mua0.01musp0.5g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p1Musp07:
                    ValidationFolder = numberPhotons + "mua0.01musp0.7g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p1Musp09:
                    ValidationFolder = numberPhotons + "mua0.01musp0.9g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p1Musp10:
                    ValidationFolder = numberPhotons + "mua0.01musp1g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p1Musp11:
                    ValidationFolder = numberPhotons + "mua0.01musp1.1g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p1Musp13:
                    ValidationFolder = numberPhotons + "mua0.01musp1.3g0.8" + bins + "/";
                    break;
                case ValidationType.Mua0p1Musp15:
                    ValidationFolder = numberPhotons + "mua0.01musp1.5g0.8" + bins + "/";
                    break;
                case ValidationType.Mua1Musp05:
                    ValidationFolder = numberPhotons + "mua0.1musp0.5g0.8" + bins + "/";
                    break;
                case ValidationType.Mua1Musp07:
                    ValidationFolder = numberPhotons + "mua0.1musp0.7g0.8" + bins + "/";
                    break;
                case ValidationType.Mua1Musp09:
                    ValidationFolder = numberPhotons + "mua0.1musp0.9g0.8" + bins + "/";
                    break;
                case ValidationType.Mua1Musp10:
                    ValidationFolder = numberPhotons + "mua0.1musp1g0.8" + bins + "/";
                    break;
                case ValidationType.Mua1Musp11:
                    ValidationFolder = numberPhotons + "mua0.1musp1.1g0.8" + bins + "/";
                    break;
                case ValidationType.Mua1Musp13:
                    ValidationFolder = numberPhotons + "mua0.1musp1.3g0.8" + bins + "/";
                    break;
                case ValidationType.Mua1Musp15:
                    ValidationFolder = numberPhotons + "mua0.1musp1.5g0.8" + bins + "/";
                    break;
            }
        }
        public static double[,] GetScaledMonteCarloRrtAtValidationPoints()
        {
            var opValidation = new OpticalProperties(muaValidation, muspValidation, gValidation, nValidation);
            double[,] Scaled_R_rt = new double[nrValidation, ntValidation];
            double[] rho, time;
            rho = (drValidation / 2).To(drValidation * nrValidation - drValidation / 2, drValidation).ToArray();
            time = (dtValidation / 2).To(dtValidation * ntValidation - dtValidation / 2, dtValidation).ToArray();
            double[] temp = new double[ntValidation];
            // the following seems to not be optimal, not using vectorized code appropriately
            for (int i = 0; i < nrValidation; i++)
            {
                temp = GetScaledMonteCarloRtAtValidationPoints(i);
                for (int j = 0; j < ntValidation; j++)
                {
                    Scaled_R_rt[i, j] = temp[j];
                }
            }
            return Scaled_R_rt;
        }
        public static double[] GetScaledMonteCarloRtAtValidationPoints(int rhoIndex)
        {
            OpticalProperties opValidation = new OpticalProperties(muaValidation, muspValidation, gValidation, nValidation); 
            double[] independentValues = new double[ntValidation];
            double[] constantValue = new double[1];
            IEnumerable<double> query;
            double[] Scaled_R_t = new double[ntValidation];
            independentValues = (dtValidation / 2).To(dtValidation * ntValidation - dtValidation / 2, dtValidation).ToArray();
            constantValue[0] = drValidation * rhoIndex + drValidation / 2;
            query = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                SolverFactory.GetForwardSolver(ForwardSolverType.MonteCarlo),
                SolutionDomainType.RofRhoAndT,
                ForwardAnalysisType.R,
                IndependentVariableAxis.T,
                independentValues,
                opValidation,
                constantValue);
            Scaled_R_t = query.ToArray();
            return Scaled_R_t;
        }
        public static double[] GetScaledMonteCarloRrAtValidationPoints()
        {
            OpticalProperties opValidation = new OpticalProperties(muaValidation, muspValidation, gValidation, nValidation);
            double[] independentValues = new double[nrValidation];
            double[] constantValue = new double[1];
            IEnumerable<double> query;
            double[] Scaled_R_r = new double[nrValidation];
            independentValues = (drValidation / 2).To(drValidation * nrValidation - drValidation / 2, drValidation).ToArray();
            query = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                SolverFactory.GetForwardSolver(ForwardSolverType.MonteCarlo),
                SolutionDomainType.RofRho,
                ForwardAnalysisType.R,
                IndependentVariableAxis.Rho,
                independentValues,
                opValidation,
                constantValue);
            Scaled_R_r = query.ToArray();
            return Scaled_R_r;
        }
    }
}
