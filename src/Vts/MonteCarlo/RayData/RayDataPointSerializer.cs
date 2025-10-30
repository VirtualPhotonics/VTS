using System.IO;
using Vts.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Implements ICustomBinaryReader&lt;RayDataPoint&gt; and 
    /// ICustomBinaryWriter&lt;RayDataPoint&gt;.
    /// </summary>
    public class RayDataPointSerializer : 
        ICustomBinaryReader<RayDataPoint>,
        ICustomBinaryWriter<RayDataPoint>
    {
        /// <summary>
        /// method to write RayDataPoint to binary CURRENTLY NOT USED
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="item">RayDataPoint</param>
        public void WriteToBinary(BinaryWriter bw, RayDataPoint item)
        {
            bw.Write(item.Position.X);
            bw.Write(item.Position.Y);
            bw.Write(item.Position.Z);
            bw.Write(item.Direction.Ux);
            bw.Write(item.Direction.Uy);
            bw.Write(item.Direction.Uz);
            bw.Write(item.Weight);
        }

        /// <summary>
        /// method to read RayDataPoint from binary
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <returns>RayDataPoint</returns>
        public RayDataPoint ReadFromBinary(BinaryReader br)
        {
            var dataPoint = new RayDataPoint(
                br.ReadDouble(), // Position X coordinate
                br.ReadDouble(), // Position Y coordinate
                br.ReadDouble(), // Position Z coordinate
                br.ReadDouble(), // Direction Ux coordinate
                br.ReadDouble(), // Direction Uy coordinate
                br.ReadDouble(), // Direction Uz coordinate
                br.ReadDouble() // Weight
            );

            return dataPoint;
        }
    }
}
