using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Vts.IO.Desktop.Test
{
    [TestFixture]
    public class FileIOTestsDesktop
    {        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestFiles = new List<string>()
        {
            "test.bin", 
        };

        [TestFixtureSetUp]
        public void clear_previously_generated_files()
        {
            foreach (var file in listOfTestFiles)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
        /// <summary>
        /// clear all newly generated files
        /// </summary>
        [TestFixtureTearDown]
        public void clear_newly_generated_files()
        {
            // delete any newly generated files
            foreach (var file in listOfTestFiles)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
        [Test]
        public void test_that_a_2D_array_can_be_serialized_to_binary_with_xml_included()
        {
            Console.Write("Trying to write a Silverlight class with Vts.IO.Desktop...");
            
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
