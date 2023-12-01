using NUnit.Framework;
using System.IO;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class ROfXAndYAndMaxDepthRecessedDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testrofxandyandmaxdepthrecessed");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testrofxandyandmaxdepthrecessed";
        var detector = new ROfXAndYAndMaxDepthRecessedDetector
        {
            X = new DoubleRange(-10, 10, 3),
            Y = new DoubleRange(-10, 10, 3),
            MaxDepth = new DoubleRange(0, 1, 4),
            ZPlane = -0.1,
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new double[,,] // X.Count-1 x Y.Count-1 x MaxDepth.Count-1: 2x2x3
            {
                { { 1, 2, 3 }, { 4, 5, 6 } },
                { { 7, 8, 9 }, { 10, 11, 12 } }
            },
            SecondMoment = new double[,,]
            {
                { { 13, 14, 15 }, { 16, 17, 18 } },
                { { 19, 20, 21 }, { 22, 23, 24 } }
            }
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

        Assert.AreEqual(1, detector.Mean[0, 0, 0]);
        Assert.AreEqual(2, detector.Mean[0, 0, 1]);
        Assert.AreEqual(3, detector.Mean[0, 0, 2]);
        Assert.AreEqual(4, detector.Mean[0, 1, 0]);
        Assert.AreEqual(5, detector.Mean[0, 1, 1]);
        Assert.AreEqual(6, detector.Mean[0, 1, 2]);
        Assert.AreEqual(7, detector.Mean[1, 0, 0]);
        Assert.AreEqual(8, detector.Mean[1, 0, 1]);
        Assert.AreEqual(9, detector.Mean[1, 0, 2]);
        Assert.AreEqual(10, detector.Mean[1, 1, 0]);
        Assert.AreEqual(11, detector.Mean[1, 1, 1]);
        Assert.AreEqual(12, detector.Mean[1, 1, 2]);
        if (!tallySecondMoment) return;
        Assert.AreEqual(13, detector.SecondMoment[0, 0, 0]);
        Assert.AreEqual(14, detector.SecondMoment[0, 0, 1]);
        Assert.AreEqual(15, detector.SecondMoment[0, 0, 2]);
        Assert.AreEqual(16, detector.SecondMoment[0, 1, 0]);
        Assert.AreEqual(17, detector.SecondMoment[0, 1, 1]);
        Assert.AreEqual(18, detector.SecondMoment[0, 1, 2]);
        Assert.AreEqual(19, detector.SecondMoment[1, 0, 0]);
        Assert.AreEqual(20, detector.SecondMoment[1, 0, 1]);
        Assert.AreEqual(21, detector.SecondMoment[1, 0, 2]);
        Assert.AreEqual(22, detector.SecondMoment[1, 1, 0]);
        Assert.AreEqual(23, detector.SecondMoment[1, 1, 1]);
        Assert.AreEqual(24, detector.SecondMoment[1, 1, 2]);
    }
}