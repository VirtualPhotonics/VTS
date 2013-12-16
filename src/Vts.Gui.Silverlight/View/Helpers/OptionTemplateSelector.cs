using System.Windows;
using Vts.Gui.Silverlight.Model;

namespace Vts.Gui.Silverlight.View.Helpers
{
    // from here: http://www.codeproject.com/Articles/92439/Silverlight-DataTemplateSelector
    public class OptionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MultiSelectTemplate
        {
            get;
            set;
        }

        public DataTemplate SingleSelectTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var option = item as OptionModel;
            if (option != null)
            {
                if (option.MultiSelectEnabled)
                {
                    return MultiSelectTemplate;
                }
            }
            
            return SingleSelectTemplate;

            //return base.SelectTemplate(item, container);
        }
    }
}
