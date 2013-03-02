using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class MuellerMatrix
    {
        public double[] _st11 { get; set; }
        public double[] _s12 { get; set; }
        public double[] _s22 { get; set; }
        public double[] _s33 { get; set; }
        public double[] _s34 { get; set; }
        public double[] _s44 { get; set; }

        public MuellerMatrix(double[] st11, double[] s12, double[] s22, double[] s33, double[] s34, double[] s44)
        {
            _st11 = st11;
            _s12 = s12;
            _s22 = s22;
            _s33 = s33;
            _s34 = s34;
            _s44 = s44;
        }

        public MuellerMatrix()
        {
            _st11 = 0.5;
            _s12 = 0.5;
            _s22 = 0.5;
            _s33 = 0;
            _s34 = 0;
            _s44 = 0;
        }
    }
}
