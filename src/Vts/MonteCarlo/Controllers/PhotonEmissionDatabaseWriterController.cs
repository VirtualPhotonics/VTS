using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.RayData;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// A controller of DatabaseWriter(s).  It handles determining whether data should be written,
    /// and if so, writing the data, and finally disposing of the database.
    /// </summary>
    public class PhotonEmissionDatabaseWriterController 
    {
        /// <summary>
        /// class that controls DatabaseWriter(s).
        /// </summary>
        /// <param name="photonEmissionDatabaseWriters">IList of PhotonEmissionDatabaseWriter</param>
        public PhotonEmissionDatabaseWriterController(IList<PhotonEmissionDatabaseWriter> photonEmissionDatabaseWriters)
        {
            PhotonEmissionDatabaseWriters = photonEmissionDatabaseWriters;
        }

        /// <summary>
        /// list of PhotonDatabaseWriter
        /// </summary>
        public IList<PhotonEmissionDatabaseWriter> PhotonEmissionDatabaseWriters { get; set; }

        /// <summary>
        /// Method to write to all surface VB databases
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        public void WriteToSurfaceVirtualBoundaryDatabases(PhotonDataPoint dp)
        {
            foreach (var writer in PhotonEmissionDatabaseWriters)
            {
                if (!DPBelongsToSurfaceVirtualBoundary(dp, writer)) continue;
                writer.Write(new PhotonEmissionDataPoint(
                    dp.Position.X, dp.Position.Y, dp.Position.Z,
                    dp.Direction.Ux, dp.Direction.Uy, dp.Direction.Uz, 
                    dp.Weight,
                    dp.TotalTime));
            }
        }

        /// <summary>
        /// Method to determine if photon data point should be tallied or not
        /// </summary>
        /// <param name="dp">PhotonEmissionDataPoint</param>
        /// <param name="photonEmissionDatabaseWriter">single PhotonDatabaseWriter</param>
        /// <returns>Boolean indicating whether photon data point belongs to virtual boundary</returns>
        public bool DPBelongsToSurfaceVirtualBoundary(PhotonDataPoint dp,
            PhotonEmissionDatabaseWriter photonEmissionDatabaseWriter)
        {
            return (dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) &&
                    photonEmissionDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseReflectance) ||
                   (dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) &&
                    photonEmissionDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseTransmittance);
        }

        /// <summary>
        /// Method to dispose of database writer(s)
        /// </summary>
        public void Dispose()
        {
            foreach (var writer in PhotonEmissionDatabaseWriters)
            {
                writer.Dispose();
            }
        }

    }
} 
