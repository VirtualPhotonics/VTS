using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vts.Gui.Silverlight.Input;
using Vts.Gui.Silverlight.ViewModel;

//using SLExtensions.Input;

namespace Vts.Gui.Silverlight.View
{
    public partial class SolverDemoView : UserControl
    {
        private int _numPlotViews;
        private int _numMapViews;
        public static SolverDemoView Current = null;

        private FloatableWindow _floatableWindow; 
        public SolverDemoView()
        {
            InitializeComponent();

            _numPlotViews = 0;
            _numMapViews = 0;

            Current = this;

            _floatableWindow = new FloatableWindow()
            {
                Name = "wndIsolatedStorageView",
                Content = new IsolatedStorageView(),
                ParentLayoutRoot = this.layoutRoot,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
#if WHITELIST
            tabFem.Visibility = Visibility.Collapsed;
#endif
            Commands.IsoStorage_IncreaseSpaceQuery.Executed += IsoStorage_IncreaseSpaceQuery_Executed;
            Commands.IsoStorage_IncreaseSpaceQuery.Executed += IsoStorage_IncreaseSpaceQuery_Executed;

            Commands.Main_DuplicatePlotView.Executed += Main_DuplicatePlotView_Executed;
            Commands.Main_DuplicateMapView.Executed += Main_DuplicateMapView_Executed;
        }
        
        void Main_DuplicatePlotView_Executed(object sender, SLExtensions.Input.ExecutedEventArgs e)
        {
            var vm = e.Parameter as PlotViewModel;
            if (vm != null)
            {
                var plotView = new PlotView
                {
                    DataContext = vm,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Margin = new Thickness(0,0,3,0)
                };
                var newPlotWindow = new FloatableWindow()
                {
                    Name = "wndPlotView" + _numPlotViews++,
                    Content = plotView,
                    ParentLayoutRoot = this.layoutRoot,
                    Background = new SolidColorBrush(Colors.White),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                newPlotWindow.Show();
            }
        }

        void Main_DuplicateMapView_Executed(object sender, SLExtensions.Input.ExecutedEventArgs e)
        {
            var vm = e.Parameter as MapViewModel;
            if (vm != null)
            {
                var mapView = new MapView
                {
                    DataContext = vm,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Margin = new Thickness(0, 0, 3, 0),
                    MapImage =
                    {
                        Stretch = Stretch.Uniform,
                        MinWidth = 500
                    }
                };
                var newPlotWindow = new FloatableWindow()
                {
                    Name = "wndMapView" + _numMapViews++,
                    Content = mapView,
                    ParentLayoutRoot = this.layoutRoot,
                    Background = new SolidColorBrush(Colors.White),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0),
                    Width = 700,
                    Height = 540
                };

                newPlotWindow.Show();
            }
        }

        void IsoStorage_IncreaseSpaceQuery_Executed(object sender, SLExtensions.Input.ExecutedEventArgs e)
        {
            _floatableWindow.ShowDialog();
        }

        private void inputTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var inputTab = sender as TabControl;
            if (inputTab != null && inputTab.Items != null &&
                outputTabControl !=null && outputTabControl.Items != null && outputTabControl.Items.Count > 1)
            {
                var tabItem = inputTab.SelectedItem as TabItem;
                if (tabItem != null)
                {
                    switch (tabItem.Name)
                    {
                        case "tabForward":
                        case "tabInverse":
                        case "tabSpectral":
                        default:
                            outputTabControl.SelectedItem = outputTabControl.Items[0];
                            ((TabItem)outputTabControl.Items[0]).Visibility = Visibility.Visible;
                            ((TabItem)outputTabControl.Items[1]).Visibility = Visibility.Collapsed;
                            ((TabItem)outputTabControl.Items[2]).Visibility = Visibility.Collapsed;
                            break;
                        case "tabFluence":
                            outputTabControl.SelectedItem = outputTabControl.Items[1];
                            ((TabItem)outputTabControl.Items[1]).Visibility = Visibility.Visible;
                            ((TabItem)outputTabControl.Items[0]).Visibility = Visibility.Collapsed;
                            ((TabItem)outputTabControl.Items[2]).Visibility = Visibility.Collapsed;
                            break;
                        case "tabFem":
                            outputTabControl.SelectedItem = outputTabControl.Items[2];                           
                            ((TabItem)outputTabControl.Items[2]).Visibility = Visibility.Visible;
                            ((TabItem)outputTabControl.Items[0]).Visibility = Visibility.Collapsed;
                            ((TabItem)outputTabControl.Items[1]).Visibility = Visibility.Collapsed;
                            break;
                        case "tabMonteCarlo":
                            ((TabItem)outputTabControl.Items[1]).Visibility = Visibility.Visible;
                            ((TabItem)outputTabControl.Items[0]).Visibility = Visibility.Visible;
                            ((TabItem)outputTabControl.Items[2]).Visibility = Visibility.Collapsed;
                            break;
                    }
                }
            }
        }
    }
}
