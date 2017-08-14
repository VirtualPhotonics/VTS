using MathNet.Numerics.Interpolation;

namespace Vts.Modeling
{
    public class ReffCalculator
    {
        private static readonly double[] nValues = 
            new double[] { 
                0, 1, 1.05, 1.1, 1.15, 1.2, 1.25, 1.3, 1.35,
                1.4, 1.45, 1.5, 1.55, 1.6, 1.65, 1.7 };

        private static readonly double[] reffValues = 
            new double[] { 
                600, 5.00E-11, 0.0731, 0.1491, 0.221, 0.2872, 
                0.3472, 0.4013, 0.4499, 0.4935, 0.5326, 0.5678, 
                0.5995, 0.6282, 0.6541, 0.6775 };

        private static IInterpolation interpolator =
            MathNet.Numerics.Interpolate.Linear(nValues,reffValues);

        public static double GetReff(double n)
        {
            return interpolator.Interpolate(n);
        }
    }
}
