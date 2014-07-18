using System.Windows.Controls;
using System.Windows.Input;

namespace Vts.Gui.Silverlight.View
{
    // Since this is a control, we want dependency properties (or attached properties?)
    // This really seems to make the ViewModel redundant (it's much more testable, but also more complex...two copies of OpticalProperties)
    // todo: maybe these don't need ViewModels, and we just bind the to the ForwardSolverViewModel.OpticalProperties
    // of course, that doesn't solve the scalability problem...
    public partial class OpticalPropertyView : UserControl
    {
        public OpticalPropertyView()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx != null && e.Key == Key.Enter)
                tbx.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
