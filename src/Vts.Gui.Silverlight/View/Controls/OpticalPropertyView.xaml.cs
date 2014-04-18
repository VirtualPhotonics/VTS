using System.Windows;
using System.Windows.Controls;

namespace Vts.Gui.Silverlight.View
{
    // Since this is a control, we want dependency properties (or attached properties?)
    // This really seems to make the ViewModel redundant (it's much more testable, but also more complex...two copies of OpticalProperties)
    // todo: maybe these don't need ViewModels, and we just bind the to the ForwardSolverViewModel.OpticalProperties
    // of course, that doesn't solve the scalability problem...
    public partial class OpticalPropertyView : UserControl
    {
        public double Mua
        {
            get { return (double)GetValue(MuaProperty); }
            set { SetValue(MuaProperty, value); }
        }
        public static readonly DependencyProperty MuaProperty =
            DependencyProperty.Register("Mua", typeof(double), typeof(OpticalPropertyView),
            new PropertyMetadata(0.01, new PropertyChangedCallback(OpticalPropertyView.OnMuaPropertyChanged)));

        private static void OnMuaPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {

        }

        public OpticalPropertyView()
        {
            InitializeComponent();
        }
    }
}
