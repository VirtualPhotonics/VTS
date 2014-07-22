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
        public PlotData(Point[][] points, string title)
        {
            Points = points;
            Title = title;
            IsComplex = false;
        }

        public PlotData(ComplexPoint[][] points, string title)
        {
            ComplexPoints = points;
            Title = title;
            IsComplex = true;
        }

        public Point[][] Points { get; set; }
        public string Title { get; set; }
        public ComplexPoint[][] ComplexPoints { get; set; }
        public bool IsComplex { get; set; }


    }
}
