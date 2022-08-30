using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for BoundedTissue
    /// </summary>
    [TestFixture]
    public class BoundedTissueTests
    {
        private BoundedTissue _oneLayerTissue, _twoLayerTissue;
        /// <summary>
        /// Validate general constructor of Tissue for a one layer and two layer tissue cylinder
        /// </summary>

        /// <summary>
        /// Test default constructor
        /// </summary>
        [Test]
        public void validate_default_constructor()
        {
            var boundedTissue = new BoundedTissue();
            Assert.IsInstanceOf<BoundedTissue>(boundedTissue);
        }

    }
}
