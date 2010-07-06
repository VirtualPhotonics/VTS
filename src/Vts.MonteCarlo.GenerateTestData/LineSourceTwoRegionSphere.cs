using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.GenerateTestData
{
    class LineSourceTwoRegionSphere
    {
        #region fields
        public static SimulationInput input;
        public static Output output;
        #endregion

        public static void InitializeLineSourceTwoRegionSphere()
        {
            input = new SimulationInput();
            output = new Output();
            input.OutputFileName = "UnitTestLineSourceTwoRegionSphere";
            input.N = 100000;
            input.SourceInput = new PointSourceInput();
            input.TissueInput = new MultiLayerTissueInput();
            input.DetectorInput = new DetectorInput();
            //input.tissptr.num_layers = 2;
            //input.Tissue.Regions = new Layer[input.tissptr.num_layers + 2];
            //input.Tissue.Regions[0].n = 1.0; /* idx of outside medium */
            //input.Tissue.Regions[1].n = 1.4; /* idx of outside sphere */
            //input.Tissue.Regions[1].mus = 100.0;
            //input.Tissue.Regions[1].mua = 0.1;
            //input.Tissue.Regions[1].g = 0.9;
            //input.Tissue.Regions[1].d = 10.0;
            //input.Tissue.Regions[2].n = 1.4; /* idx of inside sphere */
            //input.Tissue.Regions[2].mus = 120.0;
            //input.Tissue.Regions[2].mua = 0.2;
            //input.Tissue.Regions[2].g = 0.9;
            //input.Tissue.Regions[2].d = 0.0;
            //input.Tissue.Regions[3].n = 1.0; /* idx of out bot med */

            //input.source.beamtype = "r";
            //input.source.beam_center_x = 0.0; //DCFIXED
            //input.source.beam_radius = 0.05;
            //input.source.src_NA = 0.0;

            //input.detector.nr = 10;
            //input.detector.dr = 0.1;
            //input.detector.nz = 10;
            //input.detector.dz = 0.1;
            //input.detector.nx = 80;
            //input.detector.dx = 0.05;
            //input.detector.ny = 80;
            //input.detector.dy = 0.05;

            //input.source.num_photons = 100000;
            //input.detector.nt = 10;
            //input.detector.dt = 100.0;
            ITissue Tissue = new SingleEllipsoidTissue();
            Tissue.Regions.Add(new EllipsoidRegion(new Position(0, 0, 3), 2, 2, 2,
                new OpticalProperties(0.02, 1.20, 0.9, 1.4), AbsorptionWeightingType.Discrete));

            //input.tissptr.do_ellip_layer = 3; /* 0=no pert, 1=ellip, 2=layer, 3=ellip no pMC */
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
            //input.detector.radius = 4.0;
        }

        public static void GenerateLineSourceTwoRegionSphereManagedData()
        {
            InitializeLineSourceTwoRegionSphere();
            // Note seed = 0 is -1 in linux
            SimulationOptions options = new SimulationOptions(0, RandomNumberGeneratorType.Mcch,
                AbsorptionWeightingType.Discrete);
            MonteCarloSimulation managedSimulation = new MonteCarloSimulation(input, options);
            output = managedSimulation.Run();
            output.ToFile("results/LineSourceTwoRegionSphereManaged");

            //ValidateLineSourceTwoRegionSphere();
        }

        public static void GenerateLineSourceTwoRegionSphereUnManagedData()
        {
            InitializeLineSourceTwoRegionSphere();
            UnmanagedSimulationOptions options = new UnmanagedSimulationOptions(0,
                AbsorptionWeightingType.Discrete);
            var unmanagedSimulation = new UnmanagedMonteCarloSimulation(input, options);
            output = unmanagedSimulation.Run();
            output.ToFile("results/LineSourceTwoRegionSphereUnManaged");

            //ValidateLineSourceTwoRegionSphere();
        }
    }
}
