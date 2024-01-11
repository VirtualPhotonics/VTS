using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class pMCTOfRhoDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testpmctofrho");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testpmctofrho";
        var detector = new pMCTOfRhoDetector
        {
            Rho = new DoubleRange(0, 10, 3),
            TallySecondMoment = tallySecondMoment,
            PerturbedOps = new List<OpticalProperties> { new() },
            PerturbedRegionsIndices = new List<int> { 1 },
            Name = detectorName,
            Mean = new double[] { 1, 2, 3 },
            SecondMoment = new double[] { 4, 5, 6 }
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

        Assert.AreEqual(1, detector.Mean[0]);
        Assert.AreEqual(2, detector.Mean[1]);
        Assert.AreEqual(3, detector.Mean[2]);
        if (!tallySecondMoment) return;
        Assert.AreEqual(4, detector.SecondMoment[0]);
        Assert.AreEqual(5, detector.SecondMoment[1]);
        Assert.AreEqual(6, detector.SecondMoment[2]);
    }
}