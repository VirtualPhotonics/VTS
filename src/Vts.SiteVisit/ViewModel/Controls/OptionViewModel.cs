using System.Collections.Generic;
using System.ComponentModel;
using Vts.SiteVisit.Model;

namespace Vts.SiteVisit.ViewModel
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

        public OptionViewModel(string groupName, bool showTitle, params TValue[] values)
        {
            ShowTitle = showTitle;
            GroupName = groupName;

            // todo: CreateAvailableOptions should be owned by either this class or an OptionModelService class
            Options = OptionModel<TValue>.CreateAvailableOptions(OnOptionPropertyChanged, groupName, values);
            //_SubOptions =
            //    (from k in Options.Keys.Where(key=>key.HasSubOptions())
            //     select OptionModel<k.GetType>.CreateAvailableOptions(OnOptionPropertyChanged, groupName,k.GetSubOptions().ToArray())).ToArray();
        }
        public OptionViewModel(string groupName, params TValue[] values) : this(groupName, false, values) { }


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
