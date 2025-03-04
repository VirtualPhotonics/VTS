using NUnit.Framework;
using System.IO;
using System.Numerics;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class ROfFxAndMaxDepthDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testroffxandmaxdepth");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testroffxandMaxDepth";
        var detector = new ROfFxAndMaxDepthDetector
        {
            Fx = new DoubleRange(0, 10, 3),
            MaxDepth = new DoubleRange(0, 1, 4),
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new[,]
            {
                { 1 + 1 * Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
                { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne }
            },
            SecondMoment = new[,]
            {
                { 7 + 7 * Complex.ImaginaryOne, 8 + 8 * Complex.ImaginaryOne, 9 + 9 * Complex.ImaginaryOne },
                { 10 + 10 * Complex.ImaginaryOne, 11 + 11 * Complex.ImaginaryOne, 12 + 12 * Complex.ImaginaryOne }
            }
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

        Assert.That(detector.Mean[0, 0], Is.EqualTo(1 + 1 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[0, 1], Is.EqualTo(2 + 2 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[0, 2], Is.EqualTo(3 + 3 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[1, 0], Is.EqualTo(4 + 4 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[1, 1], Is.EqualTo(5 + 5 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[1, 2], Is.EqualTo(6 + 6 * Complex.ImaginaryOne));
        if (!tallySecondMoment) return;
        Assert.That(detector.SecondMoment[0, 0], Is.EqualTo(7 + 7 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[0, 1], Is.EqualTo(8 + 8 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[0, 2], Is.EqualTo(9 + 9 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[1, 0], Is.EqualTo(10 + 10 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[1, 1], Is.EqualTo(11 + 11 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[1, 2], Is.EqualTo(12 + 12 * Complex.ImaginaryOne));
    }
}