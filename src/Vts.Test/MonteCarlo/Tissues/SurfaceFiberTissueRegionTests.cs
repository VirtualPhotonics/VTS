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
        private SurfaceFiberTissueRegion _SurfaceFiberTissueRegion;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void create_instance_of_class()
        {
            _SurfaceFiberTissueRegion = new SurfaceFiberTissueRegion(
                new Position(0, 0, 2), // center
                1.0,  
                new OpticalProperties(0.01, 1.0, 0.8, 1.4),"HenyeyGreensteinKey1");
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void validate_SurfaceFiber_properties()
        {
            Assert.AreEqual(0.0,_SurfaceFiberTissueRegion.Center.X);
            Assert.AreEqual(0.0,_SurfaceFiberTissueRegion.Center.Y);
            Assert.AreEqual(2.0,_SurfaceFiberTissueRegion.Center.Z);
            Assert.AreEqual(1.0, _SurfaceFiberTissueRegion.Radius);
            Assert.AreEqual(0.01, _SurfaceFiberTissueRegion.RegionOP.Mua);
            Assert.AreEqual(1.0, _SurfaceFiberTissueRegion.RegionOP.Musp);
            Assert.AreEqual(0.8, _SurfaceFiberTissueRegion.RegionOP.G);
            Assert.AreEqual(1.4, _SurfaceFiberTissueRegion.RegionOP.N);
        }

    }
}
