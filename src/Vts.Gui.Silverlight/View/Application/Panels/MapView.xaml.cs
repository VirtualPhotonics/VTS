using System.Windows;
using System.Windows.Controls;
using Vts.Gui.Silverlight.View.Helpers;
using System.Windows.Input;

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

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx != null && e.Key == Key.Enter)
                tbx.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
