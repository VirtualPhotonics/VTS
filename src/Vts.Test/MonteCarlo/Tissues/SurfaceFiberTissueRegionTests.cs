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
                new OpticalProperties(0.01, 1.0, 0.8, 1.4));
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void Validate_SurfaceFiber_properties()
        {
            Assert.AreEqual(0.0,_surfaceFiberTissueRegion.Center.X);
            Assert.AreEqual(0.0,_surfaceFiberTissueRegion.Center.Y);
            Assert.AreEqual(2.0,_surfaceFiberTissueRegion.Center.Z);
            Assert.AreEqual(1.0, _surfaceFiberTissueRegion.Radius);
            Assert.AreEqual(0.01, _surfaceFiberTissueRegion.RegionOP.Mua);
            Assert.AreEqual(1.0, _surfaceFiberTissueRegion.RegionOP.Musp);
            Assert.AreEqual(0.8, _surfaceFiberTissueRegion.RegionOP.G);
            Assert.AreEqual(1.4, _surfaceFiberTissueRegion.RegionOP.N);
        }

    }
}
