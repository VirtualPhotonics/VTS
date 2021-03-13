using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Vts.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Describes database for storing and returning source data points (position, direction, weight).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) SourceDataPoints 
    /// list of SourceDataPoint items
    /// </summary>
    public class ZRDRayDatabase // : Database<ZRDRayDataPoint>
    {
        /// <summary>
        /// Returns an instance of SourceDatabase
        /// </summary>
        public ZRDRayDatabase()
        {
        }
        public static List<ZRDRayDataInUFD.ZRDRayDataPoint> ZRDRayDataPoints { get; private set; }
        /// <summary>
        /// Static helper method to simplify reading from file
        /// </summary>
        /// <param name="fileName">The base filename for the database (no ".txt")</param>
        /// <returns>A new instance of SourceDatabase</returns>
        public static List<ZRDRayDataInUFD.ZRDRayDataPoint> FromFile(string fileName)
        {
            ZRDRayDataPoints = new List<ZRDRayDataInUFD.ZRDRayDataPoint>();
            // open stream
            using (Stream s = StreamFinder.GetFileStream(fileName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(s))
                {
                    // read header information
                    int version = br.ReadInt32();
                    int maxSegments = br.ReadInt32();
                    int count = Marshal.SizeOf(typeof(ZRDRayDataInUFD.ZRDRayDataPoint));
                    for (int i = 0; i < 1000; i++)
                    {
                        byte[] readBuffer = new byte[count];
                        int numSegments = br.ReadInt32();  // number of segments 
                        var rayData = br.ReadBytes(count);
                        GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                        var zrdRayData = (ZRDRayDataInUFD.ZRDRayDataPoint)
                            Marshal.PtrToStructure(handle.AddrOfPinnedObject(), 
                            typeof(ZRDRayDataInUFD.ZRDRayDataPoint));
                        ZRDRayDataPoints.Add(zrdRayData);
                    }
                }
            }

            return ZRDRayDataPoints;
        }


    }
}
