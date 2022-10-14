using System.Collections.Generic;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.RayData;

namespace Vts.Zemax
{
    /// <summary>
    /// This is code that would be executed to convert ZRD DB to/from MCCL compatible DB
    /// A controller of ZrdDatabaseWriter(s).  It handles determining whether data should be written,
    /// and if so, writing the data, and finally disposing of the database.
    /// </summary>
    public class ZrdDatabaseWriterController
    {
        private readonly IList<ZrdRayDatabaseWriter> _rayDatabaseWriters;
        /// <summary>
        /// class that controls DatabaseWriter(s).
        /// </summary>
        /// <param name="rayDatabaseWriters">IList of PhotonDatabaseWriter</param>
        public ZrdDatabaseWriterController(IList<ZrdRayDatabaseWriter> rayDatabaseWriters)
        {
            _rayDatabaseWriters = rayDatabaseWriters;
        }
        /// <summary>
        /// list of PhotonDatabaseWriter
        /// </summary>
        public IList<ZrdRayDatabaseWriter> ZrdRayDatabaseWriters { get; set; }

        /// <summary>
        /// Method to write to all surface VB databases
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        public void WriteToSurfaceVirtualBoundaryDatabases(PhotonDataPoint dp)
        {
            foreach (var writer in _rayDatabaseWriters)
            {
                if (!DpBelongsToSurfaceVirtualBoundary(dp, writer)) continue;
                var ray = new ZrdRayDataPoint(
                    new RayDataPoint(
                        dp.Position,
                        dp.Direction,
                        dp.Weight));
                writer.Write(ray);
            }
        }

        /// <summary>
        /// Method to determine if photon data point should be tallied or not
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        /// <param name="rayDatabaseWriter">single ZrdRayDatabaseWriter</param>
        /// <returns></returns>
        public bool DpBelongsToSurfaceVirtualBoundary(PhotonDataPoint dp,
            ZrdRayDatabaseWriter rayDatabaseWriter)
        {
            if ((!dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) ||
                 rayDatabaseWriter.VirtualBoundaryType != VirtualBoundaryType.DiffuseReflectance) &&
                (!dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) ||
                 rayDatabaseWriter.VirtualBoundaryType != VirtualBoundaryType.DiffuseTransmittance)) return false;
            return true;
        }
        /// <summary>
        /// Method to dispose of database writer(s).  Currently not used, may be needed in future.
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
