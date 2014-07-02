using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// A controller of DatabaseWriter(s).  It handles determining whether data should be written,
    /// and if so, writing the data, and finally disposing of the database.
    /// </summary>
    public class DatabaseWriterController
    {
        IList<PhotonDatabaseWriter> _photonDatabaseWriters;
        /// <summary>
        /// class that controls DatabaseWriter(s).
        /// </summary>
        /// <param name="photonDatabaseWriters">IList of PhotonDatabaseWriter</param>
        public DatabaseWriterController(IList<PhotonDatabaseWriter> photonDatabaseWriters)
        {
            _photonDatabaseWriters = photonDatabaseWriters;
        }
        /// <summary>
        /// list of PhotonDatabaseWriter
        /// </summary>
        public IList<PhotonDatabaseWriter> PhotonDatabaseWriters { get { return _photonDatabaseWriters; } set { _photonDatabaseWriters = value; } }

        /// <summary>
        /// Method to write to all surface VB databases
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        public void WriteToSurfaceVirtualBoundaryDatabases(PhotonDataPoint dp)
        {
            foreach (var writer in _photonDatabaseWriters)
            {
                if (DPBelongsToSurfaceVirtualBoundary(dp, writer))
                {
                    writer.Write(dp);
                }
            };
        }

        /// <summary>
        /// Method to determine if photon datapoint should be tallied or not
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        /// <param name="photonDatabaseWriter">single PhotonDatabaseWriter</param>
        /// <returns></returns>
        public bool DPBelongsToSurfaceVirtualBoundary(PhotonDataPoint dp,
            PhotonDatabaseWriter photonDatabaseWriter)
        {
            if ((dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) &&
                 photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseReflectance) ||
                (dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) &&
                 photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseTransmittance) ||
                (dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) && // pMC uses regular PST
                 photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.pMCDiffuseReflectance))
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
            foreach (var writer in _photonDatabaseWriters)
            {
                writer.Dispose();
            }
        }

    }
} 
