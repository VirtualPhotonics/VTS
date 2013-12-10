using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vts.Gui.Silverlight.View.Helpers;

namespace Vts.Gui.Silverlight.View
{
    public partial class PlotView : UserControl
    {
        public PlotView()
        {
            InitializeComponent();
        }

        private void ExportImage_Click(object sender, RoutedEventArgs e)
        {
            ImageTools.SaveUIElementToJpegImage(myChart);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx != null && e.Key == Key.Enter)
                tbx.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
