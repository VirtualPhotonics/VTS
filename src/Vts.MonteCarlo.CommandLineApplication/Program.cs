//#define GENERATE_INFILE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuccia.ArrayMath;
using Vts.Common;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using System.IO;

// ParallelFx June '08 CTP
//using System.Threading.Collections;

namespace Vts.MonteCarlo.CommandLineApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //new SimulationInput().ToFile("infile.xml"); return;
            // program requires the path to have no spaces so that it can be used from the command line
            string path = "";
            string basename = "test";
            string filename;
            string outname = "results";
            bool runUnmanagedCode = false;
            bool tallyMomentumTransfer = false; // todo: allow reading of SimulationOptions file from XML
            bool writeHistories = false;

            IEnumerable<SimulationInput> batchQuery = null;
            string[] batchNameQuery = null;
            SimulationInput input = null;

#region     Infile Generation (optional)
#if GENERATE_INFILE
            var tempInput = new SimulationInput(
                1000000,  // FIX 1e6 takes about 70 minutes my laptop
                "Output",
                new PointSourceInput(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    new DoubleRange(0.0, 0, 1)),
                new MultiLayerTissueInput(
                    new LayerRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0, 2),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0, 2),
                            new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(10.0, double.PositiveInfinity, 2),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                            AbsorptionWeightingType.Discrete)
                    }
                ),
                new DetectorInput(
                    new List<TallyType>()
                    {
                        TallyType.RDiffuse,
                        TallyType.ROfAngle,
                        TallyType.ROfRho,
                        TallyType.ROfRhoAndAngle,
                        TallyType.ROfRhoAndTime,
                        TallyType.ROfXAndY,
                        TallyType.ROfRhoAndOmega,
                        TallyType.TDiffuse,
                        TallyType.TOfAngle,
                        TallyType.TOfRho,
                        TallyType.TOfRhoAndAngle,
                    },
                    new DoubleRange(0.0, 40.0, 201), // rho: nr=200 dr=0.2mm used for workshop
                    new DoubleRange(0.0, 10.0, 11),  // z
                    new DoubleRange(0.0, Math.PI / 2, 1), // angle
                    new DoubleRange(0.0, 4.0, 801), // time: nt=800 dt=0.005ns used for workshop
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-100.0, 100.0, 81), // x
                    new DoubleRange(-100.0, 100.0, 81) // y
                ));
            tempInput.ToFile("newinfile.xml");

#endif
#endregion

            if (args.Length > 0)
            {
                path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(args[0])) + "\\";
                basename = System.IO.Path.GetFileNameWithoutExtension(args[0]);
                filename = path + basename + ".xml";

                if (System.IO.File.Exists(filename))
                {
                    input = SimulationInput.FromFile(filename);
                }
                else
                {
                    Console.WriteLine("\nThe following input file could not be found: " + basename + ".xml. Hit *Enter* to exit.");
                    Console.Read();
                }
            }
            else
            {
                Console.Write("\nNo input file specified. Using input.xml from resources... ");
                input = LoadDefaultInputFile();
            }

            batchQuery = input.AsEnumerable();
            batchNameQuery = new[] { "" };

            if (args.Length > 1)
            {
                double[] sweep = null;
                int sweepArgumentStart = 1;

                while (sweepArgumentStart < args.Length
                    && (args[sweepArgumentStart] == "/o" || args[sweepArgumentStart] == "/u" || args[sweepArgumentStart] == "/h" || args[sweepArgumentStart] == "/mt"))
                {
                    // if the output name is explicitly stated (ie. "/o output"), assign it here
                    if (args[sweepArgumentStart] == "/o"
                        && sweepArgumentStart + 1 < args.Length
                        && args[sweepArgumentStart + 1] != null
                        && args[sweepArgumentStart + 1].Length > 0
                        && args[sweepArgumentStart + 1][0] != '/')
                    {
                        outname = args[sweepArgumentStart + 1];
                        sweepArgumentStart += 2;
                    }
                    else if (args[sweepArgumentStart] == "/u")
                    {
                        runUnmanagedCode = true;
                        sweepArgumentStart += 1;
                    }
                    else if (args[sweepArgumentStart] == "/mt")
                    {
                        tallyMomentumTransfer = true;
                        sweepArgumentStart += 1;
                    }
                    else if (args[sweepArgumentStart] == "/h")
                    {
                        writeHistories = true;
                        sweepArgumentStart += 1;
                    }
                }

                InputParameterType inputParameterType = InputParameterType.XSourcePosition;
                string inputParameterString = "";

                for (int i = sweepArgumentStart; i < args.Length; i += 4)
                {
                    try
                    {   // batch parameters should come in fours 
                        // eg. /x -4.0 4.0 0.05 /mus1 0.5 1.5 0.1 /mus2 0.5 1.5 0.1 ...
                        sweep = double.Parse(args[i + 1])
                            .To(double.Parse(args[i + 2]),
                                double.Parse(args[i + 3])).ToArray();
                    }
                    catch
                    {
                        Console.WriteLine("Could not parse the input arguments.");
                        break;
                    }

                    #region switch statement for input argument
                    switch (args[i].ToLower())
                    {
                        case "/mua1":
                            inputParameterType = InputParameterType.Mua1;
                            inputParameterString = "mua1";
                            break;
                        case "/mua2":
                            inputParameterType = InputParameterType.Mua2;
                            inputParameterString = "mua2";
                            break;
                        case "/mus1":
                            inputParameterType = InputParameterType.Mus1;
                            inputParameterString = "mus1";
                            break;
                        case "/mus2":
                            inputParameterType = InputParameterType.Mus2;
                            inputParameterString = "mus2";
                            break;
                        case "/g1":
                            inputParameterType = InputParameterType.G1;
                            inputParameterString = "g1";
                            break;
                        case "/g2":
                            inputParameterType = InputParameterType.G2;
                            inputParameterString = "g2";
                            break;
                        case "/n1":
                            inputParameterType = InputParameterType.N1;
                            inputParameterString = "n1";
                            break;
                        case "/n2":
                            inputParameterType = InputParameterType.N2;
                            inputParameterString = "n2";
                            break;
                        case "/d1":
                            inputParameterType = InputParameterType.D1;
                            inputParameterString = "d1";
                            break;
                        case "/d2":
                            inputParameterType = InputParameterType.D2;
                            inputParameterString = "d2";
                            break;
                        case "/xs":
                            inputParameterType = InputParameterType.XSourcePosition;
                            inputParameterString = "xs";
                            break;
                        case "/ys":
                            inputParameterType = InputParameterType.YSourcePosition;
                            inputParameterString = "xs";
                            break;
                        case "/xe":
                            inputParameterType = InputParameterType.XEllipsePosition;
                            inputParameterString = "xe";
                            break;
                        case "/ye":
                            inputParameterType = InputParameterType.YEllipsePosition;
                            inputParameterString = "ye";
                            break;
                        case "/ze":
                            inputParameterType = InputParameterType.ZEllipsePosition;
                            inputParameterString = "ze";
                            break;
                        case "/xer":
                            inputParameterType = InputParameterType.XEllipseRadius;
                            inputParameterString = "xer";
                            break;
                        case "/yer":
                            inputParameterType = InputParameterType.YEllipseRadius;
                            inputParameterString = "yer";
                            break;
                        case "/zer":
                            inputParameterType = InputParameterType.ZEllipseRadius;
                            inputParameterString = "zer";
                            break;
                    }
                    #endregion

                    if (inputParameterString.Length > 0)
                    {
                        batchQuery = batchQuery.WithParameterSweep(sweep, inputParameterType);
                        batchNameQuery =
                                        (from b in batchNameQuery
                                         from s in sweep
                                         select (b + inputParameterString + "_" + String.Format("{0:f}", s) + "_")).ToArray();
                    }
                }
            }

            SimulationInput[] inputBatch = batchQuery.ToArray();
            //string[] simulationNames = batchNameQuery.ToArray();
            //string directory = 
            string[] outNames = batchNameQuery.Select(s => path + basename + "_" + outname + "\\" + basename + "_" + outname + s).ToArray();

            for (int i = 0; i < inputBatch.Length; i++)
                inputBatch[i].OutputFileName = outNames[i];

            Parallel.For(0, inputBatch.Length, i =>
            {
                var mc = runUnmanagedCode ?
                     new UnmanagedMonteCarloSimulation(
                         inputBatch[i],
                         new UnmanagedSimulationOptions(i))
                   : new MonteCarloSimulation(
                         inputBatch[i],
                         new SimulationOptions(
                             i, 
                             RandomNumberGeneratorType.MersenneTwister, 
                             AbsorptionWeightingType.Discrete,
                             false, 
                             false,
                             writeHistories,
                             tallyMomentumTransfer,
                             i));

                var p = Path.GetDirectoryName(inputBatch[i].OutputFileName);

                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p);

                mc.Run().ToFile(inputBatch[i].OutputFileName);
            });

            // Below are the same things, performed with PLINQ instead:

            //Boolean[] success =
            //    (from i in 0.To(inputBatch.Length - 1).AsParallel()
            //     select new MonteCarloSimulation(
            //                    inputBatch[i],
            //                    new SimulationOptions(i, RandomNumberGeneratorType.Mcch, AbsorptionWeightingType.Discrete)
            //             ).Run().ToFile(inputBatch[i].outputFileName)
            //    ).ToArray();

            //Boolean[] success =
            //    (from i in 0.To(inputBatch.Length - 1).AsParallel()
            //     let m = runUnmanagedCode ?
            //          new UnmanagedMonteCarloSimulation(
            //              inputBatch[i],
            //              new UnmanagedSimulationOptions(i)) // always discrete absorption weighting, mcch
            //        : new MonteCarloSimulation(
            //              inputBatch[i],
            //              new SimulationOptions(i, RandomNumberGeneratorType.Mcch, AbsorptionWeightingType.Discrete))
            //     select m.Run().ToFile(inputBatch[i].outputFileName)).ToArray(); 

            Console.Write("\nSimulation(s) complete.");



            Console.WriteLine("Hit *Enter* to exit.");
            Console.Read();
        }

        private static SimulationInput LoadDefaultInputFile()
        {
            return SimulationInput.FromFileInResources("newinfile.xml", "mc");
        }
    }
}


