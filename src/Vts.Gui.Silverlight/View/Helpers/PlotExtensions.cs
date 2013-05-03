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

namespace Vts.Gui.Silverlight.ViewModel
{
    public static class PlotExtensions
    {
        public static bool IsValidDataPoint(this Point p)
        {
            bool isValid =
                !double.IsInfinity(Math.Abs(p.X)) && !double.IsNaN(p.X) &&
                !double.IsInfinity(Math.Abs(p.Y)) && !double.IsNaN(p.Y);

            return isValid;
        }
    }
}
