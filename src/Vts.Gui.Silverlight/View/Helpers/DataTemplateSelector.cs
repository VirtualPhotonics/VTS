using System.Windows;
using System.Windows.Controls;

namespace Vts.Gui.Silverlight.View
{
    // from here: http://www.codeproject.com/Articles/92439/Silverlight-DataTemplateSelector
    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(
            object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(
            object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }
}
