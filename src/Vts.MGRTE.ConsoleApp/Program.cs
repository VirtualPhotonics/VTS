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

            var input = new SimulationInput();

            SolverMGRTE.ExecuteMGRTE(input);

            Console.ReadLine();
        }
    }
}
