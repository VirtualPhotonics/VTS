using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vts.Gui.Silverlight.View
{
    public partial class SolutionDomainOptionView : UserControl
    {
        public SolutionDomainOptionView()
        {
            InitializeComponent();
        }

        private void StackPanel_LayoutUpdated(object sender, EventArgs e)
        {
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
                stackPanel.VerticalAlignment = VerticalAlignment.Top;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var tbx = sender as TextBox;
            if (tbx != null && e.Key == Key.Enter)
                tbx.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
