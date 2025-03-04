using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for SurfaceFiberTissueRegion
    /// </summary>
    [TestFixture]
    public class SurfaceFiberTissueRegionTests
    {
        private SurfaceFiberTissueRegion _surfaceFiberTissueRegion;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _surfaceFiberTissueRegion = new SurfaceFiberTissueRegion(
                new Position(0, 0, 2), // center
                1.0,  
                new OpticalProperties(0.01, 1.0, 0.8, 1.4),"HenyeyGreensteinKey1");
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void Validate_SurfaceFiber_properties()
        {
            Assert.That(_surfaceFiberTissueRegion.Center.X, Is.EqualTo(0.0));
            Assert.That(_surfaceFiberTissueRegion.Center.Y, Is.EqualTo(0.0));
            Assert.That(_surfaceFiberTissueRegion.Center.Z, Is.EqualTo(2.0));
            Assert.That(_surfaceFiberTissueRegion.Radius, Is.EqualTo(1.0));
            Assert.That(_surfaceFiberTissueRegion.RegionOP.Mua, Is.EqualTo(0.01));
            Assert.That(_surfaceFiberTissueRegion.RegionOP.Musp, Is.EqualTo(1.0));
            Assert.That(_surfaceFiberTissueRegion.RegionOP.G, Is.EqualTo(0.8));
            Assert.That(_surfaceFiberTissueRegion.RegionOP.N, Is.EqualTo(1.4));
        }

    }
}
