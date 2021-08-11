using Moq;
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
        private Mock<ForwardSolverBase> _forwardSolverBaseMock;
        private Mock<ForwardSolverBase> _forwardSolverBaseMockWithSetup;
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
            _forwardSolverBaseMock = new Mock<ForwardSolverBase>()
            {
                CallBase = true
            };
            _forwardSolverBaseMockWithSetup = new Mock<ForwardSolverBase>()
            {
                CallBase = true
            };
            // Setup the mocks for the not implemented methods
            // ROfRho
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfRho(It.IsAny<OpticalProperties>(), It.IsAny<double>())).Returns(0.1);
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfRho(It.IsAny<IOpticalPropertyRegion[]>(), It.IsAny<double>())).Returns(0.2);
            // ROFTheta
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfTheta(It.IsAny<OpticalProperties>(), It.IsAny<double>())).Returns(0.3);
            // ROfFx
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfFx(It.IsAny<OpticalProperties>(), It.IsAny<double>())).Returns(0.4);
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfFx(It.IsAny<IOpticalPropertyRegion[]>(), It.IsAny<double>())).Returns(0.5);
            // ROfRhoAndTime
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfRhoAndTime(It.IsAny<OpticalProperties>(), It.IsAny<double>(), It.IsAny<double>())).Returns(0.6);
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfRhoAndTime(It.IsAny<IOpticalPropertyRegion[]>(), It.IsAny<double>(), It.IsAny<double>())).Returns(0.7);
            // ROfRhoAndFt
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfRhoAndFt(It.IsAny<OpticalProperties>(), It.IsAny<double>(), It.IsAny<double>())).Returns(new Complex(0.8, 0));
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfRhoAndFt(It.IsAny<IOpticalPropertyRegion[]>(), It.IsAny<double>(), It.IsAny<double>())).Returns(new Complex(0.9, 0));
            // ROfFxAndTime
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfFxAndTime(It.IsAny<OpticalProperties>(), It.IsAny<double>(), It.IsAny<double>())).Returns(1.0);
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfFxAndTime(It.IsAny<IOpticalPropertyRegion[]>(), It.IsAny<double>(), It.IsAny<double>())).Returns(1.1);
            // ROfFxAndFt
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfFxAndFt(It.IsAny<OpticalProperties>(), It.IsAny<double>(), It.IsAny<double>())).Returns(new Complex(1.2, 0));
            _forwardSolverBaseMockWithSetup.Setup(x => x.ROfFxAndFt(It.IsAny<IOpticalPropertyRegion[]>(), It.IsAny<double>(), It.IsAny<double>())).Returns(new Complex(1.3, 0));

            // FluenceOfRhoAndZ
            _forwardSolverBaseMockWithSetup.Setup(x => x.FluenceOfRhoAndZ(It.IsAny<IEnumerable<OpticalProperties>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>())).Returns(new List<double> { 0.1, 0.2 });
            _forwardSolverBaseMockWithSetup.Setup(x => x.FluenceOfRhoAndZ(It.IsAny< IEnumerable<IOpticalPropertyRegion[]>>(), It.IsAny< IEnumerable<double>>(), It.IsAny<IEnumerable<double>>())).Returns(new List<double> { 0.3, 0.4 });
            // FluenceOfRhoAndZAndTime
            _forwardSolverBaseMockWithSetup.Setup(x => x.FluenceOfRhoAndZAndTime(It.IsAny<IEnumerable<OpticalProperties>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>())).Returns(new List<double> { 0.5, 0.6 });
            _forwardSolverBaseMockWithSetup.Setup(x => x.FluenceOfRhoAndZAndTime(It.IsAny<IEnumerable<IOpticalPropertyRegion[]>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>())).Returns(new List<double> { 0.7, 0.8 });
            // FluenceOfRhoAndZAndFt
            _forwardSolverBaseMockWithSetup.Setup(x => x.FluenceOfRhoAndZAndFt(It.IsAny<IEnumerable<OpticalProperties>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>())).Returns(new List<Complex> { new Complex(0.9, 0), new Complex(1.0, 0) });
            _forwardSolverBaseMockWithSetup.Setup(x => x.FluenceOfRhoAndZAndFt(It.IsAny<IEnumerable<IOpticalPropertyRegion[]>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>())).Returns(new List<Complex> { new Complex(1.1, 0), new Complex(1.2, 0) });
            // FluenceOfFxAndZ
            _forwardSolverBaseMockWithSetup.Setup(x => x.FluenceOfFxAndZ(It.IsAny<IEnumerable<OpticalProperties>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>())).Returns(new List<double> { 1.3, 1.4 });
            // FluenceOfFxAndZAndTime
            _forwardSolverBaseMockWithSetup.Setup(x => x.FluenceOfFxAndZAndTime(It.IsAny<IEnumerable<OpticalProperties>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>())).Returns(new List<double> { 1.5, 1.6 });
            // FluenceOfFxAndZAndFt
            _forwardSolverBaseMockWithSetup.Setup(x => x.FluenceOfFxAndZAndFt(It.IsAny<IEnumerable<OpticalProperties>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>(), It.IsAny<IEnumerable<double>>())).Returns(new List<Complex> { new Complex(1.7, 0), new Complex(1.8, 0) });

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
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfRho(_opticalProperties, 0.5));
        }

        [Test]
        public void Test_ROfRho_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfRho(
                _opticalPropertyRegions.ToArray(), 0.5));
        }

        [Test]
        public void Test_ROfTheta_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfTheta(_opticalProperties, 0.6));
        }

        [Test]
        public void Test_ROfRhoAndTime_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfRhoAndTime(_opticalProperties, 0.5, 1));
        }

        [Test]
        public void Test_ROfRhoAndTime_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfRhoAndTime(
                _opticalPropertyRegions.ToArray(), 0.5, 1));
        }

        [Test]
        public void Test_ROfRhoAndFt_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfRhoAndFt(_opticalProperties, 0.5, 0.1));
        }

        [Test]
        public void Test_ROfRhoAndFt_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfRhoAndFt(
                _opticalPropertyRegions.ToArray(), 0.5, 1));
        }

        [Test]
        public void Test_ROfFx_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfFx(_opticalProperties, 0.2));
        }

        [Test]
        public void Test_ROfFx_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfFx(
                _opticalPropertyRegions.ToArray(), 0.2));
        }

        [Test]
        public void Test_ROfFxAndTime_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfFxAndTime(_opticalProperties, 0.2, 1));
        }

        [Test]
        public void Test_ROfFxAndTime_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfFxAndTime(
                _opticalPropertyRegions.ToArray(), 0.2, 1));
        }

        [Test]
        public void Test_ROfFxAndFt_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfFxAndFt(_opticalProperties, 0.2, 0.5));
        }

        [Test]
        public void Test_ROfFxAndFt_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.ROfFxAndFt(
                _opticalPropertyRegions.ToArray(), 0.2, 0.5));
        }

        [Test]
        public void Test_ROfRho_with_IEnumerable_of_optical_properties_and_rhos()
        {
            var doubleList =
                _forwardSolverBaseMockWithSetup.Object.ROfRho(_opticalPropertiesEnumerable, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfRho_with_IEnumerable_of_arrays_of_optical_property_regions_and_rhos()
        {
            var doubleList =
                _forwardSolverBaseMockWithSetup.Object.ROfRho(_opticalPropertyRegionsEnumerable, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfTheta_with_IEnumerable_of_optical_properties_and_thetas()
        {
            var doubleList =
                _forwardSolverBaseMockWithSetup.Object.ROfTheta(_opticalPropertiesEnumerable, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfRhoAndTime_with_IEnumerable_of_optical_properties_and_rhos_and_times()
        {
            var doubleList =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertiesEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfRhoAndTime_with_IEnumerable_of_arrays_of_optical_property_regions_and_rhos_and_times()
        {
            var doubleList =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertyRegionsEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfRhoAndFt_with_IEnumerable_of_optical_properties_and_rhos_and_fts()
        {
            var complexList =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertiesEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<Complex>>(complexList);
        }

        [Test]
        public void Test_ROfRhoAndFt_with_IEnumerable_of_arrays_of_optical_property_regions_and_rhos_and_fts()
        {
            var complexList =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertyRegionsEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<Complex>>(complexList);
        }

        [Test]
        public void Test_ROfFx_with_IEnumerable_of_optical_properties_and_fxs()
        {
            var doubleList =
                _forwardSolverBaseMock.Object.ROfFx(_opticalPropertiesEnumerable, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfFx_with_IEnumerable_of_arrays_of_optical_property_regions_and_fxs()
        {
            var doubleList =
                _forwardSolverBaseMock.Object.ROfFx(_opticalPropertyRegionsEnumerable, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfFxAndTime_with_IEnumerable_of_optical_properties_and_fxs_and_times()
        {
            var doubleList =
                _forwardSolverBaseMock.Object.ROfFxAndTime(_opticalPropertiesEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfFxAndTime_with_IEnumerable_of_arrays_of_optical_property_regions_and_fxs_and_times()
        {
            var doubleList =
                _forwardSolverBaseMock.Object.ROfFxAndTime(_opticalPropertyRegionsEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<double>>(doubleList);
        }

        [Test]
        public void Test_ROfFxAndFt_with_IEnumerable_of_optical_properties_and_fxs_and_fts()
        {
            var complexList =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertiesEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<Complex>>(complexList);
        }

        [Test]
        public void Test_ROfFxAndFt_with_IEnumerable_of_arrays_of_optical_property_regions_and_fxs_and_fts()
        {
            var complexList =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertyRegionsEnumerable, _doubles, _doubles);
            Assert.IsInstanceOf<IEnumerable<Complex>>(complexList);
        }

        [Test]
        public void Test_ROfRho_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfRho(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.1, doubles[0]);
        }

        [Test]
        public void Test_ROfRho_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfRho(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.2, doubles[0]);
        }

        [Test]
        public void Test_Theta_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfTheta(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.3, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFx(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.4, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFx(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.5, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndFt_reflectance_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real, 0.001);
        }

        [Test]
        public void Test_ROfRhoAndFt_optical_property_region_reflectance_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real, 0.001);
        }

        [Test]
        public void Test_ROfFxAndTime_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(1.1, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndFt_reflectance_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real, 0.001);
        }

        [Test]
        public void Test_ROfFxAndFt_optical_property_region_reflectance_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, complexes.Length);
            Assert.AreEqual(1.3, complexes[0].Real, 0.001);
        }

        [Test]
        public void Test_ROfRho_overload_optical_properties_and_array_of_rhos()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRho(_opticalProperties, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.1, doubles[2]);
        }

        [Test]
        public void Test_ROfRho_overload_optical_property_region_array_and_array_of_rhos()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRho(_opticalPropertyRegions, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.2, doubles[0]);
        }

        [Test]
        public void Test_ROfRho_overload_optical_property_array_and_one_rho()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRho(_opticalPropertiesEnumerable.ToArray(), 0.5);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.1, doubles[0]);
        }

        [Test]
        public void Test_ROfRho_overload_array_of_optical_property_region_array_and_one_rho()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRho(_opticalPropertyRegionsEnumerable.ToArray(), 0.5);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.2, doubles[0]);
        }

        //ROfTheta
        [Test]
        public void Test_ROfTheta_overload_optical_properties_and_array_of_rhos()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfTheta(_opticalProperties, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.3, doubles[0]);
        }

        [Test]
        public void Test_ROfTheta_overload_optical_property_array_and_one_rho()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfTheta(_opticalPropertiesEnumerable.ToArray(), 0.5);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.3, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_and_double_arrays()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalProperties, _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_property_region_array_and_double_arrays()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_array_one_rho_time_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertiesEnumerable.ToArray(), 0.5, _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_array_optical_property_region_array_one_rho_time_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertyRegionsEnumerable.ToArray(), 0.5, _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_array_rho_array_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), 0.1);
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_array_optical_property_region_array_rho_array_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), 0.1);
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_property_region_array_one_rho_time_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertyRegions, 0.5, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_property_region_array_rho_array_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertyRegions, _doubles.ToArray(), 0.1);
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_property_region_array_one_rho_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertyRegionsEnumerable.ToArray(), 0.5, 0.1);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_property_region_array_rho_array_time_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertyRegions, _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, doubles.Length);
            Assert.AreEqual(0.7, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_one_rho_time_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalProperties, 0.5, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_rho_array_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalProperties, _doubles.ToArray(), 0.1);
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndTime_overload_optical_properties_array_one_rho_one_time()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfRhoAndTime(_opticalPropertiesEnumerable.ToArray(), 0.5, 0.1);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.6, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_overload_optical_properties_fx_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfFx(_opticalProperties, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.4, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_overload_optical_properties_region_array_fx_array()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfFx(_opticalPropertyRegions.ToArray(), _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(0.5, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_overload_optical_properties_array_one_fx()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfFx(_opticalPropertiesEnumerable.ToArray(), 0.1);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.4, doubles[0]);
        }

        [Test]
        public void Test_ROfFx_overload_array_of_optical_properties_region_array_one_fx()
        {
            var doubles = _forwardSolverBaseMockWithSetup.Object.ROfFx(_opticalPropertyRegionsEnumerable.ToArray(), 0.1);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(0.5, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_fx_array_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalProperties, _doubles.ToArray(),
                    _doubles.ToArray());
            Assert.AreEqual(9, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_array_one_fx_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalPropertiesEnumerable.ToArray(), 0.1,
                    _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_array_fx_array_one_time()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalPropertiesEnumerable.ToArray(),
                    _doubles.ToArray(), 0.5);
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_one_fx_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalProperties, 0.1,
                    _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_fx_array_one_time()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalProperties,
                    _doubles.ToArray(), 0.5);
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_properties_array_one_fx_one_time()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalPropertiesEnumerable.ToArray(),
                    0.1, 0.5);
            Assert.AreEqual(2, doubles.Length);
            Assert.AreEqual(1.0, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_property_region_array_fx_array_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, doubles.Length);
            Assert.AreEqual(1.1, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_array_optical_property_region_array__one_fx_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalPropertyRegionsEnumerable.ToArray(),
                    0.1, _doubles.ToArray());
            Assert.AreEqual(6, doubles.Length);
            Assert.AreEqual(1.1, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_property_region_array_one_fx_time_array()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalPropertyRegions.ToArray(),
                    0.1, _doubles.ToArray());
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(1.1, doubles[0]);
        }

        [Test]
        public void Test_ROfFxAndTime_overloads_optical_property_region_array_fx_array_one_time()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndTime(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), 0.5);
            Assert.AreEqual(3, doubles.Length);
            Assert.AreEqual(1.1, doubles[0]);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_rho_array_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalProperties,
                    _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_array_one_rho_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertiesEnumerable.ToArray(),
                    0.5, _doubles.ToArray());
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_array_rho_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertiesEnumerable.ToArray(),
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_one_rho_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalProperties,
                    0.5, _doubles.ToArray());
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_rho_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalProperties,
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_properties_array_one_rho_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertiesEnumerable.ToArray(),
                    0.5, 0.2);
            Assert.AreEqual(2, complexes.Length);
            Assert.AreEqual(0.8, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_property_region_array_rho_array_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_property_region_array_one_rho_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertyRegions.ToArray(),
                    0.5, _doubles.ToArray());
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_optical_property_region_array_rho_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_array_optical_property_region_array_rho_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertyRegionsEnumerable.ToArray(),
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real);
        }

        [Test]
        public void Test_ROfRhoAndFt_overloads_array_optical_property_region_array_one_rho_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfRhoAndFt(_opticalPropertyRegionsEnumerable.ToArray(),
                    0.5, _doubles.ToArray());
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_fx_array_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalProperties,
                    _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_array_one_fx_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertiesEnumerable.ToArray(),
                    0.2, _doubles.ToArray());
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_array_fx_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertiesEnumerable.ToArray(),
                    _doubles.ToArray(), 0.1);
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_one_fx_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalProperties,
                    0.1, _doubles.ToArray());
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_fx_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalProperties,
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_properties_array_one_fx_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertiesEnumerable.ToArray(),
                    0.1, 0.2);
            Assert.AreEqual(2, complexes.Length);
            Assert.AreEqual(1.2, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_property_region_array_fx_array_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(9, complexes.Length);
            Assert.AreEqual(1.3, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_property_region_array_one_fx_ft_array()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertyRegions.ToArray(),
                    0.1, _doubles.ToArray());
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(1.3, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_optical_property_region_array_fx_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertyRegions.ToArray(),
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(3, complexes.Length);
            Assert.AreEqual(1.3, complexes[0].Real);
        }

        [Test]
        public void Test_ROfFxAndFt_overloads_array_optical_property_region_array_fx_array_one_ft()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.ROfFxAndFt(_opticalPropertyRegionsEnumerable.ToArray(),
                    _doubles.ToArray(), 0.2);
            Assert.AreEqual(6, complexes.Length);
            Assert.AreEqual(1.3, complexes[0].Real);
        }

        [Test]
        public void Test_FluenceOfRhoAndZ_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.FluenceOfRhoAndZ(_opticalPropertiesEnumerable, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZ_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.FluenceOfRhoAndZ(_opticalPropertyRegionsEnumerable, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndTime_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.FluenceOfRhoAndZAndTime(_opticalPropertiesEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndTime_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.FluenceOfRhoAndZAndTime(_opticalPropertyRegionsEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndFt_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.FluenceOfRhoAndZAndFt(_opticalPropertiesEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndFt_with_an_array_of_optical_property_regions_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.FluenceOfRhoAndZAndFt(_opticalPropertyRegionsEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfFxAndZ_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.FluenceOfFxAndZ(_opticalPropertiesEnumerable, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfFxAndZAndTime_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.FluenceOfFxAndZAndTime(_opticalPropertiesEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfFxAndZAndFt_throws_not_implemented_exception()
        {
            Assert.Throws<NotImplementedException>(() => _forwardSolverBaseMock.Object.FluenceOfFxAndZAndFt(_opticalPropertiesEnumerable, _doubles, _doubles, _doubles));
        }

        [Test]
        public void Test_FluenceOfRhoAndZ_fluence_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.FluenceOfRhoAndZ(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(0.1, doubles[0], 0.00001);
            Assert.AreEqual(0.2, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfRhoAndZ_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.FluenceOfRhoAndZ(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(0.3, doubles[0], 0.00001);
            Assert.AreEqual(0.4, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndTime_fluence_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.FluenceOfRhoAndZAndTime(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, doubles.Length);
            Assert.AreEqual(0.5, doubles[0], 0.00001);
            Assert.AreEqual(0.6, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndTime_optical_property_region_reflectance_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.FluenceOfRhoAndZAndTime(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, doubles.Length);
            Assert.AreEqual(0.7, doubles[0], 0.00001);
            Assert.AreEqual(0.8, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndFt_fluence_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.FluenceOfRhoAndZAndFt(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, complexes.Length);
            Assert.AreEqual(0.9, complexes[0].Real, 0.00001);
            Assert.AreEqual(1.0, complexes[1].Real, 0.00001);
        }

        [Test]
        public void Test_FluenceOfRhoAndZAndFt_optical_property_region_reflectance_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.FluenceOfRhoAndZAndFt(_opticalPropertyRegionsEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, complexes.Length);
            Assert.AreEqual(1.1, complexes[0].Real, 0.00001);
            Assert.AreEqual(1.2, complexes[1].Real, 0.00001);
        }

        [Test]
        public void Test_FluenceOfFxAndZ_fluence_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.FluenceOfFxAndZ(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(18, doubles.Length);
            Assert.AreEqual(1.3, doubles[0], 0.00001);
            Assert.AreEqual(1.4, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfFxAndZAndTime_fluence_array_overloads()
        {
            var doubles =
                _forwardSolverBaseMockWithSetup.Object.FluenceOfFxAndZAndTime(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, doubles.Length);
            Assert.AreEqual(1.5, doubles[0], 0.00001);
            Assert.AreEqual(1.6, doubles[1], 0.00001);
        }

        [Test]
        public void Test_FluenceOfFxAndZAndFt_fluence_array_overloads()
        {
            var complexes =
                _forwardSolverBaseMockWithSetup.Object.FluenceOfFxAndZAndFt(_opticalPropertiesEnumerable.ToArray(), _doubles.ToArray(), _doubles.ToArray(), _doubles.ToArray());
            Assert.AreEqual(54, complexes.Length);
            Assert.AreEqual(1.7, complexes[0].Real, 0.00001);
            Assert.AreEqual(1.8, complexes[1].Real, 0.00001);
        }
    }
}
