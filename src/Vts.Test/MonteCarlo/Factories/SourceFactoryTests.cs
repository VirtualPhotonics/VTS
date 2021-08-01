using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Factories;
using MathNet.Numerics.Random;
using Vts.IO;

namespace Vts.Test.MonteCarlo.Factories
{
    /// <summary>
    /// Unit tests for SourceFactory
    /// </summary>
    [TestFixture]
    public class SourceFactoryTests
    {
        /// <summary>
        /// Simulate basic usage of SourceFactory
        /// </summary>
        [Test]
        public void Demonstrate_GetSource()
        {
            ISourceInput sourceInput = new DirectionalPointSourceInput
                {
                    SourceType =  "DirectionalPoint",
                    PointLocation = new Position(0, 0, 0),
                    Direction = new Direction(0, 0, 1),
                    InitialTissueRegionIndex = 0
                };
            var source = SourceFactory.GetSource(sourceInput, new MersenneTwister(0));

            Assert.NotNull(source);
        }
    }
}
