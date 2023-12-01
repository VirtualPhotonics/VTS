using NUnit.Framework;
using System.IO;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class ReflectedDynamicMTOfRhoAndSubregionHistDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testreflecteddynamicmtofrhoandsubregionhist");
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
        const string detectorName = "testreflecteddynamicmtofrhoandsubregionhist";
        var detector = new ReflectedDynamicMTOfRhoAndSubregionHistDetector
        {
            Rho = new DoubleRange(0, 10, 3),
            MTBins = new DoubleRange(0, 10, 4),
            Z = new DoubleRange(0, 10, 4),
            FractionalMTBins = new DoubleRange(0, 1, 1),
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }, // Rho.Count-1 x MTBins.Count-1: 2x3
            SecondMoment = new double[,] { { 7, 8, 9 }, { 10, 11, 12 } },
            TotalMTOfZ = new double[,] { { 13, 14, 15 }, { 16, 17, 18 } }, // Rho.Count-1 x Z.Count-1: 2x3
            TotalMTOfZSecondMoment = new double[,] { { 19, 20, 21 }, { 22, 23, 24 } },
            DynamicMTOfZ = new double[,] { { 25, 26, 27 }, { 28, 29, 30 } }, // Rho.Count-1 x Z.Count-1: 2x3
            DynamicMTOfZSecondMoment = new double[,] { { 31, 32, 33 }, { 34, 35, 36 } },
            // FractionalMT has dimensions Rho.Count-1, MTBins.Count-1, FractionalMTBins.Count+1=2x3x2
            FractionalMT = new double[,,]
                { { { 37, 38 }, { 39, 40 }, { 41, 42 } }, { { 43, 44 }, { 45, 46 }, { 47, 48 } } },
            SubregionCollisions = new double[,]
                { { 49, 50 }, { 51, 52 }, { 53, 54 } }, // numsubregions x 2nd index: 0=static, 1=dynamic: 3x2
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
            detector.TotalMTOfZ, detector.TotalMTOfZSecondMoment, detector.DynamicMTOfZ,
            detector.DynamicMTOfZSecondMoment,
            detector.FractionalMT, detector.SubregionCollisions);

        Assert.AreEqual(1, detector.Mean[0, 0]);
        Assert.AreEqual(2, detector.Mean[0, 1]);
        Assert.AreEqual(3, detector.Mean[0, 2]);
        Assert.AreEqual(4, detector.Mean[1, 0]);
        Assert.AreEqual(5, detector.Mean[1, 1]);
        Assert.AreEqual(6, detector.Mean[1, 2]);
        Assert.AreEqual(13, detector.TotalMTOfZ[0, 0]);
        Assert.AreEqual(14, detector.TotalMTOfZ[0, 1]);
        Assert.AreEqual(15, detector.TotalMTOfZ[0, 2]);
        Assert.AreEqual(16, detector.TotalMTOfZ[1, 0]);
        Assert.AreEqual(17, detector.TotalMTOfZ[1, 1]);
        Assert.AreEqual(18, detector.TotalMTOfZ[1, 2]);
        Assert.AreEqual(25, detector.DynamicMTOfZ[0, 0]);
        Assert.AreEqual(26, detector.DynamicMTOfZ[0, 1]);
        Assert.AreEqual(27, detector.DynamicMTOfZ[0, 2]);
        Assert.AreEqual(28, detector.DynamicMTOfZ[1, 0]);
        Assert.AreEqual(29, detector.DynamicMTOfZ[1, 1]);
        Assert.AreEqual(30, detector.DynamicMTOfZ[1, 2]);
        Assert.AreEqual(37, detector.FractionalMT[0, 0, 0]);
        Assert.AreEqual(38, detector.FractionalMT[0, 0, 1]);
        Assert.AreEqual(39, detector.FractionalMT[0, 1, 0]);
        Assert.AreEqual(40, detector.FractionalMT[0, 1, 1]);
        Assert.AreEqual(41, detector.FractionalMT[0, 2, 0]);
        Assert.AreEqual(42, detector.FractionalMT[0, 2, 1]);
        Assert.AreEqual(43, detector.FractionalMT[1, 0, 0]);
        Assert.AreEqual(44, detector.FractionalMT[1, 0, 1]);
        Assert.AreEqual(45, detector.FractionalMT[1, 1, 0]);
        Assert.AreEqual(46, detector.FractionalMT[1, 1, 1]);
        Assert.AreEqual(47, detector.FractionalMT[1, 2, 0]);
        Assert.AreEqual(48, detector.FractionalMT[1, 2, 1]);
        Assert.AreEqual(49, detector.SubregionCollisions[0, 0]);
        Assert.AreEqual(50, detector.SubregionCollisions[0, 1]);
        Assert.AreEqual(51, detector.SubregionCollisions[1, 0]);
        Assert.AreEqual(52, detector.SubregionCollisions[1, 1]);
        Assert.AreEqual(53, detector.SubregionCollisions[2, 0]);
        Assert.AreEqual(54, detector.SubregionCollisions[2, 1]);
        if (!tallySecondMoment) return;
        Assert.AreEqual(7, detector.SecondMoment[0, 0]);
        Assert.AreEqual(8, detector.SecondMoment[0, 1]);
        Assert.AreEqual(9, detector.SecondMoment[0, 2]);
        Assert.AreEqual(10, detector.SecondMoment[1, 0]);
        Assert.AreEqual(11, detector.SecondMoment[1, 1]);
        Assert.AreEqual(12, detector.SecondMoment[1, 2]);
        Assert.AreEqual(19, detector.TotalMTOfZSecondMoment[0, 0]);
        Assert.AreEqual(20, detector.TotalMTOfZSecondMoment[0, 1]);
        Assert.AreEqual(21, detector.TotalMTOfZSecondMoment[0, 2]);
        Assert.AreEqual(22, detector.TotalMTOfZSecondMoment[1, 0]);
        Assert.AreEqual(23, detector.TotalMTOfZSecondMoment[1, 1]);
        Assert.AreEqual(24, detector.TotalMTOfZSecondMoment[1, 2]);
        Assert.AreEqual(31, detector.DynamicMTOfZSecondMoment[0, 0]);
        Assert.AreEqual(32, detector.DynamicMTOfZSecondMoment[0, 1]);
        Assert.AreEqual(33, detector.DynamicMTOfZSecondMoment[0, 2]);
        Assert.AreEqual(34, detector.DynamicMTOfZSecondMoment[1, 0]);
        Assert.AreEqual(35, detector.DynamicMTOfZSecondMoment[1, 1]);
        Assert.AreEqual(36, detector.DynamicMTOfZSecondMoment[1, 2]);
    }
}