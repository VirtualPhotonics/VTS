using System.Runtime.InteropServices;

namespace Vts.MonteCarlo
{
//    public unsafe static class UnmanagedDetectorDefinitionExtension
//    {
//        public static UnmanagedDetectorDefinition ToUnmanagedDetector(this Detector detector)
//        {
//            UnmanagedDetectorDefinition unmanagedDetector = new UnmanagedDetectorDefinition();

//            unmanagedDetector.dz = detector.Z.Delta;
//            unmanagedDetector.dr = detector.Rho.Delta;
//            unmanagedDetector.da = detector.Angle.Delta;
//            unmanagedDetector.dt = detector.Time.Delta;
//            unmanagedDetector.dx = detector.X.Delta;
//            unmanagedDetector.dy = detector.Y.Delta;
//            unmanagedDetector.nz = detector.Z.Count;
//            unmanagedDetector.nr = detector.Rho.Count;
//            unmanagedDetector.na = detector.Angle.Count;
//            unmanagedDetector.nt = detector.Time.Count;
//            unmanagedDetector.nx = detector.X.Count;
//            unmanagedDetector.ny = detector.Y.Count;
//            unmanagedDetector.na = detector.Angle.Count;
//            return unmanagedDetector;
//        }
//    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct UnmanagedDetectorDefinition
    {
        public double dz, dr, da, dt, dx, dy; //DCFIXED
        public int nz, nr, na, nt, nx, ny; //DCFIXED
        public int num_photons;
        public int num_layers;
        public double z_focus;    /* focus depth */
        public double NA;         /* NA */
        public double cylinder_radius, cylinder_height;
        public double zup, zlow;
        public int do_ellip_layer;
        public double ellip_x, ellip_y, ellip_z, ellip_rad_x, ellip_rad_y, ellip_rad_z;
        public double layer_z_min;
    }
}
