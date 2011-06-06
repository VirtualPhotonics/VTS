using System;
using System.Linq;
using System.Collections.Generic;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Controllers;

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
        private DetectorController _detectorController;
        private long numberOfPhotons;

        protected SimulationInput _input;
        private Random _rng;

        public MonteCarloSimulation(SimulationInput input)
        {
            // all field/property defaults should be set here
            _input = input;

            var result = SimulationInputValidation.ValidateInput(_input);
            if (result.IsValid == false)
            {
                throw new ArgumentException(result.ValidationRule + (!string.IsNullOrEmpty(result.Remarks) ? "; " + result.Remarks : ""));
            }

            numberOfPhotons = input.N;

            WRITE_DATABASES = input.Options.WriteDatabases; // modified ckh 4/9/11
            ABSORPTION_WEIGHTING = input.Options.AbsorptionWeightingType; // CKH add 12/14/09

            _rng = RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                input.Options.RandomNumberGeneratorType, input.Options.Seed);

            this.SimulationIndex = input.Options.SimulationIndex;

            _tissue = TissueFactory.GetTissue(
                input.TissueInput, 
                input.Options.AbsorptionWeightingType, 
                input.Options.PhaseFunctionType);
            _source = SourceFactory.GetSource(input.SourceInput, _tissue, _rng);
            _detectorController = DetectorControllerFactory.GetStandardDetectorController(
                input.DetectorInputs, 
                _tissue,
                input.Options.TallySecondMoment);
        }

        /// <summary>
        /// Default constructor to allow quick-and-easy simulation
        /// </summary>
        public MonteCarloSimulation() : this(new SimulationInput()) { }

        // private properties
        private int SimulationIndex { get; set; }

        // public properties
        private IList<DatabaseType> WRITE_DATABASES { get; set; }  // modified ckh 4/9/11
        private AbsorptionWeightingType ABSORPTION_WEIGHTING { get; set; }
        public PhaseFunctionType PHASE_FUNCTION { get; set; }

        public Output Results { get; private set; }

        /// <summary>
        /// Run the simulation
        /// </summary>
        /// <returns></returns>
        public Output Run()
        {
            DisplayIntro();

            ExecuteMCLoop();

            // todo: consider statistics, other checks for reporting. SimulationOutput class?
            Results = new Output(_input, _detectorController.Detectors);

            ReportResults();

            return Results;
        }

        /// <summary>
        /// Executes the Monte Carlo Loop
        /// </summary>
        protected virtual void ExecuteMCLoop()
        {
            PhotonDatabaseWriter terminationWriter = null;
            CollisionInfoDatabaseWriter collisionWriter = null;

            try
            {
                if (WRITE_DATABASES != null)
                {
                    if (WRITE_DATABASES.Contains(DatabaseType.PhotonExitDataPoints))
                    {
                        terminationWriter = new PhotonDatabaseWriter(_input.OutputName + "\\photonExitDatabase");
                    }
                    if (WRITE_DATABASES.Contains(DatabaseType.CollisionInfo))
                    {
                        collisionWriter = new CollisionInfoDatabaseWriter(
                            _input.OutputName + "\\collisionInfoDatabase", _tissue.Regions.Count());
                    }
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

                    _detectorController.TerminationTally(photon.DP);

                    if (terminationWriter != null)
                    {
                        //dc: how to check if detector contains DP  ckh: check is on reading side, may need to fix
                        terminationWriter.Write(photon.DP);
                    }
                    if (collisionWriter != null)
                    {
                        collisionWriter.Write(photon.History.SubRegionInfoList);
                    }

                    _detectorController.HistoryTally(photon.History);

                } /* end of for n loop */
            }
            finally
            {
                if (terminationWriter != null) terminationWriter.Dispose();
                if (collisionWriter != null) collisionWriter.Dispose();
            }

            // normalize all detectors by the total number of photons (each tally records it's own "local" count as well)
            _detectorController.NormalizeDetectors(numberOfPhotons);
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
