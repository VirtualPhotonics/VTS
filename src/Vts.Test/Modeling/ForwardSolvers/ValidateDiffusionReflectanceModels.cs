using System;
using NUnit.Framework;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class ValidateDiffusionReflectanceModels
    {
        const double thresholdValue = 1e-5;
        const double mua = 0.01;
        const double musp = 1;
        const double n = 1.4;
        const double g = 0.8;
        static double f1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(n);
        static double F1 = CalculatorToolbox.GetFresnelReflectionMomentOfOrderM(1, n, 1.0);
        static double f2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(n);


        private static OpticalProperties ops = new OpticalProperties(mua, musp, g, n);
        private DiffusionParameters dp = DiffusionParameters.Create(ops, ForwardModel.SDA);


        private double[] rhos = new double[] { 1, 3, 10 }; //[mm]


        #region SteadyState Reflectance

        [Test]
        public void SteadyStatePointSourceReflectanceTest()
        {
            var _pointSourceForwardSolver = new PointSourceSDAForwardSolver();
            double[] RofRhos = new double[] { 0.0222732, 0.0044358133, 0.0001689322 };

            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(_pointSourceForwardSolver.StationaryReflectance(dp, rhos[irho], f1, f2)
                    - RofRhos[irho]) / RofRhos[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void SteadyStateDistributedPointSourceTest()
        {
            var _distributedPointSourceForwardSolver = new DistributedPointSourceSDAForwardSolver();
            double[] RofRhos = new double[] { 0.02091553763578, 0.00405627898546423, 0.000161842146761328 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(_distributedPointSourceForwardSolver.StationaryReflectance(dp, rhos[irho], f1, f2)
                    - RofRhos[irho]) / RofRhos[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void SteadyStateGaussianBeamSourceTest()
        {
            var _gaussianSourceForwardSolver = new DistributedGaussianSourceSDAForwardSolver(1.0);
            double[] RofRhos = new double[] { 0.0275484377948659, 0.0056759402180221, 0.000216099942550358 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(_gaussianSourceForwardSolver.StationaryReflectance(dp, rhos[irho], f1, f2) -
                    RofRhos[irho]) / RofRhos[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test not passed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        #endregion SteadyState Reflectance

        #region Temporal Reflectance
        const double t = 0.05; //ns
        [Test]
        public void TemporalPointSourceReflectanceTest()
        {
            var _pointSourceForwardSolver = new PointSourceSDAForwardSolver();
            double[] RofRhoAndTs = new double[] { 0.0687161839, 0.039016833775, 6.240182423e-5 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(_pointSourceForwardSolver.TemporalReflectance(dp, rhos[irho], t, f1, f2)
                    - RofRhoAndTs[irho]) / RofRhoAndTs[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void TemporalDistributedPointSourceTest()
        {
            var _distributedPointSourceForwardSolver = new DistributedPointSourceSDAForwardSolver();
            double[] RofRhoAndTs = new double[] { 0.062455516676, 0.035462046554, 5.67164524087361e-5 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(_distributedPointSourceForwardSolver.TemporalReflectance(dp, rhos[irho], t, f1, f2)
                    - RofRhoAndTs[irho]) / RofRhoAndTs[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        #endregion Temporal Reflectance

        #region Temporal Frequency Reflectance

        [Test]
        public void TemporalFrequencyPointSourceTest()
        {
            var _pointSourceForwardSolver = new PointSourceSDAForwardSolver();
            //SDAForwardSolver(SourceConfiguration.Point, 0.0);
            double ft = 0.5; //GHz
            double[] RofRhoAndFts = new double[] { 0.0222676367133622, 0.00434066741026309, 0.000145460413024483 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = RofRhoAndFts[irho] != 0
                    ? Math.Abs(_pointSourceForwardSolver.RofRhoAndFt(ops, rhos[irho], ft).Magnitude - RofRhoAndFts[irho]) / RofRhoAndFts[irho]
                    : Math.Abs(_pointSourceForwardSolver.RofRhoAndFt(ops, rhos[irho], ft).Magnitude);
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void TemporalFrequencyDistributedPointSourceTest()
        {
            var _distributedPointSourceForwardSolver = new DistributedPointSourceSDAForwardSolver();
            //SDAForwardSolver(SourceConfiguration.Distributed, 0.0);
            double ft = 0.5; //GHz
            double[] RofRhoAndFts = new double[] { 0.0206970326199628, 0.00392410286453726, 0.00013761216706729 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = RofRhoAndFts[irho] != 0
                  ? Math.Abs(_distributedPointSourceForwardSolver.RofRhoAndFt(ops, rhos[irho], ft).Magnitude - RofRhoAndFts[irho]) / RofRhoAndFts[irho]
                  : Math.Abs(_distributedPointSourceForwardSolver.RofRhoAndFt(ops, rhos[irho], ft).Magnitude);

                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                "mm, with relative difference " + relDiff);
            }
        }

        #endregion Temporal Frequency Reflectance


        #region Stationary Spatial Frequency Reflectance
        //[Test]
        //public void stationary_spatialFrequencyReflectance_test()
        //{
        //    var _sdaForwardSolver = new SDAForwardSolver();
        //    double[] fxs = new double[] { 0.02, 0.3 };
        //    double[] RofFxs = new double[] { 0.5018404077939, 0.0527595185230724 };

        //    for (int ifx = 0; ifx < fxs.Length; ifx++)
        //    {
        //        var relDiff = _sdaForwardSolver.RofFx(ops, fxs[ifx])
        //            - RofFxs[ifx]) / RofFxs[ifx];
        //        Assert.IsTrue(relDiff < thresholdValue, "Test failed for fx =" + fxs[ifx] +
        //            "1/mm, with relative difference " + relDiff);
        //    }
        //}
        #endregion Stationary Spatial Frequency Reflectance




    }
}
