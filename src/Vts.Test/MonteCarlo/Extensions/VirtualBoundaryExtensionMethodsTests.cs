using NUnit.Framework;
using System;
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
            Assert.That(virtualBoundaryType.IsSurfaceVirtualBoundary(), Is.True);
            virtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            Assert.That(virtualBoundaryType.IsSurfaceVirtualBoundary(), Is.True);
            virtualBoundaryType = VirtualBoundaryType.SpecularReflectance;
            Assert.That(virtualBoundaryType.IsSurfaceVirtualBoundary(), Is.True);
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.That(virtualBoundaryType.IsSurfaceVirtualBoundary(), Is.True);
            virtualBoundaryType = VirtualBoundaryType.InternalSurface;
            Assert.That(virtualBoundaryType.IsSurfaceVirtualBoundary(), Is.True);
            // validate those that are false
            virtualBoundaryType = VirtualBoundaryType.GenericVolumeBoundary;
            Assert.That(virtualBoundaryType.IsSurfaceVirtualBoundary(), Is.False);
            virtualBoundaryType = VirtualBoundaryType.BoundingVolume;
            Assert.That(virtualBoundaryType.IsSurfaceVirtualBoundary(), Is.False);
            // check if enum set to something out of range
            virtualBoundaryType = (VirtualBoundaryType)Enum.GetNames(typeof(VirtualBoundaryType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => virtualBoundaryType.IsSurfaceVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsVolumeBoundary
        /// </summary>
        [Test]
        public void Validate_IsVolumeBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.GenericVolumeBoundary;
            Assert.That(virtualBoundaryType.IsVolumeVirtualBoundary(), Is.True);
            virtualBoundaryType = VirtualBoundaryType.BoundingVolume;
            Assert.That(virtualBoundaryType.IsVolumeVirtualBoundary(), Is.True);
            // validate those that are false
            virtualBoundaryType = VirtualBoundaryType.DiffuseReflectance;
            Assert.That(virtualBoundaryType.IsVolumeVirtualBoundary(), Is.False);
            virtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            Assert.That(virtualBoundaryType.IsVolumeVirtualBoundary(), Is.False);
            virtualBoundaryType = VirtualBoundaryType.SpecularReflectance;
            Assert.That(virtualBoundaryType.IsVolumeVirtualBoundary(), Is.False);
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.That(virtualBoundaryType.IsVolumeVirtualBoundary(), Is.False);
            virtualBoundaryType = VirtualBoundaryType.InternalSurface;
            Assert.That(virtualBoundaryType.IsVolumeVirtualBoundary(), Is.False);
            // check if enum set to something out of range
            virtualBoundaryType = (VirtualBoundaryType)Enum.GetNames(typeof(VirtualBoundaryType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => virtualBoundaryType.IsVolumeVirtualBoundary());
        }

        /// <summary>
        /// Validate method IsReflectanceSurfaceVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IsReflectanceSurfaceVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.DiffuseReflectance;
            Assert.That(virtualBoundaryType.IsReflectanceSurfaceVirtualBoundary(), Is.True);
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.That(virtualBoundaryType.IsReflectanceSurfaceVirtualBoundary(), Is.True);
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            Assert.That(virtualBoundaryType.IsReflectanceSurfaceVirtualBoundary(), Is.False);
            // check if enum set to something out of range
            virtualBoundaryType = (VirtualBoundaryType)Enum.GetNames(typeof(VirtualBoundaryType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => virtualBoundaryType.IsReflectanceSurfaceVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsTransmittanceSurfaceVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IsTransmittanceSurfaceVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            Assert.That(virtualBoundaryType.IsTransmittanceSurfaceVirtualBoundary(), Is.True);
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.That(virtualBoundaryType.IsTransmittanceSurfaceVirtualBoundary(), Is.False);
            // check if enum set to something out of range
            virtualBoundaryType = (VirtualBoundaryType)Enum.GetNames(typeof(VirtualBoundaryType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => virtualBoundaryType.IsTransmittanceSurfaceVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsSpecularSurfaceVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IsSpecularSurfaceVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.SpecularReflectance;
            Assert.That(virtualBoundaryType.IsSpecularSurfaceVirtualBoundary(), Is.True);
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.That(virtualBoundaryType.IsSpecularSurfaceVirtualBoundary(), Is.False);
            // check if enum set to something out of range
            virtualBoundaryType = (VirtualBoundaryType)Enum.GetNames(typeof(VirtualBoundaryType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => virtualBoundaryType.IsSpecularSurfaceVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsInternalSurfaceVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IsInternalSurfaceVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.InternalSurface;
            Assert.That(virtualBoundaryType.IsInternalSurfaceVirtualBoundary(), Is.True);
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.That(virtualBoundaryType.IsInternalSurfaceVirtualBoundary(), Is.False);
            // check if enum set to something out of range
            virtualBoundaryType = (VirtualBoundaryType)Enum.GetNames(typeof(VirtualBoundaryType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => virtualBoundaryType.IsInternalSurfaceVirtualBoundary());
        }
        /// <summary>
        /// Validate method IsGenericVolumeVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IsGenericVolumeVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.GenericVolumeBoundary;
            Assert.That(virtualBoundaryType.IsGenericVolumeVirtualBoundary(), Is.True);
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.That(virtualBoundaryType.IsGenericVolumeVirtualBoundary(), Is.False);
            // check if enum set to something out of range
            virtualBoundaryType = (VirtualBoundaryType)Enum.GetNames(typeof(VirtualBoundaryType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => virtualBoundaryType.IsGenericVolumeVirtualBoundary());
        }
        /// <summary>
        /// Validate method IspMCVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_IspMCVirtualBoundary_returns_correct_value()
        {
            // validate those that are true
            var virtualBoundaryType = VirtualBoundaryType.pMCDiffuseReflectance;
            Assert.That(virtualBoundaryType.IspMCVirtualBoundary(), Is.True); 
            // validate one that is false
            virtualBoundaryType = VirtualBoundaryType.DiffuseReflectance;
            Assert.That(virtualBoundaryType.IspMCVirtualBoundary(), Is.False);
            virtualBoundaryType = VirtualBoundaryType.DiffuseTransmittance;
            Assert.That(virtualBoundaryType.IspMCVirtualBoundary(), Is.False);
            virtualBoundaryType = VirtualBoundaryType.SpecularReflectance;
            Assert.That(virtualBoundaryType.IspMCVirtualBoundary(), Is.False);
            virtualBoundaryType = VirtualBoundaryType.InternalSurface;
            Assert.That(virtualBoundaryType.IspMCVirtualBoundary(), Is.False);
            // check if enum set to something out of range
            virtualBoundaryType = (VirtualBoundaryType)Enum.GetNames(typeof(VirtualBoundaryType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => virtualBoundaryType.IspMCVirtualBoundary());
        }
    }
}


