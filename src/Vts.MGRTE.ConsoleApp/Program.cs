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

            para.Alevel = 4;
            para.Alevel0 = 0;
            para.Fmg = 1;
            para.G = 0.9;
            para.Index_i = 1.0;
            para.Index_o = 1.0;
            para.L = 1.0;
            para.N_max = 100;
            para.N1 = 3;
            para.N2 = 3;
            para.N3 = 1;
            para.Slevel = 3;
            para.Slevel0 = 0;
            para.Tol = 1e-4;
            para.Whichmg = 6;
           
            
            
            SolverMGRTE.ExecuteMGRTE(para);

            Console.ReadLine();
        }
    }
}
