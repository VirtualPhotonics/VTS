using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for SingleInclusionTissue
    /// </summary>
    [TestFixture]
    public class SingleInclusionTissueTests
    {
        private SingleInclusionTissue _tissueWithEllipsoid, _tissueWithCylinder, _tissueWithThinCylinder;
        /// <summary>
        /// Validate general constructor of Tissue
        /// </summary>
        [OneTimeSetUp]
        public void create_instance_of_class()
        {
            _tissueWithEllipsoid = new SingleInclusionTissue(new EllipsoidTissueRegion(
                new Position(0, 0, 3), 1.0, 1.0, 2.0, new OpticalProperties()),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });
            _tissueWithThinCylinder = new SingleInclusionTissue(new CylinderTissueRegion(
                    new Position(0, 0, 0), 1.0, 0.0, new OpticalProperties()),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct boolean
        /// </summary>
        [Test]
        public void verify_GetRegionIndex_method_returns_correct_result()
        {
            int index = _tissueWithEllipsoid.GetRegionIndex(new Position(0, 0, 0.5)); // outside ellipsoid
            Assert.AreEqual(index, 1);
            index = _tissueWithEllipsoid.GetRegionIndex(new Position(0, 0, 2.5)); // inside ellipsoid
            Assert.AreEqual(index, 3);
            index = _tissueWithEllipsoid.GetRegionIndex(new Position(0, 0, 1.0)); // on ellipsoid is considered in
            Assert.AreEqual(index, 3);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex for tissueWithEllipsoid return correct boolean
        /// </summary>
        [Test]
        public void verify_tissueWithEllipsoid_GetNeighborRegionIndex_method_correct_when_photon_on_ellipsoid()
        {
            // Ellipsoid
            Photon photon = new Photon( // on top of ellipsoid pointed into it
                new Position(0, 0, 1.0),
                new Direction(0.0, 0, 1.0),
                1,
                _tissueWithEllipsoid,
                1,
                new Random());
            var index = _tissueWithEllipsoid.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 3);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex for tissueWithEllipsoid return correct boolean
        /// </summary>
        [Test]
        public void verify_tissueWithEllipsoid_GetNeighborRegionIndex_method_correct_when_photon_bottom_slab()
        {
            // on bottom of slab pointed out
            Photon photon = new Photon( // have to reinitialize photon so that _onBoundary is set to false
                new Position(0, 0, 100.0),
                new Direction(0.0, 0, 1.0),
                1,
                _tissueWithEllipsoid,
                1,
                new Random());
            var index = _tissueWithEllipsoid.GetNeighborRegionIndex(photon);
            Assert.AreEqual(index, 2);
        }


        ///// <summary>
        ///// Validate method GetAngleRelativeToBoundaryNormal return correct boolean
        ///// </summary>
        //[Test]
        //public void verify_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
        //{
        //    Photon photon = new Photon( // on top of ellipsoid pointed into it
        //        new Position(0, 0, 1.0),
        //        new Direction(0.0, 0, 1.0),
        //        _tissue,
        //        1,
        //        new Random());
        //    double cosTheta = _tissue.GetAngleRelativeToBoundaryNormal(photon);
        //    Assert.AreEqual(cosTheta, 1);
        //}

    }
}
