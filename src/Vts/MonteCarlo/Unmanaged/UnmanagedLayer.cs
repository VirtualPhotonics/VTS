using System.Runtime.InteropServices;

namespace Vts.MonteCarlo
{
    public unsafe static class UnmanagedLayerExtension
    {
        //public static UnmanagedLayer ToUnmanagedLayer(this Layer tissptr)
        //{
        //    UnmanagedLayer unmanagedLayer = new UnmanagedLayer();

        //    unmanagedLayer.n = tissptr.n;
        //    unmanagedLayer.mua = tissptr.mua;
        //    unmanagedLayer.mus = tissptr.mus;
        //    unmanagedLayer.albedo = tissptr.albedo;
        //    unmanagedLayer.g = tissptr.g;
        //    unmanagedLayer.d = tissptr.d;
        //    unmanagedLayer.zbegin = tissptr.zend;
        //    unmanagedLayer.scatter_length = tissptr.scatter_length;

        //    return unmanagedLayer;
        //}
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct UnmanagedLayer
    {
        public double n;
        public double mua;
        public double mus;
        public double albedo;
        public double g;
        public double d;
        public double zbegin, zend;
        public double scatter_length;  // added for DAW/CAW
    }
}
