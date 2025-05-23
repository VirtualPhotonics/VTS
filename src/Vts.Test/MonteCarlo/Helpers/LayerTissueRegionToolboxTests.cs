﻿using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    [TestFixture]
    public class LayerTissueRegionToolboxTests
    {
        /// <summary>
        /// Validate RayExtendToInfinitePlane provides correct results
        /// </summary>
        [Test]
        public void Validate_RayExtendToInfinitePlane_results()
        {
            const int zPlane = -1;
            var pos = new Position(0, 0, 0);
            // check vertical direction
            var dir = new Direction(0, 0, -1);
            var newPos = LayerTissueRegionToolbox.RayExtendToInfinitePlane(pos, dir, zPlane);
            Assert.That(Math.Abs(pos.X - newPos.X), Is.LessThan(0.000001));
            Assert.That(Math.Abs(pos.Y - newPos.Y), Is.LessThan(0.000001));
            Assert.That(Math.Abs(-1 - newPos.Z), Is.LessThan(0.000001));
            dir = new Direction(1.0 / Math.Sqrt(2), 0, -1.0 / Math.Sqrt(2));
            newPos = LayerTissueRegionToolbox.RayExtendToInfinitePlane(pos, dir, zPlane);
            Assert.That(Math.Abs(1.0 - newPos.X), Is.LessThan(0.000001));
            Assert.That(Math.Abs(0.0 - newPos.Y), Is.LessThan(0.000001));
            Assert.That(Math.Abs(-1 - newPos.Z), Is.LessThan(0.000001));
            dir = new Direction(-1.0 / Math.Sqrt(2), 0, -1.0 / Math.Sqrt(2));
            newPos = LayerTissueRegionToolbox.RayExtendToInfinitePlane(pos, dir, zPlane);
            Assert.That(Math.Abs(-1.0 - newPos.X), Is.LessThan(0.000001));
            Assert.That(Math.Abs(0.0 - newPos.Y), Is.LessThan(0.000001));
            Assert.That(Math.Abs(-1 - newPos.Z), Is.LessThan(0.000001));
            dir = new Direction(0.0, 1.0 / Math.Sqrt(2), -1.0 / Math.Sqrt(2));
            newPos = LayerTissueRegionToolbox.RayExtendToInfinitePlane(pos, dir, zPlane);
            Assert.That(Math.Abs(0.0 - newPos.X), Is.LessThan(0.000001));
            Assert.That(Math.Abs(1.0 - newPos.Y), Is.LessThan(0.000001));
            Assert.That(Math.Abs(-1 - newPos.Z), Is.LessThan(0.000001));
            dir = new Direction(0.0, -1.0 / Math.Sqrt(2), -1.0 / Math.Sqrt(2));
            newPos = LayerTissueRegionToolbox.RayExtendToInfinitePlane(pos, dir, zPlane);
            Assert.That(Math.Abs(0.0 - newPos.X), Is.LessThan(0.000001));
            Assert.That(Math.Abs(-1.0 - newPos.Y), Is.LessThan(0.000001));
            Assert.That(Math.Abs(-1 - newPos.Z), Is.LessThan(0.000001));
            // try parallel direction
            dir = new Direction(1.0 / Math.Sqrt(2), -1.0 / Math.Sqrt(2), 0.0);
            newPos = LayerTissueRegionToolbox.RayExtendToInfinitePlane(pos, dir, zPlane);
            Assert.That(newPos == null, Is.True);
        }
    }
}

