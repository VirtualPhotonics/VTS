using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class ReynoldsMcCormickPhaseFunctionTests
    {
        /// <summary>
        /// Tests whether ScatterToNewTheta samples the polar angle correctly.  
        /// </summary>
        [Test]
        public void validate_theta_sampling_for_alpha0p5()
        {
            var drm = new Direction(1, 0, 0);
            var dhg = drm;
            var alpha = 0.5;
            var g = 0.8;
            var rng = new Random(0);
            var rm = new ReynoldsMcCormickPhaseFunction(g, alpha, rng);
            rm.ScatterToNewDirection(drm); // incoming direction gets updated by method
            Assert.That(Math.Abs(drm.Ux - 0.981334) < 1e-6, Is.True);
            Assert.That(Math.Abs(drm.Uy + 0.175357) < 1e-6, Is.True);
            Assert.That(Math.Abs(drm.Uz + 0.078945) < 1e-6, Is.True);
        }
        /// <summary>
        /// Tests whether ScatterToNewTheta samples the polar angle correctly.  
        /// Reynolds-McCormick reduces to Henyey-Greenstein when alpha=0.5.
        /// Note: in the next two tests, the Direction modified by the 
        /// rm.ScatterToNewDirection gets overwritten when H-G is called so
        /// saved result before calling H-G
        /// </summary>
        [Test]
        public void validate_theta_sampling_for_alpha0p5_agrees_with_HG()
        {
            var drm = new Direction(1, 0, 0);
            var dhg = new Direction(1, 0, 0); // make new class because gets overwritten
            var alpha = 0.5;
            var g = 0.8;
            var rng = new Random(0);
            var rm = new ReynoldsMcCormickPhaseFunction(g, alpha, rng);
            rm.ScatterToNewDirection(drm); // incoming direction gets updated by method
            // reset seed
            rng = new Random(0);
            var hg = new HenyeyGreensteinPhaseFunction(g, rng);
            hg.ScatterToNewDirection(dhg);
            Assert.That(Math.Abs(drm.Ux - dhg.Ux) < 1e-6, Is.True);
            Assert.That(Math.Abs(drm.Uy - dhg.Uy) < 1e-6, Is.True);
            Assert.That(Math.Abs(drm.Uz - dhg.Uz) < 1e-6, Is.True);
        }
        /// <summary>
        /// Tests whether ScatterToNewTheta samples the polar angle correctly.  
        /// Reynolds-McCormick should not agree with Henyey-Greenstein if alpha=1
        /// </summary>
        [Test]
        public void validate_theta_sampling_for_alpha1_does_not_agree_with_HG()
        {
            var drm = new Direction(1, 0, 0);
            var dhg = new Direction(1, 0, 0); // make new class because gets overwritten
            var alpha = -0.25;
            var g = 0.8;
            var rng = new Random(0);
            var rm = new ReynoldsMcCormickPhaseFunction(g, alpha, rng);
            rm.ScatterToNewDirection(drm); // incoming direction gets updated by method
            // reset seed
            rng = new Random(0);
            var hg = new HenyeyGreensteinPhaseFunction(g, rng);
            hg.ScatterToNewDirection(dhg);
            Assert.That(Math.Abs(drm.Ux - dhg.Ux) > 1e-6, Is.True);
            Assert.That(Math.Abs(drm.Uy - dhg.Uy) > 1e-6, Is.True);
            Assert.That(Math.Abs(drm.Uz - dhg.Uz) > 1e-6, Is.True);
        }
    }
}