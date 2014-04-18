using SLExtensions.Input;
using Vts.Gui.Silverlight.Input;

namespace Vts.Gui.Silverlight.ViewModel
{
    public class TextOutputViewModel : BindableObject
    {
        private string _Text;

        public TextOutputViewModel()
        {
            Commands.TextOutput_PostMessage.Executed += PostMessage_Executed;
        }

        public string Text
        {
            get { return _Text; }
            set 
            {
                _Text = value; 
                OnPropertyChanged("Text");
            }
        }

        void PostMessage_Executed(object sender, ExecutedEventArgs e)
        {
            string s = e.Parameter as string;
            if (s != null)
                Text += s;
        }

        public void AppendText(string s)
        {
            Text += s;
        }
    }
}
