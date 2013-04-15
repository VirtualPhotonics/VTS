namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// Data structure to hold data for plot information
    /// </summary>
    public class PlotAxesLabels
    {
        public string IndependentAxisUnits { get; set; }
        public string IndependentAxisName { get; set; }
        public IndependentVariableAxis IndependentAxisType { get; set; }
        public string DependentAxisUnits { get; set; }
        public string DependentAxisName { get; set; }
        public string ConstantAxisName { get; set; }
        public string ConstantAxisUnits { get; set; }
        public double ConstantAxisValue { get; set; }

        public PlotAxesLabels(
            string independentAxisName, string independentAxisUnits,
            IndependentVariableAxis independentAxisType,
            string dependentAxisName, string dependentAxisUnits,
            string constantAxisName, string constantAxisUnits, 
            double constantAxisValue)
        {
            IndependentAxisName = independentAxisName;
            IndependentAxisUnits = independentAxisUnits;
            IndependentAxisType = independentAxisType;
            DependentAxisName = dependentAxisName;
            DependentAxisUnits = dependentAxisUnits;
            ConstantAxisName = constantAxisName;
            ConstantAxisUnits = constantAxisUnits;
            ConstantAxisValue = constantAxisValue;
        }
        public PlotAxesLabels(
            string independentAxisName, string independentAxisUnits, IndependentVariableAxis independentAxisType,
            string dependentAxisName, string dependentAxisUnits)
            : this(independentAxisName, independentAxisUnits, independentAxisType, 
            dependentAxisName, dependentAxisUnits, "", "", 0) { }
    }
}
