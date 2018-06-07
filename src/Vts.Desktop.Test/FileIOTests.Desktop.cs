using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Vts.IO.Desktop.Test
{
    [TestFixture]
    public class FileIOTestsDesktop
    {        
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "test.bin", 
        };

        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        [Test]
        public void test_that_a_2D_array_can_be_serialized_to_binary_with_xml_included()
        {
            Console.Write("Trying to write a class with Vts.IO.Desktop...");
            
            var array = new double[,]
            {
                {1D, 2D},
                {3D,4D}
            };
            try { FileIO.WriteToBinary(array, "test.bin" ); }
            catch (Exception e) 
            { 
                Console.WriteLine("problem encountered:\n"); 
                Console.WriteLine(e); 
            }

            Console.WriteLine("success!");
        }
    }
}
