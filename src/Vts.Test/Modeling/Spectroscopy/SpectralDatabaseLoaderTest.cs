using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.SpectralMapping;
using Vts.IO;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class SpectralDatabaseLoaderTest
    {
        [Test]
        public void validate_Loading_Spectral_Database()
        {
            var _testDictionary = Vts.SpectralMapping.SpectralDatabaseLoader.GetDatabaseFromFile();
            Assert.IsNotNull(_testDictionary);
        }
    }
}
