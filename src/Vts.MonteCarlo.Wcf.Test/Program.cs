using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Wcf.Test.MonteCarloServiceProxy;

namespace Vts.MonteCarlo.Wcf.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new MonteCarloServiceClient())
            {
                bool[] results =
                    client.ExecuteBatch(
                        new SimulationInput[]{
                            new SimulationInput()
                            {
                                OutputFileName = @"V:/Simulations/David/2008-02-16/test"
                            }});
            }
        }
    }
}
