using System;
using System.Collections.Generic;
using System.IO;

namespace Vts.IO
{
    /// <summary>
    /// Class to write specified types to a binary stream
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    public class CustomBinaryStreamWriter<T> : IDisposable
    {
        private string _filename;
        private Action<BinaryWriter, T> _writeMap;

        private Stream _stream;
        private BinaryWriter _binaryWriter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">Name of the binary file to write</param>
        /// <param name="binaryWriter"></param>
        public CustomBinaryStreamWriter(
            string fileName,
            ICustomBinaryWriter<T> binaryWriter)
            : this(fileName, binaryWriter.WriteToBinary)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Name of the binary file</param>
        /// <param name="writeMap"></param>
        public CustomBinaryStreamWriter(
            string fileName,
            Action<BinaryWriter, T> writeMap)
        {
            _filename = fileName;
            _writeMap = writeMap;

            IsOpen = false;
            Count = 0;

            OpenStream();
        }

        /// <summary>
        /// Value to check if the file is open for writing
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <summary>
        /// The number of items that have been written
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Optional code that will be run before the database begins writing (after call to Open())
        /// </summary>
        public Action PreWriteAction { get; set; }

        /// <summary>
        /// Optional code that will be run after the database has completed writing (on call to Close() or Dispose())
        /// </summary>
        public Action PostWriteAction { get; set; }

        /// <summary>
        /// Opens the filestream for subsequent calls to WriteDataPoint or WriteDataPoints
        /// </summary>
        private void OpenStream()
        {
            try
            {
                _stream = StreamFinder.GetFileStream(_filename, FileMode.Create);
                _binaryWriter = new BinaryWriter(_stream);

                IsOpen = true;

                if (PreWriteAction != null)
                {
                    PreWriteAction();
                }
            }
            catch (IOException e)
            {
                Close();
            }
        }

        /// <summary>
        /// Writes a single data point to the underlying filestream
        /// </summary>
        /// <param name="item">Single data point to be written</param>
        public void Write(T item)
        {
            _writeMap(_binaryWriter, item);
            Count++;
        }

        /// <summary>
        /// Writes an enumerable list of data points to the underlying filestream
        /// </summary>
        /// <param name="items">A list of data points to be written</param>
        public void Write(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Write(item);
                Count++;
            }
        }

        /// <summary>
        /// Closes the filestream and writes the accompanying .xml
        /// </summary>
        public virtual void Close()
        {
            Dispose();
        }

        #region IDisposable Members

        private bool _disposed = false;
        // Do not make this method virtual.
        // A derived class should not be able to override this method.

        /// <summary>
        /// Closes the file and writes the accompanying .xml
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_binaryWriter != null)
                    {
                        _binaryWriter.Close();
                    }

                    if (_stream != null)
                    {
                        _stream.Close();
                    }

                    if (PostWriteAction != null)
                    {
                        PostWriteAction();
                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                _disposed = true;
            }

        }

        // Use interop to call the method necessary
        // to clean up the unmanaged resource.
        //[System.Runtime.InteropServices.DllImport("Kernel32")]
        //private extern static Boolean CloseHandle(IntPtr handle);

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        /// <summary>
        /// Custom binary stream writer
        /// </summary>
        ~CustomBinaryStreamWriter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion
    }
}
