using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vts.Extensions;
using Vts.Gui.Silverlight.ViewModel;

namespace Vts.Gui.Silverlight.View
{
    public partial class RadioButtonOptionView : UserControl
    {
        public RadioButtonOptionView()
        {
            InitializeComponent();

        }

        //private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        //{
        //    var checkbox = sender as CheckBox;
        //    if (checkbox != null)
        //    {
        //        if (this.DataContext is OptionViewModel<IndependentVariableAxis>)
        //        {
        //            var dc = (OptionViewModel<IndependentVariableAxis>)this.DataContext;

        //            if (dc.EnableMultiSelect)
        //            {
        //                if (dc.Options.Count > 2 && dc.SelectedValues.Length == dc.Options.Count - 1)
        //                {
        //                    dc.Options.Where(option => !option.Value.IsSelected).ForEach(option =>
        //                    {
        //                        option.Value.IsSelected = false;
        //                        option.Value.IsEnabled = false;
        //                    });
        //                }
        //                else
        //                {
        //                    dc.Options.Where(option => !option.Value.IsSelected).ForEach(option => option.Value.IsEnabled = true);
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
