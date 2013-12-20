using System.Collections.Generic;
using System.ComponentModel;
using Vts.Gui.Silverlight.Model;

namespace Vts.Gui.Silverlight.ViewModel
{
    // todo: consider refactoring to derive from ObservableCollection:
    // http://www.thejoyofcode.com/ViewModels_and_CheckListBoxes.aspx

    /// <summary>
    /// View model exposing Enum choices with change notification
    /// </summary>
    public class OptionViewModel<TValue> : BindableObject
    {
        private TValue _SelectedValue;
        private string _SelectedDisplayName;
        private bool _ShowTitle;
        private string _GroupName;
        //private ReadOnlyCollection<OptionModel<TValue>> _Options;
        private Dictionary<TValue, OptionModel<TValue>> _Options;
        //private Dictionary<TValue, Dictionary<TValue, OptionModel<TValue>>> _SubOptions;

        
        public OptionViewModel(string groupName, bool showTitle, TValue initialValue, TValue[] allValues)
        {
            ShowTitle = showTitle;
            GroupName = groupName;

            // todo: CreateAvailableOptions should be owned by either this class or an OptionModelService class
            Options = OptionModel<TValue>.CreateAvailableOptions(OnOptionPropertyChanged, groupName, initialValue, allValues);

            SelectedValue = initialValue;
        }

        public OptionViewModel(string groupName, bool showTitle, TValue[] allValues)
            : this(groupName, showTitle, default(TValue), allValues)
        {
        }

        public OptionViewModel(string groupName, bool showTitle, TValue initialValue)
            : this(groupName, showTitle, initialValue, null)
        {
        }

        public OptionViewModel(string groupName, TValue initialValue)
            : this(groupName, true, initialValue, null)
        {
        }

        public OptionViewModel(string groupName, TValue[] allValues)
            : this(groupName, true, default(TValue), allValues)
        {
        }

        public OptionViewModel(string groupName, bool showTitle)
            : this(groupName, showTitle, default(TValue), null)
        {
        }

        public OptionViewModel(string groupName)
            : this(groupName, true, default(TValue), null)
        {
        }

        public TValue SelectedValue
        {
            get { return _SelectedValue; }
            set
            {
                _SelectedValue = value;
                this.OnPropertyChanged("SelectedValue");
            }
        }
        public string SelectedDisplayName
        {
            get { return _SelectedDisplayName; }
            set
            {
                _SelectedDisplayName = value;
                this.OnPropertyChanged("SelectedDisplayName");
            }
        }

        public bool ShowTitle
        {
            get { return _ShowTitle; }
            set
            {
                _ShowTitle = value;
                this.OnPropertyChanged("ShowTitle");
            }
        }

        public string GroupName
        {
            get { return _GroupName; }
            set
            {
                _GroupName = value;
                this.OnPropertyChanged("GroupName");
            }
        }

        //public ReadOnlyCollection<OptionViewModel<TValue>> Options
        public Dictionary<TValue, OptionModel<TValue>> Options
        {
            get { return _Options; }
            set
            {
                _Options = value;
                this.OnPropertyChanged("Options");
            }
        }

        void OnOptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OptionModel<TValue> option = sender as OptionModel<TValue>;
            if (option.IsSelected)
            {
                this.SelectedValue = option.Value;
                this.SelectedDisplayName = option.DisplayName;
            }
        }
    }
}
