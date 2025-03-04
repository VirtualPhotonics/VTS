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
            Assert.That(collisionInfo, Is.InstanceOf<CollisionInfo>());
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
            Assert.That(clone, Is.InstanceOf<CollisionInfo>());
            Assert.That(clone[0].NumberOfCollisions, Is.EqualTo(collisionInfo[0].NumberOfCollisions));
            clone[0].NumberOfCollisions = 200;
            Assert.That(clone[0].NumberOfCollisions, Is.Not.EqualTo(collisionInfo[0].NumberOfCollisions));
        }
    }
}
