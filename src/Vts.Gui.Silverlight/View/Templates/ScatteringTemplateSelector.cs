using System.Windows;
using System.Windows.Controls;
using Vts.SpectralMapping;

namespace Vts.Gui.Silverlight.View
{
    public class ScatteringTemplateSelector : UserControl
    {
        public DataTemplate MieScatteringTemplate { get; set; }
        public DataTemplate PowerLawScatteringTemplate { get; set; }
        public DataTemplate IntralipidScatteringTemplate { get; set; }

        private static void UpdateScatteringType(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var selector = obj as ScatteringTemplateSelector;

            string scattType = e.NewValue as string;
            if (scattType == typeof(MieScatterer).FullName)
            {
                selector.Content = selector.MieScatteringTemplate.LoadContent() as UIElement;
            }
            else if (scattType == typeof(PowerLawScatterer).FullName)
            {
                selector.Content = selector.PowerLawScatteringTemplate.LoadContent() as UIElement;
            }
            else if (scattType == typeof(IntralipidScatterer).FullName)
            {
                selector.Content = selector.IntralipidScatteringTemplate.LoadContent() as UIElement;
            }
        }

        public static readonly DependencyProperty ScatteringTypeProperty = DependencyProperty.Register(
            "ScatteringType", 
            typeof(string), 
            typeof(ScatteringTemplateSelector),
            new PropertyMetadata(string.Empty, UpdateScatteringType));

        public string ScatteringType
        {
            get { return (string)GetValue(ScatteringTypeProperty); }
            set { SetValue(ScatteringTypeProperty, value); }
        }

        public ScatteringTemplateSelector()
        {
            Loaded += (s, a) => ScatteringType = "Vts.SpectralMapping.PowerLawScatterer";
        }
    }
}
