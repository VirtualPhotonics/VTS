using System.ComponentModel;

namespace Vts.Test
{
    public class BindableTestClass : BindableObjectWithChangeTracking
    {
        private int _first;
        private int _third;

        public int First
        {
            get => _first;
            set
            {
                HandleChangeTracking(nameof(First), ref _first, ref _first);
                SetProperty(nameof(First), ref _first, ref value);
                _first = value;
            }
        }

        [DependsOn("Third")]
        public int Second { get; set; }

        public int Third
        {
            get => _third;
            set
            {
                _third = value;
                OnPropertyChanged(nameof(Third));
            }
        }

        /// <summary>
        /// counter for how many properties were changed
        /// </summary>
        public int PropertyChangedCount { get; set; }

        public BindableTestClass()
        {
            PropertyChanged += MyTestClass_PropertyChanged;
        }

        private void MyTestClass_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // increment the change counter by 1
            PropertyChangedCount += 1;
        }
    }
}
