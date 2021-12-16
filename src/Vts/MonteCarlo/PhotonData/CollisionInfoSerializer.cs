using System.IO;
using Vts.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Implements ICustomBinaryReader(OfCollisionInfo) and ICustomBinaryWriter(OfCollisionInfo).
    /// </summary>
    public class CollisionInfoSerializer :
        ICustomBinaryReader<CollisionInfo>,
        ICustomBinaryWriter<CollisionInfo>
    {
        private int _numberofSubRegions;
        /// <summary>
        /// class that takes care of serializing CollisionInfo
        /// </summary>
        /// <param name="numberOfSubRegions">number of tissue subregions</param>
        public CollisionInfoSerializer(int numberOfSubRegions)
        {
            _numberofSubRegions = numberOfSubRegions;
        }
        /// <summary>
        /// method to write CollisionInfo to binary file
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="item">CollisionInfo</param>
        public void WriteToBinary(BinaryWriter bw, CollisionInfo item)
        {
            for (int i = 0; i < item.Count; i++)
            {
                bw.Write(item[i].PathLength);
                bw.Write(item[i].NumberOfCollisions);
            }
        }
        /// <summary>
        /// method to read CollisionInfo binary
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <returns>SollisionInfo</returns>
        public CollisionInfo ReadFromBinary(BinaryReader br)
        {
            var collisionInfo = new CollisionInfo(_numberofSubRegions);

            for (int i = 0; i < _numberofSubRegions; i++)
            {
                collisionInfo.Add(new SubRegionCollisionInfo(
                    br.ReadDouble(), // pathLength
                    br.ReadInt64()));  // numberOfCollisions
            }

            return collisionInfo;
        }
    }
}
