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
        private readonly int _numberOfSubRegions;
        /// <summary>
        /// class that takes care of serializing CollisionInfo
        /// </summary>
        /// <param name="numberOfSubRegions">number of tissue sub-regions</param>
        public CollisionInfoSerializer(int numberOfSubRegions)
        {
            _numberOfSubRegions = numberOfSubRegions;
        }
        /// <summary>
        /// method to write CollisionInfo to binary file
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        /// <param name="item">CollisionInfo</param>
        public void WriteToBinary(BinaryWriter bw, CollisionInfo item)
        {
            foreach (var t in item)
            {
                bw.Write(t.PathLength);
                bw.Write(t.NumberOfCollisions);
            }
        }
        /// <summary>
        /// method to read CollisionInfo binary
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <returns>CollisionInfo</returns>
        public CollisionInfo ReadFromBinary(BinaryReader br)
        {
            var collisionInfo = new CollisionInfo(_numberOfSubRegions);

            for (var i = 0; i < _numberOfSubRegions; i++)
            {
                collisionInfo.Add(new SubRegionCollisionInfo(
                    br.ReadDouble(), // pathLength
                    br.ReadInt64()));  // numberOfCollisions
            }

            return collisionInfo;
        }
    }
}
