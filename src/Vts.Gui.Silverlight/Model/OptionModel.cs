using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vts.Gui.Silverlight.ViewModel;

namespace Vts.Gui.Silverlight.Model
{

    public abstract class OptionModel : BindableObject
    {

        protected const int UNSET_SORT_VALUE = Int32.MinValue;

        protected readonly string _displayName;
        protected readonly string _groupName;
        protected bool _isSelected;
        protected bool _isEnabled;
        protected readonly int _sortValue;
        protected readonly int _ID;
        protected readonly bool _multiSelectEnabled;
        
        public OptionModel(string displayName, int id, string groupName, bool enableMultiSelect, int sortValue)
        {
            _displayName = displayName;
            _groupName = groupName;
            _ID = id;
            _sortValue = sortValue;
            _multiSelectEnabled = enableMultiSelect;
            _isEnabled = true;
        }

        /// <summary>
        /// Returns the user-friendly name of this option.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
        }

        /// <summary>
        /// Returns the user-friendly name of this option.
        /// </summary>
        public string GroupName
        {
            get { return _groupName; }
        }

        /// <summary>
        /// Gets/sets whether this option is in the selected state.
        /// When this property is set to a new value, this object's
        /// PropertyChanged event is raised.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                    return;

                _isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets/sets whether this option is in the enabled state.
        /// When this property is set to a new value, this object's
        /// PropertyChanged event is raised.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                this.OnPropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        /// Returns the value used to sort this option.
        /// The default sort value is Int32.MinValue.
        /// </summary>
        public int SortValue
        {
            get { return _sortValue; }
        }

        /// <summary>
        /// Returns the value used to identify this option.
        /// </summary>
        public int ID
        {
            get { return _ID; }
        }

        /// <summary>
        /// multi-select
        /// </summary>
        public bool MultiSelectEnabled
        {
            get { return _multiSelectEnabled; }
        }
    }

    /// <summary>
    /// Represents a value with a user-friendly name that can be selected by the user.
    /// From Josh Smith &amp; Karl Schifflett's Code Project article on localization:
    /// <see cref="http://www.codeproject.com/KB/WPF/InternationalizedWizard.aspx">"http://www.codeproject.com/KB/WPF/InternationalizedWizard.aspx"</see>
    /// </summary>
    /// <typeparam name="TValue">The type of value represented by the option.</typeparam>
    public class OptionModel<TValue> :
        OptionModel,
        IComparable<OptionModel<TValue>> // where TValue : struct 
    {
        protected readonly TValue _value;

        public OptionModel(string displayName, TValue value, int id, string groupName, bool enableMultiSelect)
            : this(displayName, value, id, groupName, enableMultiSelect, UNSET_SORT_VALUE)
        {
        }

        public OptionModel(string displayName, TValue value, int id, string groupName, bool enableMultiSelect, int sortValue)
            : base(displayName, id, groupName, enableMultiSelect, sortValue)
        {
            _value = value;
        }

        /// <summary>
        /// Returns the user-friendly name of this option.
        /// </summary>
        public TValue Value
        {
            get { return _value; }
        }
        
        //public static ReadOnlyCollection<OptionViewModel<TValue>> CreateAvailableOptions(PropertyChangedEventHandler handler)
        /// <summary>
        /// Creates a Dictionary of options. If no options (params TValue[] values) are specified, it will use all of the available choices
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="allValues"></param>
        /// <returns></returns>
        /// 
        public static Dictionary<TValue, OptionModel<TValue>> CreateAvailableOptions(PropertyChangedEventHandler handler, string groupName, TValue initialValue, TValue[] allValues, bool enableMultiSelect)
        {
            Type enumType = typeof(TValue);
            var isStringEnum = enumType.Equals(typeof (string));
            if (!enumType.IsEnum && !isStringEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum or a string.");
            }

            List<OptionModel<TValue>> list = new List<OptionModel<TValue>>();
            if (allValues == null || allValues.Length == 0)
            {
                allValues = isStringEnum ? new TValue[0] : EnumHelper.GetValues<TValue>();
            }
            var names = allValues.Select(value => isStringEnum ? (value as string) : (value as Enum).GetInternationalizedString()).ToArray();

            for (int i = 0; i < allValues.Length; i++)
            {
                string name = names[i].Length > 0 ? names[i] : allValues[i].ToString();
                OptionModel<TValue> option = new OptionModel<TValue>(name, allValues[i], i, groupName, enableMultiSelect);
                option.PropertyChanged += handler;
                list.Add(option);
            }

            //removed call to sort, which was re-arranging the enum choices
            //convention is to let the first enum choice be the default selection
            //list.Sort();

            if (list.Count > 0)
            {
                var option = list.FirstOrDefault(optionModel => EqualityComparer<TValue>.Default.Equals(optionModel.Value, initialValue));
                option.IsSelected = true;
                option.IsEnabled = !enableMultiSelect;
                //list[0].IsSelected = true;
            }
            return list.ToDictionary(item => item.Value);
            //return new ReadOnlyCollection<OptionViewModel<TValue>>(list);
        }

        public int CompareTo(OptionModel<TValue> other)
        {
            if (other == null)
                return -1;

            if (this.SortValue == UNSET_SORT_VALUE && other.SortValue == UNSET_SORT_VALUE)
            {
                return this.DisplayName.CompareTo(other.DisplayName);
            }
            else if (this.SortValue != UNSET_SORT_VALUE && other.SortValue != UNSET_SORT_VALUE)
            {
                return this.SortValue.CompareTo(other.SortValue);
            }
            else if (this.SortValue != UNSET_SORT_VALUE && other.SortValue == UNSET_SORT_VALUE)
            {
                return -1;
            }
            else
            {
                return +1;
            }
        }
    }
}