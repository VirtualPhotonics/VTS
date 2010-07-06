using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using SLExtensions.Input;
using Vts.Extensions;
using Vts.IO;
using Vts.SiteVisit.Input;
using Vts.SiteVisit.Model;
using System.IO;

namespace Vts.SiteVisit.ViewModel
{
    /// <summary>
    /// View model implementing Plot panel functionality
    /// </summary>
    public class PlotViewModel : BindableObject
    {
        // change from Point to our own custom class so we can bind to color, style, etc, too

        private string _Title;
        private IList<string> _PlotTitles;
        private ReflectancePlotType _PlotType;
        private bool _HoldOn;
        private ObservableCollection<IList<Point>> _PlotSeriesCollection;
        private IList<string> _Labels;
        private OptionViewModel<ScalingType> _XAxisSpacingOptionVM;
        private OptionViewModel<ScalingType> _YAxisSpacingOptionVM;
        private OptionViewModel<PlotNormalizationType> _PlotNormalizationTypeOptionVM;
        private string _CustomPlotLabel;
        private bool _ShowAxes;

        public PlotViewModel()
        {
            Labels = new List<string>();
            PlotTitles = new List<string>();
            DataSeriesCollection = new List<IList<Point>>();
            PlotSeriesCollection = new ObservableCollection<IList<Point>>();

            PlotType = ReflectancePlotType.ForwardSolver;
            HoldOn = true;
            ShowAxes = false;

            XAxisSpacingOptionVM = new OptionViewModel<ScalingType>("XAxisSpacing", false);
            XAxisSpacingOptionVM.PropertyChanged += (sender, args) => UpdatePlotSeries();

            YAxisSpacingOptionVM = new OptionViewModel<ScalingType>("YAxisSpacing", false);
            YAxisSpacingOptionVM.PropertyChanged += (sender, args) => UpdatePlotSeries();

            PlotNormalizationTypeOptionVM = new OptionViewModel<PlotNormalizationType>("NormalizationType", false);
            PlotNormalizationTypeOptionVM.PropertyChanged += (sender, args) => UpdatePlotSeries();

            CustomPlotLabel = "";

            //Commands.Plot_PlotValuesLinearly.Executed += PlotLinearly_Executed;
            Commands.Plot_ClearPlot.Executed += Plot_Cleared;
            Commands.Plot_ClearPlotSingle.Executed += Plot_ClearedSingle;
            Commands.Plot_PlotValues.Executed += Plot_Executed;
            Commands.Plot_SetAxesLabels.Executed += Plot_SetAxesLabels_Executed;
            Commands.Plot_ExportDataToText.Executed += Plot_ExportDataToText_Executed;
        }

        private IList<IList<Point>> DataSeriesCollection { get; set; }

        public ObservableCollection<IList<Point>> PlotSeriesCollection
        {
            get
            {
                return _PlotSeriesCollection;
            }
            set
            {
                _PlotSeriesCollection = value;
                this.OnPropertyChanged("PlotSeriesCollection");
            }
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        public ReflectancePlotType PlotType
        {
            get { return _PlotType; }
            set
            {
                _PlotType = value;
                this.OnPropertyChanged("PlotType");
            }
        }

        public bool HoldOn
        {
            get { return _HoldOn; }
            set
            {
                _HoldOn = value;
                OnPropertyChanged("HoldOn");
            }
        }

        public bool ShowAxes
        {
            get { return _ShowAxes; }
            set
            {
                _ShowAxes = value;
                OnPropertyChanged("ShowAxes");
            }
        }

        public OptionViewModel<ScalingType> XAxisSpacingOptionVM
        {
            get { return _XAxisSpacingOptionVM; }
            set
            {
                _XAxisSpacingOptionVM = value;
                OnPropertyChanged("XAxisSpacingOptionVM");
            }
        }

        public OptionViewModel<ScalingType> YAxisSpacingOptionVM
        {
            get { return _YAxisSpacingOptionVM; }
            set
            {
                _YAxisSpacingOptionVM = value;
                OnPropertyChanged("YAxisSpacingOptionVM");
            }
        }

        public OptionViewModel<PlotNormalizationType> PlotNormalizationTypeOptionVM
        {
            get { return _PlotNormalizationTypeOptionVM; }
            set
            {
                _PlotNormalizationTypeOptionVM = value;
                OnPropertyChanged("PlotNormalizationTypeOptionVM");
            }
        }

        public string CustomPlotLabel
        {
            get { return _CustomPlotLabel; }
            set
            {
                _CustomPlotLabel = value;
                OnPropertyChanged("CustomPlotLabel");
            }
        }

        public IList<string> Labels
        {
            get { return _Labels; }
            set
            {
                _Labels = value;
                this.OnPropertyChanged("Labels");
            }
        }

        public IList<string> PlotTitles
        {
            get { return _PlotTitles; }
            set
            {
                _PlotTitles = value;
                this.OnPropertyChanged("PlotTitles");
            }
        }


        void Plot_SetAxesLabels_Executed(object sender, ExecutedEventArgs e)
        {
            if (e.Parameter is PlotAxesLabels)
            {
                var labels = e.Parameter as PlotAxesLabels;
                if (labels.ConstantAxisName == null || labels.ConstantAxisName.Length == 0)
                {
                    Title =
                        labels.DependentAxisName + " [" + labels.DependentAxisUnits + "] versus " +
                        labels.IndependentAxisName + " [" + labels.IndependentAxisUnits + "]";
                }
                else
                {
                    Title =
                        labels.DependentAxisName + " [" + labels.DependentAxisUnits + "]  versus " +
                        labels.IndependentAxisName + " [" + labels.IndependentAxisUnits + "]" +
                        " at " + labels.ConstantAxisName + " = " + labels.ConstantAxisValue + " " + labels.ConstantAxisUnits;
                }
            }
        }

        /// <summary>
        /// Writes tab-delimited 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Plot_ExportDataToText_Executed(object sender, ExecutedEventArgs e)
        {
            if (_Labels != null && _Labels.Count > 0 && _PlotSeriesCollection != null && _PlotSeriesCollection.Count > 0)
            {
                using (var stream = StreamFinder.GetLocalFilestreamFromFileDialog(".txt"))
                {
                    if (stream != null)
                    {
                        using (StreamWriter sw = new StreamWriter(stream))
                        {
                            sw.Write("%");
                            _Labels.ForEach(label => sw.Write(label + " (X)" + "\t" + label + " (Y)" + "\t"));
                            sw.WriteLine();
                            for (int i = 0; i < _PlotSeriesCollection[0].Count; i++)
                            {
                                sw.WriteLine();
                                for (int j = 0; j < _PlotSeriesCollection.Count; j++)
                                {
                                    sw.Write(_PlotSeriesCollection[j][i].X + "\t" + _PlotSeriesCollection[j][i].Y + "\t");
                                }
                            }
                        }
                    }
                }
            }
        }

        void Plot_Cleared(object sender, ExecutedEventArgs e)
        {
            ClearPlot();
            UpdatePlotSeries();
        }

        void Plot_ClearedSingle(object sender, ExecutedEventArgs e)
        {
            ClearPlotSingle();
            UpdatePlotSeries();
        }

        void Plot_Executed(object sender, ExecutedEventArgs e)
        {
            var data = e.Parameter as PlotData;
            if (data != null)
            {
                AddValuesToPlotData(data.Points, data.Title);
            }
        }

        //static int labelCounter = 0;
        private void AddValuesToPlotData(IList<Point> points, string title)
        {
            if (!HoldOn) // HoldOn
            {
                ClearPlot();
            }

            DataSeriesCollection.Add(points);

            var customLabel = CustomPlotLabel.Length > 0 ? "\n(" + CustomPlotLabel + ")" : "";
            Labels.Add(title + customLabel); // has to happen before updating the bound collection

            PlotTitles.Add(Title);

            UpdatePlotSeries();
        }

        private void ClearPlot()
        {
            Title = "";
            PlotTitles.Clear();
            DataSeriesCollection.Clear();
            PlotSeriesCollection.Clear();
            Labels.Clear();
        }

        private void ClearPlotSingle()
        {
            if (DataSeriesCollection.Count > 0)
            {
                if (PlotTitles.Count <= 1)
                {
                    Title = "";
                    PlotTitles.Clear();
                }
                else
                {
                    PlotTitles.RemoveAt(PlotTitles.Count - 1);
                    Title = PlotTitles.Last();
                }
                DataSeriesCollection.RemoveAt(DataSeriesCollection.Count - 1);
                PlotSeriesCollection.RemoveAt(PlotSeriesCollection.Count - 1);
                Labels.RemoveAt(Labels.Count - 1);
            }
        }

        /// <summary>
        /// Updates the plot. 
        /// Current (bad) implementation uses creation of a new list to trigger the binding. 
        /// </summary>
        private void UpdatePlotSeries()
        {
            // this is one of a number of scenarios where we need to keep track of an individual
            // plot curve or value. need a more general representation that is UI-friendly.
            int normCurveNumber = 0;

            // now this computes O(M*N) regardless...yuck
            var normalizationPoints =
                (from ds in DataSeriesCollection
                 let normToCurve =
                    PlotNormalizationTypeOptionVM.SelectedValue == PlotNormalizationType.RelativeToCurve
                    && DataSeriesCollection.Count > 1
                 let normToMax =
                    PlotNormalizationTypeOptionVM.SelectedValue == PlotNormalizationType.RelativeToMax
                    && DataSeriesCollection.Count > 0
                 let maxValue = ds.Select(p => p.Y).Max()
                 select DataSeriesCollection[normCurveNumber].Select(p =>
                        normToCurve ? p.Y : normToMax ? maxValue : 1.0)).ToList();

            var newCollection = new ObservableCollection<IList<Point>>();

            var pointsToPlot =
                from ds in DataSeriesCollection.Zip(
                    normalizationPoints, (p, n) => new { DataPoints = p, NormValues = n })
                let xValues = ds.DataPoints.Select(dp => dp.X)
                let yValues = ds.DataPoints.Zip(ds.NormValues, (dp, nv) => dp.Y / nv)
                let useLogX = XAxisSpacingOptionVM.SelectedValue == ScalingType.Log
                let useLogY = YAxisSpacingOptionVM.SelectedValue == ScalingType.Log
                select xValues.Zip(yValues, (x, y) =>
                    new Point(
                        useLogX ? Math.Log10(x) : x,
                        useLogY ? Math.Log10(y) : y))
                    .Where(p => p.IsValidDataPoint());

            foreach (var curve in pointsToPlot.ToList())
            {
                newCollection.Add(curve.ToList());
            }

            PlotSeriesCollection = newCollection;

            //Only display the x and y axes if there is a plot to display
            if (DataSeriesCollection.Count > 0)
                ShowAxes = true;
            else
                ShowAxes = false;
        }
    }
}
