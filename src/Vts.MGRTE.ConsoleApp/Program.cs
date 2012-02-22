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

            SimulationInputs inputParameters = new SimulationInputs();
              
            //User sp                 
            inputParameters.MedG = 0.8;
            inputParameters.NTissue = 1.0;
            inputParameters.NExt = 1.0;
            inputParameters.AMeshLevel = 4;   
            inputParameters.SMeshLevel = 4;            
            inputParameters.ConvTol = 1e-4;
            inputParameters.MgMethod = 6;
            inputParameters.Length = 1.0;       
            
            SolverMGRTE.ExecuteMGRTE(inputParameters);

            Console.ReadLine();
        }
    }
}
