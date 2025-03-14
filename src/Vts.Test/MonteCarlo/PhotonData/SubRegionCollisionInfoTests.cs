﻿using System;
using NUnit.Framework;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class SubRegionCollisionInfoTests
    {
        /// <summary>
        /// test to verify that CollisionInfo constructor - only
        /// used for serialization purposes
        /// </summary>
        [Test]
        public void validate_constructor()
        {
            var subRegionCollisionInfo = new SubRegionCollisionInfo(10.0, 100);
            Assert.That(subRegionCollisionInfo, Is.InstanceOf<SubRegionCollisionInfo>());
        }

        /// <summary>
        /// test to verify SubRegionCollisionInfo Clone method
        /// </summary>
        [Test]
        public void validate_default_Clone()
        {
            var subRegionCollisionInfo = new SubRegionCollisionInfo(10, 100);
            var clone = subRegionCollisionInfo.Clone();
            Assert.That(clone, Is.InstanceOf<SubRegionCollisionInfo>());
            Assert.That(clone.NumberOfCollisions, Is.EqualTo(subRegionCollisionInfo.NumberOfCollisions));
            clone.NumberOfCollisions = 200;
            Assert.That(clone.NumberOfCollisions, Is.Not.EqualTo(subRegionCollisionInfo.NumberOfCollisions));
        }
    }
}
