using NUnit.Framework;
using System.IO;
using System.Numerics;
using System.Reflection;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class TOfFxDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testtoffx");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testtoffx";
        var detector = new TOfFxDetector
        {
            Fx = new DoubleRange(0, 10, 3),
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new[]
                { 1 + 1 * Complex.ImaginaryOne, 2 + 2 * Complex.ImaginaryOne, 3 + 3 * Complex.ImaginaryOne },
            SecondMoment = new[]
                { 4 + 4 * Complex.ImaginaryOne, 5 + 5 * Complex.ImaginaryOne, 6 + 6 * Complex.ImaginaryOne },
        };

        DetectorBinarySerializationHelper.WriteClearAndReReadArrays(detector, detector.Mean, detector.SecondMoment);

        Assert.That(detector.Mean[0], Is.EqualTo(1 + 1 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[1], Is.EqualTo(2 + 2 * Complex.ImaginaryOne));
        Assert.That(detector.Mean[2], Is.EqualTo(3 + 3 * Complex.ImaginaryOne));
        if (!tallySecondMoment) return;
        Assert.That(detector.SecondMoment[0], Is.EqualTo(4 + 4 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[1], Is.EqualTo(5 + 5 * Complex.ImaginaryOne));
        Assert.That(detector.SecondMoment[2], Is.EqualTo(6 + 6 * Complex.ImaginaryOne));
    }
}