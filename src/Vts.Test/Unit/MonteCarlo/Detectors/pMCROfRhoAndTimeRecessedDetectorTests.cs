using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class pMCROfRhoAndTimeRecessedDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testpmcrofrhoandtimerecessed");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void
        Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testpmcrofrhoandtimerecessed";
        var detector = new pMCROfRhoAndTimeRecessedDetector
        {
            Rho = new DoubleRange(0, 10, 3),
            Time = new DoubleRange(0, 1, 4),
            ZPlane = -0.1,
            PerturbedOps = new List<OpticalProperties> { new() },
            PerturbedRegionsIndices = new List<int> { 1 },
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } },
            SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } }
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

        Assert.That(detector.Mean[0, 0], Is.EqualTo(1));
        Assert.That(detector.Mean[0, 1], Is.EqualTo(2));
        Assert.That(detector.Mean[0, 2], Is.EqualTo(3));
        Assert.That(detector.Mean[1, 0], Is.EqualTo(4));
        Assert.That(detector.Mean[1, 1], Is.EqualTo(5));
        Assert.That(detector.Mean[1, 2], Is.EqualTo(6));
        if (!tallySecondMoment) return;
        Assert.That(detector.SecondMoment[0, 0], Is.EqualTo(7));
        Assert.That(detector.SecondMoment[0, 1], Is.EqualTo(8));
        Assert.That(detector.SecondMoment[0, 2], Is.EqualTo(9));
        Assert.That(detector.SecondMoment[1, 0], Is.EqualTo(10));
        Assert.That(detector.SecondMoment[1, 1], Is.EqualTo(11));
        Assert.That(detector.SecondMoment[1, 2], Is.EqualTo(12));
    }

}