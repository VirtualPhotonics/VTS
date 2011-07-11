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
                    para.Index_i = double.Parse(bits[count]); count++;
                    para.Index_o = double.Parse(bits[count]); count++;
                    temp = double.Parse(bits[count]); para.Alevel = (int)temp - 1; count++;
                    temp = double.Parse(bits[count]); para.Alevel0 = (int)temp - 1; count++;
                    temp = double.Parse(bits[count]); para.Slevel = (int)temp - 1; count++;
                    temp = double.Parse(bits[count]); para.Slevel0 = (int)temp - 1; count++;
                    para.Tol = double.Parse(bits[count]); count++;
                    temp = double.Parse(bits[count]); para.Whichmg = (int)temp; count++;
                    temp = double.Parse(bits[count]); para.Fmg = (int)temp; count++;
                    temp = double.Parse(bits[count]); para.N1 = (int)temp; count++;
                    temp = double.Parse(bits[count]); para.N2 = (int)temp; count++;
                    temp = double.Parse(bits[count]); para.N3 = (int)temp; count++;
                    temp = double.Parse(bits[count]); para.N_max = (int)temp;
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
