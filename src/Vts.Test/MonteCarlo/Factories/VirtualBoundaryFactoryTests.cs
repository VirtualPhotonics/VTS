using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Factories
{
    /// <summary>
    /// Unit tests for VirtualBoundaryFactory
    /// </summary>
    [TestFixture]
    public class VirtualBoundaryFactoryTests
    {
        /// <summary>
        /// Simulate basic usage of VirtualBoundaryFactory
        /// </summary>
        [Test]
        public void Demonstrate_GetVirtualBoundary_successful_return()
        {
            var detectorList = new List<IDetector>() {new RDiffuseDetector()};
            var tissue = new MultiLayerTissue();
            Assert.That(VirtualBoundaryFactory.GetVirtualBoundary(
                VirtualBoundaryType.DiffuseReflectance,
                tissue, new DetectorController(detectorList)), Is.InstanceOf<IVirtualBoundary>());
            Assert.That(VirtualBoundaryFactory.GetVirtualBoundary(
                    VirtualBoundaryType.DiffuseTransmittance,
                    tissue, new DetectorController(detectorList)), Is.InstanceOf<IVirtualBoundary>());
            Assert.That(VirtualBoundaryFactory.GetVirtualBoundary(
                    VirtualBoundaryType.SpecularReflectance,
                    tissue, new DetectorController(detectorList)), Is.InstanceOf<IVirtualBoundary>());
            Assert.That(VirtualBoundaryFactory.GetVirtualBoundary(
                    VirtualBoundaryType.InternalSurface,
                    tissue, new DetectorController(detectorList)), Is.InstanceOf<IVirtualBoundary>());
            Assert.That(VirtualBoundaryFactory.GetVirtualBoundary(
                    VirtualBoundaryType.pMCDiffuseReflectance,
                    tissue, new DetectorController(detectorList)), Is.InstanceOf<IVirtualBoundary>());
            // set up different tissue for this BoundingCylinder VB
            var boundingTissueInput = new BoundingCylinderTissueInput();
            var boundingTissue = boundingTissueInput.CreateTissue(
                AbsorptionWeightingType.Analog,
                new Dictionary<string, IPhaseFunction>(),
                0.0);
            Assert.That(VirtualBoundaryFactory.GetVirtualBoundary(
                    VirtualBoundaryType.BoundingVolume,
                    boundingTissue, new DetectorController(detectorList)), Is.InstanceOf<IVirtualBoundary>());
        }
        /// <summary>
        /// Simulate erroneous invocation
        /// </summary>
        [Test]
        public void Demonstrate_GetVirtualBoundary_throws_exception_on_faulty_input()
        {
            var detectorList = new List<IDetector>() { new RDiffuseDetector() };
            var tissue = new MultiLayerTissue();
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                VirtualBoundaryFactory.GetVirtualBoundary(
                     (VirtualBoundaryType)(-1),
                     tissue, new DetectorController(detectorList)));
        }
    }
}
