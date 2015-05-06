namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// Data structure to hold data for plot information
    /// </summary>
    public class PlotAxesLabels
    {
        public PlotAxesLabels(
            string dependentAxisName, string dependentAxisUnits,
            IndependentAxisViewModel independentAxis,
            ConstantAxisViewModel[] constantAxes = null)
        {
            DependentAxisName = dependentAxisName;
            DependentAxisUnits = dependentAxisUnits;
            IndependentAxis = independentAxis;
            ConstantAxes = constantAxes ?? new ConstantAxisViewModel[0];
        }

        public string DependentAxisUnits { get; set; }
        public string DependentAxisName { get; set; }
        public IndependentAxisViewModel IndependentAxis { get; set; }
        public ConstantAxisViewModel[] ConstantAxes { get; set; }
    }
}
