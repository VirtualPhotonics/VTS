using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace Vts
{
    /// <summary>
    /// Implements the INotifyPropertyChanged interface and 
    /// exposes a RaisePropertyChanged method for derived 
    /// classes to raise the PropertyChange event.  The event 
    /// arguments created by this class are cached to prevent 
    /// managed heap fragmentation.
    /// 
    /// Adapted from Josh Smith's post:
    /// http://joshsmithonwpf.wordpress.com/2007/08/29/a-base-class-which-implements-inotifypropertychanged/
    /// 
    /// Added SetProperty method to simplify set operations.
    /// Changed "RaisePropertyChanged" to "OnPropertyChanged" so as to work with INotifyPropertyChangedPlus,
    /// enabling compatibility with the DependsOn attribute and PropertyDependencyManager classes
    /// </summary>
    //[Serializiable]
    // svn test
    [DataContract]
    public abstract class BindableObject : INotifyPropertyChangedPlus
    {
        #region Data

        private static readonly Dictionary<string, PropertyChangedEventArgs> eventArgCache;
        private const string ERROR_MSG = "{0} is not a public property of {1}";

        #endregion // Data

        #region Constructors

        static BindableObject()
        {
            eventArgCache = new Dictionary<string, PropertyChangedEventArgs>();
        }

        protected BindableObject()
        {
        }

        #endregion // Constructors

        #region Public Members

        /// <summary>
        /// Raised when a public property of this object is set.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns an instance of PropertyChangedEventArgs for 
        /// the specified property name.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to create event args for.
        /// </param>		
        public static PropertyChangedEventArgs
            GetPropertyChangedEventArgs(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException(
                    "propertyName cannot be null or empty.");

            PropertyChangedEventArgs args;
            
            // Get the event args from the cache, creating them and adding to the cache if necessary.
            if (!eventArgCache.TryGetValue(propertyName, out args)) // ContainsKey and indexer together
            {
                eventArgCache.Add(propertyName, args = new PropertyChangedEventArgs(propertyName)); 
            }
            return args;
        }

        #endregion // Public Members

        #region Protected Members

        ///// <summary>
        ///// Helper class to consolidate set operations
        ///// </summary>
        ///// <typeparam name="Time"></typeparam>
        ///// <param name="parameter"></param>
        ///// <param name="value"></param>
        //protected void SetProperty<Time>(string propertyName, ref Time parameter, ref Time value)
        //{
        //    if (!EqualityComparer<Time>.Default.Equals(parameter, value))
        //    {
        //        parameter = value;
        //        OnPropertyChanged(propertyName);
        //    }
        //}

        /// <summary>
        /// Derived classes can override this method to
        /// execute logic after a property is set. The 
        /// base implementation does nothing.
        /// </summary>
        /// <param name="propertyName">
        /// The property which was changed.
        /// </param>
        protected virtual void AfterPropertyChanged(string propertyName)
        {
        }

        /// <summary>
        /// Attempts to raise the PropertyChanged event, and 
        /// invokes the virtual AfterPropertyChanged method, 
        /// regardless of whether the event was raised or not.
        /// </summary>
        /// <param name="propertyName">
        /// The property which was changed.
        /// </param>
        public void OnPropertyChanged(string propertyName)
        {
            this.VerifyProperty(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                // Get the cached event args.
                PropertyChangedEventArgs args = GetPropertyChangedEventArgs(propertyName);

                // Raise the PropertyChanged event.
                handler(this, args);
            }

            this.AfterPropertyChanged(propertyName);
        }


        #endregion // Protected Members

        #region Private Helpers

        [Conditional("DEBUG")]
        private void VerifyProperty(string propertyName)
        {
            Type type = this.GetType();

            // Look for a public property with the specified name.
            PropertyInfo propInfo = type.GetProperty(propertyName);

            if (propInfo == null)
            {
                // The property could not be found,
                // so alert the developer of the problem.

                string msg = string.Format(
                    ERROR_MSG,
                    propertyName,
                    type.FullName);

                throw new Exception(msg);
            }
        }

        #endregion // Private Helpers

    }
}
