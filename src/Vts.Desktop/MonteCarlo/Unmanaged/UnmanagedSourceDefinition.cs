using System.Runtime.InteropServices;

namespace Vts.MonteCarlo
{
    public unsafe static class UnmanagedSourceDefinitionExtension
    {
        public static UnmanagedSourceDefinition ToUnmanagedSourceDefinition(this ISource source)
        {
            UnmanagedSourceDefinition unmanagedSourceDefinition = new UnmanagedSourceDefinition();
            // comment until rework
            //unmanagedSourceDefinition.num_photons = source.num_photons;
            //unmanagedSourceDefinition.beam_radius = source.beam_radius;
            //unmanagedSourceDefinition.beam_center_x = source.beam_center_x;
            //unmanagedSourceDefinition.beamtype = source.beamtype;
            //unmanagedSourceDefinition.src_NA = source.src_NA;
            //unmanagedSourceDefinition.z_focus = source.z_focus;
            //unmanagedSourceDefinition.Rspec = source.Rspec;
            return unmanagedSourceDefinition;
        }
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct UnmanagedSourceDefinition
    {
        #region Public Fields
        public int num_photons;
        public double beam_radius;
        public double beam_center_x; //DCFIXED
        public string beamtype;
        public double src_NA;
        public double z_focus;
        public double Rspec;
        #endregion
    }
}
