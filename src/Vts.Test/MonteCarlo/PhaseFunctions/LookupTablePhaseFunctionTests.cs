using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhaseFunctions;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Meta.Numerics.Statistics;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class LookupTablePhaseFunctionTests
    {
        /// <summary>
        /// Tests whether ScatterToNewTheta samples the polar angle correctly.  Obtains a sample of 500 polar angles and performs a KS test on the sample.
        /// </summary>
        [Test]
        public void validate_theta_sampling(int sampleSize)
        {
            //initialize variables.
            List<double> sampleXi = new List<double>();
            Random rng = new Random();
            
            PolarLookupTablePhaseFunctionData data = new PolarLookupTablePhaseFunctionData();
            data.LutAngles = new[] { 0, Math.PI/6, Math.PI/3, Math.PI/2, 2*Math.PI/3, Math.PI*5/6, Math.PI };
            data.LutPdf = new[] { 1 / (4 * Math.PI), 1 / (4 * Math.PI), 1 / (4 * Math.PI), 1 / (4 * Math.PI), 1 / (4 * Math.PI), 1 / (4 * Math.PI), 1 / (4 * Math.PI) };
            data.LutCdf = new[] { 0, 0.5 * (1 - Math.Sqrt(3) / 2), 0.25 , 0.5, 0.75 , 0.5 * (1 + Math.Sqrt(3) / 2), 1};
            LookupTablePhaseFunction tester = new LookupTablePhaseFunction(rng, data);
            Direction dir = new Direction(1, 0, 0);
            
            //obtaining a sample of the distribution and sort it in ascending order.
            for (int i = 0; i < sampleSize; i++)
                sampleXi.Add(tester.ScatterToNewTheta(dir));
            sampleXi.Sort();

            //do a KS test at alpha = 0.01 to see if sample is compliant to distribution.
            KolmogorovDistribution k = new KolmogorovDistribution();
            double K_alpha = k.LeftProbability(1-0.01);
            double Dn = 0;                                        //variable for KS test statistic.
            double [] Fn = new double[sampleSize+1];              //empircal distribution function.
            
            //construct Fn.  Note: values stored in Fn and the sorted list sampleXi form the tabulated empirical distribution function.
            for (int j = 0; j < sampleSize; j++)
                Fn[j] = 1/sampleSize*(j+1);

            //find the supremum of |Fn - F|
            //check for supremum if Fn underestimates F.
            double temp=Math.Abs(Vts.Common.Math.Interpolation.interp1(data.LutCdf, data.LutAngles, sampleXi[0]));
            if (temp > Dn)
                    Dn = temp;
            for (int i = 0; i < sampleSize-1; i++)
            {
                temp = Math.Abs(Fn[i]-Vts.Common.Math.Interpolation.interp1(data.LutCdf, data.LutAngles, sampleXi[i+1]));
                if (temp > Dn)
                    Dn = temp;
            }
            temp = Math.Abs(1-Vts.Common.Math.Interpolation.interp1(data.LutCdf, data.LutAngles, sampleXi[sampleSize-1]));
            if (temp > Dn)
                Dn = temp;
            
            //check for supremum if Fn overestimates F.
            for (int i = 0; i < sampleSize; i++)
            {
                temp = Math.Abs(Fn[i] - Vts.Common.Math.Interpolation.interp1(data.LutCdf, data.LutAngles, sampleXi[i]));
                if (temp > Dn)
                    Dn = temp;
            }
            
            //if sqrt(n)*Dn > K_alpha, then reject the null hypothesis.  Null hypothesis is that the sample came from this probability distribution.
            Assert.IsTrue(Math.Sqrt(sampleSize)*Dn < K_alpha);
        }
        public void validate_Scatter()
        {
            var d1 = new Direction(1, 0, 0);
            var rng = new Random();
            var lutData = new PolarLookupTablePhaseFunctionData();
            var phaseFunc = new LookupTablePhaseFunction(rng, lutData);
            phaseFunc.Scatter(d1, Math.PI / 6, Math.PI);
            Assert.AreEqual(d1.Ux, -Math.Sqrt(3)/2);
            Assert.AreEqual(d1.Uy, 0.0);
            Assert.AreEqual(d1.Uz, 0.5);
        }
    }
}