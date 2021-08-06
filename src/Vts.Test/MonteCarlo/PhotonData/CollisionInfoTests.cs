using System;
using NUnit.Framework;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class CollisionInfoTests
    {
        /// <summary>
        /// test to verify that CollisionInfo default constructor - only
        /// used for serialization purposes
        /// </summary>
        [Test]
        public void validate_default_constructor()
        {
            var collisionInfo = new CollisionInfo();
            Assert.IsInstanceOf<CollisionInfo>(collisionInfo);
        }

        /// <summary>
        /// test to verify CollisionInfo Clone method
        /// </summary>
        [Test]
        public void validate_default_Clone()
        {
            var collisionInfo = new CollisionInfo(2);
            var subRegionCollisionInfo1 = new SubRegionCollisionInfo(10.0, 100);
            var subRegionCollisionInfo2 = new SubRegionCollisionInfo(20.0, 200);
            collisionInfo.Add(subRegionCollisionInfo1);
            collisionInfo.Add(subRegionCollisionInfo2);
            var clone = collisionInfo.Clone();
            Assert.IsInstanceOf<CollisionInfo>(clone);
            Assert.AreEqual(collisionInfo[0].NumberOfCollisions, clone[0].NumberOfCollisions);
            clone[0].NumberOfCollisions = 200;
            Assert.AreNotEqual(collisionInfo[0].NumberOfCollisions, clone[0].NumberOfCollisions);
        }
    }
}
