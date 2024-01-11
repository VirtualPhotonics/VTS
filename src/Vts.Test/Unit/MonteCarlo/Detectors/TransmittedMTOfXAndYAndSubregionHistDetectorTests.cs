using NUnit.Framework;
using System.IO;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class TransmittedMTOfXAndYAndSubregionHistDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testtransmittedmtofxandyandsubregionhist");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testtransmittedmtofxandyandsubregionhist";
        var detector = new TransmittedMTOfXAndYAndSubregionHistDetector
        {
            X = new DoubleRange(-10, 10, 3),
            Y = new DoubleRange(-10, 10, 3),
            MTBins = new DoubleRange(0, 100, 4),
            FractionalMTBins = new DoubleRange(0, 10, 1),
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new double[,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1: 2x2x3
            {
                { { 1, 2, 3 }, { 4, 5, 6 } },
                { { 7, 8, 9 }, { 10, 11, 12 } }
            },
            SecondMoment = new double[,,]
            {
                { { 13, 14, 15 }, { 16, 17, 18 } },
                { { 19, 20, 21 }, { 22, 23, 24 } }
            },
            FractionalMT = new double[,,,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1 x NumRegions x FractionalMTBins.Count+1: 2x2x3x2x2
            {
                {
                    {
                        { { 1, 2 }, { 3, 4 }, { 5, 6 } },
                        { { 7, 8 }, { 9, 10 }, { 11, 12 } }
                    },
                    {
                        { { 13, 14 }, { 15, 16 }, { 17, 18 } },
                        { { 19, 20 }, { 21, 22 }, { 23, 24 } }
                    }

                },
                {
                    {
                        { { 25, 26 }, { 27, 28 }, { 29, 30 } },
                        { { 31, 32 }, { 33, 34 }, { 35, 36 } },
                    },
                    {
                        { { 37, 38 }, { 39, 40 }, { 41, 42 } },
                        { { 43, 44 }, { 45, 46 }, { 47, 48 } },
                    }
                }
            },

        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
            detector.FractionalMT);

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

        Assert.AreEqual(1, detector.FractionalMT[0, 0, 0, 0, 0]);
        Assert.AreEqual(2, detector.FractionalMT[0, 0, 0, 0, 1]);
        Assert.AreEqual(3, detector.FractionalMT[0, 0, 0, 1, 0]);
        Assert.AreEqual(4, detector.FractionalMT[0, 0, 0, 1, 1]);
        Assert.AreEqual(5, detector.FractionalMT[0, 0, 0, 2, 0]);
        Assert.AreEqual(6, detector.FractionalMT[0, 0, 0, 2, 1]);
        Assert.AreEqual(7, detector.FractionalMT[0, 0, 1, 0, 0]);
        Assert.AreEqual(8, detector.FractionalMT[0, 0, 1, 0, 1]);
        Assert.AreEqual(9, detector.FractionalMT[0, 0, 1, 1, 0]);
        Assert.AreEqual(10, detector.FractionalMT[0, 0, 1, 1, 1]);
        Assert.AreEqual(11, detector.FractionalMT[0, 0, 1, 2, 0]);
        Assert.AreEqual(12, detector.FractionalMT[0, 0, 1, 2, 1]);
        Assert.AreEqual(13, detector.FractionalMT[0, 1, 0, 0, 0]);
        Assert.AreEqual(14, detector.FractionalMT[0, 1, 0, 0, 1]);
        Assert.AreEqual(15, detector.FractionalMT[0, 1, 0, 1, 0]);
        Assert.AreEqual(16, detector.FractionalMT[0, 1, 0, 1, 1]);
        Assert.AreEqual(17, detector.FractionalMT[0, 1, 0, 2, 0]);
        Assert.AreEqual(18, detector.FractionalMT[0, 1, 0, 2, 1]);
        Assert.AreEqual(19, detector.FractionalMT[0, 1, 1, 0, 0]);
        Assert.AreEqual(20, detector.FractionalMT[0, 1, 1, 0, 1]);
        Assert.AreEqual(21, detector.FractionalMT[0, 1, 1, 1, 0]);
        Assert.AreEqual(22, detector.FractionalMT[0, 1, 1, 1, 1]);
        Assert.AreEqual(23, detector.FractionalMT[0, 1, 1, 2, 0]);
        Assert.AreEqual(24, detector.FractionalMT[0, 1, 1, 2, 1]);
        Assert.AreEqual(25, detector.FractionalMT[1, 0, 0, 0, 0]);
        Assert.AreEqual(26, detector.FractionalMT[1, 0, 0, 0, 1]);
        Assert.AreEqual(27, detector.FractionalMT[1, 0, 0, 1, 0]);
        Assert.AreEqual(28, detector.FractionalMT[1, 0, 0, 1, 1]);
        Assert.AreEqual(29, detector.FractionalMT[1, 0, 0, 2, 0]);
        Assert.AreEqual(30, detector.FractionalMT[1, 0, 0, 2, 1]);
        Assert.AreEqual(31, detector.FractionalMT[1, 0, 1, 0, 0]);
        Assert.AreEqual(32, detector.FractionalMT[1, 0, 1, 0, 1]);
        Assert.AreEqual(33, detector.FractionalMT[1, 0, 1, 1, 0]);
        Assert.AreEqual(34, detector.FractionalMT[1, 0, 1, 1, 1]);
        Assert.AreEqual(35, detector.FractionalMT[1, 0, 1, 2, 0]);
        Assert.AreEqual(36, detector.FractionalMT[1, 0, 1, 2, 1]);
        Assert.AreEqual(37, detector.FractionalMT[1, 1, 0, 0, 0]);
        Assert.AreEqual(38, detector.FractionalMT[1, 1, 0, 0, 1]);
        Assert.AreEqual(39, detector.FractionalMT[1, 1, 0, 1, 0]);
        Assert.AreEqual(40, detector.FractionalMT[1, 1, 0, 1, 1]);
        Assert.AreEqual(41, detector.FractionalMT[1, 1, 0, 2, 0]);
        Assert.AreEqual(42, detector.FractionalMT[1, 1, 0, 2, 1]);
        Assert.AreEqual(43, detector.FractionalMT[1, 1, 1, 0, 0]);
        Assert.AreEqual(44, detector.FractionalMT[1, 1, 1, 0, 1]);
        Assert.AreEqual(45, detector.FractionalMT[1, 1, 1, 1, 0]);
        Assert.AreEqual(46, detector.FractionalMT[1, 1, 1, 1, 1]);
        Assert.AreEqual(47, detector.FractionalMT[1, 1, 1, 2, 0]);
        Assert.AreEqual(48, detector.FractionalMT[1, 1, 1, 2, 1]);

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