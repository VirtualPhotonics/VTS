﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Meta.Numerics.Statistics.Distributions;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class LookupTablePhaseFunctionTests
    {
        /// <summary>
        /// Tests whether ScatterToNewTheta samples the polar angle correctly.  Obtains a sample of 500 polar angles 
        /// and performs a Komogorov-Smirnov (KS) test on the sample to verify it abides by the distribution.
        /// </summary>
        [Test]
        public void validate_theta_sampling_of_uniform_pdf_with_KS_test()
        {
            int sampleSize = 500;
            Random rng = new Random(0); // seed rng so test has same sequence every time it is executed
            
            PolarLookupTablePhaseFunctionData data = new PolarLookupTablePhaseFunctionData();
            data.LutAngles = new[] { 0, Math.PI/6, Math.PI/3, Math.PI/2, 2*Math.PI/3, Math.PI*5/6, Math.PI };
            double[] mu = new[] { -1, -Math.Sqrt(3) / 2, -1.0 / 2, 0, 1.0 / 2, Math.Sqrt(3) / 2, 1 };
            data.LutPdf = new[] { 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 }; // define uniform pdf
            data.LutCdf = new[] { 0, 0.5 * (1 - Math.Sqrt(3) / 2), 0.25 , 0.5, 0.75 , 0.5 * (1 + Math.Sqrt(3) / 2), 1};
            LookupTablePhaseFunction tester = new LookupTablePhaseFunction(data, rng);
            Direction dir = new Direction(0, 0, 1);
            
            // Kolmogorov-Smirnov statistic for a given CDF F(x) is Dn=sup_x|Fn(x)-F(x)|
            // where Fn is the empirical distribution function and F is the actual.
            // Goodness-of-fit test is if sqrt(n)Dn > Kalpha where n is number of samples and
            // K_alpha is found from Pr(K<=K_alpha)=1-alpha

            // obtain a empirical sample of the theta distribution and sort it in ascending order
            List<double> sampleMu = new List<double>();
            for (int i = 0; i < sampleSize; i++)
            {
                var prevDir = dir;
                tester.ScatterToNewDirection(dir); // update direction
                var cosTheta = Direction.GetDotProduct(dir, prevDir); // get cos(theta) between prev and current
                sampleMu.Add(cosTheta);   
            } 
            sampleMu.Sort(); // sort sample so that can create CDF
            // create cdf from samples. CDF goes from [sampleMu[0]/sum - 1.0]
            var sum = sampleMu.Sum();
            List<double> sampleCdf = new List<double>();
            double cdf = 0.0;
            for (int i = 0; i < sampleSize; i++)
            {
                cdf = cdf + sampleMu[i] / sum;
                sampleCdf.Add(cdf);   
            }

            //do a KS test at alpha = 0.01 to see if sample is compliant to distribution.
            KolmogorovDistribution k = new KolmogorovDistribution();
            double K_alpha = k.LeftProbability(1-0.01);
            double Dn = 0;                                        //variable for KS test statistic.
            double [] Fn = new double[sampleSize];              //empircal distribution function.
            
            // construct Fn. Fn goes from [1/sampleSize-1.0] in uniform steps since pdf uniform
            // Note: values stored in Fn and the sorted list sampleMu form the tabulated empirical distribution function
            double test = (double)1/sampleSize;
            for (int j = 0; j < sampleSize; j++)
            {
                Fn[j] = test * (j + 1);   
            }

            //find the supremum of |Fn - F|
            double temp;
            //check for supremum if Fn underestimates F.
            // CKH 8/23/17 not sure what following was doing so commented out
            //double temp = Math.Abs(Vts.Common.Math.Interpolation.interp1(mu, data.LutCdf, sampleCdf[0]));
            //if (temp > Dn)
            //{
            //    Dn = temp;       
            //}
            //for (int i = 0; i < sampleSize-1; i++)
            //{
            //    temp = Math.Abs(Fn[i] - Vts.Common.Math.Interpolation.interp1(mu, data.LutCdf, sampleCdf[i]));
            //    if (temp > Dn)
            //    {
            //        Dn = temp;   
            //    }
            //}
            //temp = Math.Abs(1 - Vts.Common.Math.Interpolation.interp1(mu, data.LutCdf, sampleCdf[sampleSize - 1]));
            //if (temp > Dn)
            //{
            //    Dn = temp;   
            //}
            //check for supremum if Fn overestimates F.
            for (int i = 0; i < sampleSize; i++)
            {
                //temp = Math.Abs(Fn[i] - Vts.Common.Math.Interpolation.interp1(mu, data.LutCdf, sampleCdf[i]));
                temp = Math.Abs(Fn[i] - sampleCdf[i]);
                if (temp > Dn)
                {
                    Dn = temp;   
                }
            }
            //if sqrt(n)*Dn > K_alpha, then reject the null hypothesis
            // Null hypothesis is that the sample came from this probability distribution
            Assert.That(Math.Sqrt((double)sampleSize) * Dn <= K_alpha, Is.True);
        }
        /// <summary>
        /// this test validates the Scatter method in PolarAndAzimuthalPhaseFunction
        /// </summary>
        [Test]
        public void validate_Scatter_method_calculates_new_angle_correctly()
        {
            var d1 = new Direction(1, 0, 0);
            var rng = new Random(0);
            var lutData = new PolarLookupTablePhaseFunctionData();
            var phaseFunc = new LookupTablePhaseFunction(lutData, rng);
            phaseFunc.Scatter(d1, Math.PI / 6, Math.PI); // incoming direction, theta, phi
            // validation results based on prior run
            Assert.That(Math.Abs(d1.Ux - Math.Sqrt(3)/2), Is.LessThan(1e-6));
            Assert.That(Math.Abs(d1.Uy), Is.LessThan(1e-6));
            Assert.That(Math.Abs(d1.Uz - 0.5), Is.LessThan(1e-6));
        }
/*        [Test]
        public void ScatterToNewTheta_validate()
        {
            List<double> sampleXi = new List<double>();
            Random rng = new Random();          
            PolarLookupTablePhaseFunctionData data = new PolarLookupTablePhaseFunctionData();
            data.LutAngles = new[] { 0, Math.PI/6, Math.PI/3, Math.PI/2, 2*Math.PI/3, Math.PI*5/6, Math.PI };
            double[] mu = new[] { -1, -Math.Sqrt(3) / 2, -1 / 2, 0, 1 / 2, Math.Sqrt(3) / 2, 1 };
            data.LutPdf = new[] { 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 };
            data.LutCdf = new[] { 0, 0.5 * (1 - Math.Sqrt(3) / 2), 0.25 , 0.5, 0.75 , 0.5 * (1 + Math.Sqrt(3) / 2), 1};

            LookupTablePhaseFunction tester = new LookupTablePhaseFunction(rng, data);
            Direction dir = new Direction(1, 0, 0);
            System.Diagnostics.Debug.WriteLine("Thetas: (from scattertonewtheta routine)");            
            //obtaining a sample of the distribution and sort it in ascending order.
            for (int i = 0; i < 10000; i++)
            {
                System.Diagnostics.Debug.WriteLine(tester.ScatterToNewTheta(dir));
            }
        }*/
    }
}