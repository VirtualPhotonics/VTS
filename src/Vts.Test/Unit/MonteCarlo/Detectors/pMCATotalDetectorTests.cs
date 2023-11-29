﻿using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.Unit.MonteCarlo.Detectors;

[TestFixture]
public class pMCATotalDetectorTests
{
    /// <summary>
    /// clear all test generated files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_previously_generated_files()
    {
        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FolderCleanup.DeleteFileContaining(currentPath, "testatotal");
    }

    /// <summary>
    /// Test to verify that GetBinarySerializers are working correctly for this detector.
    /// </summary>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Validate_deserialized_class_is_correct_when_using_GetBinarySerializers(bool tallySecondMoment)
    {
        const string detectorName = "testpmcatotal";
        var detector = new pMCATotalDetector
        {
            TallySecondMoment = tallySecondMoment,
            Name = detectorName,
            Mean = new double(),
            SecondMoment = new double()
        };

        var serializers = detector.GetBinarySerializers();
        Assert.AreEqual(Array.Empty<BinaryArraySerializer>(), serializers);
    }
}