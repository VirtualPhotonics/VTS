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

        Assert.That(detector.Mean[0, 0, 0], Is.EqualTo(1));
        Assert.That(detector.Mean[0, 0, 1], Is.EqualTo(2));
        Assert.That(detector.Mean[0, 0, 2], Is.EqualTo(3));
        Assert.That(detector.Mean[0, 1, 0], Is.EqualTo(4));
        Assert.That(detector.Mean[0, 1, 1], Is.EqualTo(5));
        Assert.That(detector.Mean[0, 1, 2], Is.EqualTo(6));
        Assert.That(detector.Mean[1, 0, 0], Is.EqualTo(7));
        Assert.That(detector.Mean[1, 0, 1], Is.EqualTo(8));
        Assert.That(detector.Mean[1, 0, 2], Is.EqualTo(9));
        Assert.That(detector.Mean[1, 1, 0], Is.EqualTo(10));
        Assert.That(detector.Mean[1, 1, 1], Is.EqualTo(11));
        Assert.That(detector.Mean[1, 1, 2], Is.EqualTo(12));
        if (!tallySecondMoment) return;
        Assert.That(detector.SecondMoment[0, 0, 0], Is.EqualTo(13));
        Assert.That(detector.SecondMoment[0, 0, 1], Is.EqualTo(14));
        Assert.That(detector.SecondMoment[0, 0, 2], Is.EqualTo(15));
        Assert.That(detector.SecondMoment[0, 1, 0], Is.EqualTo(16));
        Assert.That(detector.SecondMoment[0, 1, 1], Is.EqualTo(17));
        Assert.That(detector.SecondMoment[0, 1, 2], Is.EqualTo(18));
        Assert.That(detector.SecondMoment[1, 0, 0], Is.EqualTo(19));
        Assert.That(detector.SecondMoment[1, 0, 1], Is.EqualTo(20));
        Assert.That(detector.SecondMoment[1, 0, 2], Is.EqualTo(21));
        Assert.That(detector.SecondMoment[1, 1, 0], Is.EqualTo(22));
        Assert.That(detector.SecondMoment[1, 1, 1], Is.EqualTo(23));
        Assert.That(detector.SecondMoment[1, 1, 2], Is.EqualTo(24));
    }
}