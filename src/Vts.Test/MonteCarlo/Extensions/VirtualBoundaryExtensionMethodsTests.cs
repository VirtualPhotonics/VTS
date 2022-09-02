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
        public void Validate_IsSurfaceBoundary_returns_correct_value()
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
            virtualBoundaryType = VirtualBoundaryType.BoundingVolume;
            Assert.IsFalse(virtualBoundaryType.IsSurfaceVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsVolumeBoundary
        /// </summary>
        [Test]
        public void Validate_IsVolumeBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.GenericVolumeBoundary;
            Assert.IsTrue(virtualBoundaryType.IsVolumeVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.BoundingVolume;
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

        /// <summary>
        /// Validate method IsReflectanceSurfaceVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IsReflectanceSurfaceVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundarType = VirtualBoundaryType.DiffuseReflectance;
            Assert.IsTrue(virtualBoundarType.IsReflectanceSurfaceVirtualBoundary());
            virtualBoundarType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.IsTrue(virtualBoundarType.IsReflectanceSurfaceVirtualBoundary());
            // validate one that is false
            virtualBoundarType = VirtualBoundaryType.DiffuseTransmittance;
            Assert.IsFalse(virtualBoundarType.IsReflectanceSurfaceVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsTransmittanceSurfaceVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IsTransmittanceSurfaceVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            Assert.IsTrue(virtualBoundaryType.IsTransmittanceSurfaceVirtualBoundary());
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.IsFalse(virtualBoundaryType.IsTransmittanceSurfaceVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsSpecularSurfaceVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IsSpecularSurfaceVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.SpecularReflectance;
            Assert.IsTrue(virtualBoundaryType.IsSpecularSurfaceVirtualBoundary());
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.IsFalse(virtualBoundaryType.IsSpecularSurfaceVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsDosimetryVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IsDosimetryVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.Dosimetry;
            Assert.IsTrue(virtualBoundaryType.IsDosimetryVirtualBoundary());
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.IsFalse(virtualBoundaryType.IsDosimetryVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsGenericVolumeVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IsGenericVolumeVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.GenericVolumeBoundary;
            Assert.IsTrue(virtualBoundaryType.IsGenericVolumeVirtualBoundary());
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.IsFalse(virtualBoundaryType.IsGenericVolumeVirtualBoundary());
        }
        /// <summary>
        /// Validate method IspMCVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IspMCVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.IsTrue(virtualBoundaryType.IspMCVirtualBoundary()); 
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.DiffuseReflectance;
            Assert.IsFalse(virtualBoundaryType.IspMCVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            Assert.IsFalse(virtualBoundaryType.IspMCVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.SpecularReflectance;
            Assert.IsFalse(virtualBoundaryType.IspMCVirtualBoundary());
            virtualBoundaryType = VirtualBoundaryType.Dosimetry;
            Assert.IsFalse(virtualBoundaryType.IspMCVirtualBoundary());
        }
    }
}


