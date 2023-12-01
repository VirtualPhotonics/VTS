using NUnit.Framework;
using System.IO;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class ROfXAndYRecessedDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testrofxandyrecessed");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testrofxandyrecessed";
        var detector = new ROfXAndYRecessedDetector
        {
            X = new DoubleRange(0, 10, 3),
            Y = new DoubleRange(0, 1, 4),
            ZPlane = -0.1,
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } },
            SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } }
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

        Assert.AreEqual(1, detector.Mean[0, 0]);
        Assert.AreEqual(2, detector.Mean[0, 1]);
        Assert.AreEqual(3, detector.Mean[0, 2]);
        Assert.AreEqual(4, detector.Mean[1, 0]);
        Assert.AreEqual(5, detector.Mean[1, 1]);
        Assert.AreEqual(6, detector.Mean[1, 2]);
        if (!tallySecondMoment) return;
        Assert.AreEqual(7, detector.SecondMoment[0, 0]);
        Assert.AreEqual(8, detector.SecondMoment[0, 1]);
        Assert.AreEqual(9, detector.SecondMoment[0, 2]);
        Assert.AreEqual(10, detector.SecondMoment[1, 0]);
        Assert.AreEqual(11, detector.SecondMoment[1, 1]);
        Assert.AreEqual(12, detector.SecondMoment[1, 2]);
    }
}