using System.Linq;
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
        public PlotData(IDataPoint[][] points, string[] titles)
        {
            Points = points;
            Titles = titles;
        }

        // todo: temp to get things working...want to eveutally remove
        public PlotData(Point[][] points, string[] titles)
            : this(points.Select(pArray => pArray.Select(point => new DoubleDataPoint(point.X, point.Y)).ToArray()).ToArray(), titles)
        {
        }

        public IDataPoint[][] Points { get; set; }
        public string[] Titles { get; set; }

    }
}
