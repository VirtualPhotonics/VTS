using NUnit.Framework;
using System;
using System.Collections.Generic;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Factories;

namespace Vts.Test.MonteCarlo.Factories
{
    /// <summary>
    /// Unit tests for DetectorControllerFactory
    /// </summary>
    [TestFixture]
    public class DetectorControllerFactoryTests
    {
        /// <summary>
        /// Simulate basic usage of DetectorControllerFactory
        /// </summary>
        [Test]
        public void Demonstrate_GetDetectorController_successful_return()
        {
            var detectorList = new List<IDetector>() {new RDiffuseDetector()};
            Assert.IsInstanceOf<IDetectorController>(
                DetectorControllerFactory.GetDetectorController(
                VirtualBoundaryType.DiffuseReflectance,
                detectorList, null));
            Assert.IsInstanceOf<IDetectorController>(
                DetectorControllerFactory.GetDetectorController(
                    VirtualBoundaryType.DiffuseTransmittance,
                    detectorList, null));
            Assert.IsInstanceOf<IDetectorController>(
                DetectorControllerFactory.GetDetectorController(
                    VirtualBoundaryType.SpecularReflectance,
                    detectorList, null));
            Assert.IsInstanceOf<IDetectorController>(
                DetectorControllerFactory.GetDetectorController(
                    VirtualBoundaryType.InternalSurface,
                    detectorList, null));
            Assert.IsInstanceOf<IDetectorController>(
                DetectorControllerFactory.GetDetectorController(
                    VirtualBoundaryType.pMCDiffuseReflectance,
                    detectorList, null));
            Assert.IsInstanceOf<IDetectorController>(
                DetectorControllerFactory.GetDetectorController(
                    VirtualBoundaryType.BoundingVolume,
                    detectorList, null));
        }
        /// <summary>
        /// Simulate erroneous invocation
        /// </summary>
        [Test]
        public void Demonstrate_GetDetectorController_throws_exception_on_faulty_input()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                DetectorControllerFactory.GetDetectorController(
                     (VirtualBoundaryType)(-1),
                     new List<IDetector>() {new RDiffuseDetector()},
                     null));
        }
    }
}
