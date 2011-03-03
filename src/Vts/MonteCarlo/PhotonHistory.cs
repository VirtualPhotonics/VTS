using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.PhotonData;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Stores list of PhotonDataPoint that captures one photon's biography data. 
    /// </summary>
    public class PhotonHistory
    {
        private IList<SubRegionCollisionInfo> _SubRegionInfoList;

        public PhotonHistory(int numSubRegions)
        {
            HistoryData = new List<PhotonDataPoint>();
            _SubRegionInfoList = Enumerable.Range(0, numSubRegions)
                .Select(i => new SubRegionCollisionInfo(0.0, 0)).ToArray();
        }

        public IList<PhotonDataPoint> HistoryData{ get; private set; }

        public IList<SubRegionCollisionInfo> SubRegionInfoList
        {
            get { return _SubRegionInfoList; } 
            set { _SubRegionInfoList = value; }
        }

        /// <summary>
        /// Method to add PhotonDataPoint to History.  
        /// </summary>
        /// <param name="dp"></param>
        public void AddDPToHistory(PhotonDataPoint dp)
        {
            HistoryData.Add(dp.Clone()); 
        }
    }

    ///// <summary>
    ///// This class writes a PhotonExitHistory class to file.
    ///// </summary>
    //public class PhotonExitHistoryWriter : IDisposable
    //{
    //    private string _filename;
    //    private Stream _stream;
    //    private BinaryWriter _binaryWriter;
    //    private PhotonExitHistory _photonExitHistory;

    //    public PhotonExitHistoryWriter(string fileName, PhotonExitHistory photonExitHistory)
    //    {
    //        _filename = fileName;
    //        _photonExitHistory = photonExitHistory;
    //    }

    //    /// <summary>
    //    /// Opens the filestream for subsequent calls to WriteDataPoint or WriteDataPoints
    //    /// </summary>
    //    public void Open()
    //    {
    //        _stream = Vts.IO.FileIO.GetFileStream(_filename, FileMode.Create);
    //        _binaryWriter = new BinaryWriter(_stream);
    //    }

    //    /// <summary>
    //    /// Writes a single PhotonExitDataPoint to the underlying filestream
    //    /// </summary>
    //    /// <param name="dataPoint"></param>
    //    public void WriteDataPoint(PhotonExitDataPoint dataPoint)
    //    {
    //        dataPoint.WriteBinary(_binaryWriter);
    //        _photonExitHistory.NumberOfPhotons++;
    //    }

    //    /// <summary>
    //    /// Writes an enumerable list of PhotonExitDataPoints to the underlying filestream
    //    /// </summary>
    //    /// <param name="dataPoints"></param>
    //    public void WriteDataPoints(IEnumerable<PhotonExitDataPoint> dataPoints)
    //    {
    //        foreach (var dataPoint in dataPoints)
    //        {
    //            WriteDataPoint(dataPoint);
    //        }
    //    }

    //    /// <summary>
    //    /// Closes the filestream and writes the accompanying .xml
    //    /// </summary>
    //    public void Close()
    //    {
    //        Dispose();
    //    }

    //    #region IDisposable Members

    //    private bool _disposed = false;
    //    // Do not make this method virtual.
    //    // A derived class should not be able to override this method.

    //    /// <summary>
    //    /// Closes the file and writes the accompanying .xml
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        // This object will be cleaned up by the Dispose method.
    //        // Therefore, you should call GC.SupressFinalize to
    //        // take this object off the finalization queue
    //        // and prevent finalization code for this object
    //        // from executing a second time.
    //        GC.SuppressFinalize(this);
    //    }

    //    // Dispose(bool disposing) executes in two distinct scenarios.
    //    // If disposing equals true, the method has been called directly
    //    // or indirectly by a user's code. Managed and unmanaged resources
    //    // can be disposed.
    //    // If disposing equals false, the method has been called by the
    //    // runtime from inside the finalizer and you should not reference
    //    // other objects. Only unmanaged resources can be disposed.
    //    private void Dispose(bool disposing)
    //    {
    //        // Check to see if Dispose has already been called.
    //        if (!this._disposed)
    //        {
    //            // If disposing equals true, dispose all managed
    //            // and unmanaged resources.
    //            if (disposing)
    //            {
    //                // write the actual
    //                _photonExitHistory.WriteToXML(_filename + ".xml");

    //                // Dispose managed resources.
    //                if (_binaryWriter != null)
    //                    _binaryWriter.Close();

    //                if (_stream != null)
    //                    _stream.Close();
    //            }

    //            // Call the appropriate methods to clean up
    //            // unmanaged resources here.
    //            // If disposing is false,
    //            // only the following code is executed.

    //            // Note disposing has been done.
    //            _disposed = true;
    //        }

    //    }

    //    // Use interop to call the method necessary
    //    // to clean up the unmanaged resource.
    //    //[System.Runtime.InteropServices.DllImport("Kernel32")]
    //    //private extern static Boolean CloseHandle(IntPtr handle);

    //    // Use C# destructor syntax for finalization code.
    //    // This destructor will run only if the Dispose method
    //    // does not get called.
    //    // It gives your base class the opportunity to finalize.
    //    // Do not provide destructors in types derived from this class.
    //    ~PhotonExitHistoryWriter()
    //    {
    //        // Do not re-create Dispose clean-up code here.
    //        // Calling Dispose(false) is optimal in terms of
    //        // readability and maintainability.
    //        Dispose(false);
    //    }

    //    #endregion
    //}

    //public class PhotonExitHistory
    //{

    //    // not sure this constructor was actually needed
    //    //public PhotonExitHistory(
    //    //    long numberOfPhotons,
    //    //    long numberOfSubRegions,
    //    //    IEnumerable<PhotonExitDataPoint> dataPoints)
    //    //{
    //    //    NumberOfPhotons = 0;
    //    //    NumberOfSubRegions = numberOfSubRegions;
    //    //    DataPoints = dataPoints;
    //    //}
    //    //public PhotonExitHistory()
    //    //    : this(0, 1, null) { }

    //    public PhotonExitHistory() { NumberOfPhotons = 0; }

    //    public long NumberOfPhotons { get; set; }
    //    public long NumberOfSubRegions { get; set; }

    //    [IgnoreDataMember]
    //    public IEnumerable<PhotonExitDataPoint> DataPoints { get; set; }




    //    public static PhotonExitHistory FromFile(string fileName)
    //    {
    //        var photonExitHistory = FileIO.ReadFromXML<PhotonExitHistory>(fileName);
    //        photonExitHistory.DataPoints =
    //            FileIO.ReadCustomBinaryFromFile<PhotonExitDataPoint>(fileName, br =>
    //                PhotonExitDataPoint.ReadBinary(br, photonExitHistory.NumberOfSubRegions));
    //        return photonExitHistory;
    //    }

    //    public static PhotonExitHistory FromFileInResources(string fileName, string projectName)
    //    {
    //        var photonExitHistory = FileIO.ReadFromXML<PhotonExitHistory>(fileName);
    //        photonExitHistory.DataPoints =
    //            FileIO.ReadCustomBinaryFromFileInResources<PhotonExitDataPoint>(fileName, projectName, br =>
    //                PhotonExitDataPoint.ReadBinary(br, photonExitHistory.NumberOfSubRegions));
    //        return photonExitHistory;
    //    }

    //    //// I placed these generic functions below in Vts.IO to be useful outside this class
    //    //private static IEnumerable<T> ReadCustomBinaryFromFile<T>(string fileName, Func<BinaryReader, T> readerMap)
    //    //{
    //    //    using (Stream s = Vts.IO.FileIO.GetFileStream(fileName, FileMode.Open))
    //    //    {
    //    //        return ReadCustomBinaryStream<T>(s, readerMap);
    //    //    }
    //    //}

    //    //private static IEnumerable<T> ReadCustomBinaryFromFileInResources<T>(string fileName, string projectName, Func<BinaryReader, T> readerMap)
    //    //{
    //    //    using (Stream s = Vts.IO.FileIO.GetFileStreamFromResources(fileName, projectName))
    //    //    {
    //    //        return ReadCustomBinaryStream<T>(s, readerMap);
    //    //    }
    //    //}

    //    //// both versions of ReadBinary<T> call this method to actually read the data
    //    //private static IEnumerable<T> ReadCustomBinaryStream<T>(Stream s, Func<BinaryReader, T> readerMap)
    //    //{
    //    //    using (BinaryReader br = new BinaryReader(s))
    //    //    {
    //    //        while (s.Length < s.Position)
    //    //        {
    //    //            yield return readerMap(br);
    //    //        }
    //    //    }
    //    //}

    //}

    //public struct PhotonExitDataPoint
    //{
    //    public double X, Y, Ux, Uy, Uz;

    //    private IList<SubRegionCollisionInfo> _SubRegionInfoList;

    //    public PhotonExitDataPoint(
    //        double x, double y,
    //        double ux, double uy, double uz,
    //        IList<SubRegionCollisionInfo> subRegionInfoList)
    //    {
    //        X = x;
    //        Y = y;
    //        Ux = ux;
    //        Uy = uy;
    //        Uz = uz;
    //        _SubRegionInfoList = subRegionInfoList;
    //    }

    //    public IList<SubRegionCollisionInfo> SubRegionInfoList
    //    {
    //        get { return _SubRegionInfoList; }
    //        set { _SubRegionInfoList = value; }
    //    }

    //    public void WriteBinary(BinaryWriter binaryWriter)
    //    {
    //        binaryWriter.Write(X);
    //        binaryWriter.Write(Y);
    //        binaryWriter.Write(Ux);
    //        binaryWriter.Write(Uy);
    //        binaryWriter.Write(Uz);

    //        for (int i = 0; i < SubRegionInfoList.Count; i++)
    //        {
    //            SubRegionInfoList[i].WriteBinary(binaryWriter);
    //        }
    //    }

    //    public static PhotonExitDataPoint ReadBinary(BinaryReader br, long numberOfSubRegions)
    //    {
    //        var dataPoint = new PhotonExitDataPoint(
    //            br.ReadDouble(), // X
    //            br.ReadDouble(), // Y
    //            br.ReadDouble(), // Ux
    //            br.ReadDouble(), // Uy
    //            br.ReadDouble(), // Uz
    //            new SubRegionCollisionInfo[numberOfSubRegions]);

    //        for (int i = 0; i < numberOfSubRegions; i++)
    //            dataPoint.SubRegionInfoList[i] = SubRegionCollisionInfo.ReadBinary(br);

    //        return dataPoint;
    //    }
    //}
    //public struct SubRegionCollisionInfo
    //{
    //    public double PathLength;
    //    public long NumberOfCollisions;

    //    public SubRegionCollisionInfo(double pathLength, long numberOfCollisions)
    //    {
    //        PathLength = pathLength;
    //        NumberOfCollisions = numberOfCollisions;
    //    }

    //    public void WriteBinary(BinaryWriter bw)
    //    {
    //        bw.Write(PathLength);
    //        bw.Write(NumberOfCollisions);
    //    }

    //    public static SubRegionCollisionInfo ReadBinary(BinaryReader br)
    //    {
    //        return new SubRegionCollisionInfo(
    //            br.ReadDouble(), // pathLength
    //            br.ReadInt64()); // numberOfCollisions
    //    }
    //}

}
