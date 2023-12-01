using NUnit.Framework;
using System.IO;
using System.Numerics;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class RadianceOfFxAndZAndAngleDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testradianceoffxandzandangle");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testradianceoffxandzandangle";
        var detector = new RadianceOfFxAndZAndAngleDetector
        {
            Fx = new DoubleRange(-0, 10, 2),
            Z = new DoubleRange(0, 10, 3),
            Angle = new DoubleRange(0, 1, 4),
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new[, ,] // Fx.Count x Z.Count-1 x Angle.Count-1: 2x2x3
            {
                {
                    { 1 + Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                    { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
                },
                {
                    { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                    { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
                }
            },
            SecondMoment = new[, ,]
            {
                {
                    { 13 + 13 * Complex.ImaginaryOne, 14 + 14 * Complex.ImaginaryOne, 15 + 15 * Complex.ImaginaryOne },
                    { 16 + 16 * Complex.ImaginaryOne, 17 + 17 * Complex.ImaginaryOne, 18 + 18 * Complex.ImaginaryOne }
                },
                {
                    { 19 + 19 * Complex.ImaginaryOne, 20 + 20 * Complex.ImaginaryOne, 21 + 21 * Complex.ImaginaryOne },
                    { 22 + 22 * Complex.ImaginaryOne, 23 + 23 * Complex.ImaginaryOne, 24 + 24 * Complex.ImaginaryOne }
                }
            }
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

        Assert.AreEqual(1 + Complex.ImaginaryOne, detector.Mean[0, 0, 0]);
        Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[0, 0, 1]);
        Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[0, 0, 2]);
        Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.Mean[0, 1, 0]);
        Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.Mean[0, 1, 1]);
        Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.Mean[0, 1, 2]);
        Assert.AreEqual(7 + 7 * Complex.ImaginaryOne, detector.Mean[1, 0, 0]);
        Assert.AreEqual(8 + 8 * Complex.ImaginaryOne, detector.Mean[1, 0, 1]);
        Assert.AreEqual(9 + 9 * Complex.ImaginaryOne, detector.Mean[1, 0, 2]);
        Assert.AreEqual(10 + 10 * Complex.ImaginaryOne, detector.Mean[1, 1, 0]);
        Assert.AreEqual(11 + 11 * Complex.ImaginaryOne, detector.Mean[1, 1, 1]);
        Assert.AreEqual(12 + 12 * Complex.ImaginaryOne, detector.Mean[1, 1, 2]);
        if (!tallySecondMoment) return;
        Assert.AreEqual(13 + 13 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 0]);
        Assert.AreEqual(14 + 14 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 1]);
        Assert.AreEqual(15 + 15 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 2]);
        Assert.AreEqual(16 + 16 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 0]);
        Assert.AreEqual(17 + 17 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 1]);
        Assert.AreEqual(18 + 18 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 2]);
        Assert.AreEqual(19 + 19 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 0]);
        Assert.AreEqual(20 + 20 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 1]);
        Assert.AreEqual(21 + 21 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 2]);
        Assert.AreEqual(22 + 22 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 0]);
        Assert.AreEqual(23 + 23 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 1]);
        Assert.AreEqual(24 + 24 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 2]);
    }
}