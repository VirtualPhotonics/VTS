

using System.Threading.Tasks;

namespace Vts.MonteCarlo.GenerateValidationData
{
    class Program
    {
        static void Main(string[] args)
        {
            double[] mus_temp = new double[7] {2.5, 3.5, 4.5, 5.0, 5.5, 6.5, 7.5}; // mm-1
            // with a g = 0.8 this produces musp = {0.9, 1, 1.1} mm-1
            double[] mua_temp = new double[3] {0.001, 0.01, 0.1}; // mm-1

            // write current schema to file so can edit
            //SimulationInput input = new SimulationInput();
            //input.ToFile("temp.xml");
            // Read from in from xml file in bin/Debug/
            //SimulationInput input = SimulationInput.FromFile("PointSourceHomogeneous.xml"); 
            // Read in from code

            // Read in from xml file in resources(doesn't work) or in src/files
            //SimulationInput input = 
              //(SimulationInput)FileIO.ReadFromXMLInResources<SimulationInput>("Resources/PointSourceHomogeneous.xml",
                  //"Vts.MonteCarlo.GenerateValidationData");
            //SimulationInput input = SimulationInput.FromFile("..\\..\\files\\MonteCarloTest\\infile_sphere.xml");

            Parallel.For(0, mus_temp.Length, j =>
            {
                for (int i = 0; i < mua_temp.Length; i++)
                {
                    SimulationInput input = GetDefaultInput();

                    // overwrite mua, mus settings
                    input.TissueInput.Regions[1].RegionOP.Mua = mua_temp[i];
                    input.TissueInput.Regions[1].RegionOP.Mus = mus_temp[j];
                    
                    Output output = new Output();
                    output.input = input;

                    //SimulationOptions options = new SimulationOptions(10*i*j,
                    //    RandomNumberGeneratorType.Mcch, AbsorptionWeightingType.Continuous);
                    SimulationOptions options = new SimulationOptions();
                    MonteCarloSimulation simulation = new MonteCarloSimulation(input, options);

                    output = simulation.Run();
                    output.ToFile("results/N1e7mua" + mua_temp[i] +
                        "musp" + mus_temp[j] * (1 - input.TissueInput.Regions[1].RegionOP.G) +
                        "g" + output.input.TissueInput.Regions[1].RegionOP.G +
                        "dr" + output.input.DetectorInput.Rho.Delta * 10 +
                        "dt" + output.input.DetectorInput.Time.Delta);
                }
            });
        }

        private static SimulationInput GetDefaultInput()
        {
            SimulationInput input = new SimulationInput();

            input.OutputFileName = "results";
            // comment for compile

            //input.tissptr.num_layers = 1;
            //input.Tissue.Regions[0].n = 1.0; /* idx of outside medium */
            //input.Tissue.Regions[1].n = 1.4; /* idx of layer 1 */
            //input.Tissue.Regions[1].mus = 100.0;
            //input.Tissue.Regions[1].mua = 0.1;
            //input.Tissue.Regions[1].g = 0.80;
            //input.Tissue.Regions[1].d = 10.0;
            //input.Tissue.Regions[2].n = 1.0; /* idx of out bot med */

            //input.source.beamtype = "f";
            //input.source.beam_center_x = 0.0;
            //input.source.beam_radius = 0.0;
            //input.source.src_NA = 0.0;
            //input.detector.nr = 50;  // with dr specifies out to 1cm
            //input.detector.dr = 0.02; // this should agree with GenerateReferenceData
            //input.detector.nz = 10;
            //input.detector.dz = 0.1;
            //input.detector.nx = 80;
            //input.detector.dx = 0.05;
            //input.detector.ny = 80;
            //input.detector.dy = 0.05;

            //input.source.num_photons = 100000; // Usually 1e7
            //input.detector.nt = 400;   // this is 0.5 in reference data, with dt specifies out to 1000 ps
            //input.detector.dt = 5; // this should agree with GenerateReferenceData
            //input.tissptr.do_ellip_layer = 0; /* 0=no pert, 1=ellip, 2=layer, 3=ellip no pMC */
            //input.tissptr.ellip_x = 0.0;
            //input.tissptr.ellip_y = 0.0;
            //input.tissptr.ellip_z = 0.3;
            //input.tissptr.ellip_rad_x = 0.2;
            //input.tissptr.ellip_rad_y = 0.2;
            //input.tissptr.ellip_rad_z = 0.2;
            //input.tissptr.layer_z_min = 0.0;
            //input.tissptr.layer_z_max = 0.02;
            //input.detector.reflect_flag = true;
            //input.detector.det_ctr = new double[] { 0.0 };
            //input.detector.radius = 3.0;

            return input;
        }
    }
}
