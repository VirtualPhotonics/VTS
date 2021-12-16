using System.Collections.Generic;

namespace Vts
{
    /// <summary>
    /// Adds change tracking to a bindable object class
    /// </summary>
    public class BindableObjectWithChangeTracking : BindableObject, IChangeTracking
    {
        /// <summary>
        /// Holds the "original" values to monitor if values have changed 
        /// </summary>
        private Dictionary<string, object> originalValues = new Dictionary<string, object>();

        /// <summary>
        /// Specifies if anything in the class (that's being monitored) has changed
        /// </summary>
        public bool SettingsChanged { get; protected set; }

        /// <summary>
        /// Reset all changes once they have been handled
        /// </summary>
        public void Reset()
        {
            // assign new dictionary with current values as "original"?
            originalValues.Clear();
            SettingsChanged = false;
        }

        /// <summary>
        /// Helper method to consolidate set operations
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <param name="parameter">the parameter value</param>
        /// <param name="value">The new value</param>
        protected void SetProperty<T>(string propertyName, ref T parameter, ref T value)
        {
            if (parameter != null && value != null && !EqualityComparer<T>.Default.Equals(parameter, value))
            {
                parameter = value;
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Method to handle change tracking
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <param name="parameter">not used - need to remove</param>
        /// <param name="value">The property value</param>
        protected void HandleChangeTracking<T>(string propertyName, ref T parameter, ref T value) where T : struct
        {
            object originalValue;
            // If there's an "original" value stored
            if (originalValues.TryGetValue(propertyName, out originalValue))
            {
                // and if it's the same value as the new one
                if (EqualityComparer<T>.Default.Equals((T)originalValue, value)) // Throws InvalidCastException
                {
                    // then there's no reason to flag the value as changed 
                    // (makes for a smarter UI - value can be modified and "unmodified" transparently
                    SettingsChanged = false;
                }
                else
                {   // otherwise, set t
                    SettingsChanged = true;
                }
            }
            else
            {
                // add the value to the list so we can compare next time
                originalValues.Add(propertyName, value);
                SettingsChanged = true;
            }
        }
    }
}
