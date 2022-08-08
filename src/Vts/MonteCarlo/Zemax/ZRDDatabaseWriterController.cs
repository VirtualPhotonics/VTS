using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.RayData;

namespace Vts.MonteCarlo.Zemax
{
    /// <summary>
    /// A controller of ZRDDatabaseWriter(s).  It handles determining whether data should be written,
    /// and if so, writing the data, and finally disposing of the database.
    /// </summary>
    public class ZRDDatabaseWriterController
    {
        IList<ZRDRayDatabaseWriter> _rayDatabaseWriters;
        /// <summary>
        /// class that controls DatabaseWriter(s).
        /// </summary>
        /// <param name="rayDatabaseWriters">IList of PhotonDatabaseWriter</param>
        public ZRDDatabaseWriterController(IList<ZRDRayDatabaseWriter> rayDatabaseWriters)
        {
            _rayDatabaseWriters = rayDatabaseWriters;
        }
        /// <summary>
        /// list of PhotonDatabaseWriter
        /// </summary>
        public IList<ZRDRayDatabaseWriter> ZRDRayDatabaseWriters { get { return _rayDatabaseWriters; } set { _rayDatabaseWriters = value; } }

        /// <summary>
        /// Method to write to all surface VB databases
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        public void WriteToSurfaceVirtualBoundaryDatabases(PhotonDataPoint dp)
        {
            foreach (var writer in _rayDatabaseWriters)
            {
                if (DPBelongsToSurfaceVirtualBoundary(dp, writer))
                {
                    var ray = new ZRDRayDataPoint(
                       new RayDataPoint(
                           dp.Position,
                           dp.Direction,
                           dp.Weight));
                    writer.Write(ray);
                }
            };
        }

        /// <summary>
        /// Method to determine if photon datapoint should be tallied or not
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        /// <param name="rayDatabaseWriter">single ZRDRayDatabaseWriter</param>
        /// <returns></returns>
        public bool DPBelongsToSurfaceVirtualBoundary(PhotonDataPoint dp,
            ZRDRayDatabaseWriter rayDatabaseWriter)
        {
            if ((dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) &&
                 rayDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseReflectance) ||
                (dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) &&
                 rayDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseTransmittance))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Method to dispose of database writer(s)
        /// </summary>
        public void Dispose()
        {
            foreach (var writer in _rayDatabaseWriters)
            {
                writer.Dispose();
            }
        }

    }
} 
