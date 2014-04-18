using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Vts.Gui.Silverlight.View
{
    /// <summary>
    /// Work-around class for proper databinding, implemented by Rocky Lhotka and described here:
    /// http://www.lhotka.net/weblog/CommentView,guid,f3353b7c-a1b5-41f2-a9bf-00f0c4e6a999.aspx#commentstart
    /// </summary>
    public class ListBox : System.Windows.Controls.ListBox
    {
        private object _selection;

        public ListBox()
        {
            this.Loaded += new RoutedEventHandler(ListBox_Loaded);
            this.SelectionChanged += new SelectionChangedEventHandler(ListBox_SelectionChanged);
        }

        #region SelectedValue DependencyProperty

        public static DependencyProperty SelectedValueProperty =
          DependencyProperty.Register("SelectedValue", typeof(object), typeof(ListBox),
          new PropertyMetadata((o, e) =>
          {
              ((ListBox)o).SetSelectionFromValue();
          }));

        public object SelectedValue
        {
            get
            {
                return ((object)(base.GetValue(ListBox.SelectedValueProperty)));
            }
            set
            {
                base.SetValue(ListBox.SelectedValueProperty, value);
            }
        }

        #endregion

        #region ValueMemberPath DependencyProperty

        public static DependencyProperty ValueMemberPathProperty =
          DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(ListBox), null);

        public string ValueMemberPath
        {
            get
            {
                return ((string)(base.GetValue(ListBox.ValueMemberPathProperty)));
            }
            set
            {
                base.SetValue(ListBox.ValueMemberPathProperty, value);
            }
        }

        #endregion

        #region Methods

        void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            SetSelectionFromValue();
        }

        void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
