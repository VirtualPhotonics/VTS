using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vts
{
    /// <summary>
    /// Immutable class that specifies a range of values and allows enumeration
    /// </summary>
    /// <typeparam name="T">The type of the values in the range</typeparam>
    /// <remarks>Explicit data contract necessary for JSON.Net: http://stackoverflow.com/questions/19231367/serializing-poco-class-derived-from-baseclass-with-datacontract </remarks>
    [DataContract]
    [JsonObject]
    public abstract class Range<T> : BindableObject, IEnumerable<T> where T : struct
    {
        private T _start;
        private T _stop;
        private T _delta;
        private int _count;

        /// <summary>
        /// Defines the range
        /// </summary>
        /// <param name="start">The start of the range</param>
        /// <param name="stop">The end of the range</param>
        /// <param name="number">The number of values in the range, inclusive of the endpoints</param>
        protected Range(T start, T stop, int number)
        {
            Start = start;
            Stop = stop;
            Count = number;
        }

        /// <summary>
        /// The only reason this is here is to satisfy the DataContractSerializer
        /// </summary>
        protected Range()
            : this(default, default, 1)
        {
        }

        /// <summary>
        /// The start of the range
        /// </summary>
        [DataMember]
        public T Start
        {
            get => _start;
            set
            {
                _start = value;
                _delta = GetDelta();
                OnPropertyChanged(nameof(Start));
                OnPropertyChanged(nameof(Delta));
            }
        }

        /// <summary>
        /// The end of the range
        /// </summary>
        [DataMember]
        public T Stop
        {
            get => _stop;
            set
            {
                _stop = value;
                _delta = GetDelta();
                OnPropertyChanged(nameof(Stop));
                OnPropertyChanged(nameof(Delta));
            }
        }
        
        /// <summary>
        /// The increment between successive numbers
        /// </summary>
        [IgnoreDataMember]
        public T Delta
        {
            get => _delta;
            set
            {
                _delta = value;
                OnPropertyChanged(nameof(Delta));
            }
        }
        /// <summary>
        /// The number of values in the range, inclusive of the endpoints
        /// </summary>
        [DataMember]
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                _delta = GetDelta();
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(nameof(Delta));
            }
        }

        /// <summary>
        /// An abstract method to get the increment value. To be defined by the subclass
        /// </summary>
        /// <returns>the increment function</returns>
        protected abstract Func<T, T> GetIncrement();
        /// <summary>
        /// An abstract method to get the delta value. To be defined by the subclass
        /// </summary>
        /// <returns>the Delta value of class</returns>
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

        /// <summary>
        /// Returns an IEnumerator of type T that enumerates the range
        /// </summary>
        /// <returns>An IEnumerator of type T that enumerates the range</returns>
        public IEnumerator<T> GetEnumerator() => AsEnumerable().GetEnumerator();

        /// <summary>
        /// Explicit interface implementation to hide the non-generic version
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <summary>
        /// Returns an IEnumerable of type T that enumerates the range
        /// </summary>
        /// <returns>An IEnumerable of type T that enumerates the range</returns>
        [Obsolete("This method is deprecated. Use built-in IEnumerable implementation instead.")]
        public IEnumerable<T> AsEnumerable()
        {
            var increment = GetIncrement();

            var currentValue = Start;
            for (var i = 0; i < Count; i++, currentValue = increment(currentValue))
                yield return currentValue;
        }
    }
}
