using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vts.Extensions;
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
        private bool _enableMultiSelect;
        private TValue[] _selectedValues;
        private string[] _selectedDisplayNames;
        private TValue[] _unSelectedValues;
        private string[] _unSelectedDisplayNames;
        private Dictionary<TValue, OptionModel<TValue>> _Options;

        
        public OptionViewModel(string groupName, bool showTitle, TValue initialValue, TValue[] allValues, bool enableMultiSelect = false)
        {
            ShowTitle = showTitle;
            GroupName = groupName;
            _enableMultiSelect = enableMultiSelect;

            // todo: CreateAvailableOptions should be owned by either this class or an OptionModelService class
            Options = OptionModel<TValue>.CreateAvailableOptions(OnOptionPropertyChanged, groupName, initialValue, allValues, _enableMultiSelect);

            SelectedValue = initialValue;

            UpdateOptionsNamesAndValues();
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

        public bool EnableMultiSelect
        {
            get { return _enableMultiSelect; }
            set
            {
                _enableMultiSelect = value;
                this.OnPropertyChanged("EnableMultiSelect");
            }
        }

        // todo: created this in parallel with SelectedValue, so as not to break other code. need to merge functionality across codebase to use this version
        public TValue[] SelectedValues
        {
            get { return _selectedValues; }
            set
            {
                _selectedValues = value;
                this.OnPropertyChanged("SelectedValues");
            }
        }

        public string[] SelectedDisplayNames
        {
            get { return _selectedDisplayNames; }
            set
            {
                _selectedDisplayNames = value;
                this.OnPropertyChanged("SelectedDisplayNames");
            }
        }

        public TValue[] UnSelectedValues
        {
            get { return _unSelectedValues; }
            set
            {
                _unSelectedValues = value;
                this.OnPropertyChanged("UnSelectedValues");
            }
        }

        public string[] UnSelectedDisplayNames
        {
            get { return _unSelectedDisplayNames; }
            set
            {
                _unSelectedDisplayNames = value;
                this.OnPropertyChanged("UnSelectedDisplayNames");
            }
        }

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

            UpdateOptionsNamesAndValues();

            if (e.PropertyName != "IsEnabled" && EnableMultiSelect && Options != null)
            {
                var MIN_CHOICES = 1;
                var MAX_CHOICES = 2;
                var numSelected = (from o in _Options where o.Value.IsSelected select o).Count();

                // disable the unselected choices beyond a MAX_CHOICES number of concurrent multi-select options
                Options.Where(o => !o.Value.IsSelected).ForEach(o => o.Value.IsEnabled = numSelected < MAX_CHOICES);

                // if there is only MIN_CHOICES selected choice in multi-select mode, disable others from being further unselected
                Options.Where(o => o.Value.IsSelected).ForEach(o => o.Value.IsEnabled = numSelected > MIN_CHOICES);
            }
        }

        private void UpdateOptionsNamesAndValues()
        {
            if (_Options == null)
                return;

            // todo: created these in parallel with SelectedValue, so as not to break other code. need to merge functionality across codebase to use SelectedValues
            var selectedOptions = (from o in _Options where o.Value.IsSelected select o).ToArray();
            var unSelectedOptions = (from o in _Options where !o.Value.IsSelected select o).ToArray();

            // update arrays and explicitly fire property changed, so we don't trip on intermediate changes 
            _selectedValues = selectedOptions.Select(item => item.Value.Value).ToArray();
            _selectedDisplayNames = selectedOptions.Select(item => item.Value.DisplayName).ToArray();
            _unSelectedValues = unSelectedOptions.Select(item => item.Value.Value).ToArray();
            _unSelectedDisplayNames = unSelectedOptions.Select(item => item.Value.DisplayName).ToArray();
            this.OnPropertyChanged("SelectedValues");
            this.OnPropertyChanged("SelectedDisplayNames");
            this.OnPropertyChanged("UnSelectedValues");
            this.OnPropertyChanged("UnSelectedDisplayNames");
        }
    }
}
