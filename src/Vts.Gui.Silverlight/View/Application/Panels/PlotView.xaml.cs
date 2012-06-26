using System.Windows;
using System.Windows.Controls;
using Vts.IO;
using Vts.SiteVisit.View.Helpers;

using System.Windows.Media.Imaging;
using Vts.SiteVisit.ViewModel;

namespace Vts.SiteVisit.View
{
    public partial class PlotView : UserControl
    {
        public PlotView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImageTools.SaveUIElementToJpegImage(myChart);
        }
    }
}
