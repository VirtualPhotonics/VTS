using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Vts.Gui.Silverlight.View
{
    /// <summary>
    /// Work-around class for proper databinding, implemented by Rocky Lhotka and described here:
    /// http://www.lhotka.net/weblog/CommentView,guid,f3353b7c-a1b5-41f2-a9bf-00f0c4e6a999.aspx#commentstart
    /// </summary>
    public class ComboBox : System.Windows.Controls.ComboBox
    {
        private object _selection;

        public ComboBox()
        {
            Loaded += ComboBox_Loaded;
            SelectionChanged +=ComboBox_SelectionChanged;
        }

        #region SelectedValue DependencyProperty

        public static DependencyProperty SelectedValueProperty =
          DependencyProperty.Register("SelectedValue", typeof(object), typeof(ComboBox),
          new PropertyMetadata((o, e) =>
          {
              ((ComboBox)o).SetSelectionFromValue();
          }));

        public object SelectedValue
        {
            get
            {
                return GetValue(SelectedValueProperty);
            }
            set
            {
                SetValue(SelectedValueProperty, value);
            }
        }

        #endregion

        #region ValueMemberPath DependencyProperty

        public static DependencyProperty ValueMemberPathProperty =
          DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(ComboBox), null);

        public string ValueMemberPath
        {
            get
            {
                return (string)GetValue(ValueMemberPathProperty);
            }
            set
            {
                SetValue(ValueMemberPathProperty, value);
            }
        }

        #endregion

        #region Methods

        void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            SetSelectionFromValue();
        }

        void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _selection = e.AddedItems[0];
                SelectedValue = GetMemberValue(_selection);
            }
            else
            {
                _selection = null;
                SelectedValue = null;
            }
        }

        private object GetMemberValue(object item)
        {
            if (string.IsNullOrEmpty(ValueMemberPath))
            {
                return item;
            }

            return item.GetType().GetProperty(ValueMemberPath).GetValue(item, null);
        }

        private void SetSelectionFromValue()
        {
            var value = SelectedValue;
            if (Items.Count > 0 && value != null)
            {
                var sel = (from item in Items
                           where GetMemberValue(item).Equals(value)
                           select item).Single();
                _selection = sel;
                SelectedItem = sel;
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            SetSelectionFromValue();
        }

        #endregion
    } 
}
