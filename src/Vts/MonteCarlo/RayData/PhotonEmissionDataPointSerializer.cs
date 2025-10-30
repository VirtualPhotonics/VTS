using System.IO;
using Vts.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Implements ICustomBinaryReader&lt;PhotonEmissionDataPoint&gt; and 
    /// ICustomBinaryWriter&lt;PhotonEmissionDataPoint&gt;.
    /// </summary>
    public class PhotonEmissionDataPointSerializer : 
        ICustomBinaryReader<PhotonEmissionDataPoint>,
        ICustomBinaryWriter<PhotonEmissionDataPoint>
    {
        /// <summary>
        /// method to write PhotonEmissionDataPoint to binary
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="item">RayDataPoint</param>
        public void WriteToBinary(BinaryWriter bw, PhotonEmissionDataPoint item)
        {
            bw.Write(item.Position.X);
            bw.Write(item.Position.Y);
            bw.Write(item.Position.Z);
            bw.Write(item.Direction.Ux);
            bw.Write(item.Direction.Uy);
            bw.Write(item.Direction.Uz);
            bw.Write(item.Weight);
            bw.Write(item.TotalTime);
        }

        /// <summary>
        /// method to read PhotonEmissionDataPoint from binary (used by unit tests)
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <returns>PhotonEmissionDataPoint</returns>
        public PhotonEmissionDataPoint ReadFromBinary(BinaryReader br)
        {
            var dataPoint = new PhotonEmissionDataPoint(
                br.ReadDouble(), // Position X coordinate
                br.ReadDouble(), // Position Y coordinate
                br.ReadDouble(), // Position Z coordinate
                br.ReadDouble(), // Direction Ux coordinate
                br.ReadDouble(), // Direction Uy coordinate
                br.ReadDouble(), // Direction Uz coordinate
                br.ReadDouble(), // Weight
                br.ReadDouble() // TotalTime
            );

            return dataPoint;
        }
    }
}
