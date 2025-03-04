using NUnit.Framework;
using System.IO;
using System.Numerics;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class ReflectedDynamicMTOfFxAndSubregionHistDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testreflecteddynamicmtoffxandsubregionhist");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void  Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testreflecteddynamicmtoffxandsubregionhist";
        var detector = new ReflectedDynamicMTOfFxAndSubregionHistDetector
        {
            Fx = new DoubleRange(0, 10, 2),
            MTBins = new DoubleRange(0, 10, 4),
            Z = new DoubleRange(0, 10, 4),
            FractionalMTBins = new DoubleRange(0, 1, 1),
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new[,] // Fx.Count x MTBins.Count-1: 2x3
            {
                { 1 + Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
            },
            SecondMoment = new[,]
            {
                { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
            },
            TotalMTOfZ = new[,] // Fx.Count x Z.Count-1: 2x3
            {
                { 13 + 13 * Complex.ImaginaryOne, 14 + 14 * Complex.ImaginaryOne, 15 + 15 * Complex.ImaginaryOne },
                { 16 + 16 * Complex.ImaginaryOne, 17 + 17 * Complex.ImaginaryOne, 18 + 18 * Complex.ImaginaryOne }
            },
            TotalMTOfZSecondMoment = new[,]
            {
                { 19 + 19 * Complex.ImaginaryOne, 20 + 20 * Complex.ImaginaryOne, 21 + 21 * Complex.ImaginaryOne },
                { 22 + 22 * Complex.ImaginaryOne, 23 + 23 * Complex.ImaginaryOne, 24 + 24 * Complex.ImaginaryOne },
            },
            DynamicMTOfZ = new[,] // Fx.Count x Z.Count-1: 2x3
            {
                { 25 + 25 * Complex.ImaginaryOne, 26 + 26 * Complex.ImaginaryOne, 27 + 27 * Complex.ImaginaryOne },
                { 28 + 28 * Complex.ImaginaryOne, 29 + 29 * Complex.ImaginaryOne, 30 + 30 * Complex.ImaginaryOne }
            },
            DynamicMTOfZSecondMoment = new[,]
            {
                { 31 + 31 * Complex.ImaginaryOne, 32 + 32 * Complex.ImaginaryOne, 33 + 33 * Complex.ImaginaryOne },
                { 34 + 34 * Complex.ImaginaryOne, 35 + 35 * Complex.ImaginaryOne, 36 + 36 * Complex.ImaginaryOne },
            },
            FractionalMT = new[, ,] // Fx.Count x MTBins.Count-1 x FractionalMTBins.Count+1=2x3x2
            {
                {
                    { 37 + 37 * Complex.ImaginaryOne, 38 + 38 * Complex.ImaginaryOne },
                    { 39 + 39 * Complex.ImaginaryOne, 40 + 40 * Complex.ImaginaryOne },
                    { 41 + 41 * Complex.ImaginaryOne, 42 + 42 * Complex.ImaginaryOne }
                },
                {
                    { 43 + 43 * Complex.ImaginaryOne, 44 + 44 * Complex.ImaginaryOne },
                    { 45 + 45 * Complex.ImaginaryOne, 46 + 46 * Complex.ImaginaryOne },
                    { 47 + 47 * Complex.ImaginaryOne, 48 + 48 * Complex.ImaginaryOne }
                }
            },
            SubregionCollisions = new double[,] // NumSubregions x 2: 3x2
                { { 49, 50 }, { 51, 52 }, { 53, 54 } }, // 2nd index: 0=static, 1=dynamic
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment,
            detector.TotalMTOfZ, detector.TotalMTOfZSecondMoment, detector.DynamicMTOfZ,
            detector.DynamicMTOfZSecondMoment,
            detector.FractionalMT, detector.SubregionCollisions);

        Assert.That(detector.Mean[0, 0], Is.EqualTo(1 + Complex.ImaginaryOne));
        Assert.That(detector.Mean[0, 1], Is.EqualTo(2 + 2 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[0, 2], Is.EqualTo(3 + 3 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[1, 0], Is.EqualTo(4 + 4 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[1, 1], Is.EqualTo(5 + 5 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[1, 2], Is.EqualTo(6 + 6 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZ[0, 0], Is.EqualTo(13 + 13 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZ[0, 1], Is.EqualTo(14 + 14 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZ[0, 2], Is.EqualTo(15 + 15 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZ[1, 0], Is.EqualTo(16 + 16 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZ[1, 1], Is.EqualTo(17 + 17 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZ[1, 2], Is.EqualTo(18 + 18 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZ[0, 0], Is.EqualTo(25 + 25 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZ[0, 1], Is.EqualTo(26 + 26 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZ[0, 2], Is.EqualTo(27 + 27 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZ[1, 0], Is.EqualTo(28 + 28 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZ[1, 1], Is.EqualTo(29 + 29 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZ[1, 2], Is.EqualTo(30 + 30 * Complex.ImaginaryOne)); 
        Assert.That(detector.FractionalMT[0, 0, 0], Is.EqualTo(37 + 37 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[0, 0, 1], Is.EqualTo(38 + 38 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[0, 1, 0], Is.EqualTo(39 + 39 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[0, 1, 1], Is.EqualTo(40 + 40 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[0, 2, 0], Is.EqualTo(41 + 41 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[0, 2, 1], Is.EqualTo(42 + 42 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[1, 0, 0], Is.EqualTo(43 + 43 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[1, 0, 1], Is.EqualTo(44 + 44 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[1, 1, 0], Is.EqualTo(45 + 45 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[1, 1, 1], Is.EqualTo(46 + 46 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[1, 2, 0], Is.EqualTo(47 + 47 * Complex.ImaginaryOne));
        Assert.That(detector.FractionalMT[1, 2, 1], Is.EqualTo(48 + 48 * Complex.ImaginaryOne));
        Assert.That(detector.SubregionCollisions[0, 0], Is.EqualTo(49));
        Assert.That(detector.SubregionCollisions[0, 1], Is.EqualTo(50));
        Assert.That(detector.SubregionCollisions[1, 0], Is.EqualTo(51));
        Assert.That(detector.SubregionCollisions[1, 1], Is.EqualTo(52));
        Assert.That(detector.SubregionCollisions[2, 0], Is.EqualTo(53));
        Assert.That(detector.SubregionCollisions[2, 1], Is.EqualTo(54));
        if (!tallySecondMoment) return;
        Assert.That(detector.SecondMoment[0, 0], Is.EqualTo(7 + 7 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[0, 1], Is.EqualTo(8 + 8 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[0, 2], Is.EqualTo(9 + 9 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[1, 0], Is.EqualTo(10 + 10 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[1, 1], Is.EqualTo(11 + 11 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[1, 2], Is.EqualTo(12 + 12 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 0], Is.EqualTo(19 + 19 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 1], Is.EqualTo(20 + 20 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZSecondMoment[0, 2], Is.EqualTo(21 + 21 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 0], Is.EqualTo(22 + 22 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 1], Is.EqualTo(23 + 23 * Complex.ImaginaryOne));
        Assert.That(detector.TotalMTOfZSecondMoment[1, 2], Is.EqualTo(24 + 24 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 0], Is.EqualTo(31 + 31 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 1], Is.EqualTo(32 + 32 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZSecondMoment[0, 2], Is.EqualTo(33 + 33 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 0], Is.EqualTo(34 + 34 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 1], Is.EqualTo(35 + 35 * Complex.ImaginaryOne));
        Assert.That(detector.DynamicMTOfZSecondMoment[1, 2], Is.EqualTo(36 + 36 * Complex.ImaginaryOne));
    }
}