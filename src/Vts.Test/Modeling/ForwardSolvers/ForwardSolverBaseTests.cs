using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Common;
using Vts.Modeling.ForwardSolvers;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class ForwardSolverBaseTests
    {
        private ForwardSolverBase _forwardSolverBaseMock;
        private ForwardSolverBase _forwardSolverBaseMockWithSetup;
        private TestForwardSolverBase _testForwardSolverBase;
        private OpticalProperties _opticalProperties;
        private IOpticalPropertyRegion[] _opticalPropertyRegions;
        private IEnumerable<OpticalProperties> _opticalPropertiesEnumerable;
        private IEnumerable<IOpticalPropertyRegion[]> _opticalPropertyRegionsEnumerable;
        private IEnumerable<double> _doubles;

        internal class TestForwardSolverBase : ForwardSolverBase
        {
            internal TestForwardSolverBase() : base(SourceConfiguration.Point, 0.1) { }
        }

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _forwardSolverBaseMock = Substitute.ForPartsOf<ForwardSolverBase>();
            _forwardSolverBaseMockWithSetup = Substitute.ForPartsOf<ForwardSolverBase>();
            // Setup the mocks for the not implemented methods
            // ROfRho
            _forwardSolverBaseMockWithSetup.ROfRho(Arg.Any<OpticalProperties>(), Arg.Any<double>()).Returns(0.1);
            _forwardSolverBaseMockWithSetup.ROfRho(Arg.Any<IOpticalPropertyRegion[]>(), Arg.Any<double>()).Returns(0.2);
            // ROFTheta
            _forwardSolverBaseMockWithSetup.ROfTheta(Arg.Any<OpticalProperties>(), Arg.Any<double>()).Returns(0.3);
            // ROfFx
            _forwardSolverBaseMockWithSetup.ROfFx(Arg.Any<OpticalProperties>(), Arg.Any<double>()).Returns(0.4);
            _forwardSolverBaseMockWithSetup.ROfFx(Arg.Any<IOpticalPropertyRegion[]>(), Arg.Any<double>()).Returns(0.5);
            // ROfRhoAndTime
            _forwardSolverBaseMockWithSetup.ROfRhoAndTime(Arg.Any<OpticalProperties>(), Arg.Any<double>(), Arg.Any<double>()).Returns(0.6);
            _forwardSolverBaseMockWithSetup.ROfRhoAndTime(Arg.Any<IOpticalPropertyRegion[]>(), Arg.Any<double>(), Arg.Any<double>()).Returns(0.7);
            // ROfRhoAndFt
            _forwardSolverBaseMockWithSetup.ROfRhoAndFt(Arg.Any<OpticalProperties>(), Arg.Any<double>(), Arg.Any<double>()).Returns(new Complex(0.8, 0));
            _forwardSolverBaseMockWithSetup.ROfRhoAndFt(Arg.Any<IOpticalPropertyRegion[]>(), Arg.Any<double>(), Arg.Any<double>()).Returns(new Complex(0.9, 0));
            // ROfFxAndTime
            _forwardSolverBaseMockWithSetup.ROfFxAndTime(Arg.Any<OpticalProperties>(), Arg.Any<double>(), Arg.Any<double>()).Returns(1.0);
            _forwardSolverBaseMockWithSetup.ROfFxAndTime(Arg.Any<IOpticalPropertyRegion[]>(), Arg.Any<double>(), Arg.Any<double>()).Returns(1.1);
            // ROfFxAndFt
            _forwardSolverBaseMockWithSetup.ROfFxAndFt(Arg.Any<OpticalProperties>(), Arg.Any<double>(), Arg.Any<double>()).Returns(new Complex(1.2, 0));
            _forwardSolverBaseMockWithSetup.ROfFxAndFt(Arg.Any<IOpticalPropertyRegion[]>(), Arg.Any<double>(), Arg.Any<double>()).Returns(new Complex(1.3, 0));

            // FluenceOfRhoAndZ
            _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZ(Arg.Any<IEnumerable<OpticalProperties>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>()).Returns(new List<double> { 0.1, 0.2 });
            _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZ(Arg.Any< IEnumerable<IOpticalPropertyRegion[]>>(), Arg.Any< IEnumerable<double>>(), Arg.Any<IEnumerable<double>>()).Returns(new List<double> { 0.3, 0.4 });
            // FluenceOfRhoAndZAndTime
            _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZAndTime(Arg.Any<IEnumerable<OpticalProperties>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>()).Returns(new List<double> { 0.5, 0.6 });
            _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZAndTime(Arg.Any<IEnumerable<IOpticalPropertyRegion[]>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>()).Returns(new List<double> { 0.7, 0.8 });
            // FluenceOfRhoAndZAndFt
            _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZAndFt(Arg.Any<IEnumerable<OpticalProperties>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>()).Returns(new List<Complex> { new Complex(0.9, 0), new Complex(1.0, 0) });
            _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZAndFt(Arg.Any<IEnumerable<IOpticalPropertyRegion[]>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>()).Returns(new List<Complex> { new Complex(1.1, 0), new Complex(1.2, 0) });
            // FluenceOfFxAndZ
            _forwardSolverBaseMockWithSetup.FluenceOfFxAndZ(Arg.Any<IEnumerable<OpticalProperties>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>()).Returns(new List<double> { 1.3, 1.4 });
            // FluenceOfFxAndZAndTime
            _forwardSolverBaseMockWithSetup.FluenceOfFxAndZAndTime(Arg.Any<IEnumerable<OpticalProperties>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>()).Returns(new List<double> { 1.5, 1.6 });
            // FluenceOfFxAndZAndFt
            _forwardSolverBaseMockWithSetup.FluenceOfFxAndZAndFt(Arg.Any<IEnumerable<OpticalProperties>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>(), Arg.Any<IEnumerable<double>>()).Returns(new List<Complex> { new Complex(1.7, 0), new Complex(1.8, 0) });

            _testForwardSolverBase = new TestForwardSolverBase();
            // create the optical properties, arrays of optical property regions and IEnumberables
            _opticalProperties = new OpticalProperties(0.1, 1, 0.8, 1.4);
            _opticalPropertyRegions = new IOpticalPropertyRegion[]
            {
                new LayerOpticalPropertyRegion(new DoubleRange(0, 9, 10), _opticalProperties)
            };
            _opticalPropertiesEnumerable = new List<OpticalProperties>
            {
                new OpticalProperties(0.1, 1, 0.8, 1.4),
                new OpticalProperties(0.01, 1, 0.8, 1.4)
            };
            _opticalPropertyRegionsEnumerable = new List<IOpticalPropertyRegion[]>
            {
                new IOpticalPropertyRegion[]
                {
                    new LayerTissueRegion(new DoubleRange(0, 9, 10), _opticalProperties),
                },
                _opticalPropertyRegions
            };
            _doubles = new List<double>
            {
                0.1,
                0.2,
                0.3
            };
        }

        [Test]
        public void Test_constructors()
        {
            Assert.AreEqual(SourceConfiguration.Point, _testForwardSolverBase.SourceConfiguration);
            Assert.AreEqual(0.1, _testForwardSolverBase.BeamDiameter);
        }

        [Test]
        public void Test_ROfRho_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfRho(_opticalProperties, 0.5));
        }

        [Test]
        public void Test_ROfRho_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfRho(
                _opticalPropertyRegions.ToArray(), 0.5));
        }

        [Test]
        public void Test_ROfTheta_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfTheta(_opticalProperties, 0.6));
        }

        [Test]
        public void Test_ROfRhoAndTime_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfRhoAndTime(_opticalProperties, 0.5, 1));
        }

        [Test]
        public void Test_ROfRhoAndTime_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfRhoAndTime(
                _opticalPropertyRegions.ToArray(), 0.5, 1));
        }

        [Test]
        public void Test_ROfRhoAndFt_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfRhoAndFt(_opticalProperties, 0.5, 0.1));
        }

        [Test]
        public void Test_ROfRhoAndFt_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfRhoAndFt(
                _opticalPropertyRegions.ToArray(), 0.5, 1));
        }

        [Test]
        public void Test_ROfFx_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfFx(_opticalProperties, 0.2));
        }

        [Test]
        public void Test_ROfFx_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfFx(
                _opticalPropertyRegions.ToArray(), 0.2));
        }

        [Test]
        public void Test_ROfFxAndTime_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfFxAndTime(_opticalProperties, 0.2, 1));
        }

        [Test]
        public void Test_ROfFxAndTime_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfFxAndTime(
                _opticalPropertyRegions.ToArray(), 0.2, 1));
        }

        [Test]
        public void Test_ROfFxAndFt_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfFxAndFt(_opticalProperties, 0.2, 0.5));
        }

        [Test]
        public void Test_ROfFxAndFt_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.ROfFxAndFt(
                _opticalPropertyRegions.ToArray(), 0.2, 0.5));
        }

        [Test]
        public void Test_ROfRho_with_IEnumerable_of_optical_properties_and_rhos()
        {
            var doubleList =
                _forwardSolverBaseMockWithSetup.ROfRho(_opticalPropertiesEnumerable, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfRho_with_IEnumerable_of_arrays_of_optical_property_regions_and_rhos()
        {
            var doubleList =
                _forwardSolverBaseMockWithSetup.ROfRho(_opticalPropertyRegionsEnumerable, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfTheta_with_IEnumerable_of_optical_properties_and_thetas()
        {
            var doubleList =
                _forwardSolverBaseMockWithSetup.ROfTheta(_opticalPropertiesEnumerable, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfRhoAndTime_with_IEnumerable_of_optical_properties_and_rhos_and_times()
        {
            var doubleList =
                _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertiesEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfRhoAndTime_with_IEnumerable_of_arrays_of_optical_property_regions_and_rhos_and_times()
        {
            var doubleList =
                _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertyRegionsEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfRhoAndFt_with_IEnumerable_of_optical_properties_and_rhos_and_fts()
        {
            var complexList =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertiesEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<Complex>>(complexList);
        }

        [Test]
        public void Test_ROfRhoAndFt_with_IEnumerable_of_arrays_of_optical_property_regions_and_rhos_and_fts()
        {
            var complexList =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertyRegionsEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<Complex>>(complexList);
        }

        [Test]
        public void Test_ROfFx_with_IEnumerable_of_optical_properties_and_fxs()
        {
            var doubleList =
                _forwardSolverBaseMock.ROfFx(_opticalPropertiesEnumerable, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfFx_with_IEnumerable_of_arrays_of_optical_property_regions_and_fxs()
        {
            var doubleList =
                _forwardSolverBaseMock.ROfFx(_opticalPropertyRegionsEnumerable, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfFxAndTime_with_IEnumerable_of_optical_properties_and_fxs_and_times()
        {
            var doubleList =
                _forwardSolverBaseMock.ROfFxAndTime(_opticalPropertiesEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfFxAndTime_with_IEnumerable_of_arrays_of_optical_property_regions_and_fxs_and_times()
        {
            var doubleList =
                _forwardSolverBaseMock.ROfFxAndTime(_opticalPropertyRegionsEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfFxAndFt_with_IEnumerable_of_optical_properties_and_fxs_and_fts()
        {
            var complexList =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertiesEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<Complex>>(complexList);
        }

        [Test]
        public void Test_ROfFxAndFt_with_IEnumerable_of_arrays_of_optical_property_regions_and_fxs_and_fts()
        {
            var complexList =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertyRegionsEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<Complex>>(complexList);
        }

        [Test]
        public void Test_ROfRho_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfRho(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.1, doubles[0]);
        }

        [Test]
        public void Test_ROfRho_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfRho(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.2, doubles[0]);
        }

        [Test]
        public void Test_Theta_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfTheta(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.3, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFx(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.4, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFx(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.5, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndFt_reflectance_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real, 0.001);
        }

        [Test]
        public void Test_ROfRhoAndFt_optical_property_region_reflectance_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real, 0.001);
        }

        [Test]
        public void Test_ROfFxAndTime_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(1.1, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndFt_reflectance_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real, 0.001);
        }

        [Test]
        public void Test_ROfFxAndFt_optical_property_region_reflectance_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, complexes.Length);
            Assert.AreEqual(1.3, complexes[0].Real, 0.001);
        }

        [Test]
        public void Test_ROfRho_overload_optical_properties_and_array_of_rhos()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRho(_opticalProperties, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.1, doubles[2]);
        }

        [Test]
        public void Test_ROfRho_overload_optical_property_region_array_and_array_of_rhos()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRho(_opticalPropertyRegions, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.2, doubles[0]);
        }

        [Test]
        public void Test_ROfRho_overload_optical_property_array_and_one_rho()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRho(_opticalPropertiesEnumerable.ToArray(), 0.5);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.1, doubles[0]);
        }

        [Test]
        public void Test_ROfRho_overload_array_of_optical_property_region_array_and_one_rho()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRho(_opticalPropertyRegionsEnumerable.ToArray(), 0.5);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.2, doubles[0]);
        }

        //ROfTheta
        [Test]
        public void Test_ROfTheta_overload_optical_properties_and_array_of_rhos()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfTheta(_opticalProperties, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.3, doubles[0]);
        }

        [Test]
        public void Test_ROfTheta_overload_optical_property_array_and_one_rho()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfTheta(_opticalPropertiesEnumerable.ToArray(), 0.5);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.3, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_and_double_arrays()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalProperties, _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_array_one_rho_time_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertiesEnumerable.ToArray(), 0.5, _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_array_optical_property_region_array_one_rho_time_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertyRegionsEnumerable.ToArray(), 0.5, _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_array_rho_array_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), 0.1);
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_array_optical_property_region_array_rho_array_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), 0.1);
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_property_region_array_one_rho_time_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertyRegions, 0.5, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_property_region_array_rho_array_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertyRegions, _doubles.ToArray(), 0.1);
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_property_region_array_one_rho_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertyRegionsEnumerable.ToArray(), 0.5, 0.1);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_property_region_array_rho_array_time_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertyRegions, _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_one_rho_time_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalProperties, 0.5, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_rho_array_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalProperties, _doubles.ToArray(), 0.1);
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_array_one_rho_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfRhoAndTime(_opticalPropertiesEnumerable.ToArray(), 0.5, 0.1);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_overload_optical_properties_fx_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfFx(_opticalProperties, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.4, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_overload_optical_properties_region_array_fx_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfFx(_opticalPropertyRegions.ToArray(), _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.5, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_overload_optical_properties_array_one_fx()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfFx(_opticalPropertiesEnumerable.ToArray(), 0.1);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.4, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_overload_array_of_optical_properties_region_array_one_fx()
        {
            var doubles = _forwardSolverBaseMockWithSetup.ROfFx(_opticalPropertyRegionsEnumerable.ToArray(), 0.1);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.5, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_fx_array_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalProperties, _doubles.ToArray(),
                    _doubles.ToArray());
            Assert.AreEqual(9, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_array_one_fx_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalPropertiesEnumerable.ToArray(), 0.1,
                    _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_array_fx_array_one_time()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalPropertiesEnumerable.ToArray(),
                    _doubles.ToArray(), 0.5);
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_one_fx_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalProperties, 0.1,
                    _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_fx_array_one_time()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalProperties,
                    _doubles.ToArray(), 0.5);
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_array_one_fx_one_time()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalPropertiesEnumerable.ToArray(),
                    0.1, 0.5);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_property_region_array_fx_array_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, doubles.Length);
            Assert.AreEqual(1.1, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_array_optical_property_region_array__one_fx_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalPropertyRegionsEnumerable.ToArray(),
                    0.1, _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(1.1, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_property_region_array_one_fx_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalPropertyRegions.ToArray(),
                    0.1, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(1.1, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_property_region_array_fx_array_one_time()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.ROfFxAndTime(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), 0.5);
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(1.1, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_rho_array_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalProperties,
                    _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_array_one_rho_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertiesEnumerable.ToArray(),
                    0.5, _doubles.ToArray());
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_array_rho_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertiesEnumerable.ToArray(),
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_one_rho_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalProperties,
                    0.5, _doubles.ToArray());
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_rho_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalProperties,
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_array_one_rho_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertiesEnumerable.ToArray(),
                    0.5, 0.2);
            Assert.AreEqual(2, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_property_region_array_rho_array_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_property_region_array_one_rho_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertyRegions.ToArray(),
                    0.5, _doubles.ToArray());
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_property_region_array_rho_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_array_optical_property_region_array_rho_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertyRegionsEnumerable.ToArray(),
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_array_optical_property_region_array_one_rho_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfRhoAndFt(_opticalPropertyRegionsEnumerable.ToArray(),
                    0.5, _doubles.ToArray());
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_fx_array_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalProperties,
                    _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_array_one_fx_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertiesEnumerable.ToArray(),
                    0.2, _doubles.ToArray());
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_array_fx_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertiesEnumerable.ToArray(),
                    _doubles.ToArray(), 0.1);
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_one_fx_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalProperties,
                    0.1, _doubles.ToArray());
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_fx_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalProperties,
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_array_one_fx_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertiesEnumerable.ToArray(),
                    0.1, 0.2);
            Assert.AreEqual(2, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_property_region_array_fx_array_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, complexes.Length);
            Assert.AreEqual(1.3, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_property_region_array_one_fx_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertyRegions.ToArray(),
                    0.1, _doubles.ToArray());
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(1.3, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_property_region_array_fx_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(1.3, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_array_optical_property_region_array_fx_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.ROfFxAndFt(_opticalPropertyRegionsEnumerable.ToArray(),
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(1.3, complexes[0].Real);
        }

        [Test]
        public void Test_FluenceOfRhoAndZ_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.FluenceOfRhoAndZ(_opticalPropertiesEnumerable, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZ_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.FluenceOfRhoAndZ(_opticalPropertyRegionsEnumerable, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndTime_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.FluenceOfRhoAndZAndTime(_opticalPropertiesEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndTime_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.FluenceOfRhoAndZAndTime(_opticalPropertyRegionsEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndFt_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.FluenceOfRhoAndZAndFt(_opticalPropertiesEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndFt_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.FluenceOfRhoAndZAndFt(_opticalPropertyRegionsEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfFxAndZ_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.FluenceOfFxAndZ(_opticalPropertiesEnumerable, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfFxAndZAndTime_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.FluenceOfFxAndZAndTime(_opticalPropertiesEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfFxAndZAndFt_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.FluenceOfFxAndZAndFt(_opticalPropertiesEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZ_fluence_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZ(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(0.1, doubles[0], 0.00001);
            Assert.AreEqual(0.2, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfRhoAndZ_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZ(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(0.3, doubles[0], 0.00001);
            Assert.AreEqual(0.4, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndTime_fluence_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZAndTime(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, doubles.Length);
            Assert.AreEqual(0.5, doubles[0], 0.00001);
            Assert.AreEqual(0.6, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndTime_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZAndTime(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, doubles.Length);
            Assert.AreEqual(0.7, doubles[0], 0.00001);
            Assert.AreEqual(0.8, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndFt_fluence_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZAndFt(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real, 0.00001);
            Assert.AreEqual(1.0, complexes[1].Real, 0.00001);
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndFt_optical_property_region_reflectance_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.FluenceOfRhoAndZAndFt(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, complexes.Length);
            Assert.AreEqual(1.1, complexes[0].Real, 0.00001);
            Assert.AreEqual(1.2, complexes[1].Real, 0.00001);
        }

        [Test]
        public void Test_FluenceOfFxAndZ_fluence_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.FluenceOfFxAndZ(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(1.3, doubles[0], 0.00001);
            Assert.AreEqual(1.4, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfFxAndZAndTime_fluence_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.FluenceOfFxAndZAndTime(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, doubles.Length);
            Assert.AreEqual(1.5, doubles[0], 0.00001);
            Assert.AreEqual(1.6, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfFxAndZAndFt_fluence_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.FluenceOfFxAndZAndFt(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, complexes.Length);
            Assert.AreEqual(1.7, complexes[0].Real, 0.00001);
            Assert.AreEqual(1.8, complexes[1].Real, 0.00001);
        }
    }
}
