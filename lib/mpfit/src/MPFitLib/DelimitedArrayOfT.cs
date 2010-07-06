/* 
 * MINPACK-1 Least Squares Fitting Library
 *
 * Original public domain version by B. Garbow, K. Hillstrom, J. More'
 *   (Argonne National Laboratory, MINPACK project, March 1980)
 * See the file DISCLAIMER for copyright information.
 * 
 * Tranlation to C Language by S. Moshier (moshier.net)
 * Translation to C# Language by D. Cuccia (http://davidcuccia.wordpress.com)
 * 
 * Enhancements and packaging by C. Markwardt
 *   (comparable to IDL fitting routine MPFIT
 *    see http://cow.physics.wisc.edu/~craigm/idl/idl.html)
 */

/* Helper class for strongly-typed IList<T> subarray. Allows indexing into 
 * larger parent array for safety (no pointers or unsafe compiler flags) 
   $Id: DelimitedArrayOfT.cs,v 1.0 2010/04/23 dcuccia Exp $
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MPFitLib
{
    /// <summary>
    /// Class that represents a "sub-array" within a larger array by implementing
    /// appropriate indexing using an offset and sub-count. This was implemented in
    /// the C# version in order to preserve the existing code semantics while also
    /// allowing the code to be compiled w/o use of /unsafe compilation flag. This
    /// permits execution of the code in low-trust environments, such as that required
    /// by the CoreCLR runtime of Silverlight (Mac/PC) and Moonlight (Linux)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>Note - modifications to this structure will modify the parent (source) array!</remarks>
    public class DelimitedArray<T> : IList<T> where T : struct
    {
        private int _offset;
        private int _count;
        private T[] _array;

        public DelimitedArray(T[] array, int offset, int count)
        {
            _array = array;
            _offset = offset;
            _count = count;
        }

        public void SetOffset(int offset)
        {
            if (offset + _count > _array.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            _offset = offset;
        }

        public void SetOffsetAndCount(int offset, int count)
        {
            if (offset + count > _array.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            _offset = offset;
            _count = count;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _offset; i < _offset + _count; i++)
            {
                yield return _array[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IList<T> Members

        public int Count
        {
            get { return _count; }
        }

        public T this[int index]
        {
            get
            {
                return _array[_offset + index];
            }
            set
            {
                _array[_offset + index] = value;
            }
        }

        public int IndexOf(T item)
        {
            var query =
                (from i in Enumerable.Range(_offset, _count)
                 where _array[i].Equals(item)
                 select i);

            foreach (var i in query)
            {
                return i - _offset;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection<T> Members

        public bool Contains(T item)
        {
            return ((IEnumerable<T>)this).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_array, _offset, array, arrayIndex, _count);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}