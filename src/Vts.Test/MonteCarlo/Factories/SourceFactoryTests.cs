using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Factories;
using MathNet.Numerics.Random;
using Moq;
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
        public void Demonstrate_GetSource_successful_return()
        {
            ISourceInput sourceInput = new DirectionalPointSourceInput
                {
                    SourceType =  "DirectionalPoint",
                    PointLocation = new Position(0, 0, 0),
                    Direction = new Direction(0, 0, 1),
                    InitialTissueRegionIndex = 0
                };
            var source = SourceFactory.GetSource(sourceInput, new MersenneTwister(0));

            Assert.IsInstanceOf<DirectionalPointSource>(source);
        }
        /// <summary>
        /// Simulate erroneous invocation
        /// </summary>
        [Test]
        public void Demonstrate_GetSource_returns_null_on_faulty_tissue_input()
        {
            var sourceInputMock = new Mock<ISourceInput>();
            sourceInputMock.Setup(x => x.CreateSource(
                It.IsAny<Random>())).Returns((ISource) null);

            Assert.IsNull(SourceFactory.GetSource(
                sourceInputMock.Object,
                new MersenneTwister(0)));
        }
    }
}
