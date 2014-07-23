using System.Windows;

namespace Vts.Gui.Silverlight.Model
{
    /// <summary>
    /// Class to hold all data necessary for describing a single line plot
    /// Needs to be bolstered to allow for multiple descriptors, powerful enough
    /// to support a view of a particular data set
    /// </summary>
    public class PlotData
    {
        public PlotData(Point[][] points, string[] titles)
        {
            Points = points;
            Titles = titles;
            IsComplex = false;
        }

        public PlotData(ComplexPoint[][] points, string[] titles)
        {
            ComplexPoints = points;
            Titles = titles;
            IsComplex = true;
        }

        public Point[][] Points { get; set; }
        public string[] Titles { get; set; }
        public ComplexPoint[][] ComplexPoints { get; set; }
        public bool IsComplex { get; set; }


    }
}
