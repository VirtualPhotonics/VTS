using System.Runtime.InteropServices;

namespace Vts.MonteCarlo
{
    public unsafe static class UnmanagedPhotonExtension
    {
        public static UnmanagedPhoton ToUnmanagedPhoton(this Photon photptr)
        {
            UnmanagedPhoton unmanagedPhoton = new UnmanagedPhoton();
            // comment out until managed code done
            //unmanagedPhoton.x = photptr.x;
            //unmanagedPhoton.y = photptr.y;
            //unmanagedPhoton.z = photptr.z;
            //unmanagedPhoton.ux = photptr.ux;
            //unmanagedPhoton.uy = photptr.uy;
            //unmanagedPhoton.uz = photptr.uz;
            //unmanagedPhoton.s = photptr.s;
            //unmanagedPhoton.sleft = photptr.sleft;
            //unmanagedPhoton.dead = photptr.dead;
            //unmanagedPhoton.w = photptr.w;
            ////unmanagedPhoton.Rspec = photptr.Rspec;
            //unmanagedPhoton.max_path_length = photptr.max_path_length;
            ////unmanagedPhoton.history = photptr.history;
            //unmanagedPhoton.hit_bdry = photptr.hit_bdry;
            //unmanagedPhoton.curr_layer = photptr.curr_layer;
            ////UnmanagedIO.Assign1DPointer(ref photptr.layerprops, ref unmanagedPhoton.layerprops);
            //UnmanagedIO.Assign1DPointer(ref photptr.num_photons_written, ref unmanagedPhoton.num_photons_written);
            //unmanagedPhoton.curr_n = (int)photptr.curr_n;
            //UnmanagedIO.Assign1DPointer(ref photptr.col_in_layer, ref unmanagedPhoton.col_in_layer);
            //UnmanagedIO.Assign1DPointer(ref photptr.pathlen_in_layer, ref unmanagedPhoton.pathlen_in_layer);
            //unmanagedPhoton.tot_out_top = photptr.tot_out_top;
            //unmanagedPhoton.tot_out_bot = photptr.tot_out_bot;
            //unmanagedPhoton.col_hit_bdry = photptr.col_hit_bdry; obsolete
            return unmanagedPhoton;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct UnmanagedPhoton
    {
        public double x, y, z;
        public double ux, uy, uz;
        public double s;
        public double sleft; /* remaining step size when photon hits boundary */
        public PhotonStateType dead;
        public double w;
        //public double Rspec;			/* Specular reflectance */
        public double max_path_length;
        //public History history;
        // next has to be short not bool
        public short hit_bdry;
        public short curr_layer;		/* index for layerprops */
        //public Layer* layerprops;	/* pointer to Layer structure */
        public double* num_photons_written;
        public int curr_n;
        public int* col_in_layer;
        public double* pathlen_in_layer;
        public int tot_out_top, tot_out_bot;

    }
}
