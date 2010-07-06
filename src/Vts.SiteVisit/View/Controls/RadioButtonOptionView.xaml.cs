﻿using System;
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

namespace Vts.SiteVisit.View
{
    public partial class RadioButtonOptionView : UserControl
    {
        //public string SelectedOption
        //{
        //    get { return (string)GetValue(SelectedOptionProperty); }
        //    set { SetValue(SelectedOptionProperty, value); }
        //}
        //public static readonly DependencyProperty SelectedOptionProperty = DependencyProperty.Register("SelectedOption", typeof(string), typeof(OptionControlView),
        //    new PropertyMetadata(new PropertyChangedCallback(SelectedOptionChanged)));
        //static void SelectedOptionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        public RadioButtonOptionView()
        {
            InitializeComponent();

        }
#if SILVERLIGHT2p0
        /// <summary>
        /// This is a work-around for a Silverlight 2.0 bug (not necessary in WPF). 
        /// Un-checks the previously checked radio buttons. Code from here:
        /// http://silverlight.net/forums/p/47810/165241.aspx#165241
        /// </summary>
        /// <param name="p_Container"></param>
        /// <param name="p_GroupName"></param>
        /// <param name="p_IgnoreId"></param>
        private void UncheckGroupRadioButtons(FrameworkElement p_Container, string p_GroupName, string p_IgnoreId)
        {
            int p_ChildCount = VisualTreeHelper.GetChildrenCount(p_Container);
            for (int i = 0; i < p_ChildCount; i++)
            {
                DependencyObject p_Obj = VisualTreeHelper.GetChild(p_Container, i);
                if (p_Obj is RadioButton)
                {
                    RadioButton p_curRadio = (RadioButton)p_Obj;
                    if (p_curRadio.GroupName == p_GroupName && p_curRadio.Tag.ToString() != p_IgnoreId)
                        p_curRadio.IsChecked = false;
                }
                else
                {
                    UncheckGroupRadioButtons((FrameworkElement)p_Obj, p_GroupName, p_IgnoreId);
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton p_curRadio = (RadioButton)sender;

            UncheckGroupRadioButtons(myOptionControl, p_curRadio.GroupName, p_curRadio.Tag.ToString());
        }
#endif
    }
}
