using System.ComponentModel;

namespace Vts
{
    /// <summary>
    /// An adaptation of the INotifyPropertyChanged interface to include
    /// OnPropertyChanged method.
    /// </summary>
    public interface INotifyPropertyChangedPlus : INotifyPropertyChanged
    {
        /// <summary>
        /// Method to raises the property changed event
        /// </summary>
        /// <param name="propertyName">The property that changed</param>
        void OnPropertyChanged(string propertyName);
    }

}
