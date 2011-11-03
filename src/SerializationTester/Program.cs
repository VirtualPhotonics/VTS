using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Vts.MonteCarlo;

namespace SerializationTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //var input = SimulationInput.FromFile(@"C:\Dropbox\Temp\2011-10-15 monte carlo results\dc_ROfFx_mua1_0.02\dc_ROfFx_mua1_0.02.xml");

            var input2 = SimulationInput.FromFile("test.xml");


            string output = JsonConvert.SerializeObject(input2, Formatting.Indented);
            using (var textWriter = new StreamWriter("test.json"))
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonWriter.WriteRaw(output);
            }
            SimulationInput input = JsonConvert.DeserializeObject<SimulationInput>(output);



            //SimulationInput test = null;
            //using (var textReader = new StreamReader("test_input.json"))
            //{
            //    string text = textReader.ReadToEnd();
            //    Console.WriteLine(text);
            //    test = JsonConvert.DeserializeObject<SimulationInput>(text);
            //}

            Console.Read();
        }
    }
}
