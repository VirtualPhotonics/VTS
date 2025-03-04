﻿using NUnit.Framework;
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
            Assert.That(DetectorControllerFactory.GetDetectorController(
                VirtualBoundaryType.DiffuseReflectance,
                detectorList, null), Is.InstanceOf<IDetectorController>());
            Assert.That(DetectorControllerFactory.GetDetectorController(
                    VirtualBoundaryType.DiffuseTransmittance,
                    detectorList, null), Is.InstanceOf<IDetectorController>());
            Assert.That(DetectorControllerFactory.GetDetectorController(
                    VirtualBoundaryType.SpecularReflectance,
                    detectorList, null), Is.InstanceOf<IDetectorController>());
            Assert.That(DetectorControllerFactory.GetDetectorController(
                    VirtualBoundaryType.InternalSurface,
                    detectorList, null), Is.InstanceOf<IDetectorController>());
            Assert.That(DetectorControllerFactory.GetDetectorController(
                    VirtualBoundaryType.pMCDiffuseReflectance,
                    detectorList, null), Is.InstanceOf<IDetectorController>());
            Assert.That(DetectorControllerFactory.GetDetectorController(
                    VirtualBoundaryType.BoundingVolume,
                    detectorList, null), Is.InstanceOf<IDetectorController>());
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
