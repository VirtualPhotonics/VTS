using System;
using System.Windows;

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
