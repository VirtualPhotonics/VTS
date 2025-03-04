using NUnit.Framework;
using System.IO;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class ROfXAndYAndThetaAndPhiDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testrofxandyandthetaandphi");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testrofxandyandthetaandphi";
        var detector = new ROfXAndYAndThetaAndPhiDetector
        {
            X = new DoubleRange(-10, 10, 3),
            Y = new DoubleRange(-10, 10, 3),
            Theta = new DoubleRange(0, 1, 3),
            Phi = new DoubleRange(0, 1, 4),
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new double[,,,] // X.Count-1 x Y.Count-1 x Theta.Count-1 x Phi.Count-1: 2x2x2x3
            {
                {
                    { { 1, 2, 3 }, { 4, 5, 6 } },
                    { { 7, 8, 9 }, { 10, 11, 12 } }
                },
                {
                    { { 13, 14, 15 }, { 16, 17, 18 } },
                    { { 19, 20, 21 }, { 22, 23, 24 } }
                }
            },
            SecondMoment = new double[,,,]
            {
                {
                    { { 25, 26, 27 }, { 28, 29, 30 } },
                    { { 31, 32, 33 }, { 34, 35, 36 } }
                },
                {
                    { { 37, 38, 39 }, { 40, 41, 42 } },
                    { { 43, 44, 45 }, { 46, 47, 48 } }
                }
            }
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

        Assert.That(detector.Mean[0, 0, 0, 0], Is.EqualTo(1));
        Assert.That(detector.Mean[0, 0, 0, 1], Is.EqualTo(2));
        Assert.That(detector.Mean[0, 0, 0, 2], Is.EqualTo(3));
        Assert.That(detector.Mean[0, 0, 1, 0], Is.EqualTo(4));
        Assert.That(detector.Mean[0, 0, 1, 1], Is.EqualTo(5));
        Assert.That(detector.Mean[0, 0, 1, 2], Is.EqualTo(6));
        Assert.That(detector.Mean[0, 1, 0, 0], Is.EqualTo(7));
        Assert.That(detector.Mean[0, 1, 0, 1], Is.EqualTo(8));
        Assert.That(detector.Mean[0, 1, 0, 2], Is.EqualTo(9));
        Assert.That(detector.Mean[0, 1, 1, 0], Is.EqualTo(10));
        Assert.That(detector.Mean[0, 1, 1, 1], Is.EqualTo(11));
        Assert.That(detector.Mean[0, 1, 1, 2], Is.EqualTo(12));
        Assert.That(detector.Mean[1, 0, 0, 0], Is.EqualTo(13));
        Assert.That(detector.Mean[1, 0, 0, 1], Is.EqualTo(14));
        Assert.That(detector.Mean[1, 0, 0, 2], Is.EqualTo(15));
        Assert.That(detector.Mean[1, 0, 1, 0], Is.EqualTo(16));
        Assert.That(detector.Mean[1, 0, 1, 1], Is.EqualTo(17));
        Assert.That(detector.Mean[1, 0, 1, 2], Is.EqualTo(18));
        Assert.That(detector.Mean[1, 1, 0, 0], Is.EqualTo(19));
        Assert.That(detector.Mean[1, 1, 0, 1], Is.EqualTo(20));
        Assert.That(detector.Mean[1, 1, 0, 2], Is.EqualTo(21));
        Assert.That(detector.Mean[1, 1, 1, 0], Is.EqualTo(22));
        Assert.That(detector.Mean[1, 1, 1, 1], Is.EqualTo(23));
        Assert.That(detector.Mean[1, 1, 1, 2], Is.EqualTo(24));
        if (!tallySecondMoment) return;
        Assert.That(detector.SecondMoment[0, 0, 0, 0], Is.EqualTo(25));
        Assert.That(detector.SecondMoment[0, 0, 0, 1], Is.EqualTo(26));
        Assert.That(detector.SecondMoment[0, 0, 0, 2], Is.EqualTo(27));
        Assert.That(detector.SecondMoment[0, 0, 1, 0], Is.EqualTo(28));
        Assert.That(detector.SecondMoment[0, 0, 1, 1], Is.EqualTo(29));
        Assert.That(detector.SecondMoment[0, 0, 1, 2], Is.EqualTo(30));
        Assert.That(detector.SecondMoment[0, 1, 0, 0], Is.EqualTo(31));
        Assert.That(detector.SecondMoment[0, 1, 0, 1], Is.EqualTo(32));
        Assert.That(detector.SecondMoment[0, 1, 0, 2], Is.EqualTo(33));
        Assert.That(detector.SecondMoment[0, 1, 1, 0], Is.EqualTo(34));
        Assert.That(detector.SecondMoment[0, 1, 1, 1], Is.EqualTo(35));
        Assert.That(detector.SecondMoment[0, 1, 1, 2], Is.EqualTo(36));
        Assert.That(detector.SecondMoment[1, 0, 0, 0], Is.EqualTo(37));
        Assert.That(detector.SecondMoment[1, 0, 0, 1], Is.EqualTo(38));
        Assert.That(detector.SecondMoment[1, 0, 0, 2], Is.EqualTo(39));
        Assert.That(detector.SecondMoment[1, 0, 1, 0], Is.EqualTo(40));
        Assert.That(detector.SecondMoment[1, 0, 1, 1], Is.EqualTo(41));
        Assert.That(detector.SecondMoment[1, 0, 1, 2], Is.EqualTo(42));
        Assert.That(detector.SecondMoment[1, 1, 0, 0], Is.EqualTo(43));
        Assert.That(detector.SecondMoment[1, 1, 0, 1], Is.EqualTo(44));
        Assert.That(detector.SecondMoment[1, 1, 0, 2], Is.EqualTo(45));
        Assert.That(detector.SecondMoment[1, 1, 1, 0], Is.EqualTo(46));
        Assert.That(detector.SecondMoment[1, 1, 1, 1], Is.EqualTo(47));
        Assert.That(detector.SecondMoment[1, 1, 1, 2], Is.EqualTo(48));
    }
}