using Vts.FemModeling.MGRTE._2D;

namespace Vts.Mgrte.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new SimulationInput();

            SolverMGRTE.ExecuteMGRTE(input);

            System.Console.ReadLine();
        }
    }
}
