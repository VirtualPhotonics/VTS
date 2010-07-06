﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections;

using Meta.Numerics;
using Meta.Numerics.Functions;
using Meta.Numerics.Statistics;

namespace Test {
    
    
    /// <summary>
    ///This is a test class for SampleTest and is intended
    ///to contain all SampleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SampleTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void SampleManipulationsTest () {
            
            // create a sample
            double[] data = new double[] { -1.1, 2.2, -3.3, 4.4 };
            Sample sample = new Sample(data);

            // check the length
            Assert.IsTrue(sample.Count == data.Length);

            // add a datum and check the length
            sample.Add(5.5);
            Assert.IsTrue(sample.Count == data.Length + 1);

            // check wether an elements exists, remove it, check the length, check that it doesn't exist
            Assert.IsTrue(sample.Contains(2.2));
            Assert.IsTrue(sample.Remove(2.2));
            Assert.IsTrue(sample.Count == data.Length);
            Assert.IsFalse(sample.Contains(2.2));

            // clear the sample and check the length
            sample.Clear();
            Assert.IsTrue(sample.Count == 0);

        }

        private static Sample CreateSample (Distribution distribution, int count) {
            return (CreateSample(distribution, count, 1));
        }

        private static Sample CreateSample (Distribution distribution, int count, int seed) {

            Sample sample = new Sample();

            Random rng = new Random(seed);
            for (int i = 0; i < count; i++) {
                double x = distribution.InverseLeftProbability(rng.NextDouble());
                sample.Add(x);
            }

            return (sample);
        }

        private Distribution[] distributions = new Distribution[] {
            new UniformDistribution(Interval.FromEndpoints(-2.0,1.0)),
            new NormalDistribution(3.0,2.0),
            new ExponentialDistribution(2.0),
        };

        [TestMethod]
        public void SampleMomentsTest () {
            foreach (Distribution distribution in distributions) {
                //Distribution distribution = new NormalDistribution(3.0, 2.0);
                Sample sample = CreateSample(distribution, 100);

                Assert.IsTrue(sample.Count == 100);

                UncertainValue m = sample.PopulationMean;
                Interval mi = m.ConfidenceInterval(0.95);
                Assert.IsTrue(mi.ClosedContains(distribution.Mean));

                UncertainValue s = sample.PopulationStandardDeviation;
                Interval si = s.ConfidenceInterval(0.95);
                Assert.IsTrue(si.ClosedContains(distribution.StandardDeviation));

                for (int n = 3; n < 4; n++) {
                    UncertainValue c = sample.PopulationMomentAboutMean(3);
                    Interval ci = c.ConfidenceInterval(0.95);
                    Assert.IsTrue(ci.ClosedContains(distribution.MomentAboutMean(n)));

                    UncertainValue r = sample.PopulationMoment(n);
                    Interval ri = r.ConfidenceInterval(0.95);
                    Assert.IsTrue(ri.ClosedContains(distribution.Moment(n)));
                }
            }

        }

        [TestMethod]
        public void SampleInterquartileRangeTest () {
            foreach (Distribution distribution in distributions) {
                //Distribution distribution = new NormalDistribution(3.0, 2.0);
                Sample sample = CreateSample(distribution, 100);

                Interval iqr = sample.InterquartileRange;
                Assert.IsTrue(TestUtilities.IsNearlyEqual(iqr.LeftEndpoint, sample.InverseLeftProbability(0.25)));
                Assert.IsTrue(TestUtilities.IsNearlyEqual(iqr.RightEndpoint, sample.InverseLeftProbability(0.75)));
            }

        }

        [TestMethod]
        public void SampleFitNormalTest () {

            // pick mu >> sigma so that we get no negative values;
            // otherwise the attempt to fit to an exponential will fail
            Distribution distribution = new NormalDistribution(5.0, 2.0);
            Sample sample = CreateSample(distribution, 100);

            // fit to normal should be good
            FitResult nfit = sample.FitToNormalDistribution();
            Console.WriteLine("P_n = {0}", nfit.GoodnessOfFit.LeftProbability);
            Assert.IsTrue(nfit.GoodnessOfFit.LeftProbability < 0.95, String.Format("P_n = {0}", nfit.GoodnessOfFit.LeftProbability));

            // fit to exponential should be bad
            FitResult efit = sample.FitToExponentialDistribution();
            Console.WriteLine("P_e = {0}", efit.GoodnessOfFit.LeftProbability);
            Assert.IsTrue(efit.GoodnessOfFit.LeftProbability > 0.95, String.Format("P_e = {0}", efit.GoodnessOfFit.LeftProbability));

        }

        [TestMethod]
        public void SampleFitExponentialTest () {

            Distribution distribution = new ExponentialDistribution(5.0);
            Sample sample = CreateSample(distribution, 100);

            // fit to normal should be bad
            FitResult nfit = sample.FitToNormalDistribution();
            Console.WriteLine("P_n = {0}", nfit.GoodnessOfFit.LeftProbability);
            Assert.IsTrue(nfit.GoodnessOfFit.LeftProbability > 0.95, String.Format("P_n = {0}", nfit.GoodnessOfFit.LeftProbability));

            // fit to exponential should be good
            FitResult efit = sample.FitToExponentialDistribution();
            Console.WriteLine("P_e = {0}", efit.GoodnessOfFit.LeftProbability);
            Assert.IsTrue(efit.GoodnessOfFit.LeftProbability < 0.95, String.Format("P_e = {0}", efit.GoodnessOfFit.LeftProbability));

        }

        [TestMethod]
        public void SampleFitExponentialUncertaintyTest () {

            // check that the uncertainty in reported fit parameters is actually meaningful
            // it should be the standard deviation of fit parameter values in a sample of many fits

            // define a population distribution 
            Distribution distribution = new ExponentialDistribution(4.0);

            // draw a lot of samples from it; fit each sample and
            // record the reported parameter value and error of each
            Sample values = new Sample();
            Sample uncertainties = new Sample();
            for (int i = 0; i < 50; i++) {
                Sample sample = CreateSample(distribution, 10, i);
                FitResult fit = sample.FitToExponentialDistribution();
                UncertainValue lambda = fit.Parameter(0);
                values.Add(lambda.Value);
                uncertainties.Add(lambda.Uncertainty);
            }

            Console.WriteLine(uncertainties.Mean);
            Console.WriteLine(values.PopulationStandardDeviation);

            // the reported errors should agree with the standard deviation of the reported parameters
            Assert.IsTrue(values.PopulationStandardDeviation.ConfidenceInterval(0.95).ClosedContains(uncertainties.Mean));

        }

        [TestMethod]
        public void SampleFitChiSquaredTest () {

            Distribution distribution = new ChiSquaredDistribution(4);
            Sample sample = CreateSample(distribution, 100);

            // fit to normal should be bad
            // this is harder than others, because a chi^2 isn't so very different from a normal; to help, increse N or decrease vu
            FitResult nfit = sample.FitToNormalDistribution();
            Console.WriteLine("P_n = {0}", nfit.GoodnessOfFit.LeftProbability);
            Assert.IsTrue(nfit.GoodnessOfFit.LeftProbability > 0.95, String.Format("P_n = {0}", nfit.GoodnessOfFit.LeftProbability));

            // fit to exponential should also be bad
            FitResult efit = sample.FitToExponentialDistribution();
            Console.WriteLine("P_e = {0}", efit.GoodnessOfFit.LeftProbability);
            Assert.IsTrue(efit.GoodnessOfFit.LeftProbability > 0.95, String.Format("P_e = {0}", efit.GoodnessOfFit.LeftProbability));

        }

        [TestMethod]
        public void SampleTTestTest () {

            // start with a normally distributed population
            Distribution xDistribution = new NormalDistribution(2.0, 3.0);

            // draw 100 samples from it and compute the t statistic for each
            Sample tSample = new Sample();
            for (int i = 0; i < 50; i++) {
                Sample xSample = CreateSample(xDistribution, 10, i);
                TestResult tResult = xSample.StudentTTest(2.0);
                double t = tResult.Statistic;
                Console.WriteLine("t = {0}", t);
                tSample.Add(t);
            }

            // sanity check our sample of t's
            Assert.IsTrue(tSample.Count == 50);

            // check that the t statistics are distributed as expected
            Distribution tDistribution = new StudentDistribution(10);

            // check on the mean
            Console.WriteLine("m = {0} vs. {1}", tSample.PopulationMean, tDistribution.Mean);
            Assert.IsTrue(tSample.PopulationMean.ConfidenceInterval(0.95).ClosedContains(tDistribution.Mean), String.Format("{0} vs. {1}", tSample.PopulationMean, tDistribution.Mean));

            // check on the standard deviation
            Console.WriteLine("s = {0} vs. {1}", tSample.PopulationStandardDeviation, tDistribution.StandardDeviation);
            Assert.IsTrue(tSample.PopulationStandardDeviation.ConfidenceInterval(0.95).ClosedContains(tDistribution.StandardDeviation));

            // do a KS test
            TestResult ksResult = tSample.KolmogorovSmirnovTest(tDistribution);
            Assert.IsTrue(ksResult.LeftProbability < 0.95);
            Console.WriteLine("D = {0}", ksResult.Statistic);

            // check that we can distinguish the t distribution from a normal distribution?
        }

        [TestMethod]
        public void SampleComparisonTest () {

            // create one set of samples from our distributions
            Sample[] aSamples = new Sample[distributions.Length];
            for (int i = 0; i < distributions.Length; i++) {
                aSamples[i] = CreateSample(distributions[i], 40, 1);
            }

            // create another set
            Sample[] bSamples = new Sample[distributions.Length];
            for (int i = 0; i < distributions.Length; i++) {
                bSamples[i] = CreateSample(distributions[i], 80, 2);
            }

            KolmogorovDistribution kd = new KolmogorovDistribution();
            Console.WriteLine("P={0} => D={1}", 0.50, kd.InverseLeftProbability(0.50));
            Console.WriteLine("P={0} => D={1}", 0.90, kd.InverseLeftProbability(0.90));
            Console.WriteLine("P={0} => D={1}", 0.95, kd.InverseLeftProbability(0.95));
            Console.WriteLine("P={0} => D={1}", 0.99, kd.InverseLeftProbability(0.99));

            // cross-test using KS; like samples should agree and unlike samples should be distinguished
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {

                    //aSamples[0] = new Sample(new double[] { 10, 19, 15, 20, 12, 8, 15, 21 });
                    //bSamples[0] = new Sample(new double[] { 15, 22, 17, 9, 12, 10, 29, 11, 25, 31 });

                    //foreach (double datum in aSamples[i]) Console.WriteLine("a={0}", datum);
                    //foreach (double datum in bSamples[j]) Console.WriteLine("b={0}", datum);

                    TestResult result = aSamples[i].KolmogorovSmirnovTest(bSamples[j]);
                    Console.WriteLine("{0} v. {1}: D={2} P={3}", i, j, result.Statistic, result.LeftProbability);
                    if (i == j) {
                        Assert.IsTrue(result.LeftProbability < 0.90);
                    } else {
                        Assert.IsTrue(result.LeftProbability > 0.90);
                    }

                    // the order shouldn't matter
                    TestResult reverse = bSamples[j].KolmogorovSmirnovTest(aSamples[i]);
                    Assert.IsTrue(reverse.Statistic == result.Statistic);
                    Assert.IsTrue(reverse.RightProbability == result.RightProbability);

                }
            }

        }

        public void TestMoments (Distribution d) {

            // the support gives the limits of integration
            Interval support = d.Support;

            // raw moments
            double[] M = new double[6];
            for (int n = 0; n < 6; n++) {
                // define x^n p(x)
                Function<double, double> raw = delegate(double x) {
                    return (Math.Pow(x, n) * d.ProbabilityDensity(x));
                }; 
                // integrate it
                M[n] = FunctionMath.Integrate(raw, support);
                // compare with the claimed result
                Console.WriteLine("M{0} {1} v. {2}", n, M[n], d.Moment(n));
            }

            // central moments
            double[] C = new double[6];
            for (int n = 0; n < 6; n++) {
                // define (x-m)^n p(x)
                Function<double, double> central = delegate(double x) {
                    return (Math.Pow(x - M[1], n) * d.ProbabilityDensity(x));
                };
                // integrate it
                C[n] = FunctionMath.Integrate(central, support);
                // compare with the claimed result
                Console.WriteLine("C{0} {1} v. {2}", n, C[n], d.MomentAboutMean(n));
            }

            Console.WriteLine("Mean {0} v. {1}", M[1], d.Mean);
            Console.WriteLine("Standard Deviation {0} v. {1}", Math.Sqrt(C[2]), d.StandardDeviation);

        }

        [TestMethod]
        public void TestADistribution () {

            TestMoments(new ExponentialDistribution(1.0));

        }

        [TestMethod]
        public void SampleKolmogorovSmirnovTest () {

            // this test has a whiff of meta-statistics about it
            // we want to make sure that the KS test statistic D is distributed according to the Kolmogorov
            // distribution; to do this, we create a sample of D statistics and do KS/Kuiper tests
            // comparing it to the claimed Kolmogorov distribution

            // start with any 'ol underlying distribution
            Distribution distribution = new UniformDistribution(Interval.FromEndpoints(-2.0, 4.0));

            // generate some samples from it, and for each one get a D statistic from a KS test
            Sample DSample = new Sample();
            Distribution DDistribution = null;
            for (int i = 0; i < 25; i++) {
                // the sample size must be large enough that the asymptotic assumptions are satistifed
                // at the moment this test fails if we make the sample size much smaller; we should
                // be able shrink this number when we expose the finite-sample distributions
                Sample sample = CreateSample(distribution, 250, i);
                TestResult ks = sample.KolmogorovSmirnovTest(distribution);
                double D = ks.Statistic;
                Console.WriteLine("D = {0}", D);
                DSample.Add(D);
                DDistribution = ks.Distribution;
            }

            // check on the mean
            Console.WriteLine("m = {0} vs. {1}", DSample.PopulationMean, DDistribution.Mean);
            Assert.IsTrue(DSample.PopulationMean.ConfidenceInterval(0.95).ClosedContains(DDistribution.Mean), String.Format("{0} vs. {1}", DSample.PopulationMean, DDistribution.Mean));

            // check on the standard deviation
            Console.WriteLine("s = {0} vs. {1}", DSample.PopulationStandardDeviation, DDistribution.StandardDeviation);
            Assert.IsTrue(DSample.PopulationStandardDeviation.ConfidenceInterval(0.95).ClosedContains(DDistribution.StandardDeviation));

            // do a KS test comparing the sample to the expected distribution
            TestResult kst = DSample.KolmogorovSmirnovTest(DDistribution);
            Console.WriteLine("D = {0}, P = {1}", kst.Statistic, kst.LeftProbability);
            Assert.IsTrue(kst.LeftProbability < 0.95);

            // do a Kuiper test comparing the sample to the expected distribution
            TestResult kut = DSample.KuiperTest(DDistribution);
            Console.WriteLine("V = {0}, P = {1}", kut.Statistic, kut.LeftProbability);
            Assert.IsTrue(kut.LeftProbability < 0.95);


        }


        [TestMethod]
        public void SampleKuiperTest () {

            // this test has a whiff of meta-statistics about it
            // we want to make sure that the Kuiper test statistic V is distributed according to the Kuiper
            // distribution; to do this, we create a sample of V statistics and do KS/Kuiper tests
            // comparing it to the claimed Kuiper distribution
            
            // start with any 'ol underlying distribution
            Distribution distribution = new ExponentialDistribution(2.0);

            // generate some samples from it, and for each one get a V statistic from a KS test
            Sample VSample = new Sample();
            Distribution VDistribution = null;
            for (int i = 0; i < 25; i++) {
                // the sample size must be large enough that the asymptotic assumptions are satistifed
                // at the moment this test fails if we make the sample size much smaller; we should
                // be able shrink this number when we expose the finite-sample distributions
                Sample sample = CreateSample(distribution, 250, i);
                TestResult kuiper = sample.KuiperTest(distribution);
                double V = kuiper.Statistic;
                Console.WriteLine("V = {0}", V);
                VSample.Add(V);
                VDistribution = kuiper.Distribution;
            }

            // check on the mean
            Console.WriteLine("m = {0} vs. {1}", VSample.PopulationMean, VDistribution.Mean);
            Assert.IsTrue(VSample.PopulationMean.ConfidenceInterval(0.95).ClosedContains(VDistribution.Mean));

            // check on the standard deviation
            Console.WriteLine("s = {0} vs. {1}", VSample.PopulationStandardDeviation, VDistribution.StandardDeviation);
            Assert.IsTrue(VSample.PopulationStandardDeviation.ConfidenceInterval(0.95).ClosedContains(VDistribution.StandardDeviation));

            // do a KS test comparing the sample to the expected distribution
            TestResult kst = VSample.KolmogorovSmirnovTest(VDistribution);
            Console.WriteLine("D = {0}, P = {1}", kst.Statistic, kst.LeftProbability);
            Assert.IsTrue(kst.LeftProbability < 0.95);

            // do a Kuiper test comparing the sample to the expected distribution
            TestResult kut = VSample.KuiperTest(VDistribution);
            Console.WriteLine("V = {0}, P = {1}", kut.Statistic, kut.LeftProbability);
            Assert.IsTrue(kut.LeftProbability < 0.95);

        }

        [TestMethod]
        public void SampleMaximumLikelihoodFitTest () {

            // normal distriubtion

            double mu = -1.0;
            double sigma = 2.0;
            Distribution nd = new NormalDistribution(mu, sigma);
            Sample ns = CreateSample(nd, 500);
            FitResult nr = ns.MaximumLikelihoodFit(new NormalDistribution(mu + 1.0, sigma + 1.0));

            Console.WriteLine(nr.Parameter(0));
            Console.WriteLine(nr.Parameter(1));

            Assert.IsTrue(nr.Dimension == 2);
            Assert.IsTrue(nr.Parameter(0).ConfidenceInterval(0.95).ClosedContains(mu));
            Assert.IsTrue(nr.Parameter(1).ConfidenceInterval(0.95).ClosedContains(sigma));

            Console.WriteLine(nr.Covariance(0,1));

            // test analytic expression
            Assert.IsTrue(TestUtilities.IsNearlyEqual(nr.Parameter(0).Value, ns.Mean, Math.Sqrt(TestUtilities.TargetPrecision)));

            // exponential distribution
            double em = 3.0;
            Distribution ed = new ExponentialDistribution(em);
            Sample es = CreateSample(ed, 100);
            FitResult er = es.MaximumLikelihoodFit(new ExponentialDistribution(em + 1.0));

            Console.WriteLine(er.Parameter(0));

            Assert.IsTrue(er.Dimension == 1);
            Assert.IsTrue(er.Parameter(0).ConfidenceInterval(0.95).ClosedContains(em));

            // test against analytic expression
            Assert.IsTrue(TestUtilities.IsNearlyEqual(er.Parameter(0).Value, es.Mean, Math.Sqrt(TestUtilities.TargetPrecision)));
            Assert.IsTrue(TestUtilities.IsNearlyEqual(er.Parameter(0).Uncertainty, es.Mean / Math.Sqrt(es.Count), Math.Sqrt(TestUtilities.TargetPrecision)));

            // lognormal distribution

            double l1 = -4.0;
            double l2 = 5.0;

            Distribution ld = new LognormalDistribution(l1, l2);
            Sample ls = CreateSample(ld, 100);
            FitResult lr = ls.MaximumLikelihoodFit(new LognormalDistribution(l1 + 1.0, l2 + 1.0));

            Console.WriteLine(lr.Parameter(0));
            Console.WriteLine(lr.Parameter(1));
            Console.WriteLine(lr.Covariance(0, 1));

            // weibull distribution

            double w_scale = 4.0;
            double w_shape = 2.0;
            Distribution w_d = new WeibullDistribution(w_scale, w_shape);
            Sample w_s = CreateSample(w_d, 20);
            FitResult w_r = w_s.MaximumLikelihoodFit(new WeibullDistribution(1.0, 0.5));

            Console.WriteLine(w_r.Parameter(0));
            Console.WriteLine(w_r.Parameter(1));
            Console.WriteLine(w_r.Covariance(0, 1));

            Assert.IsTrue(w_r.Parameter(0).ConfidenceInterval(0.95).ClosedContains(w_scale));
            Assert.IsTrue(w_r.Parameter(1).ConfidenceInterval(0.95).ClosedContains(w_shape));

            // logistic distribution
            double logistic_m = -3.0;
            double logistic_s = 2.0;
            Distribution logistic_distribution = new LogisticDistribution(logistic_m, logistic_s);
            Sample logistic_sample = CreateSample(logistic_distribution, 100);
            FitResult logistic_result = logistic_sample.MaximumLikelihoodFit(new LogisticDistribution());

            Console.WriteLine("Logistic:");
            Console.WriteLine(logistic_result.Parameter(0));
            Console.WriteLine(logistic_result.Parameter(1));

            Assert.IsTrue(logistic_result.Dimension == 2);
            Assert.IsTrue(logistic_result.Parameter(0).ConfidenceInterval(0.95).ClosedContains(logistic_m));
            Assert.IsTrue(logistic_result.Parameter(1).ConfidenceInterval(0.95).ClosedContains(logistic_s));


        }

        [TestMethod]
        public void SampleMannWhitneyComputationTest () {

            Sample sample1 = new Sample();
            sample1.Add(new double[] { 1.2, 1.3, 1.7, 2.4, 5.3 });

            Sample sample2 = new Sample();
            sample2.Add(new double[] { 1.0, 2.0, 3.0, 4.0 });

            TestResult result = sample1.MannWhitneyTest(sample2);
            Console.WriteLine(result.Statistic);
            Console.WriteLine(result.LeftProbability);
            Console.WriteLine(result.RightProbability);

        }

        [TestMethod]
        public void SampleFisherFTest () {


            // create 3 samles
            // 1 and 2 have the same mean but different variances, the F test should catch the difference
            // 1 and 3 have different means but the same variance, the F test should rule them equivilent
            Sample sample1 = CreateSample(new NormalDistribution(1.0, 1.0), 20, 1);
            Sample sample2 = CreateSample(new NormalDistribution(1.0, 2.0), 20, 2);
            Sample sample3 = CreateSample(new NormalDistribution(3.0, 1.0), 20, 3);

            TestResult f12 = sample1.FisherFTest(sample2);
            TestResult f21 = sample2.FisherFTest(sample1);


            // sample 1 has a smaller variance
            Console.WriteLine(f12.Statistic);
            Assert.IsTrue(f12.Statistic < 1.0);

            // 1/2 is the inverse of 2/1
            Assert.IsTrue(TestUtilities.IsNearlyEqual(f12.Statistic, 1.0 / f21.Statistic));
            
            // the F test detects the difference between the variance of 1 and 2
            Console.WriteLine(f12.LeftProbability);
            Assert.IsTrue(f12.RightProbability > 0.95);

            // the F test detects no difference between the variance of 1 and 3
            TestResult f13 = sample1.FisherFTest(sample3);
            Console.WriteLine(f13.Statistic);
            Console.WriteLine(f13.LeftProbability);
            Assert.IsTrue((f13.LeftProbability > 0.05) && (f13.RightProbability > 0.05));

        }

        [TestMethod]
        public void SampleMannWhitneyTest () {

            // define two non-normal distributions
            Distribution d1 = new ExponentialDistribution(2.0);
            Distribution d2 = new ExponentialDistribution(3.0);

            // create three samples from them
            Sample s1a = CreateSample(d1, 20, 1);
            Sample s1b = CreateSample(d1, 30, 2);
            Sample s2 = CreateSample(d2, 40, 3);

            // Mann-Whitney test 1a vs. 1b; they should not be distinguished
            TestResult rab = s1a.MannWhitneyTest(s1b);
            Console.WriteLine("{0} {1}", rab.Statistic, rab.LeftProbability);
            Assert.IsTrue((rab.LeftProbability < 0.95) && (rab.RightProbability < 0.95));

            // Mann-Whitney test 1 vs. 2; they should be distinguished
            // with 1 consistently less than 2, so U abnormally small
            TestResult r12 = s1b.MannWhitneyTest(s2);
            Console.WriteLine("{0} {1}", r12.Statistic, r12.LeftProbability);
            Assert.IsTrue(r12.RightProbability > 0.95);

        }


    }
}
