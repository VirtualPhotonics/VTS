using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.CommandLineApplication
{
    public class ParameterSweep
    {
        public string Name { get; set; }
        public DoubleRange Range { get; set; }
        // Values are the individual values defined by Range or by paramsweeplist
        public double[] Values { get; set; }

        public ParameterSweep(string name, DoubleRange range)
        {
            Name = name;
            Range = range;
            Values = new double[range.Count];
            for (int i = 0; i < range.Count; i++)
            {
                Values[i] = range.Start + i * range.Delta;
            }
        }

        public ParameterSweep(string name, double[] values)
        {
            Name = name;
            Values = values;
        }

        public ParameterSweep()
            : this("mua1", new DoubleRange(0.01, 0.05, 5))
        {
        }
    }
}
