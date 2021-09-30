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
                new OpticalProperties(0.01, 1.0, 0.8, 1.4));
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void validate_SurfaceFiber_properties()
        {
            Assert.AreEqual(_SurfaceFiberTissueRegion.Center.X, 0.0);
            Assert.AreEqual(_SurfaceFiberTissueRegion.Center.Y, 0.0);
            Assert.AreEqual(_SurfaceFiberTissueRegion.Center.Z, 2.0);
            Assert.AreEqual(_SurfaceFiberTissueRegion.Radius, 1.0);
            Assert.AreEqual(_SurfaceFiberTissueRegion.RegionOP.Mua, 0.01);
            Assert.AreEqual(_SurfaceFiberTissueRegion.RegionOP.Musp, 1.0);
            Assert.AreEqual(_SurfaceFiberTissueRegion.RegionOP.G, 0.8);
            Assert.AreEqual(_SurfaceFiberTissueRegion.RegionOP.N, 1.4);
        }

    }
}
