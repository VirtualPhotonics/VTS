using NUnit.Framework;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Extensions;

namespace Vts.Test.MonteCarlo.Extensions
{
    [TestFixture]
    public class VirtualBoundaryExtensionMethodsTests
    {       
        /// <summary>
        /// Validate method IsSurfaceBoundary
        /// </summary>
        [Test]
        public void validate_IsSurfaceBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.DiffuseReflectance;
            Assert.IsTrue(virtualBoundaryType.IsSurfaceVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            Assert.IsTrue(virtualBoundaryType.IsSurfaceVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.SpecularReflectance;
            Assert.IsTrue(virtualBoundaryType.IsSurfaceVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.IsTrue(virtualBoundaryType.IsSurfaceVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.Dosimetry;
            Assert.IsTrue(virtualBoundaryType.IsSurfaceVirtualBoundary());
            // validate those that are false
            virtualBoundaryType = VirtualBoundaryType.GenericVolumeBoundary;
            Assert.IsFalse(virtualBoundaryType.IsSurfaceVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.BoundingCylinderVolume;
            Assert.IsFalse(virtualBoundaryType.IsSurfaceVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsVolumeBoundary
        /// </summary>
        [Test]
        public void validate_IsVolumeBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.GenericVolumeBoundary;
            Assert.IsTrue(virtualBoundaryType.IsVolumeVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.BoundingCylinderVolume;
            Assert.IsTrue(virtualBoundaryType.IsVolumeVirtualBoundary());
            // validate those that are false
            virtualBoundaryType = VirtualBoundaryType.DiffuseReflectance;
            Assert.IsFalse(virtualBoundaryType.IsVolumeVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            Assert.IsFalse(virtualBoundaryType.IsVolumeVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.SpecularReflectance;
            Assert.IsFalse(virtualBoundaryType.IsVolumeVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.IsFalse(virtualBoundaryType.IsVolumeVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.Dosimetry;
            Assert.IsFalse(virtualBoundaryType.IsVolumeVirtualBoundary());
        }
    }
}

