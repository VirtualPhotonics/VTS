using System;
using System.Windows;
using System.Windows.Controls;

namespace Vts.Gui.Silverlight.View
{
    public partial class SolutionDomainOptionView : UserControl
    {
        public SolutionDomainOptionView()
        {
            InitializeComponent();
        }

        private void OptionView_LayoutUpdated(object sender, EventArgs e)
        {
            var optionView = sender as RadioButtonOptionView;
            if (optionView != null)
                optionView.VerticalAlignment = VerticalAlignment.Center;
        }

        private void StackPanel_LayoutUpdated(object sender, EventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
                stackPanel.VerticalAlignment = VerticalAlignment.Top;
        }

        ///// <summary>
        ///// This is a work-around for a Silverlight 2.0 bug (not necessary in WPF). 
        ///// Un-checks the previously checked radio buttons. Code from here:
        ///// http://silverlight.net/forums/p/47810/165241.aspx#165241
        ///// </summary>
        ///// <param name="p_Container"></param>
        ///// <param name="p_GroupName"></param>
        ///// <param name="p_IgnoreId"></param>
        //private void UncheckGroupRadioButtons(FrameworkElement p_Container, string p_GroupName, string p_IgnoreId)
        //{
        //    int p_ChildCount = VisualTreeHelper.GetChildrenCount(p_Container);
        //    for (int i = 0; i < p_ChildCount; i++)
        //    {
        //        DependencyObject p_Obj = VisualTreeHelper.GetChild(p_Container, i);
        //        if (p_Obj is RadioButton)
        //        {
        //            RadioButton p_curRadio = (RadioButton)p_Obj;
        //            if (p_curRadio.GroupName == p_GroupName && p_curRadio.Tag != null && p_curRadio.Tag.ToString() != p_IgnoreId)
        //                p_curRadio.IsChecked = false;
        //        }
        //        else
        //        {
        //            UncheckGroupRadioButtons((FrameworkElement)p_Obj, p_GroupName, p_IgnoreId);
        //        }
        //    }
        //}

        //private void RadioButton_Checked(object sender, RoutedEventArgs e)
        //{
        //    RadioButton p_curRadio = (RadioButton)sender;

        //    UncheckGroupRadioButtons(myOptionGrid, p_curRadio.GroupName, p_curRadio.Tag.ToString());
        //}

        //private void myIndependentVariableAxisOptionView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (myIndependentVariableAxisOptionView.Header != null)
        //    {
        //        if (myIndependentVariableAxisOptionView.Header.Text == "")
        //            myIndependentVariableAxisOptionView.Header.Height = 0;
        //    }
        //}
    }
}
