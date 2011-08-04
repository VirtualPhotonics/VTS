using System.IO;
using System;
namespace Vts.FemModeling.MGRTE._2D.IO
{
    public class ParametersIO
    {
        public static Parameters ReadParametersFromFile(string parametersFile)
        {
            Parameters para = new Parameters();
            if (File.Exists(parametersFile))
            {
                using (TextReader reader = File.OpenText(parametersFile))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');

                    int count = 0;
                    double temp = 0.0;
                    
                    para.G = double.Parse(bits[count]); count++;
                    para.NTissue = double.Parse(bits[count]); count++;
                    para.NExt = double.Parse(bits[count]); count++;
                    temp = double.Parse(bits[count]); para.AMeshLevel = (int)temp - 1; count++;
                    temp = double.Parse(bits[count]); para.AMeshLevel0 = (int)temp - 1; count++;
                    temp = double.Parse(bits[count]); para.SMeshLevel = (int)temp - 1; count++;
                    temp = double.Parse(bits[count]); para.SMeshLevel0 = (int)temp - 1; count++;
                    para.ConvTol = double.Parse(bits[count]); count++;
                    temp = double.Parse(bits[count]); para.MgMethod = (int)temp; count++;
                    temp = double.Parse(bits[count]); para.FullMg = (int)temp; count++;
                    temp = double.Parse(bits[count]); para.NPreIteration = (int)temp; count++;
                    temp = double.Parse(bits[count]); para.NPostIteration = (int)temp; count++;
                    temp = double.Parse(bits[count]); para.NMgCycle = (int)temp; count++;
                    temp = double.Parse(bits[count]); para.NIterations = (int)temp;
                    reader.Close();

                }
            }
            else
            {
                Console.WriteLine(parametersFile + " does not exist!");
            }

            return para;
        }
    }
}
