using System.Windows;
using System.Windows.Controls;
using Vts.Gui.Silverlight.ViewModel;

namespace Vts.Gui.Silverlight.View
{
    public partial class TextOutputView : UserControl
    {
        private TextOutputViewModel _textOutputVM;
        public TextOutputView()
        {
            InitializeComponent();

            this.Loaded += TextOutputView_Loaded;
        }

        void TextOutputView_Loaded(object sender, RoutedEventArgs e)
        {
            _textOutputVM = this.DataContext as TextOutputViewModel;
            if (_textOutputVM != null)
            {
                _textOutputVM.PropertyChanged += textOutputVM_PropertyChanged;
            }
        }

        void textOutputVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                scrollViewer.ScrollToBottom();
            }
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
