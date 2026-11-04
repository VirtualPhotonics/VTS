using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class TDiffuseBoundingVolumeDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testTDiffuseboundingvolume");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_binary_arrays_are_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testtdiffuseboundingvolume";
        var detector = new TDiffuseBoundingVolumeDetector
        {
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = 0,
            SecondMoment = 0
        };

        var serializers = detector.GetBinarySerializers();
        Assert.That(serializers, Is.EqualTo(Array.Empty<BinaryArraySerializer>()));
    }
}