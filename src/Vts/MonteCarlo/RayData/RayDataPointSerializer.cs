using System.IO;
using Vts.Common;
using Vts.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Implements ICustomBinaryReader&lt;RayDataPoint&gt; and 
    /// ICustomBinaryWriter&lt;DataPoint&gt;.
    /// </summary>
    public class RayDataPointSerializer : 
        ICustomBinaryReader<RayDataPoint>, 
        ICustomBinaryWriter<RayDataPoint>
    {
        /// <summary>
        /// Method to write RayDataPoint to binary. 
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="rayDataPoint">RayDataPoint</param>
        public void WriteToBinary(BinaryWriter bw, RayDataPoint rayDataPoint)
        {
            // write data from rayDataPointsList
            bw.Write(rayDataPoint.Position.X);
            bw.Write(rayDataPoint.Position.Y);
            bw.Write(rayDataPoint.Position.Z);
            bw.Write(rayDataPoint.Direction.Ux);
            bw.Write(rayDataPoint.Direction.Uy);
            bw.Write(rayDataPoint.Direction.Uz);
            // write weight into intensity field
            bw.Write(rayDataPoint.Weight);
        }
        /// <summary>
        /// Method to read PhotonDataPoint from binary.  Header info is only read first time through.
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <returns>RayDataPoint is pertinent data from ray data that MCCL uses</returns>
        public RayDataPoint ReadFromBinary(BinaryReader br)
        {
            var rayDataPoint = new RayDataPoint(
                Position.ReadBinary(br), // Position
                Direction.ReadBinary(br), // Direction
                br.ReadDouble()); // Weight
            return rayDataPoint;
        }
    }
}
