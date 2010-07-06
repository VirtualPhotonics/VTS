using System.Runtime.InteropServices;

namespace Vts.MonteCarlo
{
    public unsafe static class UnmanagedTissueExtension
    {
        public static UnmanagedTissue ToUnmanagedTissue(this ITissue tissptr)
        {
            UnmanagedTissue unmanagedTissue = new UnmanagedTissue();

            // comment to rework later
            //unmanagedTissue.num_layers = tissptr.num_layers;
            ////unmanagedTissue.z_focus = tissptr.z_focus;
            ////unmanagedTissue.NA = tissptr.NA;
            //unmanagedTissue.cylinder_radius = tissptr.cylinder_radius;
            //unmanagedTissue.cylinder_height = tissptr.cylinder_height;
            //unmanagedTissue.zup = tissptr.zup;
            //unmanagedTissue.zlow = tissptr.zlow;

            //unmanagedTissue.do_ellip_layer = tissptr.do_ellip_layer;
            //unmanagedTissue.ellip_x = tissptr.ellip_x;
            //unmanagedTissue.ellip_y = tissptr.ellip_y;
            //unmanagedTissue.ellip_z = tissptr.ellip_z;
            //unmanagedTissue.ellip_rad_x = tissptr.ellip_rad_x;
            //unmanagedTissue.ellip_rad_y = tissptr.ellip_rad_y;
            //unmanagedTissue.ellip_rad_z = tissptr.ellip_rad_z;
            //unmanagedTissue.layer_z_min = tissptr.layer_z_min;
            //UnmanagedIO.Assign1DPointer(ref Tissue.Regions, ref unmanagedTissue.layerprops);
	
            return unmanagedTissue;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct UnmanagedTissue
    {
        //public double dz, dr, da, dt, dx, dy; //DCFIXED
        //public short nz, nr, na, nt, nx, ny; //DCFIXED
        //public int num_photons;
        //public short num_layers;
        //public double z_focus;    /* focus depth */
        //public double NA;         /* NA */
        //public double cylinder_radius, cylinder_height;
        //public double zup, zlow;
        //public int do_ellip_layer;
        //public double ellip_x, ellip_y, ellip_z, ellip_rad_x, ellip_rad_y, ellip_rad_z;
        //public double layer_z_min;
        //public Layer *layerprops;
    }
}
