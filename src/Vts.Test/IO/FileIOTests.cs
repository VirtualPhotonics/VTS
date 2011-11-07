using System.Text;
using NUnit.Framework;
using System.IO;
using Vts.IO;

namespace Vts.Test.IO
{
    [TestFixture]
    public class FileIOTests
    {
        [Test]
        public void validate_save_text_to_file()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            Vts.IO.FileIO.WriteToTextFile(myString.ToString(), "myfile.txt");
            Assert.IsTrue(true);
        }

        [Test]
        public void validate_write_text_to_stream()
        {
            StringBuilder myString = new StringBuilder();
            myString.AppendLine("This is a test string");
            Stream stream = StreamFinder.GetFileStream("myfile2.txt", FileMode.Create);
            Vts.IO.FileIO.WriteTextToStream(myString.ToString(), stream);
            Assert.IsNotNull(stream);
        }
    }
}
