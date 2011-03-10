using System;
using System.Collections.Generic;
using Vts.IO;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Provides main processing for Monte Carlo simulation. 
    /// </summary>
    public class MonteCarloSimulation
    {
        public const double COS90D = 1.0E-6;
        public const double COSZERO = (1.0 - 1e-12);

        private ISource _source;
        private ITissue _tissue;
        private DetectorController _detector;
        private long numberOfPhotons;
        private string outputFilename;

        // todo: Why is this static? (DJC 2011-01-16)
        protected static SimulationInput _input;

        private Random _rng;

        public MonteCarloSimulation(SimulationInput input)
        {
            // all field/property defaults should be set here
            _input = input;
            numberOfPhotons = input.N;
            outputFilename = input.OutputFileName;

            WRITE_EXIT_HISTORIES = input.Options.WriteHistories;// Added by DC 2009-08-01
            ABSORPTION_WEIGHTING = input.Options.AbsorptionWeightingType; // CKH add 12/14/09

            // CKH TODO: GeneratorProvider
            switch (input.Options.RandomNumberGeneratorType)
            {
                case RandomNumberGeneratorType.MersenneTwister:
                    _rng = new MathNet.Numerics.Random.MersenneTwister(input.Options.Seed);
                    break;
                //case RandomNumberGeneratorType.Mcch:
                //default:
                //    this.rng = new MCCHGenerator(input.Options.Seed);
                //    break;
            }

            this.SimulationIndex = input.Options.SimulationIndex;

            _tissue = Factories.TissueFactory.GetTissue(input.TissueInput, input.Options.AbsorptionWeightingType, PhaseFunctionType.HenyeyGreenstein);
            _source = Factories.SourceFactory.GetSource(input.SourceInput, _tissue, _rng);
            _detector = Factories.DetectorControllerFactory.GetStandardDetectorController(input.DetectorInputs, _tissue);
        }

        /// <summary>
        /// Default constructor to allow quick-and-easy simulation
        /// </summary>
        public MonteCarloSimulation() : this(new SimulationInput()) { }

        // private properties
        private int SimulationIndex { get; set; }

        // public properties
        private bool WRITE_EXIT_HISTORIES { get; set; }  // Added by DC 2009-08-01 
        private bool WRITE_ALL_HISTORIES { get; set; }  // Added by DC 2011-03-03
        private AbsorptionWeightingType ABSORPTION_WEIGHTING { get; set; }
        public static PhaseFunctionType PHASE_FUNCTION { get; set; }

        /// <summary>
        /// Run the simulation
        /// </summary>
        /// <returns></returns>
        public Output Run()
        {
            Output output = new Output(_input);

            DisplayIntro();

            ExecuteMCLoop();

            _detector.NormalizeDetectors(numberOfPhotons);

            ReportResults();

            return output;
        }

        /// <summary>
        /// This function encapsulates the managed loop. Can be overridden in derived classes.
        /// </summary>
        protected virtual void ExecuteMCLoop()
        {
            PhotonDatabaseWriter terminationWriter = null;
            try
            {
                if (WRITE_EXIT_HISTORIES)
                {
                    terminationWriter = new PhotonDatabaseWriter(_input.OutputFileName + "_photonBiographies");
                }

                for (long n = 1; n <= numberOfPhotons; n++)
                {
                    // todo: bug - num photons is assumed to be over 10 :)
                    if (n % (numberOfPhotons / 10) == 0)
                    {
                        DisplayStatus(n, numberOfPhotons);
                    }

                    var photon = _source.GetNextPhoton(_tissue);

                    do
                    { /* begin do while  */
                        photon.SetStepSize(_rng);

                        var distance = _tissue.GetDistanceToBoundary(photon);

                        bool hitBoundary = photon.Move(distance);

                        if (hitBoundary)
                        {
                            photon.CrossRegionOrReflect();
                        }
                        else
                        {
                            photon.Absorb();
                            if (photon.DP.StateFlag != PhotonStateType.Absorbed)
                            {
                                photon.Scatter();
                            }
                        }

                        /*Test_Distance(); */
                        photon.TestWeightAndDistance();

                    } while (photon.DP.StateFlag == PhotonStateType.NotSet); /* end do while */

                    _detector.TerminationTally(photon.DP);

                    if (terminationWriter != null)
                    {
                        //dc: how to check if detector contains DP  ckh: check is on reading side, may need to fix
                        terminationWriter.Write(photon.DP);
                    }

                    //if (DO_ALLVOX) Compute_Prob_allvox(source, tissptr, photptr, bananaptr, outptr, detector);  /* DCFIX */

                    _detector.HistoryTally(photon.History);

                } /* end of for n loop */

            }
            finally
            {
                if (terminationWriter != null) terminationWriter.Dispose();
            }
        }

        public void ReportResults()
        {
            // CKH TODO: fix this when these classes are updated
            //for (int i = 0; i < input.detector.det_ctr.Length; ++i)  
            //    Console.WriteLine(SimulationIndex + ": det at {0} -> {1} photons written",
            //        detector.det_ctr[i], detector.);

            //Console.WriteLine(SimulationIndex + ": tot phot out top={0}({1}) bot={2}({3})",
            //  photptr.tot_out_top, (double)photptr.tot_out_top / source.num_photons,
            //  photptr.tot_out_bot, (double)photptr.tot_out_bot / source.num_photons);
        }

        /********************************************************/
        void DisplayIntro()
        {
            string intro = "\n" +
                SimulationIndex + ":                                                  \n" +
                SimulationIndex + ":      Monte Carlo Simulation of Light Propagation \n" +
                SimulationIndex + ":              in a multi-region tissue            \n" +
                SimulationIndex + ":                                                  \n" +
                SimulationIndex + ":         written by the Virtual Photonics Team    \n" +
                SimulationIndex + ":              Beckman Laser Institute             \n" +
                SimulationIndex + ":";
            Console.WriteLine(intro);
        }

        /*****************************************************************/
        void DisplayStatus(long n, long num_phot)
        {
            /* fraction of photons completed */
            double frac = 100 * n / num_phot;
            Console.WriteLine(SimulationIndex + ": " + frac + " percent complete, " + DateTime.Now);
        }

        void Display_Status(int num_phot)
        {
            if (numberOfPhotons % num_phot == 0.0)
            {
                Console.WriteLine(SimulationIndex + ": Number of photons processed={0} {1}",
                  (int)numberOfPhotons, DateTime.Now);
            }
        }
        // Keep this commented section for reference
        ///// <summary>
        ///// This function encapsulates the managed loop. Can be overridden in derived classes.
        ///// </summary>
        //protected virtual void ExecuteMCLoop(ITissue tissptr, Photon photptr, History histptr, 
        //    SourceDefinition source, Banana bananaptr, Output outptr, DetectorDefinition detector)
        //{
        //    // DC: should the writer output go to same folder as Output?
        //    using (var photonTerminationDatabaseWriter = new PhotonTerminationDatabaseWriter(
        //            "photonBiographies", new PhotonDatabase() { NumberOfPhotons = 0,
        //            NumberOfSubRegions = tissptr.num_layers}))
        //    {
        //        if (WRITE_EXIT_HISTORIES) photonTerminationDatabaseWriter.Open(); // only open file if we want to write

        //        SetScatterLength(tissptr, photptr);
        //        for (long n = 1; n <= source.num_photons; n++)
        //        {
        //            // todo: bug - num photons is assumed to be over 10 :)
        //            if (n % (source.num_photons / 10) == 0)
        //                DisplayStatus(n, source.num_photons);
        //            init_photon(tissptr, photptr, source, outptr);
        //            do
        //            { /* begin do while  */
        //                SetStepSize(tissptr, photptr);

        //                switch (HitBoundary(tissptr, photptr))
        //                {
        //                    case 1:  // hit layer
        //                        Move_Photon(photptr, outptr);
        //                        CrossRegion(tissptr, photptr, outptr, detector);
        //                        break;
        //                    case 2:  // hit ellipse from outside
        //                    case 4:  // hit ellipse from inside
        //                        Move_Photon(photptr, outptr);
        //                        CrossEllip(photptr);
        //                        break;
        //                    case 0:  // hit nothing in homo. medium
        //                    case 3:  // hit nothing (inside ellipse)
        //                        Move_Photon(photptr, outptr);
        //                        // Call action (Discrete, Analog or Continuous absorption weighting)
        //                        ScatterAndAbsorb(tissptr, photptr, outptr, detector);
        //                        break;
        //                }
        //                /*Test_Distance(); */
        //                TestWeight(photptr);
        //            } while (photptr.DP.StateFlag == PhotonData.PhotonStateType.NotSet); /* end do while */
        //            //pert();  // ckh deleted processing done in MovePhoton

        //            if (WRITE_EXIT_HISTORIES)
        //            {
        //                WritePhotonTerminationData(photonTerminationDatabaseWriter, photptr, tissptr, outptr);
        //            }

        //            if (DO_ALLVOX) Compute_Prob_allvox(source, tissptr, photptr, bananaptr, outptr, detector);  /* DCFIX */
        //        } /* end of for n loop */

        //    } /* end exit history using scope*/
        //}
    }
}
