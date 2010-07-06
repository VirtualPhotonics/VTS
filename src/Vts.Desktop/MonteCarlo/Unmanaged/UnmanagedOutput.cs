using System.Runtime.InteropServices;

namespace Vts.MonteCarlo
{
    public unsafe static class UnmanagedOutputExtension
    {
        public static UnmanagedOutput ToUnmanagedOutput(this Output output)
        {
            UnmanagedOutput unmanagedOutput = new UnmanagedOutput();

            UnmanagedIO.Assign2DPointer(output.A_rz, ref unmanagedOutput.A_rz);
            UnmanagedIO.Assign1DPointer(output.A_z, ref unmanagedOutput.A_z);
            UnmanagedIO.Assign1DPointer(output.A_layer, ref unmanagedOutput.A_layer);
            unmanagedOutput.Atot = output.Atot;
            UnmanagedIO.Assign2DPointer(output.Flu_rz, ref unmanagedOutput.Flu_rz);
            UnmanagedIO.Assign1DPointer(output.Flu_z, ref unmanagedOutput.Flu_z);
            UnmanagedIO.Assign2DPointer(output.R_ra, ref unmanagedOutput.R_ra);
            UnmanagedIO.Assign1DPointer(output.R_r, ref unmanagedOutput.R_r);
            UnmanagedIO.Assign1DPointer(output.R_a, ref unmanagedOutput.R_a);
            UnmanagedIO.Assign1DPointer(output.R_r2, ref unmanagedOutput.R_r2);
            UnmanagedIO.Assign2DPointer(output.R_xy, ref unmanagedOutput.R_xy);
            UnmanagedIO.Assign2DPointer(output.T_ra, ref unmanagedOutput.T_ra);
            UnmanagedIO.Assign1DPointer(output.T_r, ref unmanagedOutput.T_r);
            UnmanagedIO.Assign1DPointer(output.T_a, ref unmanagedOutput.T_a);
            unmanagedOutput.Rd = output.Rd;
            unmanagedOutput.Rtot = output.Rtot;
            unmanagedOutput.Td = output.Td;
            UnmanagedIO.Assign2DPointer(output.R_rt, ref unmanagedOutput.R_rt);

            if (MonteCarloSimulation.DO_ALLVOX)
            {
                UnmanagedIO.Assign4DPointer(output.in_side_allvox, ref unmanagedOutput.in_side_allvox);
                UnmanagedIO.Assign4DPointer(output.out_side_allvox, ref unmanagedOutput.out_side_allvox);
            }
            UnmanagedIO.Assign2DPointer(output.D_rt, ref unmanagedOutput.D_rt);

            return unmanagedOutput;
        }
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct UnmanagedOutput
    {
        #region Public Fields
        public double** A_rz;
        public double* A_z;
        public double* A_layer;
        public double Atot;
        public double** Flu_rz;
        public double* Flu_z;
        public double** R_ra;
        public double* R_r, R_a, R_r2;
        public double** R_xy, T_ra;
        public double* T_r, T_a;
        public double Rd, Rtot;
        public double Td;
        public double** R_rt;
        public double wt_pathlen_out_top;
        public double wt_pathlen_out_bot;
        public double wt_pathlen_out_sides;
        public double****in_side_allvox, out_side_allvox;
        public double*** temp;
        // public double[, , ,] in_side2_allvox, out_side2_allvox; /* FIXED added */

        public double nomega;
        public double cramer_wt;
        public double Rconv, Rconv2;
        public int num_visit_conv, num_visit;
        public double** Amp_rw, Phase_rw, re_rw, im_rw;
        //public double** R0_rt; // Method DC
        public double** D_rt; // scaled MC
        public double* path_length_in_layer;
        #endregion
    }
}
