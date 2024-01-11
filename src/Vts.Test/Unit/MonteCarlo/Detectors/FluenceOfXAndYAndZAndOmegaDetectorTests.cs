using NUnit.Framework;
using System.IO;
using System.Numerics;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class FluenceOfXAndYAndZAndOmegaDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testfluenceofxandyandzandomega");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testfluenceofxandyandzandOmega";
        var detector = new FluenceOfXAndYAndZAndOmegaDetector
        {
            X = new DoubleRange(-10, 10, 3),
            Y = new DoubleRange(-10, 10, 3),
            Z = new DoubleRange(0, 1, 3),
            Omega = new DoubleRange(0, 1, 3),
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new[, , ,] // X.Count-1 x Y.Count-1 x Z.Count-1 x Omega.Count: 2x2x2x3
            {
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
                {
                    {
                        { 13 + 13 * Complex.ImaginaryOne, 14 + 14 * Complex.ImaginaryOne, 15 + 15 * Complex.ImaginaryOne},
                        { 16 + 16 * Complex.ImaginaryOne, 17 + 17 * Complex.ImaginaryOne, 18 + 18 * Complex.ImaginaryOne }
                    },
                    {
                        { 19 + 19 * Complex.ImaginaryOne, 20 + 20 * Complex.ImaginaryOne, 21 + 21 * Complex.ImaginaryOne },
                        { 22 + 22 * Complex.ImaginaryOne, 23 + 23 * Complex.ImaginaryOne, 24 + 24 * Complex.ImaginaryOne }
                    }
                }
            },
            SecondMoment = new[, , ,]
            {
                {
                    {
                        { 25 + 25 * Complex.ImaginaryOne, 26 + 26 * Complex.ImaginaryOne, 27 + 27 * Complex.ImaginaryOne},
                        { 28 + 28 * Complex.ImaginaryOne, 29 + 29 * Complex.ImaginaryOne, 30 + 30 * Complex.ImaginaryOne}
                    },
                    {
                        { 31 + 31 * Complex.ImaginaryOne, 32 + 32 * Complex.ImaginaryOne, 33 + 33 * Complex.ImaginaryOne },
                        { 34 + 34 * Complex.ImaginaryOne, 35 + 35 * Complex.ImaginaryOne, 36 + 36 * Complex.ImaginaryOne }
                    }
                },
                {
                    {
                        { 37 + 37 * Complex.ImaginaryOne, 38 + 38 * Complex.ImaginaryOne, 39 + 39 * Complex.ImaginaryOne },
                        { 40 + 40 * Complex.ImaginaryOne, 41 + 41 * Complex.ImaginaryOne, 42 + 42 * Complex.ImaginaryOne }
                    },
                    {
                        { 43 + 43 * Complex.ImaginaryOne, 44 + 44 * Complex.ImaginaryOne, 45 + 45 * Complex.ImaginaryOne },
                        { 46 + 46 * Complex.ImaginaryOne, 47 + 47 * Complex.ImaginaryOne, 48 + 48 * Complex.ImaginaryOne }
                    }
                }
            }
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

        Assert.AreEqual(1 + Complex.ImaginaryOne, detector.Mean[0, 0, 0, 0]);
        Assert.AreEqual(2 + 2 * Complex.ImaginaryOne, detector.Mean[0, 0, 0, 1]);
        Assert.AreEqual(3 + 3 * Complex.ImaginaryOne, detector.Mean[0, 0, 0, 2]);
        Assert.AreEqual(4 + 4 * Complex.ImaginaryOne, detector.Mean[0, 0, 1, 0]);
        Assert.AreEqual(5 + 5 * Complex.ImaginaryOne, detector.Mean[0, 0, 1, 1]);
        Assert.AreEqual(6 + 6 * Complex.ImaginaryOne, detector.Mean[0, 0, 1, 2]);
        Assert.AreEqual(7 + 7 * Complex.ImaginaryOne, detector.Mean[0, 1, 0, 0]);
        Assert.AreEqual(8 + 8 * Complex.ImaginaryOne, detector.Mean[0, 1, 0, 1]);
        Assert.AreEqual(9 + 9 * Complex.ImaginaryOne, detector.Mean[0, 1, 0, 2]);
        Assert.AreEqual(10 + 10 * Complex.ImaginaryOne, detector.Mean[0, 1, 1, 0]);
        Assert.AreEqual(11 + 11 * Complex.ImaginaryOne, detector.Mean[0, 1, 1, 1]);
        Assert.AreEqual(12 + 12 * Complex.ImaginaryOne, detector.Mean[0, 1, 1, 2]);
        Assert.AreEqual(13 + 13 * Complex.ImaginaryOne, detector.Mean[1, 0, 0, 0]);
        Assert.AreEqual(14 + 14 * Complex.ImaginaryOne, detector.Mean[1, 0, 0, 1]);
        Assert.AreEqual(15 + 15 * Complex.ImaginaryOne, detector.Mean[1, 0, 0, 2]);
        Assert.AreEqual(16 + 16 * Complex.ImaginaryOne, detector.Mean[1, 0, 1, 0]);
        Assert.AreEqual(17 + 17 * Complex.ImaginaryOne, detector.Mean[1, 0, 1, 1]);
        Assert.AreEqual(18 + 18 * Complex.ImaginaryOne, detector.Mean[1, 0, 1, 2]);
        Assert.AreEqual(19 + 19 * Complex.ImaginaryOne, detector.Mean[1, 1, 0, 0]);
        Assert.AreEqual(20 + 20 * Complex.ImaginaryOne, detector.Mean[1, 1, 0, 1]);
        Assert.AreEqual(21 + 21 * Complex.ImaginaryOne, detector.Mean[1, 1, 0, 2]);
        Assert.AreEqual(22 + 22 * Complex.ImaginaryOne, detector.Mean[1, 1, 1, 0]);
        Assert.AreEqual(23 + 23 * Complex.ImaginaryOne, detector.Mean[1, 1, 1, 1]);
        Assert.AreEqual(24 + 24 * Complex.ImaginaryOne, detector.Mean[1, 1, 1, 2]);
        if (!tallySecondMoment) return;
        Assert.AreEqual(25 + 25 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 0, 0]);
        Assert.AreEqual(26 + 26 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 0, 1]);
        Assert.AreEqual(27 + 27 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 0, 2]);
        Assert.AreEqual(28 + 28 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 1, 0]);
        Assert.AreEqual(29 + 29 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 1, 1]);
        Assert.AreEqual(30 + 30 * Complex.ImaginaryOne, detector.SecondMoment[0, 0, 1, 2]);
        Assert.AreEqual(31 + 31 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 0, 0]);
        Assert.AreEqual(32 + 32 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 0, 1]);
        Assert.AreEqual(33 + 33 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 0, 2]);
        Assert.AreEqual(34 + 34 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 1, 0]);
        Assert.AreEqual(35 + 35 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 1, 1]);
        Assert.AreEqual(36 + 36 * Complex.ImaginaryOne, detector.SecondMoment[0, 1, 1, 2]);
        Assert.AreEqual(37 + 37 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 0, 0]);
        Assert.AreEqual(38 + 38 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 0, 1]);
        Assert.AreEqual(39 + 39 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 0, 2]);
        Assert.AreEqual(40 + 40 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 1, 0]);
        Assert.AreEqual(41 + 41 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 1, 1]);
        Assert.AreEqual(42 + 42 * Complex.ImaginaryOne, detector.SecondMoment[1, 0, 1, 2]);
        Assert.AreEqual(43 + 43 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 0, 0]);
        Assert.AreEqual(44 + 44 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 0, 1]);
        Assert.AreEqual(45 + 45 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 0, 2]);
        Assert.AreEqual(46 + 46 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 1, 0]);
        Assert.AreEqual(47 + 47 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 1, 1]);
        Assert.AreEqual(48 + 48 * Complex.ImaginaryOne, detector.SecondMoment[1, 1, 1, 2]);
    }
}