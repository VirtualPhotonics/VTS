using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.GenerateReferenceData
{
    class Program
    {
        static void Main(string[] args)
        {
            try {
                var input = new SimulationInput(
                    1000000,  // FIX 1e6 takes about 110 minutes my laptop
                    "", // if non-empty string here, need to create sub-folder
                     new SimulationOptions(
                        0,
                        RandomNumberGeneratorType.MersenneTwister,
                        AbsorptionWeightingType.Continuous,
                        PhaseFunctionType.HenyeyGreenstein,
                        new List<DatabaseType>() { }, // databases to be written
                        true, // compute Second Moment
                        false, // track statistics
                        0.0, // RR threshold -> 0 = no RR performed
                        1),
                    new DirectionalPointSourceInput(
                        new Position(0.0, 0.0, 0.0),
                        new Direction(0.0, 0.0, 1.0),
                        0),
                    new MultiLayerTissueInput(
                        new LayerRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0)
                            ),
                        //new LayerRegion(
                        //    new DoubleRange(0.0, 0.1),
                        //    new OpticalProperties(0.033, 1.0, 0.8, 1.38)
                        //    ),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.0, 1.0, 0.8, 1.38)
                            ),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0, 1e-10, 0.0, 1.0)
                            )
                    }
                    ),
                    new List<IDetectorInput>()
                    {
                        new ROfRhoAndTimeDetectorInput(
                            new DoubleRange(0.0, 40, 201), // numbers for scaled MC
                            new DoubleRange(0.0, 4, 801)) // numbers for scaled MC
                    }
                );

            SimulationOutput output = new MonteCarloSimulation(input).Run();
            input.ToXMLFile("infile.xml");

            // the following gets are R(rho,time) for scaled.
            //var rOfRhoAndTime = output.ResultsDictionary[TallyType.ROfRhoAndTime.ToString()];

            //string folderPath = "results";
            //if (!Directory.Exists(folderPath))
            //    Directory.CreateDirectory(folderPath);

            //DetectorIO.WriteDetectorToFile(rOfRhoAndTime, folderPath);
        }
            catch (Exception e)
            {
                Console.WriteLine("Failed to run: Reason: " + e.Message);
                    throw;
                //return false;
            }
        }
        //public static void SetCommonInput(ref SimulationInput input)
        //{
            // comment for compile
        //input.tissptr.num_layers = 1;
        //input.Tissue.Regions[0].n = 1.0; /* idx of outside medium */
        //input.Tissue.Regions[1].n = 1.4; /* idx of layer 1 */
        //input.Tissue.Regions[1].mus = 50.0;
        //input.Tissue.Regions[1].mua = 0.0;
        //input.Tissue.Regions[1].g = 0.80;
        //input.Tissue.Regions[1].d = 10.0;
        //input.Tissue.Regions[2].n = 1.0; /* idx of out bot med */
            //input.source.beamtype = "f";
            //input.source.beam_center_x = 0.0;
            //input.source.beam_radius = 0.0;
            //input.source.src_NA = 0.0;

            //input.detector.nr = 200; // 200 used for workshop
            //input.detector.dr = 0.02; // 0.02 used for workshop
            //input.detector.nz = 10;
            //input.detector.dz = 0.1;
            //input.detector.nx = 80;
            //input.detector.dx = 0.05;
            //input.detector.ny = 80;
            //input.detector.dy = 0.05;

            //input.source.num_photons = 10000000; // 1e7 takes about 6.7 hours my laptop
            //input.detector.nt = 800; // 800 used for workshop
            //input.detector.dt = 5; // 5 used for workshop
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
            //input.detector.radius = 4.0;
        //}       
    }
}
