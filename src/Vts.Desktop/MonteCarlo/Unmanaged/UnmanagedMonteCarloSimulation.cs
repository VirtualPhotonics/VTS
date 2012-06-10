using System.Runtime.InteropServices;

namespace Vts.MonteCarlo
{
    public unsafe class UnmanagedMonteCarloSimulation : MonteCarloSimulation
    {
        //public UnmanagedPhoton unmanagedPhoton;
        //public UnmanagedPerturbation unmanagedPerturbation;
        //public UnmanagedTissue unmanagedTissue;
        //public UnmanagedOutput unmanagedOutput;
        //public UnmanagedHistory unmanagedHistory;
        public UnmanagedFlags unmanagedFlags;
        //Output outptr;

        [DllImport(@"Vts.MonteCarlo.Unmanaged.dll", EntryPoint = "RunMCLoopExternal")]
        public static extern void RunUnmanagedMC(ref UnmanagedPhoton unmanagedPhoton,
            ref UnmanagedTissue unmanagedTissue, ref UnmanagedSourceDefinition unmanagedSourceDefinition,
            ref UnmanagedOutput unmanagedOutput, ref UnmanagedHistory unmanagedHistory,
            ref UnmanagedFlags unmanagedFlags);

        //[DllImport(@"Vts.MonteCarlo.Unmanaged.dll", EntryPoint = "RunTest")]
        //public static extern void RunUnmanagedMC(ref UnmanagedPhoton unmanagedPhoton,
        //    ref UnmanagedTissue unmanagedTissue, ref UnmanagedPerturb unmanagedPerturb, 
        //    ref UnmanagedOutput unmanagedOutput);

        public UnmanagedMonteCarloSimulation(SimulationInput input)
            : base(input)
        {
        }

        protected void InitializeOptions(SimulationOptions options)
        {
            unmanagedFlags = new UnmanagedFlags();
            unmanagedFlags.AbsWeightingType = (int)options.AbsorptionWeightingType;
            unmanagedFlags.Seed = options.Seed;
        }

        public UnmanagedMonteCarloSimulation() : this(new SimulationInput()) { }

        //protected override void ExecuteMCLoop(ITissue tissptr, Photon photptr, History histptr,
        //    ISource source, Banana bananaptr, Output outptr, IDetector detector)
        protected override void ExecuteMCLoop()
        {
            //IOFunctions.InitializeStructs(_input, out photptr, out tissptr,
            //    out pertptr, out outptr, out bananaptr, out histptr);

            // comment to rework later
            //Console.WriteLine("photptr.beamtype=" + source.beamtype);
            //Console.WriteLine("tissptr.num_photons=" + source.num_photons);

            //var unmanagedTissue = tissptr.ToUnmanagedTissue();
            //var unmanagedSourceDefinition = source.ToUnmanagedSourceDefinition();
            //var unmanagedOutput = outptr.ToUnmanagedOutput();
            //var unmanagedPhoton = photptr.ToUnmanagedPhoton();
            //var unmanagedHistory = histptr.ToUnmanagedHistory();

            //RunUnmanagedMC(ref unmanagedPhoton, ref unmanagedTissue, ref unmanagedSourceDefinition,
            //    ref unmanagedOutput, ref unmanagedHistory, ref unmanagedFlags);

            //ToManagedOutput(ref unmanagedOutput, ref outptr);
        }
        
        public static void ToManagedOutput(ref UnmanagedOutput unmanagedOutput, ref SimulationOutput output)
        {
            //output.Atot = unmanagedOutput.Atot;
            //output.Rd = unmanagedOutput.Rd;
            //output.Rtot = unmanagedOutput.Rtot;
            //output.Td = unmanagedOutput.Td;
            //output.wt_pathlen_out_top = unmanagedOutput.wt_pathlen_out_top;
            //output.wt_pathlen_out_bot = unmanagedOutput.wt_pathlen_out_bot;
            //output.wt_pathlen_out_sides = unmanagedOutput.wt_pathlen_out_sides;
            //output.nomega = unmanagedOutput.nomega;
            //output.cramer_wt = unmanagedOutput.cramer_wt;
            //output.Rconv = unmanagedOutput.Rconv;
            //output.Rconv2 = unmanagedOutput.Rconv2;
            //output.num_visit_conv = unmanagedOutput.num_visit_conv;
            //output.num_visit = unmanagedOutput.num_visit;
        }
    }
}
