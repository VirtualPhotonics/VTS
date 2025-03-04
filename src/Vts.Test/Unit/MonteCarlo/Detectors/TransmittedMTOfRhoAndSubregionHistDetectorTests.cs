using NUnit.Framework;
using System.IO;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class TransmittedMTOfRhoAndSubregionHistDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testtransmittedmtofrhoandsubregionhist");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testtransmittedmtofrhoandsubregionhist";
        var detector = new TransmittedMTOfRhoAndSubregionHistDetector
        {
            Rho = new DoubleRange(0, 10, 3),
            MTBins = new DoubleRange(0, 10, 4),
            FractionalMTBins = new DoubleRange(0, 1, 1),
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }, // Rho.Count-1 x MTBins.Count-1: 2x3
            SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } },
            // FractionalMT has dimensions [Rho.Count-1, MTBins.Count-1, NumSubregions, FractionalMTBins.Count + 1]=[2,3,3,2]
            FractionalMT = new double[,,,] // 2x3x3x2
            {
                {
                    { { 1, 2 }, { 3, 4 }, { 5, 6 },},
                    { { 7, 8 }, { 9, 10 }, { 11, 12 } },
                    { { 13, 14 }, { 15, 16 }, { 17, 18 } }
                },
                {
                    { { 19, 20 }, { 21, 22 }, { 23, 24 }, },
                    { { 25, 26 }, { 27, 28 }, { 29, 30 } },
                    { { 31, 32 }, { 33, 34 }, { 35, 36 } }
                }
            }
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
            detector.FractionalMT);

        Assert.That(detector.Mean[0, 0], Is.EqualTo(1));
        Assert.That(detector.Mean[0, 1], Is.EqualTo(2));
        Assert.That(detector.Mean[0, 2], Is.EqualTo(3));
        Assert.That(detector.Mean[1, 0], Is.EqualTo(4));
        Assert.That(detector.Mean[1, 1], Is.EqualTo(5));
        Assert.That(detector.Mean[1, 2], Is.EqualTo(6));

        Assert.That(detector.FractionalMT[0, 0, 0, 0], Is.EqualTo(1));
        Assert.That(detector.FractionalMT[0, 0, 0, 1], Is.EqualTo(2));
        Assert.That(detector.FractionalMT[0, 0, 1, 0], Is.EqualTo(3));
        Assert.That(detector.FractionalMT[0, 0, 1, 1], Is.EqualTo(4));
        Assert.That(detector.FractionalMT[0, 0, 2, 0], Is.EqualTo(5));
        Assert.That(detector.FractionalMT[0, 0, 2, 1], Is.EqualTo(6));
        Assert.That(detector.FractionalMT[0, 1, 0, 0], Is.EqualTo(7));
        Assert.That(detector.FractionalMT[0, 1, 0, 1], Is.EqualTo(8));
        Assert.That(detector.FractionalMT[0, 1, 1, 0], Is.EqualTo(9));
        Assert.That(detector.FractionalMT[0, 1, 1, 1], Is.EqualTo(10));
        Assert.That(detector.FractionalMT[0, 1, 2, 0], Is.EqualTo(11));
        Assert.That(detector.FractionalMT[0, 1, 2, 1], Is.EqualTo(12));
        Assert.That(detector.FractionalMT[0, 2, 0, 0], Is.EqualTo(13));
        Assert.That(detector.FractionalMT[0, 2, 0, 1], Is.EqualTo(14));
        Assert.That(detector.FractionalMT[0, 2, 1, 0], Is.EqualTo(15));
        Assert.That(detector.FractionalMT[0, 2, 1, 1], Is.EqualTo(16));
        Assert.That(detector.FractionalMT[0, 2, 2, 0], Is.EqualTo(17));
        Assert.That(detector.FractionalMT[0, 2, 2, 1], Is.EqualTo(18));
        Assert.That(detector.FractionalMT[1, 0, 0, 0], Is.EqualTo(19));
        Assert.That(detector.FractionalMT[1, 0, 0, 1], Is.EqualTo(20));
        Assert.That(detector.FractionalMT[1, 0, 1, 0], Is.EqualTo(21));
        Assert.That(detector.FractionalMT[1, 0, 1, 1], Is.EqualTo(22));
        Assert.That(detector.FractionalMT[1, 0, 2, 0], Is.EqualTo(23));
        Assert.That(detector.FractionalMT[1, 0, 2, 1], Is.EqualTo(24));
        Assert.That(detector.FractionalMT[1, 1, 0, 0], Is.EqualTo(25));
        Assert.That(detector.FractionalMT[1, 1, 0, 1], Is.EqualTo(26));
        Assert.That(detector.FractionalMT[1, 1, 1, 0], Is.EqualTo(27));
        Assert.That(detector.FractionalMT[1, 1, 1, 1], Is.EqualTo(28));
        Assert.That(detector.FractionalMT[1, 1, 2, 0], Is.EqualTo(29));
        Assert.That(detector.FractionalMT[1, 1, 2, 1], Is.EqualTo(30));
        Assert.That(detector.FractionalMT[1, 2, 0, 0], Is.EqualTo(31));
        Assert.That(detector.FractionalMT[1, 2, 0, 1], Is.EqualTo(32));
        Assert.That(detector.FractionalMT[1, 2, 1, 0], Is.EqualTo(33));
        Assert.That(detector.FractionalMT[1, 2, 1, 1], Is.EqualTo(34));
        Assert.That(detector.FractionalMT[1, 2, 2, 0], Is.EqualTo(35));
        Assert.That(detector.FractionalMT[1, 2, 2, 1], Is.EqualTo(36));
        if (!tallySecondMoment) return;
        Assert.That(detector.SecondMoment[0, 0], Is.EqualTo(7));
        Assert.That(detector.SecondMoment[0, 1], Is.EqualTo(8));
        Assert.That(detector.SecondMoment[0, 2], Is.EqualTo(9));
        Assert.That(detector.SecondMoment[1, 0], Is.EqualTo(10));
        Assert.That(detector.SecondMoment[1, 1], Is.EqualTo(11));
        Assert.That(detector.SecondMoment[1, 2], Is.EqualTo(12));
    }
}