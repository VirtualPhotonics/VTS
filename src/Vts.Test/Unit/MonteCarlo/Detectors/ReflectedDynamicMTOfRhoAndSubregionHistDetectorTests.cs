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

        Assert.That(detector.Mean[0, 0], Is.EqualTo(1));
        Assert.That(detector.Mean[0, 1], Is.EqualTo(2));
        Assert.That(detector.Mean[0, 2], Is.EqualTo(3));
        Assert.That(detector.Mean[1, 0], Is.EqualTo(4));
        Assert.That(detector.Mean[1, 1], Is.EqualTo(5));
        Assert.That(detector.Mean[1, 2], Is.EqualTo(6));
        Assert.That(detector.TotalMTOfZ[0, 0], Is.EqualTo(13));
        Assert.That(detector.TotalMTOfZ[0, 1], Is.EqualTo(14));
        Assert.That(detector.TotalMTOfZ[0, 2], Is.EqualTo(15));
        Assert.That(detector.TotalMTOfZ[1, 0], Is.EqualTo(16));
        Assert.That(detector.TotalMTOfZ[1, 1], Is.EqualTo(17));
        Assert.That(detector.TotalMTOfZ[1, 2], Is.EqualTo(18));
        Assert.That(detector.DynamicMTOfZ[0, 0], Is.EqualTo(25));
        Assert.That(detector.DynamicMTOfZ[0, 1], Is.EqualTo(26));
        Assert.That(detector.DynamicMTOfZ[0, 2], Is.EqualTo(27));
        Assert.That(detector.DynamicMTOfZ[1, 0], Is.EqualTo(28));
        Assert.That(detector.DynamicMTOfZ[1, 1], Is.EqualTo(29));
        Assert.That(detector.DynamicMTOfZ[1, 2], Is.EqualTo(30));
        Assert.That(detector.FractionalMT[0, 0, 0], Is.EqualTo(37));
        Assert.That(detector.FractionalMT[0, 0, 1], Is.EqualTo(38));
        Assert.That(detector.FractionalMT[0, 1, 0], Is.EqualTo(39));
        Assert.That(detector.FractionalMT[0, 1, 1], Is.EqualTo(40));
        Assert.That(detector.FractionalMT[0, 2, 0], Is.EqualTo(41));
        Assert.That(detector.FractionalMT[0, 2, 1], Is.EqualTo(42));
        Assert.That(detector.FractionalMT[1, 0, 0], Is.EqualTo(43));
        Assert.That(detector.FractionalMT[1, 0, 1], Is.EqualTo(44));
        Assert.That(detector.FractionalMT[1, 1, 0], Is.EqualTo(45));
        Assert.That(detector.FractionalMT[1, 1, 1], Is.EqualTo(46));
        Assert.That(detector.FractionalMT[1, 2, 0], Is.EqualTo(47));
        Assert.That(detector.FractionalMT[1, 2, 1], Is.EqualTo(48));
        Assert.That(detector.SubregionCollisions[0, 0], Is.EqualTo(49));
        Assert.That(detector.SubregionCollisions[0, 1], Is.EqualTo(50));
        Assert.That(detector.SubregionCollisions[1, 0], Is.EqualTo(51));
        Assert.That(detector.SubregionCollisions[1, 1], Is.EqualTo(52));
        Assert.That(detector.SubregionCollisions[2, 0], Is.EqualTo(53));
        Assert.That(detector.SubregionCollisions[2, 1], Is.EqualTo(54));
        if (!tallySecondMoment) return;
        Assert.That(detector.SecondMoment[0, 0], Is.EqualTo(7));
        Assert.That(detector.SecondMoment[0, 1], Is.EqualTo(8));
        Assert.That(detector.SecondMoment[0, 2], Is.EqualTo(9));
        Assert.That(detector.SecondMoment[1, 0], Is.EqualTo(10));
        Assert.That(detector.SecondMoment[1, 1], Is.EqualTo(11));
        Assert.That(detector.SecondMoment[1, 2], Is.EqualTo(12));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 0], Is.EqualTo(19));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 1], Is.EqualTo(20));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 2], Is.EqualTo(21));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 0], Is.EqualTo(22));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 1], Is.EqualTo(23));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 2], Is.EqualTo(24));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 0], Is.EqualTo(31));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 1], Is.EqualTo(32));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 2], Is.EqualTo(33));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 0], Is.EqualTo(34));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 1], Is.EqualTo(35));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 2], Is.EqualTo(36));
    }
}