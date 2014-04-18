using System.Windows;
using System.Windows.Controls;
using Vts.Gui.Silverlight.View.Helpers;

namespace Vts.Gui.Silverlight.View
{
    public partial class MeshView : UserControl
    {
        public MeshView() 
        {
           InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImageTools.SaveUIElementToJpegImage(this);
        }
    }
}
