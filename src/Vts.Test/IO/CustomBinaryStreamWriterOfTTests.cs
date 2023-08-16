using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Vts.IO;

namespace Vts.Test.IO;

/// <summary>
/// Stream finder tests
/// </summary>
[TestFixture]
public class CustomBinaryStreamWriterOfTTests
{
    /// <summary>
    /// list of temporary files created by these unit tests
    /// </summary>
    private readonly List<string> _listOfTestGeneratedFolders = new List<string>()
    {
        "results_CustomBinaryStreamWriterOfTTests",
    };

    /// <summary>
    /// clear all generated folders and files
    /// </summary>
    [OneTimeSetUp]
    [OneTimeTearDown]
    public void Clear_folders_and_files()
    {
        foreach (var folder in _listOfTestGeneratedFolders)
        {
            if(!Directory.Exists(folder))
                continue;
            Directory.Delete(folder, recursive: true);
        }
    }

    /// <summary>
    /// Validate writing text to a stream
    /// </summary>
    [Test]
    public void Validate_does_not_throw_when_no_folder_exists()
    {
        // constructor opens the stream directly
        Assert.DoesNotThrow(()=>
        {
            using var sut = new CustomBinaryStreamWriter<double>(
                fileName: @"results_CustomBinaryStreamWriterOfTTests/Nested/myfile.txt",
                writeMap: (bw, nextValue) => bw.Write(37));
        });
    }
}
