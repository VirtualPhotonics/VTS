using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vts.FemModeling.MGRTE._2D;


namespace Vts.MGRTE.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {           
            
            Parameters para = new Parameters();
              
            //User sp                 
            para.G = 0.8;
            para.NTissue = 1.0;
            para.NExt = 1.0;
            para.AMeshLevel = 4;   
            para.SMeshLevel = 4;            
            para.ConvTol = 1e-4;
            para.MgMethod = 6;
            para.Length = 1.0;       
            
            SolverMGRTE.ExecuteMGRTE(para);

            Console.ReadLine();
        }
    }
}
