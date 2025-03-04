using NUnit.Framework;
using System.IO;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class ReflectedDynamicMTOfXAndYAndSubregionHistDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testreflecteddynamicmtofxandyandsubregionhist");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testreflecteddynamicmtofxandyandsubregionhist";
        var detector = new ReflectedDynamicMTOfXAndYAndSubregionHistDetector
        {
            X = new DoubleRange(-10, 10, 3),
            Y = new DoubleRange(-10, 10, 3),
            MTBins = new DoubleRange(0, 100, 4),
            Z = new DoubleRange(0, 100, 4),
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
            TotalMTOfZ = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
            {
                { { 25, 26, 27 }, { 28, 29, 30 } },
                { { 31, 32, 33 }, { 34, 35, 36 } }
            },
            TotalMTOfZSecondMoment = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
            {
                { { 37, 38, 39 }, { 40, 41, 42 } },
                { { 43, 44, 45 }, { 46, 47, 48 } }
            },
            DynamicMTOfZ = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
            {
                { { 49, 50, 51 }, { 52, 53, 54 } },
                { { 55, 56, 57 }, { 58, 59, 60 } }
            },
            DynamicMTOfZSecondMoment = new double[,,] // X.Count-1 x Y.Count-1 x Z.Count-1: 2x2x3
            {
                { { 61, 62, 63 }, { 64, 65, 66 } },
                { { 67, 68, 69 }, { 70, 71, 72 } }
            },
            FractionalMT = new double[,,,] // X.Count-1 x Y.Count-1 x MTBins.Count-1, FractionalMTBins.Count+1: 2x2x3x2
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
            SubregionCollisions = new double[,] // NumSubregions x 2: 3x2
                { { 25, 26 }, { 27, 28 }, { 29, 30 } }, // 2nd index: 0=static, 1=dynamic
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
            detector.TotalMTOfZ, detector.TotalMTOfZSecondMoment, detector.DynamicMTOfZ, detector.DynamicMTOfZSecondMoment,
            detector.FractionalMT, detector.SubregionCollisions);

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

        Assert.That(detector.TotalMTOfZ[0, 0, 0], Is.EqualTo(25));
        Assert.That(detector.TotalMTOfZ[0, 0, 1], Is.EqualTo(26));
        Assert.That(detector.TotalMTOfZ[0, 0, 2], Is.EqualTo(27));
        Assert.That(detector.TotalMTOfZ[0, 1, 0], Is.EqualTo(28));
        Assert.That(detector.TotalMTOfZ[0, 1, 1], Is.EqualTo(29));
        Assert.That(detector.TotalMTOfZ[0, 1, 2], Is.EqualTo(30));
        Assert.That(detector.TotalMTOfZ[1, 0, 0], Is.EqualTo(31));
        Assert.That(detector.TotalMTOfZ[1, 0, 1], Is.EqualTo(32));
        Assert.That(detector.TotalMTOfZ[1, 0, 2], Is.EqualTo(33));
        Assert.That(detector.TotalMTOfZ[1, 1, 0], Is.EqualTo(34));
        Assert.That(detector.TotalMTOfZ[1, 1, 1], Is.EqualTo(35));
        Assert.That(detector.TotalMTOfZ[1, 1, 2], Is.EqualTo(36));

        Assert.That(detector.DynamicMTOfZ[0, 0, 0], Is.EqualTo(49));
        Assert.That(detector.DynamicMTOfZ[0, 0, 1], Is.EqualTo(50));
        Assert.That(detector.DynamicMTOfZ[0, 0, 2], Is.EqualTo(51));
        Assert.That(detector.DynamicMTOfZ[0, 1, 0], Is.EqualTo(52));
        Assert.That(detector.DynamicMTOfZ[0, 1, 1], Is.EqualTo(53));
        Assert.That(detector.DynamicMTOfZ[0, 1, 2], Is.EqualTo(54));
        Assert.That(detector.DynamicMTOfZ[1, 0, 0], Is.EqualTo(55));
        Assert.That(detector.DynamicMTOfZ[1, 0, 1], Is.EqualTo(56));
        Assert.That(detector.DynamicMTOfZ[1, 0, 2], Is.EqualTo(57));
        Assert.That(detector.DynamicMTOfZ[1, 1, 0], Is.EqualTo(58));
        Assert.That(detector.DynamicMTOfZ[1, 1, 1], Is.EqualTo(59));
        Assert.That(detector.DynamicMTOfZ[1, 1, 2], Is.EqualTo(60));

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
        Assert.That(detector.FractionalMT[1, 0, 0, 0], Is.EqualTo(13));
        Assert.That(detector.FractionalMT[1, 0, 0, 1], Is.EqualTo(14));
        Assert.That(detector.FractionalMT[1, 0, 1, 0], Is.EqualTo(15));
        Assert.That(detector.FractionalMT[1, 0, 1, 1], Is.EqualTo(16));
        Assert.That(detector.FractionalMT[1, 0, 2, 0], Is.EqualTo(17));
        Assert.That(detector.FractionalMT[1, 0, 2, 1], Is.EqualTo(18));
        Assert.That(detector.FractionalMT[1, 1, 0, 0], Is.EqualTo(19));
        Assert.That(detector.FractionalMT[1, 1, 0, 1], Is.EqualTo(20));
        Assert.That(detector.FractionalMT[1, 1, 1, 0], Is.EqualTo(21));
        Assert.That(detector.FractionalMT[1, 1, 1, 1], Is.EqualTo(22));
        Assert.That(detector.FractionalMT[1, 1, 2, 0], Is.EqualTo(23));
        Assert.That(detector.FractionalMT[1, 1, 2, 1], Is.EqualTo(24));

        Assert.That(detector.SubregionCollisions[0, 0], Is.EqualTo(25));
        Assert.That(detector.SubregionCollisions[0, 1], Is.EqualTo(26));
        Assert.That(detector.SubregionCollisions[1, 0], Is.EqualTo(27));
        Assert.That(detector.SubregionCollisions[1, 1], Is.EqualTo(28));
        Assert.That(detector.SubregionCollisions[2, 0], Is.EqualTo(29));
        Assert.That(detector.SubregionCollisions[2, 1], Is.EqualTo(30));

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
        Assert.That(detector.TotalMTOfZSecondMoment[0, 0, 0], Is.EqualTo(37));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 0, 1], Is.EqualTo(38));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 0, 2], Is.EqualTo(39));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 1, 0], Is.EqualTo(40));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 1, 1], Is.EqualTo(41));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 1, 2], Is.EqualTo(42));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 0, 0], Is.EqualTo(43));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 0, 1], Is.EqualTo(44));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 0, 2], Is.EqualTo(45));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 1, 0], Is.EqualTo(46));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 1, 1], Is.EqualTo(47));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 1, 2], Is.EqualTo(48));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 0, 0], Is.EqualTo(61));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 0, 1], Is.EqualTo(62));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 0, 2], Is.EqualTo(63));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 1, 0], Is.EqualTo(64));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 1, 1], Is.EqualTo(65));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 1, 2], Is.EqualTo(66));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 0, 0], Is.EqualTo(67));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 0, 1], Is.EqualTo(68));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 0, 2], Is.EqualTo(69));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 1, 0], Is.EqualTo(70));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 1, 1], Is.EqualTo(71));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 1, 2], Is.EqualTo(72));
    }
}