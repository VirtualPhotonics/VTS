using System.Runtime.InteropServices;

namespace Vts.MonteCarlo
{
    public unsafe static class UnmanagedHistoryExtension
    {
        public static UnmanagedHistory ToUnmanagedHistory(this PhotonHistory histptr)
        {
            UnmanagedHistory unmanagedHistory = new UnmanagedHistory();

            //UnmanagedIO.Assign1DPointer(ref histptr.xh, ref unmanagedHistory.xh);
            //UnmanagedIO.Assign1DPointer(ref histptr.yh, ref unmanagedHistory.yh);
            //UnmanagedIO.Assign1DPointer(ref histptr.zh, ref unmanagedHistory.zh);
            //UnmanagedIO.Assign1DPointer(ref histptr.uxh, ref unmanagedHistory.uxh);
            //UnmanagedIO.Assign1DPointer(ref histptr.uyh, ref unmanagedHistory.uyh);
            //UnmanagedIO.Assign1DPointer(ref histptr.uzh, ref unmanagedHistory.uzh);
            //UnmanagedIO.Assign1DPointer(ref histptr.weight, ref unmanagedHistory.weight); 
            //UnmanagedIO.Assign1DPointer(ref histptr.pert_wt, ref unmanagedHistory.pert_wt); 
            //UnmanagedIO.Assign1DPointer(ref histptr.path_length, ref unmanagedHistory.path_length);
            //UnmanagedIO.Assign1DPointer(ref histptr.boundary_col, ref unmanagedHistory.boundary_col);
            //unmanagedHistory.num_pts_stored = histptr.num_pts_stored;
            //unmanagedHistory.cum_path_length = histptr.cum_path_length;
            return unmanagedHistory;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct UnmanagedHistory
    {
        public double* xh;
        public double* yh;
        public double* zh;
        public double* uxh;
        public double* uyh;
        public double* uzh;
        public double* weight;
        public double* pert_wt;
        public double* path_length;
        public int* boundary_col;
        public int num_pts_stored;
        public double cum_path_length;
    }
}
