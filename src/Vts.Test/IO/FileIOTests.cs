using System.IO;
using System.Text;
using NUnit.Framework;
using Vts.IO;

namespace Vts.Test.IO
{
    [TestFixture]
    public class FileIOTests
    {
        /// <summary>
        /// clear all previously generated folders and files
        /// </summary>
        [TestFixtureSetUp]
        public void clear_folders_and_files()
        {
            if(FileIO.FileExists("file1.txt"))
            {
                FileIO.FileDelete("file1.txt");
            }
            if (FileIO.FileExists("file2.txt"))
            {
                FileIO.FileDelete("file2.txt");
            }
            if (FileIO.FileExists("file3.txt"))
            {
                FileIO.FileDelete("file3.txt");
            }
            if (FileIO.FileExists("file4.txt"))
            {
                FileIO.FileDelete("file4.txt");
            }
        }

        [Test]
        public void validate_file_exists()
        {
            FileIO.WriteToTextFile("Text", "file1.txt");
            Assert.IsTrue(FileIO.FileExists("file1.txt"));
        }

        [Test]
        public void validate_file_delete()
        {
            FileIO.WriteToTextFile("Text", "file2.txt");
            FileIO.FileDelete("file2.txt");
            Assert.IsFalse(FileIO.FileExists("file2.txt"));
        }

        [Test]
        public void validate_save_text_to_file()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            FileIO.WriteToTextFile(myString.ToString(), "file3.txt");
            Assert.IsTrue(true);
        }

        [Test]
        public void validate_write_text_to_stream()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            Stream stream = StreamFinder.GetFileStream("file4.txt", FileMode.Create);
            FileIO.WriteTextToStream(myString.ToString(), stream);
            Assert.IsNotNull(stream);
        }
    }
}
