using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Vts.Gui.Silverlight.Model
{
    /// <summary>
    /// Class to hold all data necessary for describing a single line plot
    /// Needs to be bolstered to allow for multiple descriptors, powerful enough
    /// to support a view of a particular data set
    /// </summary>
    public class PlotData
    {
        public PlotData() 
            : this(null, null)
        {
        }

        public PlotData(IEnumerable<Point> points, string title)
            : this(points, title, Colors.Blue)
        {
        }

        public PlotData(IEnumerable<Point> points, string title, Color c)
        {
            Points = points.ToList();
            Title = title;
            plotColor = c;
        }
        public PlotData(IEnumerable<ComplexPoint> points, string title, string title2)
            : this(points, title, title2, Colors.Blue, Colors.Red)
        {
        }
        public PlotData(IEnumerable<ComplexPoint> points, string title, string title2, Color c, Color c2)
        {
            ComplexPoints = points.ToList();
            Title = title;
            Title2 = title2;
            plotColor = c;
            plotColor2 = c2;
        }

        public IList<Point> Points { get; set; }
        public string Title { get; set; }
        public Color plotColor { get; set; }
        public IList<ComplexPoint> ComplexPoints { get; set; }
        public string Title2 { get; set; }
        public Color plotColor2 { get; set; }

    }
}
