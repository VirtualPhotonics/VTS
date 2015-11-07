using System;
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

        private void MapImage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Thickness zMargin;
            var newHeight = MapImage.ActualHeight;
            //if (e != null && (Math.Abs(e.NewSize.Height - MapImage.ActualHeight) > 1))
            //{
            //    newHeight = e.NewSize.Height;
            //}
            var threshold = Math.Abs(e.PreviousSize.Height - MapImage.ActualHeight);
            zMargin = new Thickness(0, newHeight, 0, 0);
            ZMax.Margin = zMargin;
        }
    }
}
