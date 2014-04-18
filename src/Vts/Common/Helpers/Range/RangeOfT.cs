using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

//[assembly: InternalsVisibleTo("Vts.IO")]
//[assembly: InternalsVisibleTo("Vts.Common.Test")] 

namespace Vts
{
    /// <summary>
    /// Immutable class that specifies a range of values and allows enumeration
    /// </summary>
    /// <typeparam name="T">The type of the values in the range</typeparam>
    /// <remarks>Explicit data contract necessary for JSON.Net: http://stackoverflow.com/questions/19231367/serializing-poco-class-derived-from-baseclass-with-datacontract </remarks>
    [DataContract]
    public abstract class Range<T> : BindableObject where T : struct
    {
        private T _Start;
        private T _Stop;
        private T _Delta;
        private int _Count;

        /// <summary>
        /// Defines the range
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        /// <param name="number">The number of values in the range, inclusive of the endpoints</param>
        public Range(T start, T stop, int number)
        {
            _Start = start;
            _Stop = stop;
            _Count = number;
            _Delta = GetDelta();
        }

        /// <summary>
        /// The only reason this is here is to satisfy the DataContractSerializer
        /// </summary>
        /// todo: Figure out how to remove
        public Range()
            : this(default(T), default(T), 1)
        {
        }

        /// <summary>
        /// The start of the range
        /// </summary>
        [DataMember]
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
        [DataMember]
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
        [IgnoreDataMember]
        public T Delta
        {
            get { return _Delta; }
            set
            {
                _Delta = value;
                //_Count = GetNewCount();
                OnPropertyChanged("Delta");
                //OnPropertyChanged("Count");
            }
        }

        [DataMember]
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

        /// <summary>
        /// An abstract method to get the increment value. To be defined by the subclass
        /// </summary>
        /// <returns></returns>
        protected abstract Func<T, T> GetIncrement();
        /// <summary>
        /// An abstract method to get the delta value. To be defined by the subclass
        /// </summary>
        /// <returns>A value of type Time</returns>
        protected abstract T GetDelta();

        /// <summary>
        /// Returns a string that represents the range
        /// </summary>
        /// <returns>A string that represents the current range</returns>
        public override string ToString()
        {
            return "Start: " + Start.ToString() + 
                 ", Stop: " + Stop.ToString() + 
                 ", Count: " + Count.ToString() + 
                 ", Delta: " + Delta.ToString();
        }

        // todo: dc - revisit Ayende's INotifyPropertyChanged generics implementation
        /// <summary>
        /// Returns an IEnumerable of type Time that represents the range
        /// </summary>
        /// <returns>An IEnumerable of type Time that represents the range</returns>
        public IEnumerable<T> AsEnumerable()
        {
            var increment = GetIncrement();

            T currentValue = Start;
            for (int i = 0; i < Count; i++, currentValue = increment(currentValue))
                yield return currentValue;
        }
    }
}
