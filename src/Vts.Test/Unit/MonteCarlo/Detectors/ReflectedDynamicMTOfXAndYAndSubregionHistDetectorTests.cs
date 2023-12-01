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

        Assert.AreEqual(25, detector.TotalMTOfZ[0, 0, 0]);
        Assert.AreEqual(26, detector.TotalMTOfZ[0, 0, 1]);
        Assert.AreEqual(27, detector.TotalMTOfZ[0, 0, 2]);
        Assert.AreEqual(28, detector.TotalMTOfZ[0, 1, 0]);
        Assert.AreEqual(29, detector.TotalMTOfZ[0, 1, 1]);
        Assert.AreEqual(30, detector.TotalMTOfZ[0, 1, 2]);
        Assert.AreEqual(31, detector.TotalMTOfZ[1, 0, 0]);
        Assert.AreEqual(32, detector.TotalMTOfZ[1, 0, 1]);
        Assert.AreEqual(33, detector.TotalMTOfZ[1, 0, 2]);
        Assert.AreEqual(34, detector.TotalMTOfZ[1, 1, 0]);
        Assert.AreEqual(35, detector.TotalMTOfZ[1, 1, 1]);
        Assert.AreEqual(36, detector.TotalMTOfZ[1, 1, 2]);

        Assert.AreEqual(49, detector.DynamicMTOfZ[0, 0, 0]);
        Assert.AreEqual(50, detector.DynamicMTOfZ[0, 0, 1]);
        Assert.AreEqual(51, detector.DynamicMTOfZ[0, 0, 2]);
        Assert.AreEqual(52, detector.DynamicMTOfZ[0, 1, 0]);
        Assert.AreEqual(53, detector.DynamicMTOfZ[0, 1, 1]);
        Assert.AreEqual(54, detector.DynamicMTOfZ[0, 1, 2]);
        Assert.AreEqual(55, detector.DynamicMTOfZ[1, 0, 0]);
        Assert.AreEqual(56, detector.DynamicMTOfZ[1, 0, 1]);
        Assert.AreEqual(57, detector.DynamicMTOfZ[1, 0, 2]);
        Assert.AreEqual(58, detector.DynamicMTOfZ[1, 1, 0]);
        Assert.AreEqual(59, detector.DynamicMTOfZ[1, 1, 1]);
        Assert.AreEqual(60, detector.DynamicMTOfZ[1, 1, 2]);

        Assert.AreEqual(1, detector.FractionalMT[0, 0, 0, 0]);
        Assert.AreEqual(2, detector.FractionalMT[0, 0, 0, 1]);
        Assert.AreEqual(3, detector.FractionalMT[0, 0, 1, 0]);
        Assert.AreEqual(4, detector.FractionalMT[0, 0, 1, 1]);
        Assert.AreEqual(5, detector.FractionalMT[0, 0, 2, 0]);
        Assert.AreEqual(6, detector.FractionalMT[0, 0, 2, 1]);
        Assert.AreEqual(7, detector.FractionalMT[0, 1, 0, 0]);
        Assert.AreEqual(8, detector.FractionalMT[0, 1, 0, 1]);
        Assert.AreEqual(9, detector.FractionalMT[0, 1, 1, 0]);
        Assert.AreEqual(10, detector.FractionalMT[0, 1, 1, 1]);
        Assert.AreEqual(11, detector.FractionalMT[0, 1, 2, 0]);
        Assert.AreEqual(12, detector.FractionalMT[0, 1, 2, 1]);
        Assert.AreEqual(13, detector.FractionalMT[1, 0, 0, 0]);
        Assert.AreEqual(14, detector.FractionalMT[1, 0, 0, 1]);
        Assert.AreEqual(15, detector.FractionalMT[1, 0, 1, 0]);
        Assert.AreEqual(16, detector.FractionalMT[1, 0, 1, 1]);
        Assert.AreEqual(17, detector.FractionalMT[1, 0, 2, 0]);
        Assert.AreEqual(18, detector.FractionalMT[1, 0, 2, 1]);
        Assert.AreEqual(19, detector.FractionalMT[1, 1, 0, 0]);
        Assert.AreEqual(20, detector.FractionalMT[1, 1, 0, 1]);
        Assert.AreEqual(21, detector.FractionalMT[1, 1, 1, 0]);
        Assert.AreEqual(22, detector.FractionalMT[1, 1, 1, 1]);
        Assert.AreEqual(23, detector.FractionalMT[1, 1, 2, 0]);
        Assert.AreEqual(24, detector.FractionalMT[1, 1, 2, 1]);

        Assert.AreEqual(25, detector.SubregionCollisions[0, 0]);
        Assert.AreEqual(26, detector.SubregionCollisions[0, 1]);
        Assert.AreEqual(27, detector.SubregionCollisions[1, 0]);
        Assert.AreEqual(28, detector.SubregionCollisions[1, 1]);
        Assert.AreEqual(29, detector.SubregionCollisions[2, 0]);
        Assert.AreEqual(30, detector.SubregionCollisions[2, 1]);

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
        Assert.AreEqual(37, detector.TotalMTOfZSecondMoment[0, 0, 0]);
        Assert.AreEqual(38, detector.TotalMTOfZSecondMoment[0, 0, 1]);
        Assert.AreEqual(39, detector.TotalMTOfZSecondMoment[0, 0, 2]);
        Assert.AreEqual(40, detector.TotalMTOfZSecondMoment[0, 1, 0]);
        Assert.AreEqual(41, detector.TotalMTOfZSecondMoment[0, 1, 1]);
        Assert.AreEqual(42, detector.TotalMTOfZSecondMoment[0, 1, 2]);
        Assert.AreEqual(43, detector.TotalMTOfZSecondMoment[1, 0, 0]);
        Assert.AreEqual(44, detector.TotalMTOfZSecondMoment[1, 0, 1]);
        Assert.AreEqual(45, detector.TotalMTOfZSecondMoment[1, 0, 2]);
        Assert.AreEqual(46, detector.TotalMTOfZSecondMoment[1, 1, 0]);
        Assert.AreEqual(47, detector.TotalMTOfZSecondMoment[1, 1, 1]);
        Assert.AreEqual(48, detector.TotalMTOfZSecondMoment[1, 1, 2]);
        Assert.AreEqual(61, detector.DynamicMTOfZSecondMoment[0, 0, 0]);
        Assert.AreEqual(62, detector.DynamicMTOfZSecondMoment[0, 0, 1]);
        Assert.AreEqual(63, detector.DynamicMTOfZSecondMoment[0, 0, 2]);
        Assert.AreEqual(64, detector.DynamicMTOfZSecondMoment[0, 1, 0]);
        Assert.AreEqual(65, detector.DynamicMTOfZSecondMoment[0, 1, 1]);
        Assert.AreEqual(66, detector.DynamicMTOfZSecondMoment[0, 1, 2]);
        Assert.AreEqual(67, detector.DynamicMTOfZSecondMoment[1, 0, 0]);
        Assert.AreEqual(68, detector.DynamicMTOfZSecondMoment[1, 0, 1]);
        Assert.AreEqual(69, detector.DynamicMTOfZSecondMoment[1, 0, 2]);
        Assert.AreEqual(70, detector.DynamicMTOfZSecondMoment[1, 1, 0]);
        Assert.AreEqual(71, detector.DynamicMTOfZSecondMoment[1, 1, 1]);
        Assert.AreEqual(72, detector.DynamicMTOfZSecondMoment[1, 1, 2]);
    }
}