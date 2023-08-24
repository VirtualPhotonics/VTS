#nullable enable
using System;
using System.Collections.Generic;
using System.IO;

namespace Vts.IO;

/// <summary>
/// Class to write specified types to a binary stream
/// </summary>
/// <typeparam name="T">Type of the data</typeparam>
public class CustomBinaryStreamWriter<T> : IDisposable
{
    private readonly string _filename;
    private readonly Action<BinaryWriter, T> _writeMap;
    private Stream? _stream;
    private BinaryWriter? _binaryWriter;
    private bool _disposedValue;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="fileName">Name of the binary file to write</param>
    /// <param name="binaryWriter">ICustomBinaryWriter of generic type T</param>
    public CustomBinaryStreamWriter(
        string fileName,
        ICustomBinaryWriter<T> binaryWriter)
        : this(fileName, binaryWriter.WriteToBinary)
    {
    }

    /// <summary>
    /// Custom binary stream writer
    /// </summary>
    /// <param name="fileName">Name of the binary file</param>
    /// <param name="writeMap">Action on BinaryWriter and generic type T</param>
    public CustomBinaryStreamWriter(
        string fileName,
        Action<BinaryWriter, T> writeMap)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(writeMap);

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
    public Action PreWriteAction { get; set; } = () => { };

    /// <summary>
    /// Optional code that will be run after the database has completed writing (on call to Close() or Dispose())
    /// </summary>
    public Action PostWriteAction { get; set; } = () => { };

    /// <summary>
    /// Opens the filestream for subsequent calls to WriteDataPoint or WriteDataPoints
    /// </summary>
    private void OpenStream()
    {
        try
        {
            // guard against directory not existing ahead of time
            var path = Path.GetDirectoryName(_filename);
            if (!string.IsNullOrWhiteSpace(path))
            {
                Directory.CreateDirectory(path);
            }

            _stream = StreamFinder.GetFileStream(_filename, FileMode.Create);
            if (_stream is null)
            {
                throw new IOException("Could not open filestream for writing");
            }

            _binaryWriter = new BinaryWriter(_stream);

            IsOpen = true;

            PreWriteAction?.Invoke();
        }
        catch (IOException)
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
        _writeMap(_binaryWriter!, item); // _binaryWriter is guaranteed to be non-null here
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
    public void Close()
    {
        Dispose();
    }

    /// <summary>
    /// Internal method for disposing the IDisposable object
    /// </summary>
    /// <param name="disposing">Specifies whether the object is currently being disposed</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
        {
            _binaryWriter?.Close();
            _stream?.Close();
            PostWriteAction?.Invoke();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        _disposedValue = true;
    }

    /// <summary>
    /// Disposes the IDisposable object
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
