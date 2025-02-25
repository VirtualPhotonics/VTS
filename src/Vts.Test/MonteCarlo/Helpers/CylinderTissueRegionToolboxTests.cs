using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    [TestFixture]
    public class CylinderTissueRegionToolboxTests
    {
        /// <summary>
        /// Validate RayIntersectInfiniteCylinder provides correct results
        /// </summary>
        [Test]
        public void Validate_RayIntersectInfiniteCylinder_results()
        {
            var position1 = new Position(0, 0, 0);
            var position2 = new Position(0, 0, 1);
            var oneIn = false;
            var center = new Position(0, 0, 1);
            var radius = 0.5;
            double distanceToBoundary;
            // the following tests test if single intersection, no intersection, and two intersections
            // test cylinder with axis along x-axis
            var intersection = CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(
                position1, 
                position2, 
                oneIn, 
                Vts.MonteCarlo.CylinderTissueRegionAxisType.X,
                center, 
                radius, 
                out distanceToBoundary);
            Assert.That(intersection, Is.True); // single intersection
            Assert.Less(Math.Abs(distanceToBoundary - 0.5), 0.000001);
            position2 = new Position(1, 0, 0);
            intersection = CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(
                position1,
                position2,
                oneIn,
                Vts.MonteCarlo.CylinderTissueRegionAxisType.X,
                center,
                radius,
                out distanceToBoundary);
            Assert.That(intersection, Is.False); // no intersection
            Assert.Less(Math.Abs(distanceToBoundary - double.PositiveInfinity), 0.000001);
            position1 = new Position(0, 0, 0);
            position2 = new Position(0, 0, 2);
            intersection = CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(
                position1,
                position2,
                oneIn,
                Vts.MonteCarlo.CylinderTissueRegionAxisType.X,
                center,
                radius,
                out distanceToBoundary);
            Assert.That(intersection, Is.True); // double intersection
            Assert.Less(Math.Abs(distanceToBoundary - 0.5), 0.000001);
            // test cylinder with axis along y-axis
            position1 = new Position(0, 0, 0);
            position2 = new Position(0, 0, 1);
            intersection = CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(
                position1,
                position2,
                oneIn,
                Vts.MonteCarlo.CylinderTissueRegionAxisType.Y,
                center,
                radius,
                out distanceToBoundary);
            Assert.That(intersection, Is.True); // single intersection
            Assert.Less(Math.Abs(distanceToBoundary - 0.5), 0.000001);
            position2 = new Position(1, 0, 0);
            intersection = CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(
                position1,
                position2,
                oneIn,
                Vts.MonteCarlo.CylinderTissueRegionAxisType.Y,
                center,
                radius,
                out distanceToBoundary);
            Assert.That(intersection, Is.False); // no intersection
            Assert.Less(Math.Abs(distanceToBoundary - double.PositiveInfinity), 0.000001);
            position1 = new Position(0, 0, 0);
            position2 = new Position(0, 0, 2);
            intersection = CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(
                position1,
                position2,
                oneIn,
                Vts.MonteCarlo.CylinderTissueRegionAxisType.Y,
                center,
                radius,
                out distanceToBoundary);
            Assert.That(intersection, Is.True); // double intersection
            Assert.Less(Math.Abs(distanceToBoundary - 0.5), 0.000001);
            // test cylinder with axis along z-axis
            position1 = new Position(1, 0, 1);
            position2 = new Position(0, 0, 1);
            intersection = CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(
                position1,
                position2,
                oneIn,
                Vts.MonteCarlo.CylinderTissueRegionAxisType.Z,
                center,
                radius,
                out distanceToBoundary);
            Assert.That(intersection, Is.True); // single intersection
            Assert.Less(Math.Abs(distanceToBoundary - 0.5), 0.000001);
            position2 = new Position(1, 0, 0);
            intersection = CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(
                position1,
                position2,
                oneIn,
                Vts.MonteCarlo.CylinderTissueRegionAxisType.Z,
                center,
                radius,
                out distanceToBoundary);
            Assert.That(intersection, Is.False); // no intersection
            Assert.Less(Math.Abs(distanceToBoundary - double.PositiveInfinity), 0.000001);
            position2 = new Position(-1, 0, 1);
            intersection = CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(
                position1,
                position2,
                oneIn,
                Vts.MonteCarlo.CylinderTissueRegionAxisType.Z,
                center,
                radius,
                out distanceToBoundary);
            Assert.That(intersection, Is.True); // double intersection
            Assert.Less(Math.Abs(distanceToBoundary - 0.5), 0.000001);
        }
    }
}

