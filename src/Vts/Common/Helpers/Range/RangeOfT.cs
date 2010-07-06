using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//[assembly: InternalsVisibleTo("Vts.IO")]
//[assembly: InternalsVisibleTo("Vts.Common.Test")] 

namespace Vts
{
    /// <summary>
    /// Immutable class that specifies a range of values and allows enumeration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Range<T> : BindableObject where T : struct
    {
        private T _Start;
        private T _Stop;
        private T _Delta;
        private int _Count;

        public Range(T start, T stop, int number)
        {
            _Start = start;
            _Stop = stop;
            _Count = number;
            _Delta = GetDelta();
        }

        /// <summary>
        /// todo: Figure out how to remove. The only reason this is here is to satisfy the DataContractSerializer
        /// </summary>
        public Range()
            : this(default(T), default(T), 1)
        {
        }

        /// <summary>
        /// The start of the range
        /// </summary>
        public T Start
        {
            get { return _Start; }
            set
            {
                _Start = value;
                _Delta = GetDelta();
                OnPropertyChanged("Start");
                OnPropertyChanged("Delta");
            }
        }

        /// <summary>
        /// The end of the range
        /// </summary>
        public T Stop
        {
            get { return _Stop; }
            set
            {
                _Stop = value;
                _Delta = GetDelta();
                OnPropertyChanged("Stop");
                OnPropertyChanged("Delta");
            }
        }
        
        /// <summary>
        /// The increment between successive numbers
        /// </summary>
        /// <remarks>Warning - updating Start, or Stop or Count will NOT update this property!</remarks>
        public T Delta
        {
            get { return _Delta; }
            set
            {
                _Delta = value;
                // _Count = GetNewCount();
                OnPropertyChanged("Delta");
                //OnPropertyChanged("Count");
            }
        }

        /// <summary>
        /// The number of values in the range, inclusive of the endpoints
        /// </summary>
        public int Count
        {
            get { return _Count; }
            set
            {
                _Count = value;
                _Delta = GetDelta();
                OnPropertyChanged("Count");
                OnPropertyChanged("Delta");
            }
        }

        protected abstract Func<T, T> GetIncrement();
        protected abstract T GetDelta();
        protected abstract int GetNewCount();

        public override string ToString()
        {
            var a = this.AsEnumerable().ToArray();
            var sb = new StringBuilder(String.Format("{0}", a.FirstOrDefault()), a.Length * 10);
            for (int i = 1; i < a.Length; i++)
            {
                sb.Append(String.Format(", {0}", a[i]));
            }
            return sb.ToString();
        }

        // todo: dc - revisit Ayende's INotifyPropertyChanged generics implementation
        public IEnumerable<T> AsEnumerable()
        {
            var increment = GetIncrement();

            T currentValue = Start;
            for (int i = 0; i < Count; i++, currentValue = increment(currentValue))
                yield return currentValue;
        }
    }
}
