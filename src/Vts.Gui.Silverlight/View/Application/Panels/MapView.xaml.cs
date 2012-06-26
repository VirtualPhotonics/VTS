using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Vts;
using Vts.Gui.Silverlight.View.Helpers;
using Vts.Gui.Silverlight.ViewModel;
using Vts.Extensions;
using Vts.Gui.Silverlight.Input;

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
