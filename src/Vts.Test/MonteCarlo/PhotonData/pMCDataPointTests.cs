using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class pMCDataPointTests
    {
        /// <summary>
        /// test to verify that pMCDataPoint constructor 
        /// </summary>
        [Test]
        public void validate_constructor()
        {
            var pmcDataPoint = new pMCDataPoint(
                new PhotonDataPoint(
                new Position(0,0,0),
                new Direction(0,0,1),
                1.0,
                0.0,
                Vts.MonteCarlo.PhotonStateType.Alive), 
                new CollisionInfo());
            Assert.IsInstanceOf<pMCDataPoint>(pmcDataPoint);
        }

        /// <summary>
        /// test to verify pMCDataPoint Clone method
        /// </summary>
        [Test]
        public void validate_default_Clone()
        {
            var pmcDataPoint = new pMCDataPoint(
                new PhotonDataPoint(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    1.0,
                    0.0,
                    Vts.MonteCarlo.PhotonStateType.Alive),
                new CollisionInfo(2)); 
            var clone = pmcDataPoint.Clone();
            Assert.IsInstanceOf<pMCDataPoint>(clone);
            Assert.AreEqual(pmcDataPoint.PhotonDataPoint.Weight, 
                clone.PhotonDataPoint.Weight);
            clone.PhotonDataPoint.Weight = 0.01;
            Assert.AreNotEqual(pmcDataPoint.PhotonDataPoint.Weight, 
                clone.PhotonDataPoint.Weight);
        }
    }
}
