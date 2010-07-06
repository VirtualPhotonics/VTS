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
using Vts.SiteVisit.ViewModel;
using Vts.Extensions;

namespace Vts.SiteVisit.View
{
    public partial class TextOutputView : UserControl
    {
        public TextOutputView()
        {
            InitializeComponent();
        }

        //public void AppendText(string s)
        //{
        //    if (this.DataContext != null)
        //    {
        //        TextOutputViewModel vm = (TextOutputViewModel)this.DataContext;
        //        if (vm != null)
        //        {
        //            vm.Text += s;
        //        }
        //    }
        //}
    }
}
