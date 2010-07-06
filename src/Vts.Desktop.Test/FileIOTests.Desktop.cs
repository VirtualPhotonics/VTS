using System;
using NUnit.Framework;

namespace Vts.IO.Desktop.Test
{
    [TestFixture]
    public class FileIOTestsDesktop
    {
        public static void Main()
        {
            var fileIOTests = new FileIOTestsDesktop();
            
            fileIOTests.test_that_a_2D_array_can_be_serialized_to_binary_with_xml_included();
            
            Console.ReadKey();
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
