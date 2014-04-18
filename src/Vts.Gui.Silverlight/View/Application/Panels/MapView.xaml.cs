using System.Windows;
using System.Windows.Controls;
using Vts.Gui.Silverlight.View.Helpers;

namespace Vts.Gui.Silverlight.View
{
    public partial class MapView : UserControl
    {
        public MapView() 
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImageTools.SaveUIElementToJpegImage(this);
        }
    }
}
