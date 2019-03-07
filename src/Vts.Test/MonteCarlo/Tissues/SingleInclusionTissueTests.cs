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
        private SingleInclusionTissue _tissue;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void create_instance_of_class()
        {
            _tissue = new SingleInclusionTissue(new EllipsoidTissueRegion(
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
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct boolean
        /// </summary>
        [Test]
        public void verify_GetRegionIndex_method_returns_correct_result()
        {
            int index = _tissue.GetRegionIndex(new Position(0, 0, 0.5)); // outside ellipsoid
            Assert.AreEqual(index, 1);
            index = _tissue.GetRegionIndex(new Position(0, 0, 2.5)); // inside ellipsoid
            Assert.AreEqual(index, 3);
            index = _tissue.GetRegionIndex(new Position(0, 0, 1.0)); // on ellipsoid is considered in
            Assert.AreEqual(index, 3);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex return correct boolean
        /// </summary>
        [Test]
        public void verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            Photon photon = new Photon( // on top of ellipsoid pointed into it
                new Position(0, 0, 1.0),
                new Direction(0.0, 0, 1.0),
                _tissue,
                1,
                new Random());
            var index = _tissue.GetNeighborRegionIndex(photon); 
            Assert.AreEqual(index, 3);
            photon.DP.Position = new Position(0, 0, 100.0); // at bottom of slab pointed out
            index = _tissue.GetNeighborRegionIndex(photon);
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
