using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Vts.SiteVisit.Model
{
    /// <summary>
    /// Represents a value with a user-friendly name that can be selected by the user.
    /// From Josh Smith &amp; Karl Schifflett's Code Project article on localization:
    /// <see cref="http://www.codeproject.com/KB/WPF/InternationalizedWizard.aspx">"http://www.codeproject.com/KB/WPF/InternationalizedWizard.aspx"</see>
    /// </summary>
    /// <typeparam name="TValue">The type of value represented by the option.</typeparam>
    public class OptionModel<TValue> :
        BindableObject,
        IComparable<OptionModel<TValue>>
    {
        #region Fields

        const int UNSET_SORT_VALUE = Int32.MinValue;
        
        readonly string _displayName;
        readonly string _groupName;
        bool _isSelected;
        readonly int _sortValue;
        readonly int _ID;
        readonly TValue _value;

        #endregion // Fields

        #region Constructor

        public OptionModel(string displayName, TValue value, int id, string groupName)
            : this(displayName, value, id, groupName, UNSET_SORT_VALUE)
        {
        }

        public OptionModel(string displayName, TValue value, int id, string groupName, int sortValue)
        {
            _displayName = displayName;
            _groupName = groupName;
            _value = value;
            _ID = id;
            _sortValue = sortValue;
        }

        #endregion // Constructor

        #region Properties

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
        /// Returns the user-friendly name of this option.
        /// </summary>
        public TValue Value
        {
            get { return _value; }
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

        #endregion // Properties

        #region Methods

        //public static ReadOnlyCollection<OptionViewModel<TValue>> CreateAvailableOptions(PropertyChangedEventHandler handler)
        /// <summary>
        /// Creates a Dictionary of options. If no options (params TValue[] values) are specified, it will use all of the available choices
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Dictionary<TValue, OptionModel<TValue>> CreateAvailableOptions(PropertyChangedEventHandler handler, string groupName, params TValue[] values)
        {
            Type enumType = typeof(TValue);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            List<OptionModel<TValue>> list = new List<OptionModel<TValue>>();
            if (values.Length == 0)
            {
                values = EnumHelper.GetValues<TValue>();
            }
            var names = values.Select(value => (value as Enum).GetInternationalizedString()).ToArray();

            for (int i = 0; i < values.Length; i++)
            {
                string name = names[i].Length > 0 ? names[i] : values[i].ToString();
                OptionModel<TValue> option = new OptionModel<TValue>(name, values[i], i, groupName);
                option.PropertyChanged += handler;
                list.Add(option);
            }

            //removed call to sort, which was re-arranging the enum choices
            //convention is to let the first enum choice be the default selection
            //list.Sort();

            if (list.Count > 0)
                list[0].IsSelected = true;
            return list.ToDictionary(item => item.Value);
            //return new ReadOnlyCollection<OptionViewModel<TValue>>(list);
        }

        #endregion // Methods

        #region IComparable<OptionViewModel<TValue>> Members

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

        #endregion // IComparable<OptionViewModel<TValue>> Members
    }
}