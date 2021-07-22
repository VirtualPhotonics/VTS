using System.Runtime.InteropServices;

namespace Vts.MonteCarlo
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct UnmanagedFlags
    {
        public int Seed;
        public int AbsWeightingType;
    }
}
