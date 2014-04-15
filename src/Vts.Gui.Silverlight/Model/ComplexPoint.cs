using MathNet.Numerics;

namespace Vts.Gui.Silverlight.Model
{
    /// <summary>
    /// Class to represent an x- and y- coordinate with x Real and y Complex numbers
    /// Created to facilitate plotting of real/imag versus phase/amplitude 
    /// </summary>
    public class ComplexPoint
    {
        public ComplexPoint() 
            : this(0.0, 0.0)
        {
        }

        public ComplexPoint(double x, Complex y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public Complex Y { get; set; } 
    }
}
