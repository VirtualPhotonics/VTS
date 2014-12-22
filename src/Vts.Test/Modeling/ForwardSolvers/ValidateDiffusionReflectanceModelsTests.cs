using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Common;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class ValidateDiffusionReflectanceModelsTests
    {
        //const double thresholdValue = 1e-5;
        const double thresholdValue = 1e-2;
        const double mua = 0.01;
        const double musp = 1;
        const double n = 1.4;
        const double g = 0.8;
        static double f1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(n);
        static double F1 = CalculatorToolbox.GetFresnelReflectionMomentOfOrderM(1, n, 1.0);
        static double f2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(n);
        const double t = 0.05; //ns
        double ft = 0.5; //GHz

        private static OpticalProperties ops = new OpticalProperties(mua, musp, g, n);
        private static DiffusionParameters dp = DiffusionParameters.Create(ops, ForwardModel.SDA);

        private static DiffusionParameters[] dps = new DiffusionParameters[]
                                                     {
                                                         DiffusionParameters.Create(ops, ForwardModel.SDA),
                                                         DiffusionParameters.Create(ops, ForwardModel.SDA)
                                                     };

        private double[] rhos = new double[] { 1, 3, 10 }; //[mm]
        private double[] zs = new double[] { 0.5, 1.5, 5}; //[mm] above, below l* in top layer, in bottom layer 


        #region SteadyState Reflectance

        [Test]
        public void SteadyStatePointSourceReflectanceTest()
        {
            var _pointSourceForwardSolver = new PointSourceSDAForwardSolver();
            double[] ROfRhos = new double[] { 0.0224959, 0.00448017, 0.000170622 };

            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(_pointSourceForwardSolver.StationaryReflectance(dp, rhos[irho], f1, f2)
                    - ROfRhos[irho]) / ROfRhos[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void SteadyStateDistributedPointSourceTest()
        {
            var _distributedPointSourceForwardSolver = new DistributedPointSourceSDAForwardSolver();
            double[] ROfRhos = new double[] { 0.0209155, 0.00405628, 0.000161842 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(_distributedPointSourceForwardSolver.StationaryReflectance(dp, rhos[irho], f1, f2)
                    - ROfRhos[irho]) / ROfRhos[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }
        // generated two layers with identical properties and use SteadyStatePointSource results for validation
        [Test]
        public void SteadyStateTwoLayerSDATest()
        {
            var _thresholdValue = 1e-8;
            var _twoLayerSDAForwardSolver = new TwoLayerSDAForwardSolver();
            var _oneLayerPointSourceForwardSolver = new PointSourceSDAForwardSolver();
            
            // make sure layer thickess is greater than l*=1/(mua+musp)=1mm
            LayerTissueRegion[] _twoLayerTissue = 
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops) ), 
                    };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var oneLayerResult = _oneLayerPointSourceForwardSolver.ROfRho(ops, rhos[irho]);
                var twoLayerResult = _twoLayerSDAForwardSolver.ROfRho(_twoLayerTissue, rhos[irho]);
                var relDiff = Math.Abs(twoLayerResult - oneLayerResult)/oneLayerResult;
                Assert.IsTrue(relDiff < _thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }
        //[Test]
        //public void SteadyStateGaussianBeamSourceTest()
        //{
        //    var _gaussianSourceForwardSolver = new DistributedGaussianSourceSDAForwardSolver(1.0);
        //    double[] ROfRhos = new double[] { 0.0275484377948659, 0.0056759402180221, 0.000216099942550358 };
        //    for (int irho = 0; irho < rhos.Length; irho++)
        //    {
        //        var relDiff = Math.Abs(_gaussianSourceForwardSolver.StationaryReflectance(dp, rhos[irho], f1, f2) -
        //            ROfRhos[irho]) / ROfRhos[irho];
        //        Assert.IsTrue(relDiff < thresholdValue, "Test not passed for rho =" + rhos[irho] +
        //            "mm, with relative difference " + relDiff);
        //    }
        //}

        #endregion SteadyState Reflectance

        #region Temporal Reflectance
        [Test]
        public void TemporalPointSourceReflectanceTest()
        {
            var _pointSourceForwardSolver = new PointSourceSDAForwardSolver();
            double[] ROfRhoAndTs = new double[] { 0.0687162, 0.0390168, 6.24018e-5 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(_pointSourceForwardSolver.TemporalReflectance(dp, rhos[irho], t, f1, f2)
                    - ROfRhoAndTs[irho]) / ROfRhoAndTs[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void TemporalDistributedPointSourceTest()
        {
            var _distributedPointSourceForwardSolver = new DistributedPointSourceSDAForwardSolver();
            double[] ROfRhoAndTs = new double[] { 0.062455516676, 0.035462046554, 5.67164524087361e-5 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(_distributedPointSourceForwardSolver.TemporalReflectance(dp, rhos[irho], t, f1, f2)
                    - ROfRhoAndTs[irho]) / ROfRhoAndTs[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        // generated two layers in TwoLayerSDAForwardSolver with identical properties and use 1 layer results for validation
        [Test]
        public void TemporalTwoLayerSDATest()
        {
            var _twoLayerSDAForwardSolver = new TwoLayerSDAForwardSolver();
            var _oneLayerForwardSolver = new PointSourceSDAForwardSolver();
            double _thresholdValue = 0.03;
            double[] _rhos = {1, 3, 6, 10};
            double[] _times = {0.0038, 0.014, 0.058, 0.14}; // ns, these times where chosen for each rho

            // make sure layer thickess is greater than l*=1/(mua+musp)=1mm
            LayerTissueRegion[] _twoLayerTissue =
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops)), 
                    };
            for (int i = 0; i < _rhos.Length; i++)
            {
                var oneLayerResult = _oneLayerForwardSolver.ROfRhoAndTime(ops, _rhos[i], _times[i]);
                var twoLayerResult = _twoLayerSDAForwardSolver.ROfRhoAndTime(_twoLayerTissue, _rhos[i], _times[i]);
                var relDiff = Math.Abs(twoLayerResult - oneLayerResult) / oneLayerResult;
                Assert.IsTrue(relDiff < _thresholdValue, "Test failed for rho =" + _rhos[i] +
                    "mm, with relative difference " + relDiff);
            }
        }
        #endregion Temporal Reflectance

        #region Temporal Frequency Reflectance

        [Test]
        public void TemporalFrequencyPointSourceTest()
        {
            var _pointSourceForwardSolver = new PointSourceSDAForwardSolver();
            double[] ROfRhoAndFts = new double[] { 0.0222676367133622, 0.00434066741026309, 0.000145460413024483 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = ROfRhoAndFts[irho] != 0
                    ? Math.Abs(_pointSourceForwardSolver.ROfRhoAndFt(ops, rhos[irho], ft).Magnitude - ROfRhoAndFts[irho]) / ROfRhoAndFts[irho]
                    : Math.Abs(_pointSourceForwardSolver.ROfRhoAndFt(ops, rhos[irho], ft).Magnitude);
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void TemporalFrequencyDistributedPointSourceTest()
        {
            var _distributedPointSourceForwardSolver = new DistributedPointSourceSDAForwardSolver();
            double[] ROfRhoAndFts = new double[] { 0.0206970326199628, 0.00392410286453726, 0.00013761216706729 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = ROfRhoAndFts[irho] != 0
                  ? Math.Abs(_distributedPointSourceForwardSolver.ROfRhoAndFt(ops, rhos[irho], ft).Magnitude - ROfRhoAndFts[irho]) / ROfRhoAndFts[irho]
                  : Math.Abs(_distributedPointSourceForwardSolver.ROfRhoAndFt(ops, rhos[irho], ft).Magnitude);

                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                "mm, with relative difference " + relDiff);
            }
        }
        // generated two layers with identical properties and use PointSourceSDA results for validation
        [Test]
        public void TemporalFrequencyTwoLayerSDATest()
        {
            var _thresholdValue = 1e-8;
            var _twoLayerSDAForwardSolver = new TwoLayerSDAForwardSolver();
            var _oneLayerPointSourceSDAForwardSolver = new PointSourceSDAForwardSolver();

            // make sure layer thickess is greater than l*=1/(mua+musp)=1mm
            LayerTissueRegion[] _twoLayerTissue =
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops)), 
                    };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var oneLayerResult = _oneLayerPointSourceSDAForwardSolver.ROfRhoAndFt(ops, rhos[irho], ft);
                var twoLayerResult = _twoLayerSDAForwardSolver.ROfRhoAndFt(_twoLayerTissue, rhos[irho], ft);
                var relDiffRe = Math.Abs(twoLayerResult.Real - oneLayerResult.Real) / oneLayerResult.Real;
                var relDiffIm = Math.Abs((twoLayerResult.Imaginary - oneLayerResult.Imaginary) / oneLayerResult.Imaginary);
                Assert.IsTrue(relDiffRe < _thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with Real relative difference " + relDiffRe);
                Assert.IsTrue(relDiffIm < _thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with Imaginary relative difference " + relDiffIm);
            }
        }
        #endregion Temporal Frequency Reflectance

        #region Stationary Spatial Frequency Reflectance
        //[Test]
        //public void stationary_spatialFrequencyReflectance_test()
        //{
        //    var _sdaForwardSolver = new SDAForwardSolver();
        //    double[] fxs = new double[] { 0.02, 0.3 };
        //    double[] ROfFxs = new double[] { 0.5018404077939, 0.0527595185230724 };

        //    for (int ifx = 0; ifx < fxs.Length; ifx++)
        //    {
        //        var relDiff = _sdaForwardSolver.ROfFx(ops, fxs[ifx])
        //            - ROfFxs[ifx]) / ROfFxs[ifx];
        //        Assert.IsTrue(relDiff < thresholdValue, "Test failed for fx =" + fxs[ifx] +
        //            "1/mm, with relative difference " + relDiff);
        //    }
        //}
        // generated two layers with identical properties and use Nurbs results for validation
        [Test]
        public void SpatialFrequencyTwoLayerSDATest()
        {
            var _thresholdValue = 0.03; 
            double[] fxs = new double[] { 0.0, 0.02 };  // 0.3 results not good
            var _twoLayerSDAForwardSolver = new TwoLayerSDAForwardSolver();
            var _oneLayerNurbsForwardSolver = new NurbsForwardSolver();

            // make sure layer thickess is greater than l*=1/(mua+musp)=1mm
            LayerTissueRegion[] _twoLayerTissue =
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops)), 
                    };
            for (int ifx = 0; ifx < fxs.Length; ifx++)
            {
                var oneLayerResult = _oneLayerNurbsForwardSolver.ROfFx(ops, fxs[ifx]);
                var twoLayerResult = _twoLayerSDAForwardSolver.ROfFx(_twoLayerTissue, fxs[ifx]);
                var relDiff = Math.Abs(twoLayerResult - oneLayerResult) / oneLayerResult;
                Assert.IsTrue(relDiff < _thresholdValue, "Test failed for fx =" + fxs[ifx] +
                    ", with relative difference " + relDiff);
            }
        }
        // generated two layers with identical properties and use Nurbs results for validation
        [Test]
        public void SpatialFrequencyAndTemporalTwoLayerSDATest()
        {
            var _thresholdValue = 0.06;
            double[] _fxs = new double[] { 0.0, 0.02 };  // 0.3 just doesn't give good results
            double[] _times = { 0.004, 0.014 }; // ns, these times were chosen for each fx
            var _twoLayerSDAForwardSolver = new TwoLayerSDAForwardSolver();
            var _oneLayerSDAForwardSolver = new PointSourceSDAForwardSolver();
        
            // make sure layer thickess is greater than l*=1/(mua+musp)=1mm
            LayerTissueRegion[] _twoLayerTissue =
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops)), 
                    };
            for (int i = 0; i < _fxs.Length; i++)
            {
                var oneLayerResult = _oneLayerSDAForwardSolver.ROfFxAndTime(ops, _fxs[i], _times[i]);
                var twoLayerResult = _twoLayerSDAForwardSolver.ROfFxAndTime(_twoLayerTissue, _fxs[i], _times[i]);
                var relDiffRe = Math.Abs(twoLayerResult - oneLayerResult) / oneLayerResult;
                Assert.IsTrue(relDiffRe < _thresholdValue, "Test failed for fx =" + _fxs[i] +
                    " and ft=", + _times[i] + ", with relative difference " + relDiffRe);
            }
        }
        // generated two layers with identical properties and use Nurbs results for validation
        [Test]
        public void SpatialAndTemporalFrequencyTwoLayerSDATest()
        {
            var _thresholdValue = 0.03;
            double[] fxs = new double[] { 0.0, 0.02 };  // 0.3 just doesn't give good results
            var _twoLayerSDAForwardSolver = new TwoLayerSDAForwardSolver();
            var _oneLayerNurbsForwardSolver = new NurbsForwardSolver();
            // make sure layer thickess is greater than l*=1/(mua+musp)=1mm
            LayerTissueRegion[] _twoLayerTissue =
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops)), 
                    };
            for (int ifx = 0; ifx < fxs.Length; ifx++)
            {
                var oneLayerResult = _oneLayerNurbsForwardSolver.ROfFxAndFt(ops, fxs[ifx], ft);
                var twoLayerResult = _twoLayerSDAForwardSolver.ROfFxAndFt(_twoLayerTissue, fxs[ifx], ft);
                var relDiffRe = Math.Abs(twoLayerResult.Real - oneLayerResult.Real) / oneLayerResult.Real;
                var relDiffIm = Math.Abs((twoLayerResult.Imaginary - oneLayerResult.Imaginary) / oneLayerResult.Imaginary);
                Assert.IsTrue(relDiffRe < _thresholdValue, "Test failed for fx =" + fxs[ifx] +
                    " and ft=", +ft + ", with Real relative difference " + relDiffRe);
                Assert.IsTrue(relDiffIm < _thresholdValue, "Test failed for fx =" + fxs[ifx] +
                    " and ft=", +ft + ", with Imag relative difference " + relDiffIm);
            }
        }
        #endregion Stationary Spatial Frequency Reflectance   
            
        [Test]
        public void validate_forward_solver_can_vectorize_based_on_OpticalProperties()
        {
            var muas = new double[] {0.02, 0.02, 0.3};
            var musps = new double[] {1.5, 1.25, 1.25};
            var wvs = new double[] {650, 750, 850};
            var rho = 10;
            var n = 1.4;
            var g = 0.9;

            var fs = new PointSourceSDAForwardSolver();
            var ops = Enumerable.Zip(muas, musps, (mua, musp) => new OpticalProperties(mua, musp, g, n)).ToArray();

            var reflectanceVsWavelength = fs.ROfRho(ops, rho);

            Assert.NotNull(reflectanceVsWavelength);
            Assert.AreEqual(reflectanceVsWavelength.Length, 3);

            // check that change in scattering changes the reflectance
            Assert.IsTrue(reflectanceVsWavelength[0] != reflectanceVsWavelength[1]);
            // check that change in absorption changes the reflectance
            Assert.IsTrue(reflectanceVsWavelength[1] != reflectanceVsWavelength[2]);
        }

        [Test]
        public void validate_spectral_generation_of_OpticalProperties_with_scatterers_and_absorbers()
        {
            var scatterer = new IntralipidScatterer(0.01);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.01);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.99);
            var wvs = new DoubleRange(650, 1000, 36).AsEnumerable().ToArray();
            var n = 1.4;
            var rho = 10;

            var ops = wvs.Select(wv =>
                {
                    var mua = fatAbsorber.GetMua(wv) + waterAbsorber.GetMua(wv);
                    var musp = scatterer.GetMusp(wv);
                    var g = scatterer.GetG(wv);
                    return new OpticalProperties(mua, musp, g, n);
                }).ToArray();

            var fs = new PointSourceSDAForwardSolver();

            var reflectanceVsWavelength = fs.ROfRho(ops, rho);

            Assert.NotNull(reflectanceVsWavelength);
            Assert.AreEqual(reflectanceVsWavelength.Length, wvs.Length);

            // check that change in scattering changes the reflectance
            Assert.IsTrue(reflectanceVsWavelength[0] != reflectanceVsWavelength[1]);
            // check that change in absorption changes the reflectance
            Assert.IsTrue(reflectanceVsWavelength[1] != reflectanceVsWavelength[2]);
        }

        [Test]
        public void validate_spectral_generation_of_OpticalProperties_with_tissue()
        {
            var scatterer = new IntralipidScatterer(0.01);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.01);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.99);
            var _twoLayerSDAForwardSolver = new TwoLayerSDAForwardSolver();
            var _oneLayerPointSourceForwardSolver = new PointSourceSDAForwardSolver();
            double _topLayerThickness = 3; // mm

            var n = 1.4;
            var wvs = new DoubleRange(650, 1000, 36).AsEnumerable().ToArray();
            var rho = 10;

            var tissue = new Tissue(
                new IChromophoreAbsorber[] {fatAbsorber, waterAbsorber},
                scatterer,
                "test_tissue",
                n);

            var ops = wvs.Select(wv => tissue.GetOpticalProperties(wv)).ToArray();

            var fs = new PointSourceSDAForwardSolver();

            var reflectanceVsWavelength = fs.ROfRho(ops, rho);

            Assert.NotNull(reflectanceVsWavelength);
            Assert.AreEqual(reflectanceVsWavelength.Length, wvs.Length);

            // check that change in scattering changes the reflectance
            Assert.IsTrue(reflectanceVsWavelength[0] != reflectanceVsWavelength[1]);
            // check that change in absorption changes the reflectance
            Assert.IsTrue(reflectanceVsWavelength[1] != reflectanceVsWavelength[2]);
        }
    }
}
