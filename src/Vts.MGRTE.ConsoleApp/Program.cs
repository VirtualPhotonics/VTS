using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vts.FemModeling.MGRTE._2D;
using Vts.FemModeling.MGRTE._2D.IO;

namespace Vts.MGRTE.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //string parametersFile = "parameters.txt";

            //// read parameters file location from first command-line argument
            //if (args.Length > 0)
            //{
            //    parametersFile = args[0];
            //}
            //else
            //{
            //    Console.WriteLine("No parameters file found. Using default.");
            //}

            //// 0.9. load parameters from "parameters.txt"
            //Parameters para = ParametersIO.ReadParametersFromFile(parametersFile);

            // Purpose: this is the main function for RTE_2D.
            // Note: we assume the spatial domain has "nt" intervals,
            //       starting from "0" to "nt-1" with increasing "x" coordinate;
            //       the top boundary with bigger "x" is labeled as "1" and the bottom as "0";
            //       in each interval, the node with the smaller "x" is labeled as "0" and the node with the bigger "x" is labeled as "1".

            Parameters para = new Parameters();

            //para.AMeshLevel = 4;
            //para.AMeshLevel0 = 0;
            //para.Fmg = 1;
            //para.G = 0.9;
            //para.NTissue = 1.0;
            //para.NExt = 1.0;
            //para.L = 1.0;
            //para.NIterations = 100;
            //para.NPreIteration = 3;
            //para.NPostIteration = 3;
            //para.NMgCycle = 1;
            //para.SMeshLevel = 3;
            //para.SMeshLevel0 = 0;
            //para.ConvTol = 1e-4;
            //para.MgMethod = 6;
           
            
            
            SolverMGRTE.ExecuteMGRTE(para);

            Console.ReadLine();
        }
    }
}
