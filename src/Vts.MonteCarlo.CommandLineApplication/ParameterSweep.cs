using Vts.Common;

namespace Vts.MonteCarlo.CommandLineApplication
{
    public class ParameterSweep
    {
        public string Name { get; set; }
        public DoubleRange Range { get; set; }

        public ParameterSweep(string name, DoubleRange range)
        {
            Name = name;
            Range = range;
        }

        public ParameterSweep()
            : this("mua1", new DoubleRange(0.01, 0.05, 5))
        {
        }
    }
}
