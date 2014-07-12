using System;
using System.IO;

namespace Vts.IO
{
    public class BinaryArraySerializer
    {
        public Array DataArray { get; set; }
        public string Name { get; set; }
        public string FileTag { get; set; }
        public int[] Dimensions { get; set; }

        public Action<BinaryWriter> WriteData { get; set; }
        public Action<BinaryReader> ReadData { get; set; }
    }
}
