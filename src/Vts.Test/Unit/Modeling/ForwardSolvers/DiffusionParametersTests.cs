using System;
using NUnit.Framework;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Unit.Modeling.ForwardSolvers
{
    [TestFixture]
    public class DiffusionParametersTests
    {
        /// <summary>
        /// Test to verify results of Create Method
        /// </summary>
        [Test]
        public void Validate_Create_method_results()
        {
            // test SDA
            var ops = new OpticalProperties(0.01, 1.0, 0.8, 1.4);
            var diffusionParameters = DiffusionParameters.Create(
                ops,
                ForwardModel.SDA);
            Assert.IsTrue(Math.Abs(2.950078 - diffusionParameters.A) < 1e-6);
            Assert.IsTrue(Math.Abs(0.330033 - diffusionParameters.D) < 1e-6);
            Assert.IsTrue(Math.Abs(214.13747 - diffusionParameters.cn) < 1e-4);
            Assert.IsTrue(Math.Abs(0.8 - diffusionParameters.gTilde) < 1e-6);
            Assert.IsTrue(Math.Abs(0.01 - diffusionParameters.mua) < 1e-6);
            Assert.IsTrue(Math.Abs(0.174068 - diffusionParameters.mueff) < 1e-6);
            Assert.IsTrue(Math.Abs(1.0 - diffusionParameters.musTilde) < 1e-6);
            Assert.IsTrue(Math.Abs(1.01 - diffusionParameters.mutTilde) < 1e-6);
            Assert.IsTrue(Math.Abs(1.01 - diffusionParameters.mutr) < 1e-6);
            Assert.IsTrue(Math.Abs(1.947246 - diffusionParameters.zb) < 1e-6);
            Assert.IsTrue(Math.Abs(0.990099 - diffusionParameters.zp) < 1e-6);
            // test deltaP1=
            diffusionParameters = DiffusionParameters.Create(
                ops,
                ForwardModel.DeltaPOne);
            Assert.IsTrue(Math.Abs(2.950078 - diffusionParameters.A) < 1e-6);
            Assert.IsTrue(Math.Abs(0.330033 - diffusionParameters.D) < 1e-6);
            Assert.IsTrue(Math.Abs(214.13747 - diffusionParameters.cn) < 1e-4);
            Assert.IsTrue(Math.Abs(0.444444 - diffusionParameters.gTilde) < 1e-6);
            Assert.IsTrue(Math.Abs(0.01 - diffusionParameters.mua) < 1e-6);
            Assert.IsTrue(Math.Abs(0.174068 - diffusionParameters.mueff) < 1e-6);
            Assert.IsTrue(Math.Abs(1.799999 - diffusionParameters.musTilde) < 1e-6);
            Assert.IsTrue(Math.Abs(1.809999 - diffusionParameters.mutTilde) < 1e-6);
            Assert.IsTrue(Math.Abs(1.01 - diffusionParameters.mutr) < 1e-6);
            Assert.IsTrue(Math.Abs(1.947246 - diffusionParameters.zb) < 1e-6);
            Assert.IsTrue(Math.Abs(0.552486 - diffusionParameters.zp) < 1e-6);
            // add test to test warning
        }

        /// <summary>
        /// Test to verify results of Copy Method
        /// </summary>
        [Test]
        public void Validate_Copy_method_results()
        {
            // set diffusion parameters
            var ops = new OpticalProperties(0.01, 1.0, 0.8, 1.4);
            var originalDiffusionParameters = DiffusionParameters.Create(
                ops,
                ForwardModel.SDA);
            // copy the parameters
            var diffusionParameters = DiffusionParameters.Copy(originalDiffusionParameters);
            Assert.IsTrue(Math.Abs(2.950078 - diffusionParameters.A) < 1e-6);
            Assert.IsTrue(Math.Abs(0.330033 - diffusionParameters.D) < 1e-6);
            Assert.IsTrue(Math.Abs(214.13747 - diffusionParameters.cn) < 1e-4);
            Assert.IsTrue(Math.Abs(0.8 - diffusionParameters.gTilde) < 1e-6);
            Assert.IsTrue(Math.Abs(0.01 - diffusionParameters.mua) < 1e-6);
            Assert.IsTrue(Math.Abs(0.174068 - diffusionParameters.mueff) < 1e-6);
            Assert.IsTrue(Math.Abs(1.0 - diffusionParameters.musTilde) < 1e-6);
            Assert.IsTrue(Math.Abs(1.01 - diffusionParameters.mutTilde) < 1e-6);
            Assert.IsTrue(Math.Abs(1.01 - diffusionParameters.mutr) < 1e-6);
            Assert.IsTrue(Math.Abs(1.947246 - diffusionParameters.zb) < 1e-6);
            Assert.IsTrue(Math.Abs(0.990099 - diffusionParameters.zp) < 1e-6);
        }
    }
}
