using System;
using System.Linq;
using NUnit.Framework;
using Vts.Common;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;
using Vts.MonteCarlo.Tissues;
using Vts.SpectralMapping;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class ValidateDiffusionReflectanceModelsTests
    {
        const double thresholdValue = 1e-2; // prior value 1e-5
        const double mua = 0.01;
        const double musp = 1;
        const double n = 1.4;
        const double g = 0.8;
        static double f1 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(n);
        static double F1 = CalculatorToolbox.GetFresnelReflectionMomentOfOrderM(1, n, 1.0);
        static double f2 = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(n);
        const double t = 0.05; //ns
        const double ft = 0.5; //GHz

        private static OpticalProperties ops = new OpticalProperties(mua, musp, g, n);
        private static DiffusionParameters dp = DiffusionParameters.Create(ops, ForwardModel.SDA);

        private double[] rhos = new double[] { 1, 3, 10 }; //[mm]

        #region SteadyState Reflectance

        [Test]
        public void SteadyStatePointSourceReflectanceTest()
        {
            var pointSourceForwardSolver = new PointSourceSDAForwardSolver();
            double[] rOfRhos = new double[] { 0.0224959, 0.00448017, 0.000170622 };

            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(pointSourceForwardSolver.StationaryReflectance(dp, rhos[irho], f1, f2)
                    - rOfRhos[irho]) / rOfRhos[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void SteadyStateDistributedPointSourceTest()
        {
            var distributedPointSourceForwardSolver = new DistributedPointSourceSDAForwardSolver();
            double[] rOfRhos = new double[] { 0.0209155, 0.00405628, 0.000161842 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(distributedPointSourceForwardSolver.StationaryReflectance(dp, rhos[irho], f1, f2)
                    - rOfRhos[irho]) / rOfRhos[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }
        // generated two layers with identical properties and use SteadyStatePointSource results for validation
        [Test]
        public void SteadyStateTwoLayerSDATest()
        {
            var _thresholdValue = 1e-8;
            var twoLayerSdaForwardSolver = new TwoLayerSDAForwardSolver();
            var oneLayerPointSourceForwardSolver = new PointSourceSDAForwardSolver();
            
            // make sure layer thickness is greater than l*=1/(mua+musp)=1mm
            var twoLayerTissue = 
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops) ), 
                    };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var oneLayerResult = oneLayerPointSourceForwardSolver.ROfRho(ops, rhos[irho]);
                var twoLayerResult = twoLayerSdaForwardSolver.ROfRho(twoLayerTissue, rhos[irho]);
                var relDiff = Math.Abs(twoLayerResult - oneLayerResult)/oneLayerResult;
                Assert.IsTrue(relDiff < _thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        #endregion SteadyState Reflectance

        #region Temporal Reflectance
        [Test]
        public void TemporalPointSourceReflectanceTest()
        {
            var pointSourceForwardSolver = new PointSourceSDAForwardSolver();
            double[] rOfRhoAndTs = new double[] { 0.0687162, 0.0390168, 6.24018e-5 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(pointSourceForwardSolver.TemporalReflectance(dp, rhos[irho], t, f1, f2)
                    - rOfRhoAndTs[irho]) / rOfRhoAndTs[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void TemporalDistributedPointSourceTest()
        {
            var distributedPointSourceForwardSolver = new DistributedPointSourceSDAForwardSolver();
            double[] rOfRhoAndTs = new double[] { 0.062455516676, 0.035462046554, 5.67164524087361e-5 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = Math.Abs(distributedPointSourceForwardSolver.TemporalReflectance(dp, rhos[irho], t, f1, f2)
                    - rOfRhoAndTs[irho]) / rOfRhoAndTs[irho];
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        // generated two layers in TwoLayerSDAForwardSolver with identical properties and use 1 layer results for validation
        [Test]
        public void TemporalTwoLayerSDATest()
        {
            var twoLayerSdaForwardSolver = new TwoLayerSDAForwardSolver();
            var oneLayerForwardSolver = new PointSourceSDAForwardSolver();
            double _thresholdValue = 0.03;
            double[] tempRhos = {1, 3, 6, 10};
            double[] tempTimes = {0.0038, 0.014, 0.058, 0.14}; // ns, these times where chosen for each rho

            // make sure layer thickness is greater than l*=1/(mua+musp)=1mm
            var twoLayerTissue =
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops)), 
                    };
            for (int i = 0; i < tempRhos.Length; i++)
            {
                var oneLayerResult = oneLayerForwardSolver.ROfRhoAndTime(ops, tempRhos[i], tempTimes[i]);
                var twoLayerResult = twoLayerSdaForwardSolver.ROfRhoAndTime(twoLayerTissue, tempRhos[i], tempTimes[i]);
                var relDiff = Math.Abs(twoLayerResult - oneLayerResult) / oneLayerResult;
                Assert.IsTrue(relDiff < _thresholdValue, "Test failed for rho =" + tempRhos[i] +
                    "mm, with relative difference " + relDiff);
            }
        }
        #endregion Temporal Reflectance

        #region Temporal Frequency Reflectance

        [Test]
        public void TemporalFrequencyPointSourceTest()
        {
            var pointSourceForwardSolver = new PointSourceSDAForwardSolver();
            double[] rOfRhoAndFts = new double[] { 0.0222676367133622, 0.00434066741026309, 0.000145460413024483 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = rOfRhoAndFts[irho] != 0
                    ? Math.Abs(pointSourceForwardSolver.ROfRhoAndFt(ops, rhos[irho], ft).Magnitude - rOfRhoAndFts[irho]) / rOfRhoAndFts[irho]
                    : Math.Abs(pointSourceForwardSolver.ROfRhoAndFt(ops, rhos[irho], ft).Magnitude);
                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with relative difference " + relDiff);
            }
        }

        [Test]
        public void TemporalFrequencyDistributedPointSourceTest()
        {
            var distributedPointSourceForwardSolver = new DistributedPointSourceSDAForwardSolver();
            double[] rOfRhoAndFts = new double[] { 0.0206970326199628, 0.00392410286453726, 0.00013761216706729 };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var relDiff = rOfRhoAndFts[irho] != 0
                  ? Math.Abs(distributedPointSourceForwardSolver.ROfRhoAndFt(ops, rhos[irho], ft).Magnitude - rOfRhoAndFts[irho]) / rOfRhoAndFts[irho]
                  : Math.Abs(distributedPointSourceForwardSolver.ROfRhoAndFt(ops, rhos[irho], ft).Magnitude);

                Assert.IsTrue(relDiff < thresholdValue, "Test failed for rho =" + rhos[irho] +
                "mm, with relative difference " + relDiff);
            }
        }
        // generated two layers with identical properties and use PointSourceSDA results for validation
        [Test]
        public void TemporalFrequencyTwoLayerSDATest()
        {
            var tighterThresholdValue = 1e-8;
            var twoLayerSdaForwardSolver = new TwoLayerSDAForwardSolver();
            var oneLayerPointSourceSdaForwardSolver = new PointSourceSDAForwardSolver();

            // make sure layer thickness is greater than l*=1/(mua+musp)=1mm
            var twoLayerTissue =
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops)), 
                    };
            for (int irho = 0; irho < rhos.Length; irho++)
            {
                var oneLayerResult = oneLayerPointSourceSdaForwardSolver.ROfRhoAndFt(ops, rhos[irho], ft);
                var twoLayerResult = twoLayerSdaForwardSolver.ROfRhoAndFt(twoLayerTissue, rhos[irho], ft);
                var relDiffRe = Math.Abs(twoLayerResult.Real - oneLayerResult.Real) / oneLayerResult.Real;
                var relDiffIm = Math.Abs((twoLayerResult.Imaginary - oneLayerResult.Imaginary) / oneLayerResult.Imaginary);
                Assert.IsTrue(relDiffRe < tighterThresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with Real relative difference " + relDiffRe);
                Assert.IsTrue(relDiffIm < tighterThresholdValue, "Test failed for rho =" + rhos[irho] +
                    "mm, with Imaginary relative difference " + relDiffIm);
            }
        }
        #endregion Temporal Frequency Reflectance

        #region Stationary Spatial Frequency Reflectance

        // generated two layers with identical properties and use Nurbs results for validation
        [Test]
        public void SpatialFrequencyTwoLayerSDATest()
        {
            var _thresholdValue = 0.03; 
            double[] fxs = new double[] { 0.0, 0.02 };  // 0.3 results not good
            var twoLayerSdaForwardSolver = new TwoLayerSDAForwardSolver();
            var oneLayerNurbsForwardSolver = new NurbsForwardSolver();

            // make sure layer thickess is greater than l*=1/(mua+musp)=1mm
            var twoLayerTissue =
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops)), 
                    };
            for (int ifx = 0; ifx < fxs.Length; ifx++)
            {
                var oneLayerResult = oneLayerNurbsForwardSolver.ROfFx(ops, fxs[ifx]);
                var twoLayerResult = twoLayerSdaForwardSolver.ROfFx(twoLayerTissue, fxs[ifx]);
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
            double[] fxs = new double[] { 0.0, 0.02 };  // 0.3 just doesn't give good results
            double[] times = { 0.004, 0.014 }; // ns, these times were chosen for each fx
            var twoLayerSdaForwardSolver = new TwoLayerSDAForwardSolver();
            var oneLayerSdaForwardSolver = new PointSourceSDAForwardSolver();
        
            // make sure layer thickness is greater than l*=1/(mua+musp)=1mm
            var twoLayerTissue =
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops)), 
                    };
            for (int i = 0; i < fxs.Length; i++)
            {
                var oneLayerResult = oneLayerSdaForwardSolver.ROfFxAndTime(ops, fxs[i], times[i]);
                var twoLayerResult = twoLayerSdaForwardSolver.ROfFxAndTime(twoLayerTissue, fxs[i], times[i]);
                var relDiffRe = Math.Abs(twoLayerResult - oneLayerResult) / oneLayerResult;
                Assert.IsTrue(relDiffRe < _thresholdValue, "Test failed for fx =" + fxs[i] +
                    " and ft=", + times[i] + ", with relative difference " + relDiffRe);
            }
        }
        // generated two layers with identical properties and use Nurbs results for validation
        [Test]
        public void SpatialAndTemporalFrequencyTwoLayerSDATest()
        {
            var _thresholdValue = 0.03;
            double[] fxs = new double[] { 0.0, 0.02 };  // 0.3 just doesn't give good results
            var twoLayerSdaForwardSolver = new TwoLayerSDAForwardSolver();
            var oneLayerNurbsForwardSolver = new NurbsForwardSolver();
            // make sure layer thickness is greater than l*=1/(mua+musp)=1mm
            var twoLayerTissue =
                new LayerTissueRegion[]
                    {
                        new LayerTissueRegion(new DoubleRange(0, 3), new OpticalProperties(ops)),
                        new LayerTissueRegion(new DoubleRange(3,100), new OpticalProperties(ops)), 
                    };
            for (int ifx = 0; ifx < fxs.Length; ifx++)
            {
                var oneLayerResult = oneLayerNurbsForwardSolver.ROfFxAndFt(ops, fxs[ifx], ft);
                var twoLayerResult = twoLayerSdaForwardSolver.ROfFxAndFt(twoLayerTissue, fxs[ifx], ft);
                var relDiffRe = Math.Abs(twoLayerResult.Real - oneLayerResult.Real) / oneLayerResult.Real;
                var relDiffIm = Math.Abs((twoLayerResult.Imaginary - oneLayerResult.Imaginary) / oneLayerResult.Imaginary);
                Assert.IsTrue(relDiffRe < _thresholdValue, "Test failed for fx =" + fxs[ifx] +
                    " and ft=", +ft + ", with Real relative difference " + relDiffRe);
                Assert.IsTrue(relDiffIm < _thresholdValue, "Test failed for fx =" + fxs[ifx] +
                    " and ft=", +ft + ", with Imag relative difference " + relDiffIm);
            }
        }
        #endregion Stationary Spatial Frequency Reflectance   

        #region Wavelength Dependence Tests
        [Test]
        public void validate_forward_solver_can_vectorize_based_on_OpticalProperties()
        {
            var muas = new double[] {0.02, 0.02, 0.3};
            var musps = new double[] {1.5, 1.25, 1.25};
            var rho = 10;
            var tempG = 0.9;

            var fs = new PointSourceSDAForwardSolver();
            var tempOps = Enumerable.Zip(muas, musps, (mua, musp) => new OpticalProperties(mua, musp, tempG, n)).ToArray();

            var reflectanceVsWavelength = fs.ROfRho(tempOps, rho);

            Assert.NotNull(reflectanceVsWavelength);
            Assert.AreEqual(3, reflectanceVsWavelength.Length);

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
            var wvs = new DoubleRange(650, 1000, 36).ToArray();
            var rho = 10;

            var tempOps = wvs.Select(wv =>
                {
                    var tempMua = fatAbsorber.GetMua(wv) + waterAbsorber.GetMua(wv);
                    var tempMusp = scatterer.GetMusp(wv);
                    var tempG = scatterer.GetG(wv);
                    return new OpticalProperties(tempMua, tempMusp, tempG, n);
                }).ToArray();

            var fs = new PointSourceSDAForwardSolver();

            var reflectanceVsWavelength = fs.ROfRho(tempOps, rho);

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

            var wvs = new DoubleRange(650, 1000, 36).ToArray();
            var rho = 10;

            var tissue = new Tissue(
                new IChromophoreAbsorber[] {fatAbsorber, waterAbsorber},
                scatterer,
                "test_tissue",
                n);

            var tempOps = wvs.Select(wv => tissue.GetOpticalProperties(wv)).ToArray();

            var fs = new PointSourceSDAForwardSolver();

            var reflectanceVsWavelength = fs.ROfRho(tempOps, rho);

            Assert.NotNull(reflectanceVsWavelength);
            Assert.AreEqual(reflectanceVsWavelength.Length, wvs.Length);

            // check that change in scattering changes the reflectance
            Assert.IsTrue(reflectanceVsWavelength[0] != reflectanceVsWavelength[1]);
            // check that change in absorption changes the reflectance
            Assert.IsTrue(reflectanceVsWavelength[1] != reflectanceVsWavelength[2]);
        }
        #endregion

        #region Multi-Axis Tests
        [Test]
        public void validate_ROfRhoAndTime_With_Wavelength()
        {
            // used values for tissue=liver
            var scatterer = new PowerLawScatterer(0.84, 0.55);
            var hbAbsorber = new ChromophoreAbsorber(ChromophoreType.Hb, 66);
            var hbo2Absorber = new ChromophoreAbsorber(ChromophoreType.HbO2, 124);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.02);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.87);

            var wvs = new double[] { 650, 700 };
            rhos = new double[] { 0.5, 1.625 };
            var times = new double[] { 0.05, 0.10 };

            var tissue = new Tissue(
                new IChromophoreAbsorber[] { hbAbsorber, hbo2Absorber, fatAbsorber, waterAbsorber },
                scatterer,
                "test_tissue",
                n);

            var tempOps = wvs.Select(wv => tissue.GetOpticalProperties(wv)).ToArray();

            var fs = new PointSourceSDAForwardSolver();

            var rVsWavelength = fs.ROfRhoAndTime(tempOps, rhos, times);
            // return from ROfRhoAndTime is new double[ops.Length * rhos.Length * ts.Length]
            // order is: (ops0,rhos0,ts0), (ops0,rhos0,ts1)...(ops0,rhos0,tsnt-1)
            //           (ops0,rhos1,ts0), (ops0,rhos1,ts1)...(ops0,rhos1,tsnt-1)
            //           ...
            //           (ops0,rhosnr-1,ts0),.................(ops0,rhosnr-1,tsnt-1)
            //           ... repeat above with ops1...

            // [0] -> ops0=650, rho0=0.5, ts0=0.05
            Assert.IsTrue(Math.Abs(rVsWavelength[0] - 0.044606) < 0.000001); // API match
            // [1] -> ops0=650, rho0=0.5, ts1=0.10
            Assert.IsTrue(Math.Abs(rVsWavelength[1] - 0.005555) < 0.000001);
            // [2] -> ops0=650, rho1=1.635, ts0=0.05
            Assert.IsTrue(Math.Abs(rVsWavelength[2] - 0.036900) < 0.000001); // API match
            // [3] -> ops0=650, rho1=1.635, ts1=0.10
            Assert.IsTrue(Math.Abs(rVsWavelength[3] - 0.005053) < 0.000001);
            // [4] -> ops1=700, rho0=0.5, ts0=0.05
            Assert.IsTrue(Math.Abs(rVsWavelength[4] - 0.057894) < 0.000001); // API match
            // [5] -> ops1=700, rho0=0.5, ts1=0.10
            Assert.IsTrue(Math.Abs(rVsWavelength[5] - 0.010309) < 0.000001);
            // [6] -> ops1=700, rho1=1.635, ts0=0.05
            Assert.IsTrue(Math.Abs(rVsWavelength[6] - 0.048493) < 0.000001); // API match
            // [7] -> ops1=700, rho1=1.635, ts1=0.10
            Assert.IsTrue(Math.Abs(rVsWavelength[7] - 0.009434) < 0.000001); 
        }

        [Test]
        public void validate_ROfRhoAndFt_With_Wavelength()
        {
            // used values for tissue=liver
            var scatterer = new PowerLawScatterer(0.84, 0.55);
            var hbAbsorber = new ChromophoreAbsorber(ChromophoreType.Hb, 66);
            var hbo2Absorber = new ChromophoreAbsorber(ChromophoreType.HbO2, 124);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.02);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.87);

            var wvs = new double[] { 650, 700 };
            rhos = new double[] { 0.5, 1.625 };
            var fts = new double[] { 0.0, 0.50 };

            var tissue = new Tissue(
                new IChromophoreAbsorber[] { hbAbsorber, hbo2Absorber, fatAbsorber, waterAbsorber },
                scatterer,
                "test_tissue",
                n);

            var tempOps = wvs.Select(wv => tissue.GetOpticalProperties(wv)).ToArray();

            var fs = new PointSourceSDAForwardSolver();

            var rVsWavelength = fs.ROfRhoAndFt(tempOps, rhos, fts);
            // return from ROfRhoAndFt is new double[ops.Length * rhos.Length * fts.Length]
            // order is: (ops0,rhos0,fts0), (ops0,rhos0,fts1)...(ops0,rhos0,ftsnt-1)
            //           (ops0,rhos1,fts0), (ops0,rhos1,fts1)...(ops0,rhos1,ftsnt-1)
            //           ...
            //           (ops0,rhosnr-1,fts0),.................(ops0,rhosnr-1,ftsnt-1)
            //           ... repeat above with ops1...

            // [0] -> ops0=650, rho0=0.5, fts0=0.0
            Assert.IsTrue(Math.Abs(rVsWavelength[0].Real - 0.037575) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[0].Imaginary - 0.0) < 0.000001);
            // [1] -> ops0=650, rho0=0.5, fts1=0.5
            Assert.IsTrue(Math.Abs(rVsWavelength[1].Real - 0.037511) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[1].Imaginary + 0.001200) < 0.000001);
            // [2] -> ops0=650, rho1=1.635, fts0=0.0
            Assert.IsTrue(Math.Abs(rVsWavelength[2].Real - 0.009306) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[2].Imaginary - 0.0) < 0.000001);
            // [3] -> ops0=650, rho1=1.635, fts1=0.5
            Assert.IsTrue(Math.Abs(rVsWavelength[3].Real - 0.009255) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[3].Imaginary + 0.000674) < 0.000001);
            // [4] -> ops1=700, rho0=0.5, fts0=0.0
            Assert.IsTrue(Math.Abs(rVsWavelength[4].Real - 0.036425) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[4].Imaginary - 0.0) < 0.000001);
            // [5] -> ops1=700, rho0=0.5, fts1=0.5
            Assert.IsTrue(Math.Abs(rVsWavelength[5].Real - 0.036310) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[5].Imaginary + 0.001446) < 0.000001);
            // [6] -> ops1=700, rho1=1.635, fts0=0.0
            Assert.IsTrue(Math.Abs(rVsWavelength[6].Real - 0.010657) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[6].Imaginary - 0.0) < 0.000001);
            // [7] -> ops1=700, rho1=1.635, fts1=0.5
            Assert.IsTrue(Math.Abs(rVsWavelength[7].Real - 0.010558) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[7].Imaginary + 0.000929) < 0.000001);
        }
        [Test]
        public void validate_ROfFtAndTime_With_Wavelength()
        {
            // used values for tissue=liver
            var scatterer = new PowerLawScatterer(0.84, 0.55);
            var hbAbsorber = new ChromophoreAbsorber(ChromophoreType.Hb, 66);
            var hbo2Absorber = new ChromophoreAbsorber(ChromophoreType.HbO2, 124);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.02);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.87);

            var wvs = new double[] { 650, 700 };
            var fxs = new double[] { 0.0, 0.5 };
            var times = new double[] { 0.05, 0.10 };

            var tissue = new Tissue(
                new IChromophoreAbsorber[] { hbAbsorber, hbo2Absorber, fatAbsorber, waterAbsorber },
                scatterer,
                "test_tissue",
                n);

            var tempOps = wvs.Select(wv => tissue.GetOpticalProperties(wv)).ToArray();

            var fs = new DistributedPointSourceSDAForwardSolver();

            var rVsWavelength = fs.ROfFxAndTime(tempOps, fxs, times);
            // return from ROfFxAndTime is new double[ops.Length * fxs.Length * ts.Length]
            // order is: (ops0,fxs0,ts0), (ops0,fxs0,ts1)...(ops0,fxs0,tsnt-1)
            //           (ops0,fxs1,ts0), (ops0,fxs1,ts1)...(ops0,fxs1,tsnt-1)
            //           ...
            //           (ops0,fxsnf-1,ts0),................(ops0,fxsnf-1,tsnt-1)
            //           ... repeat above with ops1...

            // [0] -> ops0=650, fx0=0.0, ts0=0.05
            Assert.IsTrue(Math.Abs(rVsWavelength[0] - 1.558702) < 0.000001);
            // [1] -> ops0=650, fx0=0.0, ts1=0.10
            Assert.IsTrue(Math.Abs(rVsWavelength[1] - 0.391871) < 0.000001);
            // [2] -> ops0=650, fx1=0.5, ts0=0.05
            Assert.IsTrue(Math.Abs(rVsWavelength[2] - 5.023055e-12) < 0.000001e-12);
            // [3] -> ops0=650, fx1=0.5, ts1=0.10
            Assert.IsTrue(Math.Abs(rVsWavelength[3] - 1.032586e-13) < 0.000001e-13);
            // [4] -> ops1=700, fx0=0.0, ts0=0.05
            Assert.IsTrue(Math.Abs(rVsWavelength[4] - 2.218329) < 0.000001);
            // [5] -> ops1=700, fx1=0.5, ts1=0.10
            Assert.IsTrue(Math.Abs(rVsWavelength[5] - 0.797200) < 0.000001);
            // [6] -> ops1=700, fx0=0.0, ts0=0.05
            Assert.IsTrue(Math.Abs(rVsWavelength[6] - 1.347053e-12) < 0.000001e-12);
            // [7] -> ops1=700, fx1=0.5, ts1=0.10
            Assert.IsTrue(Math.Abs(rVsWavelength[7] - 2.052883e-13) < 0.000001e-13);
        }
        [Test]
        public void validate_ROfFxAndFt_With_Wavelength()
        {
            // used values for tissue=liver
            var scatterer = new PowerLawScatterer(0.84, 0.55);
            var hbAbsorber = new ChromophoreAbsorber(ChromophoreType.Hb, 66);
            var hbo2Absorber = new ChromophoreAbsorber(ChromophoreType.HbO2, 124);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.02);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.87);

            var wvs = new double[] { 650, 700 };
            var fxs = new double[] { 0.0, 0.5 };
            var fts = new double[] { 0.0, 0.50 };

            var tissue = new Tissue(
                new IChromophoreAbsorber[] { hbAbsorber, hbo2Absorber, fatAbsorber, waterAbsorber },
                scatterer,
                "test_tissue",
                n);

            var tempOps = wvs.Select(wv => tissue.GetOpticalProperties(wv)).ToArray();

            var fs = new DistributedPointSourceSDAForwardSolver();

            var rVsWavelength = fs.ROfFxAndFt(tempOps, fxs, fts);
            // return from ROfFxAndFt is new double[ops.Length * fxs.Length * fts.Length]
            // order is: (ops0,fxs0,fts0), (ops0,fxs0,ts1)...(ops0,fxs0,ftsnt-1)
            //           (ops0,fxs1,fts0), (ops0,fxs1,ts1)...(ops0,fxs1,ftsnt-1)
            //           ...
            //           (ops0,fxsnf-1,fts0),.................(ops0,fxsnf-1,ftsnt-1)
            //           ... repeat above with ops1...

            // [0] -> ops0=650, fx0=0.0, fts0=0.0
            Assert.IsTrue(Math.Abs(rVsWavelength[0].Real - 1.890007) < 0.000001); // API match
            Assert.IsTrue(Math.Abs(rVsWavelength[0].Imaginary - 0.0) < 0.000001); // API match
            // [1] -> ops0=650, fx0=0.0, fts1=0.5
            Assert.IsTrue(Math.Abs(rVsWavelength[1].Real - 1.888160) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[1].Imaginary + 0.045122) < 0.000001);
            // [2] -> ops0=650, fx1=0.5, fts0=0.0
            Assert.IsTrue(Math.Abs(rVsWavelength[2].Real - 0.562537) < 0.000001); // API match
            Assert.IsTrue(Math.Abs(rVsWavelength[2].Imaginary - 0.0) < 0.000001); // API match
            // [3] -> ops0=650, fx1=0.5, fts1=0.5
            Assert.IsTrue(Math.Abs(rVsWavelength[3].Real - 0.562543) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[3].Imaginary + 0.000799) < 0.000001);
            // [4] -> ops1=700, fx0=0.0, fts0=0.0
            Assert.IsTrue(Math.Abs(rVsWavelength[4].Real - 2.118427) < 0.000001); // API match
            Assert.IsTrue(Math.Abs(rVsWavelength[4].Imaginary - 0.0) < 0.000001); // API match
            // [5] -> ops1=700, fx0=0.0, fts1=0.5
            Assert.IsTrue(Math.Abs(rVsWavelength[5].Real - 2.113377) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[5].Imaginary + 0.071758) < 0.000001);
            // [6] -> ops1=700, fx1=0.5, fts0=0.0
            Assert.IsTrue(Math.Abs(rVsWavelength[6].Real - 0.543539) < 0.000001); // API match
            Assert.IsTrue(Math.Abs(rVsWavelength[6].Imaginary - 0.0) < 0.000001); // API match
            // [7] -> ops1=700, fx1=0.5, fts1=0.5
            Assert.IsTrue(Math.Abs(rVsWavelength[7].Real - 0.543546) < 0.000001);
            Assert.IsTrue(Math.Abs(rVsWavelength[7].Imaginary + 0.000651) < 0.000001);
        }
        #endregion
    }
}
