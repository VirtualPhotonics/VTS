using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Vts.Common;
using Vts.Factories;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.Optimizers;
using Vts.MonteCarlo.Tissues;
using Vts.SpectralMapping;

namespace Vts.Test.Factories
{
    [TestFixture]
    public class ComputationFactoryTests
    {
        private double[] _realFluence;
        private double[] _xAxis, _zAxis;
        private Complex[] _complexFluence;

        [SetUp]
        public void Setup()
        {
            // need to generate fluence to send into GetPHD
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            _realFluence = ComputationFactory.ComputeFluence(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZ,
                new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                // could have array of OPs, one set for each tissue region
                new[] { new OpticalProperties(0.01, 1, 0.8, 1.4) }, 0);
            _complexFluence = ComputationFactory.ComputeFluenceComplex(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZAndFt,
                new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new OpticalProperties(0.01, 1, 0.8, 1.4), 0);
        }

        #region Boolean tests

        [Test]
        public void IsSolverWithConstantValues_returns_true()
        {
            Assert.IsTrue(ComputationFactory.IsSolverWithConstantValues(SolutionDomainType.ROfFxAndFt));
            Assert.IsTrue(ComputationFactory.IsSolverWithConstantValues(FluenceSolutionDomainType.FluenceOfFxAndZAndTime));
        }

        [Test]
        public void IsSolverWithConstantValues_returns_false()
        {
            Assert.IsFalse(ComputationFactory.IsSolverWithConstantValues(SolutionDomainType.ROfRho));
            Assert.IsFalse(ComputationFactory.IsSolverWithConstantValues(SolutionDomainType.ROfFx));
            Assert.IsFalse(ComputationFactory.IsSolverWithConstantValues(FluenceSolutionDomainType.FluenceOfRhoAndZ));
            Assert.IsFalse(ComputationFactory.IsSolverWithConstantValues(FluenceSolutionDomainType.FluenceOfFxAndZ));
        }

        [Test]
        public void IsComplexSolver_returns_true()
        {
            Assert.IsTrue(ComputationFactory.IsComplexSolver(SolutionDomainType.ROfFxAndFt));
            Assert.IsTrue(ComputationFactory.IsComplexSolver(SolutionDomainType.ROfRhoAndFt));
            Assert.IsTrue(ComputationFactory.IsComplexSolver(FluenceSolutionDomainType.FluenceOfFxAndZAndFt));
            Assert.IsTrue(ComputationFactory.IsComplexSolver(FluenceSolutionDomainType.FluenceOfRhoAndZAndFt));
        }

        [Test]
        public void IsComplexSolver_returns_false()
        {
            Assert.IsFalse(ComputationFactory.IsComplexSolver(SolutionDomainType.ROfRhoAndTime));
            Assert.IsFalse(ComputationFactory.IsComplexSolver(FluenceSolutionDomainType.FluenceOfFxAndZAndTime));
        }
        #endregion

        #region ComputeReflectance tests
        /// <summary>
        /// Test against the ComputationFactory class ComputeReflectance routine using enum
        /// forward solver specification
        /// </summary>
        [Test]
        public void Validate_ComputeReflectance_can_be_called_using_enum_forward_solver()
        {
            var reflectance = ComputationFactory.ComputeReflectance(
                ForwardSolverType.MonteCarlo,
                SolutionDomainType.ROfRho,
                ForwardAnalysisType.R,
                new object[]
                {
                    // could have array of OPs, one set for each tissue region
                    new[] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                    new double[] { 1, 2, 3 }
                });
            Assert.IsTrue(Math.Abs(reflectance[0] - 0.021093) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeReflectance routine using
        /// IForwardSolver input
        /// </summary>
        [Test]
        public void Validate_ComputeReflectance_can_be_called_using_IForwardSolver()
        {
            var reflectance = ComputationFactory.ComputeReflectance(
                new NurbsForwardSolver(),
                SolutionDomainType.ROfFx,
                ForwardAnalysisType.dRdMua,
                new object[]
                {
                    new [] { new OpticalProperties(0.01, 1, 0.8, 1.4) },
                    new double[] { 1, 2, 3 }
                });
            Assert.IsTrue(Math.Abs(reflectance[0] + 0.005571) < 0.000001);
        }

        // Multi-Axis ComputeReflectance Tests: the following 4 test parallel tests in 
        // ValidateDiffusionReflectanceModelsTests in Multi-Axis region of code
        [Test]
        public void Validate_ROfRhoAndTime_With_Wavelength()
        {
            // used values for tissue=liver
            var scatterer = new PowerLawScatterer(0.84, 0.55);
            var hbAbsorber = new ChromophoreAbsorber(ChromophoreType.Hb, 66);
            var hbo2Absorber = new ChromophoreAbsorber(ChromophoreType.HbO2, 124);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.02);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.87);

            var wvs = new double[] { 650, 700 };
            var rhos = new[] { 0.5, 1.625 };
            var times = new[] { 0.05, 0.10 };

            var tissue = new Tissue(
                new IChromophoreAbsorber[] { hbAbsorber, hbo2Absorber, fatAbsorber, waterAbsorber },
                scatterer,
                "test_tissue");

            var ops = wvs.Select(wv => tissue.GetOpticalProperties(wv)).ToArray();

            var rVsWavelength = ComputationFactory.ComputeReflectance(
                new PointSourceSDAForwardSolver(),
                SolutionDomainType.ROfRhoAndTime,
                ForwardAnalysisType.R,
                new object[]
                {
                    ops,
                    rhos,
                    times 
                });
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
        public void Validate_ROfRhoAndFt_With_Wavelength()
        {
            // used values for tissue=liver
            var scatterer = new PowerLawScatterer(0.84, 0.55);
            var hbAbsorber = new ChromophoreAbsorber(ChromophoreType.Hb, 66);
            var hbo2Absorber = new ChromophoreAbsorber(ChromophoreType.HbO2, 124);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.02);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.87);

            var wvs = new double[] { 650, 700 };
            var rhos = new[] { 0.5, 1.625 };
            var fts = new[] { 0.0, 0.50 };

            var tissue = new Tissue(
                new IChromophoreAbsorber[] { hbAbsorber, hbo2Absorber, fatAbsorber, waterAbsorber },
                scatterer,
                "test_tissue");

            var ops = wvs.Select(wv => tissue.GetOpticalProperties(wv)).ToArray();

            var rVsWavelength = ComputationFactory.ComputeReflectance(
                new PointSourceSDAForwardSolver(),
                SolutionDomainType.ROfRhoAndFt,
                ForwardAnalysisType.R,
                new object[]
                {
                    ops,
                    rhos,
                    fts
                });
            // return from ROfRhoAndFt is new double[ops.Length * rhos.Length * fts.Length]
            // order is: (ops0,rhos0,fts0)real, (ops0,rhos0,fts1)real...(ops0,rhos0,ftsnt-1)real
            //           (ops0,rhos1,fts0)real, (ops0,rhos1,fts1)real...(ops0,rhos1,ftsnt-1)real
            //           ...
            //           (ops0,rhosnr-1,fts0)real,.................(ops0,rhosnr-1,ftsnt-1)real
            //           ... repeat above with imag, then next ops1...

            // [0] -> ops0=650, rho0=0.5, fts0=0.0 real
            Assert.IsTrue(Math.Abs(rVsWavelength[0] - 0.037575) < 0.000001);            
            // [1] -> ops0=650, rho0=0.5, fts1=0.5 real
            Assert.IsTrue(Math.Abs(rVsWavelength[1] - 0.037511) < 0.000001);
            // [2] -> ops0=650, rho1=1.635, fts0=0.0 real
            Assert.IsTrue(Math.Abs(rVsWavelength[2] - 0.009306) < 0.000001);
            // [3] -> ops0=650, rho1=1.635, fts1=0.5 real
            Assert.IsTrue(Math.Abs(rVsWavelength[3] - 0.009255) < 0.000001);
            // [4] -> ops1=700, rho0=0.5, fts0=0.0 real
            Assert.IsTrue(Math.Abs(rVsWavelength[4] - 0.036425) < 0.000001);
            // [5] -> ops1=700, rho0=0.5, fts1=0.5 real
            Assert.IsTrue(Math.Abs(rVsWavelength[5] - 0.036310) < 0.000001);
            // [6] -> ops1=700, rho1=1.635, fts0=0.0 real
            Assert.IsTrue(Math.Abs(rVsWavelength[6] - 0.010657) < 0.000001);
            // [7] -> ops1=700, rho1=1.635, fts1=0.5 real
            Assert.IsTrue(Math.Abs(rVsWavelength[7] - 0.010558) < 0.000001);
            // [8] -> ops0=650, rho0=0.5, fts0=0.0 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[8] - 0.0) < 0.000001);
            // [9] -> ops0=650, rho0=0.5, fts1=0.5 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[9] + 0.001200) < 0.000001);
            // [10] -> ops0=650, rho1=1.635, fts0=0.0 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[10] - 0.0) < 0.000001);
            // [11] -> ops1=650, rho1=1.635, fts1=0.5 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[11] + 0.000674) < 0.000001);
            // [12] -> ops1=700, rho0=0.5, fts0=0.0
            Assert.IsTrue(Math.Abs(rVsWavelength[12] - 0.0) < 0.000001);
            // [13] -> ops1=700, rho0=0.5, fts1=0.5 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[13] + 0.001446) < 0.000001);
            // [14] -> ops1=700, rho1=1.635, fts0=0.0 real
            Assert.IsTrue(Math.Abs(rVsWavelength[14] - 0.0) < 0.000001);
            // [15] -> ops1=700, rho1=1.635, fts1=0.5 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[15] + 0.000929) < 0.000001);
        }
        [Test]
        public void Validate_ROfFxAndTime_With_Wavelength()
        {
            // used values for tissue=liver
            var scatterer = new PowerLawScatterer(0.84, 0.55);
            var hbAbsorber = new ChromophoreAbsorber(ChromophoreType.Hb, 66);
            var hbo2Absorber = new ChromophoreAbsorber(ChromophoreType.HbO2, 124);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.02);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.87);

            var n = 1.4;
            var wvs = new double[] { 650, 700 };
            var fxs = new[] { 0.0, 0.5 };
            var times = new[] { 0.05, 0.10 };

            var tissue = new Tissue(
                new IChromophoreAbsorber[] { hbAbsorber, hbo2Absorber, fatAbsorber, waterAbsorber },
                scatterer,
                "test_tissue",
                n);

            var ops = wvs.Select(wv => tissue.GetOpticalProperties(wv)).ToArray();

            var rVsWavelength = ComputationFactory.ComputeReflectance(
                new DistributedPointSourceSDAForwardSolver(),
                SolutionDomainType.ROfFxAndTime,
                ForwardAnalysisType.R,
                new object[]
                {
                    ops,
                    fxs,
                    times
                });
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
        public void Validate_ROfFxAndFt_With_Wavelength()
        {
            // used values for tissue=liver
            var scatterer = new PowerLawScatterer(0.84, 0.55);
            var hbAbsorber = new ChromophoreAbsorber(ChromophoreType.Hb, 66);
            var hbo2Absorber = new ChromophoreAbsorber(ChromophoreType.HbO2, 124);
            var fatAbsorber = new ChromophoreAbsorber(ChromophoreType.Fat, 0.02);
            var waterAbsorber = new ChromophoreAbsorber(ChromophoreType.H2O, 0.87);

            var wvs = new double[] { 650, 700 };
            var fxs = new[] { 0.0, 0.5 };
            var fts = new[] { 0.0, 0.50 };

            var tissue = new Tissue(
                new IChromophoreAbsorber[] { hbAbsorber, hbo2Absorber, fatAbsorber, waterAbsorber },
                scatterer,
                "test_tissue");

            var ops = wvs.Select(wv => tissue.GetOpticalProperties(wv)).ToArray();

            var rVsWavelength = ComputationFactory.ComputeReflectance(
                new DistributedPointSourceSDAForwardSolver(),
                SolutionDomainType.ROfFxAndFt,
                ForwardAnalysisType.R,
                new object[]
                {
                    ops,
                    fxs,
                    fts
                });
            // return from ROfFxAndFt is new double[ops.Length * fxs.Length * fts.Length]
            // order is: (ops0,fxs0,fts0)real, (ops0,fxs0,ts1)real...(ops0,fxs0,ftsnt-1)real
            //           (ops0,fxs1,fts0)real, (ops0,fxs1,ts1)real...(ops0,fxs1,ftsnt-1)real
            //           ...
            //           (ops0,fxsnf-1,fts0)real,.................(ops0,fxsnf-1,ftsnt-1)real
            //           ... repeat above with imag, then with ops1...

            // [0] -> ops0=650, fx0=0.0, fts0=0.0 real
            Assert.IsTrue(Math.Abs(rVsWavelength[0] - 1.890007) < 0.000001); // API match
            // [1] -> ops0=650, fx0=0.0, fts1=0.5 real
            Assert.IsTrue(Math.Abs(rVsWavelength[1] - 1.888160) < 0.000001);
            // [2] -> ops0=650, fx1=0.5, fts0=0.0 real
            Assert.IsTrue(Math.Abs(rVsWavelength[2] - 0.562537) < 0.000001); // API match
            // [3] -> ops0=650, fx1=0.5, fts1=0.5 real
            Assert.IsTrue(Math.Abs(rVsWavelength[3] - 0.562543) < 0.000001);
            // [4] -> ops1=700, fx0=0.0, fts0=0.0 real
            Assert.IsTrue(Math.Abs(rVsWavelength[4] - 2.118427) < 0.000001); // API match
            // [5] -> ops1=700, fx0=0.0, fts1=0.5 real
            Assert.IsTrue(Math.Abs(rVsWavelength[5] - 2.113377) < 0.000001);
            // [6] -> ops1=700, fx1=0.5, fts0=0.0 real
            Assert.IsTrue(Math.Abs(rVsWavelength[6] - 0.543539) < 0.000001); // API match
            // [7] -> ops1=700, fx1=0.5, fts1=0.5 real
            Assert.IsTrue(Math.Abs(rVsWavelength[7] - 0.543546) < 0.000001);
            // [8] -> ops0=650, fx0=0.0, fts0=0.0 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[8] - 0.0) < 0.000001); // API match
            // [9] -> ops0=650, fx0=0.0, fts1=0.5 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[9] + 0.045122) < 0.000001);
            // [10] -> ops0=650, fx1=0.5, fts0=0.0 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[10] - 0.0) < 0.000001); // API match
            // [11] -> ops0=650, fx1=0.5, fts1=0.5 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[11] + 0.000799) < 0.000001);
            // [12] -> ops1=700, fx0=0.0, fts0=0.0 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[12] - 0.0) < 0.000001); // API match
            // [13] -> ops1=700, fx0=0.0, fts1=0.5 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[13] + 0.071758) < 0.000001);
            // [14] -> ops1=700, fx1=0.5, fts0=0.0 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[14] - 0.0) < 0.000001); // API match
            // [15] -> ops1=700, fx1=0.5, fts1=0.5 imag
            Assert.IsTrue(Math.Abs(rVsWavelength[15] + 0.000651) < 0.000001);
        }

        #endregion

        #region ComputeFluence and ComputeFluenceComplex tests
        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence routine using enum
        /// forward solver and array of optical properties
        /// </summary>
        [Test]
        public void Validate_ComputeFluence_can_be_called_using_enum_forward_solver_and_optical_property_array()
        {
            _xAxis = new double[] {1, 2, 3};
            _zAxis = new double[] {1, 2, 3, 4};
            var independentValues = new[] {_xAxis, _zAxis};
            var fluence = ComputationFactory.ComputeFluence(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZ,
                new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues, 
                // could have array of OPs, one set for each tissue region
                new[] { new OpticalProperties(0.01, 1, 0.8, 1.4) }, 0);
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0] - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence routine using enum
        /// forward solver and single set of OPs
        /// </summary>
        [Test]
        public void Validate_ComputeFluence_can_be_called_using_enum_forward_solver_and_single_optical_properties()
        {
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            var fluence = ComputationFactory.ComputeFluence(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZ,
                new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new OpticalProperties(0.01, 1, 0.8, 1.4), 0);
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0] - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence 
        /// </summary>
        [Test]
        public void Validate_ComputeFluence_can_be_called_using_overload()
        {
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            var fluence = ComputationFactory.ComputeFluence(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZ,
                new IndependentVariableAxis[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                // Optical property region
                new IOpticalPropertyRegion[] {
                    new LayerTissueRegion(
                        new DoubleRange(0, 10, 10),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                    )
                }, 0);
            Assert.IsTrue(Math.Abs(fluence[0] - 0.188294) < 0.000001);
        }

        [Test]
        public void Validate_ComputeFluence_throws_argument_exception()
        {
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            Assert.Throws<ArgumentException>(() =>
            {
                try
                {
                    ComputationFactory.ComputeFluence(
                        ForwardSolverType.PointSourceSDA,
                        FluenceSolutionDomainType.FluenceOfRhoAndZ,
                        new IndependentVariableAxis[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                        independentValues,
                        // Optical property region
                        new IOpticalPropertyRegion[] { null },
                        0);
                }
                catch (Exception e)
                {
                    Assert.AreEqual("The tissue region did not contain a valid layer region (Parameter 'tissueRegions')", e.Message);
                    throw;
                } 
            });
        }


        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence routine using IForwardSolver and
        /// array of OPs
        /// </summary>
        [Test]
        public void Validate_ComputeFluence_can_be_called_using_IForwardSolver_and_optical_property_array()
        {
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            var fluence = ComputationFactory.ComputeFluence(
                new PointSourceSDAForwardSolver(),
                FluenceSolutionDomainType.FluenceOfRhoAndZ,
                new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                // could have array of OPs, one set for each tissue region
                new[] { new OpticalProperties(0.01, 1, 0.8, 1.4) }, 0);
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0] - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence routine using IForwardSolver and
        /// single set of OPs
        /// </summary>
        [Test]
        public void Validate_ComputeFluence_can_be_called_using_IForwardSolver_and_single_optical_properties()
        {
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            var fluence = ComputationFactory.ComputeFluence(
                new PointSourceSDAForwardSolver(),
                FluenceSolutionDomainType.FluenceOfRhoAndZ,
                new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new OpticalProperties(0.01, 1, 0.8, 1.4), 0);
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0] - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeFluenceComplex routine using enum
        /// forward solver and IOpticalProperty array
        /// </summary>
        [Test]
        public void Validate_ComputeFluenceComplex_can_be_called_using_enum_forward_solver_and_IOpticalPropertyRegion_array()
        {
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            var fluence = ComputationFactory.ComputeFluenceComplex(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZAndFt,
                new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new IOpticalPropertyRegion[] {
                    new LayerTissueRegion(
                        new DoubleRange(0, 10, 10),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                        )
                }, 0);
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0].Real - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeFluenceComplex routine using enum
        /// forward solver and single set of OPs
        /// </summary>
        [Test]
        public void Validate_ComputeFluenceComplex_can_be_called_using_enum_forward_solver_and_single_optical_properties()
        {
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            var fluence = ComputationFactory.ComputeFluenceComplex(
                ForwardSolverType.PointSourceSDA,
                FluenceSolutionDomainType.FluenceOfRhoAndZAndFt,
                new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new OpticalProperties(0.01, 1, 0.8, 1.4), 0);
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0].Real - 0.188294) < 0.000001);
        }
        /// <summary>
        /// Test against the ComputationFactory class ComputeFluence routine using IForwardSolver class and
        /// array Tissue Regions
        /// </summary>
        [Test]
        public void Validate_ComputeFluenceComplex_can_be_called_using_IForwardSolver_and_IOpticalPropertyRegion_array()
        {
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            var fluence = ComputationFactory.ComputeFluenceComplex(
                new PointSourceSDAForwardSolver(),
                FluenceSolutionDomainType.FluenceOfRhoAndZAndFt,
                new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new IOpticalPropertyRegion[] {
                    new LayerTissueRegion(
                        new DoubleRange(0, 10, 10),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)
                    )
                }, 0);
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0].Real - 0.188294) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class ComputeFluenceComplex routine using IForwardSolver
        /// class and single set of OPs
        /// </summary>
        [Test]
        public void Validate_ComputeFluenceComplex_can_be_called_using_IForwardSolver_and_single_optical_properties()
        {
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            var fluence = ComputationFactory.ComputeFluenceComplex(
                new PointSourceSDAForwardSolver(),
                FluenceSolutionDomainType.FluenceOfRhoAndZAndFt,
                new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                independentValues,
                new OpticalProperties(0.01, 1, 0.8, 1.4), 0);
            // fluence is linearized to be [0-3]=>(x=1,z=1,2,3,4), [4-7]=>(x=2,z=1,2,3,4), [8-11]=>(x=3,z=1,2,3,4)
            Assert.IsTrue(Math.Abs(fluence[0].Real - 0.188294) < 0.000001);
        }

        [Test]
        public void Validate_ComputeFluenceComplex_throws_argument_exception()
        {
            _xAxis = new double[] { 1, 2, 3 };
            _zAxis = new double[] { 1, 2, 3, 4 };
            var independentValues = new[] { _xAxis, _zAxis };
            Assert.Throws<ArgumentException>(() =>
            {
                try
                {
                    ComputationFactory.ComputeFluenceComplex(
                        new PointSourceSDAForwardSolver(),
                        FluenceSolutionDomainType.FluenceOfRhoAndZAndFt,
                        new[] { IndependentVariableAxis.Rho, IndependentVariableAxis.Z },
                        independentValues,
                        // Optical property region
                        new IOpticalPropertyRegion[] { null },
                        0);
                }
                catch (Exception e)
                {
                    Assert.AreEqual("The tissue region did not contain a valid layer region (Parameter 'tissueRegions')", e.Message);
                    throw;
                }
            });
        }
        #endregion

        #region SolveInverse tests
        /// <summary>
        /// Test against the ComputationFactory class SolveInverse routine using enum
        /// forward solver and optimizer type
        /// </summary>
        [Test]
        public void Validate_SolveInverse_can_be_called_using_enum_forward_solver()
        {
            var initialGuessOPsAndXAxis = new object[] {
                new [] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                new double[] {1, 2, 3 }
            };
            var measuredData = new double[] { 4, 5, 6 };
            var solution = ComputationFactory.SolveInverse(
                ForwardSolverType.PointSourceSDA,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.ROfRho,
                measuredData,
                measuredData,
                InverseFitType.MuaMusp,
                initialGuessOPsAndXAxis
                );
            // solution is a double array with converged solution OPs
             Assert.IsTrue(Math.Abs(solution[1] - 3.75515) < 0.00001);
        }

        /// <summary>
        /// Test against the ComputationFactory class SolveInverse routine using IForwardSolver
        /// and IOptimizer classes 
        /// </summary>
        [Test]
        public void Validate_SolveInverse_can_be_called_using_IForwardSolver_and_IOptimizer()
        {
            var initialGuessOPsAndXAxis = new object[] {
                new [] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
                };
            var measuredData = new double[] { 4, 5, 6 };
            var solution = ComputationFactory.SolveInverse(
                new PointSourceSDAForwardSolver(),
                new MPFitLevenbergMarquardtOptimizer(),
                SolutionDomainType.ROfRho,
                measuredData,
                measuredData,
                InverseFitType.MuaMusp,
                initialGuessOPsAndXAxis
            );
            // solution is a double array with converged solution OPs
            Assert.IsTrue(Math.Abs(solution[1] - 3.75515) < 0.00001);
        }
        /// <summary>
        /// Test against the ComputationFactory class SolveInverse routine using IForwardSolver and
        /// array of OPs
        /// </summary>
        [Test]
        public void Validate_SolveInverse_can_be_called_using_enum_forward_solver_and_bounds()
        {
            var initialGuessOPsAndXAxis = new object[] {
                new [] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
            };
            var measuredData = new double[] { 4, 5, 6 };
            var lowerBounds = new double[] { 0, 0, 0, 0 }; // one for each OP even if not optimized
            var upperBounds = new[]
            {
                double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity
            };
            var solution = ComputationFactory.SolveInverse(
                ForwardSolverType.PointSourceSDA,
                OptimizerType.MPFitLevenbergMarquardt,
                SolutionDomainType.ROfRho,
                measuredData,
                measuredData,
                InverseFitType.MuaMusp,
                initialGuessOPsAndXAxis, 
                lowerBounds,
                upperBounds);
            // solution is a double array with converged solution OPs
            Assert.IsTrue(Math.Abs(solution[1] - 3.75530) < 0.00001);
        }

        /// <summary>
        /// Test against the ComputationFactory class SolveInverse routine using IForwardSolver and
        /// single set of OPs
        /// </summary>
        [Test]
        public void Validate_SolveInverse_can_be_called_using_IForwardSolver_IOptimizer_and_bounds()
        {
            var initialGuessOPsAndXAxis = new object[] {
                new [] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                new double[] { 1, 2, 3 }
            };
            var measuredData = new double[] { 4, 5, 6 };
            var lowerBounds = new double[] { 0, 0, 0, 0 }; // one for each OP even if not optimized
            var upperBounds = new[]
            {
                double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity
            };
            var solution = ComputationFactory.SolveInverse(
                new PointSourceSDAForwardSolver(),
                new MPFitLevenbergMarquardtOptimizer(),
                SolutionDomainType.ROfRho,
                measuredData,
                measuredData,
                InverseFitType.MuaMusp,
                initialGuessOPsAndXAxis,
                lowerBounds,
                upperBounds
            );
            // solution is a double array with converged solution OPs
            Assert.IsTrue(Math.Abs(solution[1] - 3.75530) < 0.00001);
        }
        #endregion

        #region GetPHD tests
        /// <summary>
        /// Test against the ComputationFactory class GetPHD routine using enum forward solver 
        /// </summary>
        [Test]
        public void Validate_GetPHD_can_be_called_using_enum_forward_solver()
        {
            const double sourceDetectorSeparation = 3;
            var phd = ComputationFactory.GetPHD(
                ForwardSolverType.PointSourceSDA,
                _realFluence,
                sourceDetectorSeparation,
                new[] {new OpticalProperties(0.01, 1.0, 0.8, 1.4)},
                _xAxis,
                _zAxis);
            // solution is linearized PHD, column major
            Assert.IsTrue(Math.Abs(phd[0] - 0.010336) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class GetPHD routine using IForwardSolver class
        /// </summary>
        [Test]
        public void Validate_GetPHD_can_be_called_using_IForwardSolver()
        {
            const double sourceDetectorSeparation = 3;
            var phd = ComputationFactory.GetPHD(
                new PointSourceSDAForwardSolver(),
                _realFluence,
                sourceDetectorSeparation,
                new[] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                _xAxis,
                _zAxis
            );
            // solution is linearized PHD, column major
            Assert.IsTrue(Math.Abs(phd[0] - 0.010336) < 0.000001);
        }
        /// <summary>
        /// Test against the ComputationFactory class GetPHD routine using enum forward solver type
        /// for the Temporal-Frequency domain (FluenceOfRhoAndZAndFt)
        /// </summary>
        [Test]
        public void Validate_GetPHD_can_be_called_using_enum_forward_solver_and_temporal_modulation_frequency()
        {
            const double sourceDetectorSeparation = 3;
            const double modulationFrequency = 0;
            var phd = ComputationFactory.GetPHD(
                ForwardSolverType.PointSourceSDA,
                _complexFluence,
                sourceDetectorSeparation,
                modulationFrequency,
                new [] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                _xAxis,
                _zAxis
            );
            // solution is linearized PHD, column major
            Assert.IsTrue(Math.Abs(phd[0] - 0.010336) < 0.000001);
        }

        /// <summary>
        /// Test against the ComputationFactory class SolveInverse routine using IForwardSolver for
        /// the Temporal-Frequency domain (FluenceOfRhoAndZAndFt)
        /// </summary>
        [Test]
        public void validate_GetPHD_can_be_called_using_IForwardSolver_and_temporal_modulation_frequency()
        {
            const double sourceDetectorSeparation = 3;
            const double modulationFrequency = 0;
            var phd = ComputationFactory.GetPHD(
                new PointSourceSDAForwardSolver(),
                _complexFluence,
                sourceDetectorSeparation,
                modulationFrequency,
                new[] { new OpticalProperties(0.01, 1.0, 0.8, 1.4) },
                _xAxis,
                _zAxis
            );
            // solution is linearized PHD, column major
            Assert.IsTrue(Math.Abs(phd[0] - 0.010336) < 0.000001);
        }
        #endregion

        #region GetAbsorbedEnergy tests
        /// <summary>
        /// Test GetAbsorbedEnergy for real fluence solutions and homogeneous tissue (single mua value)
        /// </summary>
        [Test]
        public void Validate_GetAbsorbedEnergy_can_be_called_for_homogeneous_real_fluence_solutions()
        {
            const double mua = 0.1;
            var absorbedEnergy = ComputationFactory.GetAbsorbedEnergy(_realFluence, mua);
            Assert.IsTrue(Math.Abs(absorbedEnergy.First() - 0.018829) < 0.000001);
        }
        /// <summary>
        /// Test GetAbsorbedEnergy for Complex fluence solutions and homogeneous tissue (single mua value)
        /// </summary>
        [Test]
        public void Validate_GetAbsorbedEnergy_can_be_called_for_homogeneous_complex_fluence_solutions()
        {
            const double mua = 0.1;
            var absorbedEnergy = ComputationFactory.GetAbsorbedEnergy(_complexFluence, mua);
            Assert.IsTrue(Math.Abs(absorbedEnergy.First().Real - 0.018829) < 0.000001);
        }
        /// <summary>
        /// Test GetAbsorbedEnergy for Complex fluence solutions and heterogeneous tissue (multiple mua values)
        /// </summary>
        [Test]
        public void Validate_GetAbsorbedEnergy_can_be_called_for_heterogeneous_real_fluence_solutions()
        {
            // use real Fluence as fluence for two layer tissue which is a 4x3 and 
            // define muas so that top 2 rows have one value, bottom have different value (matrix is column major)
            var muas = new double[] {0.1, 0.1, 0.2, 0.2, 0.1, 0.1, 0.2, 0.2, 0.1, 0.1, 0.2, 0.2};
            var absorbedEnergy = ComputationFactory.GetAbsorbedEnergy(_realFluence.AsEnumerable(), muas.AsEnumerable());
            Assert.IsTrue(Math.Abs(absorbedEnergy.First() - 0.018829) < 0.000001);
        }
        #endregion
    }
}
